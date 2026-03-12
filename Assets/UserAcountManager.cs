using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UserAcountManager : MonoBehaviour
{

    public static UserAcountManager instance;
    
    public static string LoggedInUsername;

    public string LobbySceneName = "lobby";
    
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

    }

    public void LogIn(Text username)
    {
        LoggedInUsername = username.text;
        Debug.Log(LoggedInUsername);
        SceneManager.LoadScene(LobbySceneName);
        
        
        
    }
    
}
