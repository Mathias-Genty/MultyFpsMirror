using UnityEngine;
using Mirror;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(Player))]
public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] conponentsToDisable;

    [SerializeField] private string remoteLayerName = "RemotePlayer";
    
    [SerializeField] private string DontDrawLayerName = "DontDraw";
    [SerializeField] private GameObject[] playerGraphics;
    
    [SerializeField] private GameObject playerUIPrefab;
    
    [HideInInspector]
    public GameObject playerUIInstance;
    
    
    private void Start()
    {
        if (!isLocalPlayer)
        {
            DisableComponents();
            AsignRemoteLayer();
        }
        else
        {
     

            SetLayerRecursively( LayerMask.NameToLayer(DontDrawLayerName));
            
            playerUIInstance = Instantiate(playerUIPrefab);
            
            PlayerUi ui = playerUIInstance.GetComponent<PlayerUi>();

            if (ui == null)
            {
                Debug.LogError("PlayerUI pas trouver");
            }
            else
            {
                ui.SetPlayer(GetComponentInChildren<Player>());
                GetComponent<Player>().SetUp();

                
                
                CmdSetusername(transform.name, UserAcountManager.LoggedInUsername);
            }
            
        }
        
        
        
    }

    [Command]
    void CmdSetusername(string playerId, string username)
    {
        Player player = GameManager.GetPlayer(playerId);
        if (player != null)
        {
            player.username = username;
        }
        
        
    }

    private void SetLayerRecursively(int newLayer)
    {
        for (int i = 0; i < playerGraphics.Length; i++)
        {
            playerGraphics[i].layer = newLayer;
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        string netID = GetComponent<NetworkIdentity>().netId.ToString();
        Player player = GetComponent<Player>();
        
        
        GameManager.RegisterPlayer(netID, player);
        
        
    }


    private void DisableComponents()
    {
        for (int i = 0; i < conponentsToDisable.Length; i++)
        {
            conponentsToDisable[i].enabled = false;
        }
        
        ConfigurableJoint joint = GetComponent<ConfigurableJoint>();

        if (joint != null)
        {
            joint.yDrive = new JointDrive
            {
                positionSpring = 0,
                maximumForce = 0
            };
        }
    }

    private void AsignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }
    
    private void OnDisable()
    {

        Destroy(playerUIInstance);

        if (isLocalPlayer)
        {
        GameManager.instance.SetSceneCameraActive(true);
        }
        
        GameManager.UnregisterPlayer(transform.name);
        
        
        
    }
    
    
    
    
    
}
