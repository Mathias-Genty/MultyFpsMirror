using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillFeed : MonoBehaviour
{
    
    [SerializeField]GameObject killFeedItemPrefab;
    
    void Start()
    {
        GameManager.instance.onPlayerKilledCallBack += OnKill;
    }


    public void OnKill(string player, string source)
    {
      GameObject Go = Instantiate(killFeedItemPrefab , transform); 
      Go.GetComponent<KillFeedItem>().SetUp(player, source);
      Destroy(Go, 4f);
    }
}
