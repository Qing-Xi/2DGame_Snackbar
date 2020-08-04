using Completed;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


enum FoodState
{
    empty,
    cook,
    done,
}
public class Food : MonoBehaviour
{
    FoodState foodstate;
    
    Animator foodanim;

    public float cookTime;
    public float beginTime;
    public float proTime;

    public AudioClip doneclip;
      
    private void Awake()
    {
        foodstate = FoodState.empty;
        foodanim = transform.GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        proTime = Time.fixedTime;
        if ((proTime - beginTime) >= cookTime&&foodstate == FoodState.cook)
        {
            foodstate = FoodState.done;
            SoundManager.instance.PlaySingle(doneclip);
            foodanim.SetBool("done", true);
            foodanim.SetBool("cook", false);
        }
    }

    public bool UpdateState()
    {
        switch (foodstate)
        {
            case FoodState.empty:
                beginTime= Time.fixedTime;
                foodstate = FoodState.cook;
                foodanim.SetBool("cook", true);

                return false;
            case FoodState.cook:
                return false;

            case FoodState.done:
                foodanim.SetBool("done", false);
                foodstate = FoodState.empty;
                return true;

        }
        return false;
    }
}

