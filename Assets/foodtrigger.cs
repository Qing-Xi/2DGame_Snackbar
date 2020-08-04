using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Completed
{
    public class foodtrigger : MonoBehaviour
    {
        private Player player;
        private Animator playeranim;
        private void Start()
        {
            player = transform.parent.GetComponent<Player>();
            playeranim = transform.parent.Find("bubble").GetComponent<Animator>();
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.LogError("space"+other.name);
            if (other.tag == "Food")
            {
                Food food = other.transform.GetComponent<Food>();
                if (player.foodoremptyplate==null&&food.UpdateState())
                {//手上没有菜或空盘子，才能做菜或拿菜
                    playeranim.SetTrigger(food.name);
                    player.foodoremptyplate=food.transform;
                    //playeranim.SetBool(food.name, true);
                }
                //Call the RandomizeSfx function of SoundManager and pass in two eating sounds to choose between to play the eating sound effect.
                //SoundManager.instance.RandomizeSfx (eatSound1, eatSound2);

            }
            else if (other.tag == "trash")
            {
                if (player.foodoremptyplate != null)
                {
                    playeranim.SetTrigger("empty");
                    if(player.foodoremptyplate.tag == "emptyplate")
                    {
                        GameManager.instance.PlateMoney();
                    }
                    player.foodoremptyplate = null;
                    GameManager.instance.CheckIfLevelPass();
                }
            }
            else if (other.tag == "sink")
            {
                if (player.foodoremptyplate != null&&player.foodoremptyplate.tag=="emptyplate")
                {
                    playeranim.SetTrigger("empty");
                    player.foodoremptyplate = null;
                    GameManager.instance.CheckIfLevelPass();
                }
            }
            else if (other.tag=="customer")
            {
                Customer customer = other.transform.GetComponent<Customer>();
                if(player.foodoremptyplate!=null&& player.foodoremptyplate.name == customer.orderfood)
                {
                    customer.UpdateState();
                    player.foodoremptyplate = null;
                    playeranim.SetTrigger("empty");

                    gameObject.SetActive(false);
                    return;
                }
                else if (player.foodoremptyplate == null)
                {
                    if (customer.cstate != CustomerState.waitfood)
                    {
                        customer.UpdateState();
                    }
                }
            }
            else if (other.tag == "emptyplate")
            {
                if (player.foodoremptyplate == null)
                {
                    playeranim.SetTrigger("emptyplate");
                    other.transform.parent.GetComponent<Chair>().empty = 0;
                    player.foodoremptyplate = other.transform;
                    other.gameObject.SetActive(false);
                }
            }

            gameObject.SetActive(false);
        }
    }

}
