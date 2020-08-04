using Completed;
using JetBrains.Annotations;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Completed
{
    public enum CustomerState
    {
        walktochair,
        waitorder,
        waitfood,
        eat,
        pay,
        leave,
    }

    public class Customer : MonoBehaviour
    {
        public CustomerState cstate;
        AIDestinationSetter aiDestinationSetter;
        GameObject bubble;
        Animator bubbleanim;
        Transform chair;

        public float waitTime;
        public float leaveTime;
        public float eatTime;

        public float beginTime;
        public float proTime;

        private bool isannoy;
        private bool isorder;
        private bool haspayed;

        public string orderfood;
        public AudioClip moneyclip;
        public AudioClip heartbrokenclip;
        void Start()
        {
            cstate = CustomerState.walktochair;
            aiDestinationSetter = GetComponent<AIDestinationSetter>();
            bubble = transform.Find("bubble").gameObject;
            bubbleanim = bubble.GetComponent<Animator>();
            isannoy = false;
            isorder = false;
            haspayed = false;
            orderfood = null;
        }

        // Update is called once per frame
        void Update()
        {
            switch (cstate)
            {
                case CustomerState.walktochair:
                    {
                        if (aiDestinationSetter.target == null)
                        {
                            aiDestinationSetter.target = GameManager.instance.FindEmptyChair();
                            chair = aiDestinationSetter.target;
                            bubbleanim.SetTrigger("heart");
                        }
                        break;
                    }
                case CustomerState.waitorder:
                    {
                        transform.position = chair.position;

                        proTime = Time.fixedTime;
                        if (proTime - beginTime == waitTime&&!isannoy&&!isorder)
                        {
                            bubbleanim.SetTrigger("annoy");
                            beginTime = proTime;
                            isannoy = true;
                        }
                        if (proTime - beginTime == leaveTime&&isannoy&&!isorder)
                        {
                            bubbleanim.SetTrigger("angry");
                            GameManager.instance.HeartBroken();
                            SoundManager.instance.PlaySingle(heartbrokenclip);

                            chair.GetComponent<Chair>().empty = 0;
                            isannoy = false;
                            Destroy(gameObject, 1f);
                        }
                        break;
                    }
                case CustomerState.waitfood:
                    {
                        transform.position = chair.position;
                        break;
                    }
                case CustomerState.eat:
                    {
                        transform.position = chair.position;
                        proTime = Time.fixedTime;
                        if (proTime - beginTime >= eatTime)
                        {
                            bubbleanim.SetTrigger("money");
                            beginTime = proTime;
                            cstate = CustomerState.pay;
                        }
                        break;
                    }
                case CustomerState.pay:
                    {
                        transform.position = chair.position;
                        proTime = Time.fixedTime;
                        if (proTime - beginTime == leaveTime && !isannoy)
                        {
                            bubbleanim.SetTrigger("annoy");
                            beginTime = proTime;
                            isannoy = true;
                        }
                        if (proTime - beginTime == leaveTime+2f && isannoy)
                        {
                            bubbleanim.SetTrigger("angry");
                            GameManager.instance.HeartBroken();
                            SoundManager.instance.PlaySingle(heartbrokenclip);

                            chair.GetComponent<Chair>().empty = 2;
                            chair.Find("emptyplate").gameObject.SetActive(true);
                            isannoy = false;
                            Destroy(gameObject, 1f);
                        }
                        break;
                    }
                case CustomerState.leave:
                    {
                        break;
                    }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (aiDestinationSetter.target != null)
            {
                if(other.transform.name == aiDestinationSetter.target.name)
                {
                    beginTime = Time.fixedTime;
                    cstate = CustomerState.waitorder;
                    aiDestinationSetter.target = null;
                    bubbleanim.SetTrigger("order");

                }
            }
        }
        public bool UpdateState()
        {//玩家对顾客按空格互动后
            Debug.LogError("Changestate");
            switch (cstate)
            {
                case CustomerState.walktochair:
                    {
                        break;
                    }
                case CustomerState.waitorder:
                    {
                        Debug.LogError("order");
                        orderfood = GameManager.instance.foods[Random.Range(0, 3)];
                        bubbleanim.SetTrigger(orderfood);
                        isorder = true;
                        cstate = CustomerState.waitfood;
                        break;
                    }
                case CustomerState.waitfood:
                    {
                        bubbleanim.SetTrigger("heart");
                        orderfood = "";
                        cstate = CustomerState.eat;
                        beginTime = Time.fixedTime;

                        break;
                    }
                case CustomerState.eat:
                    {
                        break;
                    }
                case CustomerState.pay:
                    {
                        if (!haspayed)
                        {
                            haspayed = true;
                            chair.GetComponent<Chair>().empty = 2;
                            GameManager.instance.AddMoney();
                            SoundManager.instance.PlaySingle(moneyclip);
                            chair.Find("emptyplate").gameObject.SetActive(true);
                            Destroy(gameObject, 1f);
                        }

                        break;
                    }
                case CustomerState.leave:
                    {
                        break;
                    }
            }
            return false;
        }

        public void SetupData(int level)
        {
            if (level == 1)
            {
                waitTime = 3;
                leaveTime = 5;
                eatTime = 15;
            }
            else if (level == 2)
            {
                waitTime = 2;
                leaveTime = 4;
                eatTime = 15;
            }
            else if (level == 3)
            {
                waitTime = 1;
                leaveTime = 3;
                eatTime = 15;
            }
        }
    }
}
