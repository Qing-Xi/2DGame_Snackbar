using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Completed
{
    public class PlayerControll : MonoBehaviour
    {
        public Character character;
        private Animator animator;					//Used to store a reference to the Player's animator component.
        float horizontal;
        float vertical;
        public float speed;

        void Update()
        {
            
            if (!GameManager.instance.playersTurn) return;

            PlayerMove();

            //if (horizontal == 0 && vertical == 0)
            //{
            //    Debug.LogError("qingling");
            //    rb2D.velocity = Vector2.zero;
            //}

        }
        void PlayerMove()
        {
            if (Input.GetKey(KeyCode.W))
            {
                character.rb2D.MovePosition(character.transform.position + Vector3.up * speed);
            }
            if (Input.GetKey(KeyCode.S))
            {
                character.rb2D.MovePosition(character.transform.position + Vector3.down * speed);
            }
            if (Input.GetKey(KeyCode.A))
            {
                character.rb2D.MovePosition(character.transform.position + Vector3.left * speed);
            }
            if (Input.GetKey(KeyCode.D))
            {
                character.RightMove();
            }
        }
    }
}

