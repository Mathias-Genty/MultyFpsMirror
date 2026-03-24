using System;
using System.Collections;
using UnityEngine;
using Mirror;


[RequireComponent(typeof(WeaponManager))]
public class playerShoot : NetworkBehaviour
{
    
    
   
    
    [SerializeField] private Camera cam;
    [SerializeField]private LayerMask mask;
    [SerializeField] private GameObject bulletHole;
    private WeaponData currentWeapon;
    private WeaponManager weaponManager;
    
    private bool hasDelay = false;
    
    
    

    
    void Start()
    {
        if (cam == null)
        {
            Debug.logger.LogError("pas de Camera pour tirer ");
            this.enabled = false;
        }
        
        weaponManager = GetComponent<WeaponManager>();
    }

    private void Update()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();
        
        if(PauseMenu.isOn)return;

        if (Input.GetKeyDown(KeyCode.R) && weaponManager.CurrentMagazineSize < currentWeapon.MagazineSize)
        {
            StartCoroutine(weaponManager.Reload());
            return;
        }

        if (currentWeapon.fireRate <= 0)
        {
            
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot", 0f,1f / currentWeapon.fireRate);
            }
            else if(Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }
        
    }

    [Command]
    void CmdOnHit(Vector3 pos, Vector3 normal, bool player)
    {
        RpcDoHitEffect(pos, normal, player);
    }
    
    [ClientRpc]
    void RpcDoHitEffect(Vector3 pos, Vector3 normal, bool player)
    {

        if (!player)
        { 
            Quaternion rot = Quaternion.LookRotation(normal);
            
            float randomZ = UnityEngine.Random.Range(0f, 360f);

            rot *= Quaternion.Euler(0f, 0f, randomZ);
            
            GameObject holeEffect = Instantiate(bulletHole, pos + normal * 0.01f, rot);
            
            Destroy(holeEffect, 5f);
            
        }
        GameObject hitEffect = Instantiate(weaponManager.GetCurrentWeaponGraphics().hitEffectPrefab, pos, Quaternion.LookRotation(normal));
        Destroy(hitEffect, 2f);
        
        
        
        
    }

    [Command]
    void CmdOnShoot()
    {
        RpcDoShootEffects();
    }

    [ClientRpc]
    void RpcDoShootEffects()
    {
        
        weaponManager.GetCurrentWeaponGraphics().muzzleFlash.Play();
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(currentWeapon.shootSound);
        
    }

    [Client]
    private void Shoot()
    {
        
        if (!isLocalPlayer || weaponManager.isReloading)return;

        if (currentWeapon.fireRate <= 0)
        {
            if (hasDelay)
            {
                return;
            }
            else
            {
                StartCoroutine(FireDelay());
            }
        }

        if (weaponManager.CurrentMagazineSize <= 0)
        {
            StartCoroutine(weaponManager.Reload());
            return;
        }
        
        
        
        weaponManager.CurrentMagazineSize--;
        
        //Debug.Log("on a " + weaponManager.CurrentMagazineSize);

        CmdOnShoot();
        
        RaycastHit hit;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, currentWeapon.range,mask))
        {
            //Debug.Log("objet toucher" + hit.collider.name);
            if (hit.collider.tag == "Player")
            {
                CmdPlayerShot(hit.collider.name, currentWeapon.damage , transform.name);
                CmdOnHit(hit.point, hit.normal, true);
            }
            else
            {
            CmdOnHit(hit.point, hit.normal, false);
                
            }
            
            
            
            //Debug.Log(hit.collider.name);
        }
        
        if (weaponManager.CurrentMagazineSize <= 0)
        {
            StartCoroutine(weaponManager.Reload());
            return;
        }
        
    }

    [Command]
    private void CmdPlayerShot(string playerId , float damage , string sourceID)
    {
        //Debug.Log(playerId +  " has been shot");
        
        Player player = GameManager.GetPlayer(playerId);

        player.RPCTakeDamage(damage, sourceID);
        
        
    }

    private IEnumerator FireDelay()
    {
        hasDelay = true;
        yield return new WaitForSeconds(currentWeapon.fireDelay);
        hasDelay = false;
        
    }
    
}
