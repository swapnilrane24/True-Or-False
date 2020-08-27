using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

	public Questions[] questions;//this is a array of question which we get from our xml file

	public const string path = "questions";        // name of xml file

	private static List<Questions> unansweredQues; // list of unanswered questions

	private Questions currentQuestion;            //ref to current asked question
	public string homeScene;                      //ref to menu scene
	public Text scoreText;                        //ref to score text
	public Text hiScoreText;                      //ref to hi score text
	public Slider sliderBar;                      //the fill image which changes with value
	public float timePerQues = 4;                 //max time for each question
	public int score;
	public int hiScore;
	private AudioSource audioSource;               //ref to audio source
	public AudioClip[] audioClips;                 //ref to audio clip, 0 is for correct, 1 for wrong and 2 for button click
	private int isMusicOn;                         //1 is ON and 0 is OFF

	[SerializeField] private Text questionText;   //ref to question text
	[SerializeField] private Text trueAnswerText; //ref to result text when we click true button
	[SerializeField] private Text falseAnswerText;//ref to result text when we click false button
	[SerializeField] private Animator animator;   //ref to game canvas animator
	[SerializeField] private Animator gameOverPanel;//ref to game over canvas animator

	[SerializeField] private float delay = 1f; //its a time between questions


	public bool isGameOver;                    //to track game status
	public bool buttonClicked;                 //to track player input status

	[HideInInspector]
    public ManageVariables vars;
	void OnEnable()
    {
        vars = Resources.Load("ManageVariablesContainer") as ManageVariables;
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

	void Start()
	{
		AdManager.instance.gamesPlayed++;
		SoundStatus ();
		score = 0;                                 //at start we want score to be zero
		hiScore = PlayerPrefs.GetInt ("HiScore");  //we get the saved hiscore
		timePerQues = 4;                           //we set max time for question
		buttonClicked = false;                     //at start the player input is nothing
		isGameOver = false;                        //at start the game is not over
		audioSource = GetComponent<AudioSource>(); // we get the component attached to game object
		//at start we need to add xml data to questions array
		QuestionsContainer qc = QuestionsContainer.Load(path);

		questions = qc.questionsList.ToArray();  //we store data from questionList into questions array

		SetCurrentQuestion ();                 //we call the question setting method
	}

	void Update()
	{
		HiScore ();                           //we keep checking for hiscore

		//here we check is game over or not and depending on that we do followin functions
		if (isGameOver) {
			StartCoroutine (GameOverAnim ());
		} else {
			TimerForQues ();
		}
	}

	//method which is used to set the question
	void SetCurrentQuestion ()
	{
		timePerQues = 4;//at start of every question we reset the time 

		//we check is question list is empty or all questions are answer , depending on that we try to load question again
		if (unansweredQues == null || unansweredQues.Count == 0) {

			unansweredQues = questions.ToList<Questions>();
		}

		//we here randomly select the question from the list
		int randomQuestionIndex = Random.Range (0, unansweredQues.Count);

		//we then store the question sellected in the current question
		currentQuestion = unansweredQues [randomQuestionIndex];
		//we change the question text
		questionText.text = currentQuestion.question;

		//set the result of true and false button
		if (currentQuestion.isTrue == "True") {
			trueAnswerText.text = "CORRECT";
			trueAnswerText.color = Color.green;
			falseAnswerText.text = "WRONG";
			falseAnswerText.color = Color.red;
		} else {
			trueAnswerText.text = "WRONG";
			trueAnswerText.color = Color.red;
			falseAnswerText.text = "CORRECT";
			falseAnswerText.color = Color.green;
		}
	}

	//when player answer correct answer we the load another question
	IEnumerator TransitionToNextQuestion()
	{
		//1st we remove the answered question from list to prevent repetition
		unansweredQues.Remove(currentQuestion);
		yield return new WaitForSeconds (delay);
		//then we play no answer animation to reset the buttons
		animator.Play ("NoAnswer");
		//reset the button click bool
		buttonClicked = false;
		//and call the question method
		SetCurrentQuestion ();
	}

	//function call when we press true button
	public void TrueButton()
	{
		//it check for game over and if its false then it perform below code
		if (isGameOver)
			return;
		//set the bool true
		buttonClicked = true;
		//call the true button result animation
		animator.SetTrigger ("True");

		//check weather answer text is equal to true , because true button will have true name on it
		//if not the game is over or increase the score
		if (currentQuestion.isTrue != "True") {
			isGameOver = true;
			audioSource.PlayOneShot (audioClips [1]);
		} else {
			score++;
			audioSource.PlayOneShot (audioClips [0]);
		}
		//when the answer is true we load another question
		if(isGameOver == false)
			StartCoroutine (TransitionToNextQuestion ());
	}

	public void FalseButton()
	{
		if (isGameOver)
			return;

		buttonClicked = true;

		animator.SetTrigger ("False");

		if (currentQuestion.isTrue != "False") {
			isGameOver = true;
			audioSource.PlayOneShot (audioClips [1]);
		}else {
			score++;
			audioSource.PlayOneShot (audioClips [0]);
		}

		if(isGameOver == false)
			StartCoroutine (TransitionToNextQuestion ());
	}

	//function called when we click retry button
	public void RetryButton()
	{
		audioSource.PlayOneShot (audioClips [2]);

		//here we reset i value so when game over take place we show ad
        if (AdManager.instance != null && AdManager.instance.gamesPlayed >= vars.showInterstitialAfter)
        {
            AdManager.instance.gamesPlayed = 0;

            #if AdmobDef
            AdManager.instance.ShowInterstitial();
            #endif
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
	}

	//function called when we click home button
	public void HomeButton()
	{
		audioSource.PlayOneShot (audioClips [2]);

		//here we reset i value so when game over take place we show ad
        if (AdManager.instance != null && AdManager.instance.gamesPlayed >= vars.showInterstitialAfter)
        {
            AdManager.instance.gamesPlayed = 0;

            #if AdmobDef
            AdManager.instance.ShowInterstitial();
            #endif
        }

		SceneManager.LoadScene(homeScene);
        
    }

	//this method update the slider value
	public void TimerForQues()
	{
		timePerQues -= Time.deltaTime;
		//we are using currentTime becoz we want value between 0-1 and for that we divide timePerQues by 4 which is max time
		float currentTime = timePerQues / 4;

		//check for game over and button click ,and if both are false it keep updating value
		if (!isGameOver && !buttonClicked) {
			sliderBar.value = currentTime;
		}

		//when the time become zero game is over
		if (timePerQues <= 0) 
		{
			audioSource.PlayOneShot (audioClips [1]);
			isGameOver = true;
		}
	}

	//this play the game over animation
	IEnumerator GameOverAnim()
	{
		yield return new WaitForSeconds (0.2f);
		scoreText.text = "Score: " + score;       //set the score text
		hiScoreText.text = "HiScore: " + hiScore; //set the hiscore text
		gameOverPanel.SetTrigger ("GameOver");    //play game over animation
	}

	//method which keep traxk of hiscore
	void HiScore()
	{
		//we keep checking hiscore and if the score become greater than hiscore we set new value to it and save it
		if (hiScore < score)
		{
			hiScore = score;
			PlayerPrefs.SetInt ("HiScore", hiScore);
		}
	}

	//here we check sound status
	public void SoundStatus()
	{
		if (PlayerPrefs.HasKey ("Music")) 
		{
			isMusicOn = PlayerPrefs.GetInt ("Music");
		}

		if (isMusicOn == 0) {
			AudioListener.volume = 0;
		} else {
			AudioListener.volume = 1;
		}
	}
}
