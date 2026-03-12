using UnityEngine;
using Mirror;
using System.Collections;

public class WeaponManager : NetworkBehaviour
{
    
    [SerializeField] private WeaponData primaryWeapon;
    [SerializeField] private WeaponData secondaryWeapon;

    private GameObject weaponIns;

    private WeaponData currentWeapon;
    private weaponGraphics currentGraphics;
    
    [SerializeField] private string WeaponLayerName = "Weapon";
    
    
    [SerializeField] private Transform weaponHolder;
    
    [HideInInspector]
    public int CurrentMagazineSize;
     
    public bool isReloading = false;
    
    void Start()
    {
        EquipWeapon(primaryWeapon);
    }

    public WeaponData GetCurrentWeapon()
    {
        return currentWeapon;
        
    }
    
    public weaponGraphics GetCurrentWeaponGraphics()
    {
        return currentGraphics;
        
    }

    public void EquipWeapon(WeaponData _weapon)
    {
        currentWeapon = _weapon;
        CurrentMagazineSize = _weapon.MagazineSize;
        
        weaponIns =  Instantiate(_weapon.graphics, weaponHolder.position, weaponHolder.rotation);
        weaponIns.transform.SetParent(weaponHolder);

        currentGraphics = weaponIns.GetComponent<weaponGraphics>();

        if (currentGraphics == null)
        {
            Debug.LogError("pas de scripts weapon graphics sur l'arme : " + weaponIns.name);
        }
        
        if (isLocalPlayer)
        {
            Util.SetLayerRecursively(weaponIns, LayerMask.NameToLayer(WeaponLayerName));
        }
        
        
    }

    public void changeWeapon()
    {
        
        if(!isLocalPlayer)return;
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
        Destroy(weaponIns);
        
        if (currentWeapon.isPrimary)
        {
            EquipWeapon(secondaryWeapon);
        }
        else
        {
            EquipWeapon(primaryWeapon);
        }
        
    }

    public IEnumerator Reload()
    {
        if (isReloading)yield break;
        
        Debug.Log("reloading...");
        
        isReloading = true;
        CmdOnReload();
        yield return new WaitForSeconds(currentWeapon.reloadTime);

        CurrentMagazineSize = currentWeapon.MagazineSize;
        
        Debug.Log("reloading finished");
        
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
