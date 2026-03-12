using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillFeedItem : MonoBehaviour
{

    [SerializeField] private Text killFeedText;

    public void SetUp(string player, string source)
    {
        killFeedText.text = source + " Killed " +  player;
    }
}
