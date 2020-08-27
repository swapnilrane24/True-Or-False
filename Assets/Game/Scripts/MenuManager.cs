using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

    public static MenuManager instance;

	public Image soundImage;     //ref to sound button image
	public Sprite[] soundImages; // 1 is for ON and 0 is for OFF
	public string gameLink = ""; //ref to the game link address
	public string playScene;     //ref to the play scene
	private int isMusicOn;       //1 is ON and 0 is OFF
	private AudioSource audioSource;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

	// Use this for initialization
	void Start () 
	{
		audioSource = GetComponent<AudioSource> ();

		if (PlayerPrefs.HasKey ("Music")) 
		{
			isMusicOn = PlayerPrefs.GetInt ("Music");
		}

		if (isMusicOn == 0) {
			soundImage.sprite = soundImages [0];
			AudioListener.volume = 0;
		} else {
			soundImage.sprite = soundImages [1];
			AudioListener.volume = 1;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//function called when play button is pressed
	public void PlayButton()
	{
		audioSource.Play ();
        SceneManager.LoadScene(playScene);
	}

	//function called when Leaderboard Button is pressed
	public void LeaderboardButton()
	{
		audioSource.Play ();
        GooglePlayInGame.instance.OpenLeaderboardsScore();
    }

	//function called when Rate Button is pressed
	public void RateButton()
	{
		audioSource.Play ();
		Application.OpenURL (gameLink);
	}

	//function called when Sound Button is pressed
	public void SoundButton()
	{
		audioSource.Play ();
		//here 1st we get the value , then we check fo music status , if its on then we make it off and vice versa
		isMusicOn = PlayerPrefs.GetInt ("Music");
		// 1 is music ON and 0 is music OFF
		if (isMusicOn == 0) {
			//here we set the button image and the value
			soundImage.sprite = soundImages [1];
			PlayerPrefs.SetInt ("Music", 1);
			AudioListener.volume = 1;
		} else {
			soundImage.sprite = soundImages [0];
			PlayerPrefs.SetInt ("Music", 0);
			AudioListener.volume = 0;
		}
	}


}
