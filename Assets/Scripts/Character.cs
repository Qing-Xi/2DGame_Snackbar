using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Completed
{
    public class Character : MovingObject
    {
        //public float speed;

        public float restartLevelDelay = 1f;        //Delay time in seconds to restart level.
        public AudioClip moveSound1;                //1 of 2 Audio clips to play when player moves.
        public AudioClip moveSound2;                //2 of 2 Audio clips to play when player moves.
        public AudioClip eatSound1;                 //1 of 2 Audio clips to play when player collects a food object.
        public AudioClip eatSound2;                 //2 of 2 Audio clips to play when player collects a food object.
        public AudioClip drinkSound1;               //1 of 2 Audio clips to play when player collects a soda object.
        public AudioClip drinkSound2;               //2 of 2 Audio clips to play when player collects a soda object.
        public AudioClip gameOverSound;             //Audio clip to play when player dies.

        private Animator animator;                  //Used to store a reference to the Player's animator component.
        private int food;                           //Used to store player food points total during level.
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        private Vector2 touchOrigin = -Vector2.one;	//Used to store location of screen touch origin for mobile controls.
#endif
        protected override void Start()
        {
            animator = GetComponentInChildren<Animator>();
            
            base.Start();
        }

        private void Update()
        {

            if (rb2D.velocity.x == 0f && rb2D.velocity.y == 0f)
            {
                animator.SetBool("right", false);
                animator.SetBool("left", false);
                animator.SetBool("up", false);
                animator.SetBool("down", false);
            }
        }
        protected override void OnCantMove<T>(T component)
        {
            //Set hitWall to equal the component passed in as a parameter.
            Wall hitWall = component as Wall;

            //Call the DamageWall function of the Wall we are hitting.
            //hitWall.DamageWall(wallDamage);

            //Set the attack trigger of the player's animation controller in order to play the player's attack animation.
            animator.SetTrigger("playerChop");
        }

        public override void AttemptMove<T>(float xDir, float yDir)
        {
            animator.SetBool("right", xDir > 0);
            animator.SetBool("left", xDir < 0);
            animator.SetBool("up", yDir > 0);
            animator.SetBool("down", yDir < 0);
            //Every time player moves, subtract from food points total.
            food--;

            //Update food text display to reflect current score.
            //foodText.text = "Food: " + food;

            //Call the AttemptMove method of the base class, passing in the component T (in this case Wall) and x and y direction to move.
            base.AttemptMove<T>(xDir, yDir);

            //Hit allows us to reference the result of the Linecast done in Move.
            RaycastHit2D hit;

            //If Move returns true, meaning Player was able to move into an empty space.
            if (Move(xDir, yDir, out hit))
            {
                Debug.LogError("move");
                Debug.LogError(rb2D.velocity.x);
                //Call RandomizeSfx of SoundManager to play the move sound, passing in two audio clips to choose from.
                SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
            }
            
            //CheckIfGameOver();

            //Set the playersTurn boolean of GameManager to false now that players turn is over.
            GameManager.instance.playersTurn = false;
        }

        public void RightMove()
        {
            rb2D.MovePosition(transform.position + Vector3.right * speed);
            animator.SetBool("right", true);
            animator.SetBool("left", false);
            animator.SetBool("up", false);
            animator.SetBool("down", false);
        }
        public void LeftMove()
        {
            rb2D.MovePosition(transform.position + Vector3.left * speed);
            animator.SetBool("right", false);
            animator.SetBool("left", true);
            animator.SetBool("up", false);
            animator.SetBool("down", false);
        }


    }
}

