using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

#if GooglePlayDef
using GooglePlayGames; 
using UnityEngine.SocialPlatforms;
#endif 

public class GooglePlayInGame : MonoBehaviour {

    public static GooglePlayInGame instance;

    [HideInInspector]
    public ManageVariables vars;

    void OnEnable()
    {
        vars = Resources.Load("ManageVariablesContainer") as ManageVariables;
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void Awake()
    {
        MakeInstance();
    }

    void MakeInstance()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }


    // Use this for initialization
    void Start ()
    {
#if GooglePlayDef
        PlayGamesPlatform.Activate();
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                
            }
        });
#endif
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        int hiScore = PlayerPrefs.GetInt("HiScore");
        ReportScore(hiScore);
    }

    //use this method for button
    public void OpenLeaderboardsScore()
    {
#if GooglePlayDef
        if (Social.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.ShowLeaderboardUI(vars.leaderBoardID);
        }
#endif
    }

    void ReportScore(int score)
    {
        if (Social.localUser.authenticated)
        {
            Social.ReportScore(score, vars.leaderBoardID, (bool success) => { });
        }
    }
}
