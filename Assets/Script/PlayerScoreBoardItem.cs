using UnityEngine;
using UnityEngine.UI;


public class PlayerScoreBoardItem : MonoBehaviour
{

    [SerializeField]Text userNameText;
    [SerializeField]Text killsText;
    [SerializeField]Text DeathsText;

    public void SetUp(Player player)
    {
        
        userNameText.text = player.username;
        killsText.text = "Kills :" + player.Kills;
        DeathsText.text = "Deaths :" + player.Deaths;
    }
    
}
