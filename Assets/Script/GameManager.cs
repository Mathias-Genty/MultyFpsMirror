using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public MatchSetting matchSetting;
    
    private const string playerIdPrefix = "Player_";

    private static Dictionary<string, Player> players = new Dictionary<string, Player>();
    
    [SerializeField]GameObject sceneCamera;

    [SyncVar] public float min = 9;
    [SyncVar] public float sec = 59.9f;
    
    
    public delegate void OnPlayerKilledCallBack(string player, string source );
    public OnPlayerKilledCallBack onPlayerKilledCallBack;
    

    public static GameManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            return;
        }
        
        Debug.LogError("plus de 1 gameManager dans la scene ");
    }

    private void Update()
    {
        if (!isServer) return;
        
        TimerStart();
    }

    public void TimerStart()
    {
        
        sec -= Time.deltaTime;
        if (sec <= 0.01f)
        { 
            sec = 59.9f;
            min--;
        }
    }
    
    
    public void SetSceneCameraActive(bool isActive)
    {
        if(sceneCamera == null)return;
        
        sceneCamera.SetActive(isActive);
    }
    
    
    

    public static void RegisterPlayer(string netID, Player player)
    {
        string playerId = playerIdPrefix + netID;
        players.Add(playerId, player);
        player.transform.name = playerId;
    }


    public static void UnregisterPlayer(string playerID)
    {
        players.Remove(playerID);
    }

    public static Player GetPlayer(string playerID)
    {
        return players[playerID];
    }

    public static Player[] GetAllPlayers()
    {
        return players.Values.ToArray();
    }
    
     
}
