using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpWeapon : MonoBehaviour
{

    [SerializeField] private WeaponData theWeapon;
    
    private GameObject pickUpGraphics;
    
    void Start()
    {
        
        ResetWeapon();
        
    }

    void ResetWeapon()
    {

        pickUpGraphics = Instantiate(theWeapon.graphics, transform);
        
        pickUpGraphics.transform.position = transform.position;
        



    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            WeaponManager weamponManager = other.GetComponent<WeaponManager>();
            EquipNewWeapon(weamponManager);
            


        }
    }

    void EquipNewWeapon(WeaponManager weamponManager)
    {
        
        Destroy(weamponManager.GetCurrentWeaponGraphics().gameObject);
        
        weamponManager.EquipWeapon(theWeapon);
        
        Destroy(pickUpGraphics);
        
        
    }
    
    
}
