using UnityEngine;
using Mirror;
using System.Collections;

public class WeaponManager : NetworkBehaviour
{
    
    [SerializeField] private WeaponData primaryWeapon;
    [SerializeField] private WeaponData secondaryWeapon;

    private GameObject primaryWeaponIns;
    private GameObject secondaryWeaponIns;
    private GameObject currentweaponIns;
    

    private WeaponData currentWeapon;
    private weaponGraphics currentGraphics;
    
    [SerializeField] private string WeaponLayerName = "Weapon";
    
    
    [SerializeField] private Transform weaponHolder;
    
    [HideInInspector]
    public int CurrentMagazineSize;
    
    private int primaryMagazineSize;
    private int secondaryMagazineSize;
     
    public bool isReloading = false;
    
    void Start()
    {
        Setup(primaryWeapon);
        Setup(secondaryWeapon);
        
        EquipWeapon(true);
    }

    public void Setup(WeaponData weapon)
    {
        
        if (weapon.isPrimary)
        { 
            Debug.logger.Log("Setup Primary");
            Destroy(primaryWeaponIns);
            primaryWeaponIns =  Instantiate(weapon.graphics, weaponHolder.position, weaponHolder.rotation);
            primaryWeaponIns.SetActive(false);
            primaryMagazineSize = weapon.MagazineSize;
            primaryWeaponIns.transform.SetParent(weaponHolder);
            primaryWeapon = weapon;

        }
        else
        {
            Debug.logger.Log("Setup Secondary");
            Destroy(secondaryWeaponIns);
            secondaryWeaponIns = Instantiate(weapon.graphics, weaponHolder.position, weaponHolder.rotation);
            secondaryWeaponIns.SetActive(false);
            secondaryMagazineSize = weapon.MagazineSize;
            secondaryWeaponIns.transform.SetParent(weaponHolder);
            secondaryWeapon = weapon;
        }
        
    }

    public WeaponData GetCurrentWeapon()
    {
        return currentWeapon;
        
    }
    
    public weaponGraphics GetCurrentWeaponGraphics()
    {
        return currentGraphics;
        
    }

    public void EquipWeapon(bool primary)
    {
        
        primaryWeaponIns.SetActive(false);
        secondaryWeaponIns.SetActive(false);


        if (primary)
        {
            currentWeapon = primaryWeapon;
            currentweaponIns = primaryWeaponIns;
            CurrentMagazineSize = primaryMagazineSize;
        }
        else
        {
            currentWeapon = secondaryWeapon;
            currentweaponIns = secondaryWeaponIns;
            CurrentMagazineSize = secondaryMagazineSize;
        }
        
        currentweaponIns.SetActive(true);

        currentGraphics = currentweaponIns.GetComponent<weaponGraphics>();

        
        if (currentGraphics == null)
        {
            Debug.LogError("pas de scripts weapon graphics sur l'arme : " + currentweaponIns.name);
        }
        
        if (isLocalPlayer)
        {
            Util.SetLayerRecursively(currentweaponIns, LayerMask.NameToLayer(WeaponLayerName));
        }
        
        
    }
    
    public void changeWeapon()
    {
        
        if(!isLocalPlayer)return;
        if(isReloading)return;
        
        CmdChangeWeapon();
        
    }

    [Command]
    void CmdChangeWeapon()
    {
        RpcChangeWeapon();
    }

    [ClientRpc]
    void RpcChangeWeapon()
    {

        bool primary = currentWeapon.isPrimary;
        
        if (primary)
        {
            primaryMagazineSize = CurrentMagazineSize;
        }
        else
        {
            secondaryMagazineSize = CurrentMagazineSize;
        }
        
        EquipWeapon(!primary);
        
        
    }

    public IEnumerator Reload()
    {
        if (isReloading)yield break;
        
        //Debug.Log("reloading...");
        
        isReloading = true;
        CmdOnReload();
        yield return new WaitForSeconds(currentWeapon.reloadTime);

        CurrentMagazineSize = currentWeapon.MagazineSize;
        
        //Debug.Log("reloading finished");
        
        isReloading = false;

    }


    [Command]
    void CmdOnReload()
    {
        RpcOnReload();
    }

    [ClientRpc]
    void RpcOnReload()
    {
        
        Animator anim = currentGraphics.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Reload");
        }
        
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(currentWeapon.reloadSound);
        
    }

    
    
}
