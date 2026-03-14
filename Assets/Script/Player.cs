using System;
using Mirror;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour
{
    [SyncVar]
    private bool _isDead = false;

    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    [SerializeField]private float MaxHealth = 100f;
    
    [SyncVar]private float CurrentHealth;

    public float GetHealthPct()
    {
        return (float)CurrentHealth / MaxHealth;
    }
    
    [SyncVar]
    public string username ="Player";
    

    [SerializeField]
    private Behaviour[] disableOnDeath;
    [SerializeField]
    private GameObject[] disableGameObjectOnDeath;
    
    private bool[] wasEnabledOnStart;
    
    [SerializeField]private GameObject deathEffect;
    [SerializeField] private GameObject spawnEffect;
    
    bool firstSetup = true;

    public int Kills;
    public int Deaths;

    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip deathSound;
    
    public void SetUp()
    {
        if (isLocalPlayer)
        {
        GameManager.instance.SetSceneCameraActive(false);
        GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
        }
        
        CmdBroadCastNewPlayerSetUp();
        
    }

    [Command(ignoreAuthority =  true)]
    private void CmdBroadCastNewPlayerSetUp()
    {
        RpcSetUpPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetUpPlayerOnAllClients()
    {
        if (firstSetup)
        {
            wasEnabledOnStart = new bool[disableOnDeath.Length];
        
            for (int i = 0; i < disableOnDeath.Length; i++)
            {
                wasEnabledOnStart[i] = disableOnDeath[i].enabled;
            }
        firstSetup = false;
        
        }
        
        SetDefault();
    }
    
    public void SetDefault()
    {
        isDead = false;
        
        CurrentHealth = MaxHealth;
        
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabledOnStart[i];
        }
        
        for (int i = 0; i < disableGameObjectOnDeath.Length; i++)
        {
            disableGameObjectOnDeath[i].SetActive(true);
            
        }
        
        
        
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }
        

        
        GameObject GfxIns = Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(GfxIns, 3f);
        
    }

    void Update()
    {
        if(!isLocalPlayer)return;
        
        if (Input.GetKey(KeyCode.K))
        {
            RPCTakeDamage(10 , "joueur");
            
        }
    }
    
    
    
    
    
    [ClientRpc]
    public void RPCTakeDamage(float amount , string sourceID)
    {
        
        if (isDead) return;
        
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(hitSound);
        
        
        
        CurrentHealth -= amount;
        Debug.Log(transform.name + "a mtn : " + CurrentHealth + " hp");

        if (CurrentHealth <= 0f)
        {
            Die(sourceID);
            audioSource.PlayOneShot(deathSound, 0.5f);
            
            
        }
    }

    private void Die(string sourceID)
    {
        _isDead = true;
        
        
        Player sourcePlayer = GameManager.GetPlayer(sourceID);
        if (sourcePlayer != null)
        {
            sourcePlayer.Kills++;
            GameManager.instance.onPlayerKilledCallBack.Invoke(username,  sourcePlayer.username);
        }
        
        
        Deaths++;
        

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
            
        }
        
        for (int i = 0; i < disableGameObjectOnDeath.Length; i++)
        {
            disableGameObjectOnDeath[i].SetActive(false);
            
        }
        
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }
        
        
        GameObject GfxIns = Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(GfxIns, 3f);

        if (isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
        }
        
        
        Debug.Log(transform.name + "est eliminer");

        StartCoroutine(Respawn());
        
        
        
    }


    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSetting.respawnTimer);
        
        
        
        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;
        
        yield return new WaitForSeconds(0.1f);
        
        SetUp();
    }
}
