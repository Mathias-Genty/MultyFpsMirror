using System;
using UnityEngine;

public class CameraFacingBillboard : MonoBehaviour
{
    private Camera cam;
    private void Start()
    {
        cam = Camera.main;
    }


    void LateUpdate()
    {

        if (cam != null)
        {
            transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
        }
            
        
    }
    
    
}
