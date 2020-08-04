using UnityEngine;
using System.Collections;
using UnityEngine.UI;	//Allows us to use UI.
using UnityEngine.SceneManagement;

namespace Completed
{
	//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
	public class Player : MovingObject
	{

		public AudioClip moveSound;				
		public AudioClip moneySound;					
		public AudioClip fooddoneSound;					
		//public AudioClip drinkSound1;				//1 of 2 Audio clips to play when player collects a soda object.
		//public AudioClip drinkSound2;				//2 of 2 Audio clips to play when player collects a soda object.
		public AudioClip gameOverSound;				//Audio clip to play when player dies.
		

		private Animator animator;					//Used to store a reference to the Player's animator component.
		private int food;                           //Used to store player food points total during level.

        public Transform detect;
        public Transform foodoremptyplate;
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        private Vector2 touchOrigin = -Vector2.one;	//Used to store location of screen touch origin for mobile controls.
#endif


        //Start overrides the Start function of MovingObject
        protected override void Start ()
		{
			//Get a component reference to the Player's animator component
			animator = GetComponentInChildren<Animator>();
            detect = transform.Find("foodtrigger");
            detect.gameObject.SetActive(false);

			//Get the current food point total stored in GameManager.instance between levels.
			//food = GameManager.instance.playerFoodPoints;
			
			//Set the foodText to reflect the current player food total.
			//foodText.text = "Food: " + food;
			
			base.Start ();
		}
		
		
		//This function is called when the behaviour becomes disabled or inactive.
		private void OnDisable ()
		{
			//When Player object is disabled, store the current local food total in the GameManager so it can be re-loaded in next level.
			//GameManager.instance.playerFoodPoints = food;
		}
		
		
		private void Update ()
		{

			float horizontal = 0;  	
			float vertical = 0;		
			
#if UNITY_STANDALONE || UNITY_WEBPLAYER
			
			horizontal = Input.GetAxis("Horizontal");
			
			vertical = Input.GetAxis("Vertical");
			
			if(horizontal != 0)
			{
				vertical = 0;
			}
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
			
			//Check if Input has registered more than zero touches
			if (Input.touchCount > 0)
			{
				//Store the first touch detected.
				Touch myTouch = Input.touches[0];
				
				//Check if the phase of that touch equals Began
				if (myTouch.phase == TouchPhase.Began)
				{
					//If so, set touchOrigin to the position of that touch
					touchOrigin = myTouch.position;
				}
				
				//If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
				else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
				{
					//Set touchEnd to equal the position of this touch
					Vector2 touchEnd = myTouch.position;
					
					//Calculate the difference between the beginning and end of the touch on the x axis.
					float x = touchEnd.x - touchOrigin.x;
					
					//Calculate the difference between the beginning and end of the touch on the y axis.
					float y = touchEnd.y - touchOrigin.y;
					
					//Set touchOrigin.x to -1 so that our else if statement will evaluate false and not repeat immediately.
					touchOrigin.x = -1;
					
					//Check if the difference along the x axis is greater than the difference along the y axis.
					if (Mathf.Abs(x) > Mathf.Abs(y))
						//If x is greater than zero, set horizontal to 1, otherwise set it to -1
						horizontal = x > 0 ? 1 : -1;
					else
						//If y is greater than zero, set horizontal to 1, otherwise set it to -1
						vertical = y > 0 ? 1 : -1;
				}
			}
			
#endif //End of mobile platform dependendent compilation section started above with #elif
			if(horizontal != 0 || vertical != 0)
			{
				AttemptMove<Wall> (horizontal, vertical);
			}

			bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f); //horizontal与0是否大约相等
			bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);     //vertical与0是否大约相等
			bool isWalking = hasHorizontalInput || hasVerticalInput;
            if (isWalking == false)
            {
				SoundManager.instance.moveSource.Stop();
				animator.SetBool("right", false);
				animator.SetBool("left", false);
				animator.SetBool("up", false);
				animator.SetBool("down", false);
			}
			
			if (Input.GetKeyDown(KeyCode.Space))
            {
                //RaycastHit2D boxhit=Physics2D.BoxCast()
                
                detect.gameObject.SetActive(true);
                Invoke("SetDetectFalse", 0.2f);
            }
        }

        private void SetDetectFalse()
        {
            detect.gameObject.SetActive(false);
        }

        public override void AttemptMove <T> (float xDir, float yDir)
		{
            animator.SetBool("right", xDir > 0);
            animator.SetBool("left", xDir < 0);
            animator.SetBool("up", yDir > 0);
            animator.SetBool("down", yDir < 0);
 
			base.AttemptMove <T> (xDir, yDir);
			
			//Hit allows us to reference the result of the Linecast done in Move.
			RaycastHit2D hit;
			
			if (Move (xDir, yDir, out hit)) 
			{
				SoundManager.instance.PlayMove();

			}
		}
		
		
		//OnCantMove overrides the abstract function OnCantMove in MovingObject.
		//It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
		protected override void OnCantMove <T> (T component)
		{
		}


	}
}

