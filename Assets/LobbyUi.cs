using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUi : MonoBehaviour
{
    [SerializeField] private GameObject playerPanel;
    [SerializeField] private GameObject hostPanel;
    
    [SerializeField] private GameObject startButton;
    
    
    
    [SerializeField] private PlayerUi playerUi;

    [SerializeField] private Text Title;
    
    bool isHost = true;
    
    void Start()
    {
        startButton.SetActive(false);
        
        if (NetworkServer.active)
        {
            playerPanel.SetActive(false);
        }
        else
        {
            hostPanel.SetActive(false);
            isHost = false;
            
        }
        

    }

    public void ChooseMode1Button()
    {
        GamePrepare(1);
        
    }

    public void ChooseMode2Button()
    {
      
    }
    public void ChooseMode3Button()
    {
       
    }

    public void GamePrepare(int _mode)
    {
        hostPanel.SetActive(false);
        playerPanel.SetActive(true);
        GameManager.instance.isGameLunch = true;
    }

    public void LunchButton()
    {
        playerUi.GameLunch(1);
        if (NetworkServer.active)
        {
            GameManager.instance.min = 9f;
            GameManager.instance.sec = 59.9f;
        }
    }
    
    void Update()
    {
        if (GameManager.instance.isGameLunch)
        {
            Title.text = "Game is chosen";
            startButton.SetActive(true);
            
        }
    }
}
