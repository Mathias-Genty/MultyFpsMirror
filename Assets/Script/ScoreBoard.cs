using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScoreBoard : MonoBehaviour
{
    [SerializeField]GameObject playerScoreBoardItem;
    [SerializeField]Transform playerScoreBoardList;
    
    private void OnEnable()
    {
        Player[] players = GameManager.GetAllPlayers();
        
        players = players.OrderByDescending(p => p.Kills).ThenBy(p => p.Deaths).ToArray();

        
        foreach (Player player in players)
        {
            
            GameObject ItemGo = Instantiate(playerScoreBoardItem, playerScoreBoardList);
            PlayerScoreBoardItem item = ItemGo.GetComponent<PlayerScoreBoardItem>();

            if (item != null)
            {
                
                item.SetUp(player);
            }
            
        }

    }

    private void OnDisable()
    {

        foreach (Transform child in playerScoreBoardList)
        {
            Destroy(child.gameObject);
        }
        
    }
    
    
    
    
    

}
