using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Completed
{
    using Pathfinding;
    using System.Collections.Generic;		//Allows us to use Lists. 
	using UnityEngine.UI;					//Allows us to use UI.
	
	public class GameManager : MonoBehaviour
	{
		public float restartLevelDelay = 1f;        

		public float levelStartDelay = 6f;						
		public float turnDelay = 0.1f;							//Delay between each Player turn.
		public static GameManager instance = null;				//Static instance of GameManager which allows it to be accessed by any other script.
		[HideInInspector] public bool playersTurn = true;       //Boolean to check if it's players turn, hidden in inspector but public.
		public GameObject customerpre;
		private Vector3 customerpos;

		public List<Chair> chairs;
        public List<Transform> hearts;
        public List<string> foods;
        private int heartsnumber;

        private Text levelText;									//Text to display current level number.
		private GameObject levelImage;							//Image to block out level as levels are being set up, background for levelText.
        private Text moneyText;
        private int money=0;
        private int newmoney = 0;
		private bool levelpass=false;

		private int level = 1;
        
		private bool doingSetup = true;                         //Boolean to check if we're setting up board, prevent Player from moving during setup.
		private AstarPath astarPath;

		[Header("Time Variables")]
		[Tooltip("设置间隔时间的两个变量")]
		public float proTime = 0.0f;
		public float NextTime = 0.0f;


		//Awake is always called before any Start functions
		void Awake()
		{
            //Check if instance already exists
            if (instance == null)

                //if not, set instance to this
                instance = this;

            //If instance already exists and it's not this:
            else if (instance != this)

                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
                Destroy(gameObject);	
			
			//Sets this to not be destroyed when reloading scene
			DontDestroyOnLoad(gameObject);

            InitFoodList();

            heartsnumber = 4;
            Debug.LogError("Awake");
            InitHeart();
            //Get a component reference to the attached BoardManager script
            //boardScript = GetComponent<BoardManager>();

            //Call the InitGame function to initialize the first level 
            InitGame();
		}

        //this is called only once, and the paramter tell it to be called only after the scene was loaded
        //(otherwise, our Scene Load callback would be called the very first load, and we don't want that)
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static public void CallbackInitialization()
        {
            //register the callback to be called everytime the scene is loaded
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        //This is called each time a scene is loaded.
        static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (instance.levelpass)
            {
                if (instance.level < 4)
                {
                    instance.level++;

                }
                else
                {
                    instance.level=1;
                }
            }

            instance.InitGame();
        }

		
		//Initializes the game for each level.
		void InitGame()
		{
			//While doingSetup is true the player can't move, prevent player from moving while title card is up.
			doingSetup = true;
			levelpass = false;
			astarPath = GameObject.Find("PathFinder").GetComponent<AstarPath>();
            
			customerpre= GameObject.Find("Customer").transform.Find("Customer").gameObject;
			customerpos = GameObject.Find("Customer").transform.position;
			GameObject canvas = GameObject.Find("Canvas");
            levelImage = canvas.transform.Find("LevelImage").gameObject;
			levelText = levelImage.transform.Find("LevelText").GetComponent<Text>();
			GameObject[] tmparray = GameObject.FindGameObjectsWithTag("chair");
            for(int i=0;i<tmparray.Length;i++)
            {
				chairs[i]=tmparray[i].GetComponent<Chair>();
            }

            money = instance.money;
            moneyText = canvas.transform.Find("MoneyText").GetComponent<Text>();
            moneyText.text= "收入：" + money;
            if (level == 1)
            {
				levelText.text = "第1年，你在一间小吃店打工，老板要求你一个人做所有的工作……/nWASD移动，空格键互动";
			}
			else if (level == 2)
            {
				levelText.text = "第2年，小吃店积累了一些口碑，但员工仍然只有你一个人。";
			}
            else if (level == 3)
            {
                levelText.text = "第3年，你想辞职，老板找不到其他人接替你的职位，于是把小吃店卖给了你，现在你也是老板了！";
            }
            else if (level == 4)
            {
                levelText.text = "游戏结束！老板辛苦了！欢迎再来~";

            }
            levelImage.SetActive(true);
            if (level < 4)
            {
                Invoke("HideLevelImage", levelStartDelay+level);
			
			    customerpre.GetComponent<Customer>().SetupData(level);
            }
            if (level == 4)
            {
                instance.money = 0;
                instance.heartsnumber = 4;
                instance.level = 0;
                Invoke("Restart", 10f);
            }

        }
		
        void InitHeart()
        {
            heartsnumber = instance.heartsnumber;
            GameObject heartspre = GameObject.Find("hearts");
            Transform heartpre = heartspre.transform.Find("heart");
            hearts.Clear();
            for (int i = 0; i < heartsnumber; i++)
            {
                Debug.LogError(i);
                GameObject newheart = Instantiate(heartpre.gameObject, heartspre.transform);
                newheart.transform.position = heartspre.transform.position + new Vector3(i, 0, 0);
                newheart.SetActive(true);
                hearts.Add(newheart.transform);
                Debug.LogError(i);
            }
        }
		
		void HideLevelImage()
		{
			levelImage.SetActive(false);
			
			doingSetup = false;
		}

        void InitFoodList()
        {
            foods.Add("Chicken");
            foods.Add("Fish");
            foods.Add("Sashimi");
        }

        void Update()
		{
			if(doingSetup)
				return;

			proTime = Time.fixedTime;
			if (proTime - NextTime >= 10)
			{
                if (chairs.Count > 0&&!levelpass)
                {
					GameObject customer= Instantiate(customerpre);
					customer.transform.position = customerpos;
					customer.SetActive(true);
                }
				NextTime = proTime;
			}
			astarPath.Scan();

		}



		public void HeartBroken()
        {
            Destroy(hearts[heartsnumber - 1].gameObject);
            hearts.RemoveAt(heartsnumber-1);
            heartsnumber--;
            CheckIfGameOver();
        }

        private void CheckIfGameOver()
        {
            if (hearts.Count==0)
            {
                //SoundManager.instance.PlaySingle(gameOverSound);
                
                //SoundManager.instance.musicSource.Stop();
                
                GameOver();
            }
        }

        public Transform FindEmptyChair()
		{
			for(int i = 0; i < chairs.Count; i++)
            {
                if (chairs[i].empty == 0)
                {
					chairs[i].empty = 1;
					return chairs[i].transform;
				}
			}
			return null;
		}
        
		public void GameOver()
		{
            //Set levelText to display number of levels passed and game over message
            levelText.text = "由于收到过多投诉，小吃店倒闭，你失业了";//"After " + level + " days, you starved.";
			
			//Enable black background image gameObject.
			levelImage.SetActive(true);

            enabled = false;

        }

        public void PlateMoney()
        {
            money -= 1;
        }
        public void AddMoney()
        {
            money += 10;
            newmoney += 10;
            moneyText.text = "收入：" + money;
			CheckIfLevelPass();

		}
		public void CheckIfLevelPass()
        {
            if (newmoney>=level*30)
            {
                if (!levelpass)
                {
					levelpass = true;
                }
				else if (GameObject.Find("Customer(Clone)") == null&& GameObject.FindGameObjectsWithTag("emptyplate").Length==0)
                {
                    instance.money = money;
                    instance.heartsnumber = heartsnumber;
					Invoke("Restart", restartLevelDelay);
                    instance.levelpass = true;
				}

			}
        }
		private void Restart()
		{
			//Load the last scene loaded, in this case Main, the only scene in the game. And we load it in "Single" mode so it replace the existing one
			//and not load all the scene object in the current scene.
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);//
        }
	}
}

