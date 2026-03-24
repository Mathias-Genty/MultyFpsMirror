using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NewNetworkManager : NetworkManager
{
 
    public class MyNetworkManager : NetworkManager
    {
    
    }
    
public override void OnServerAddPlayer(NetworkConnection conn)
{
    
}
    
void Update()
{
    if (!NetworkServer.active) return;

    if (Input.GetKeyDown(KeyCode.P))
    {
        SpawnPlayers();
    }
}


public void SpawnPlayers()
{
    foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
    {
        GameObject player = Instantiate(playerPrefab);

        NetworkServer.AddPlayerForConnection(conn, player);
    }
}





}
