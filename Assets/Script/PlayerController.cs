using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(Animator))]

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;    
    [SerializeField]
    private float MouseSensitivity = 3f;
    
    [SerializeField]
    private float thrusterForce = 1000f;
    
    [SerializeField] private float thrusterFuelBurnSpeed = 1f;
    [SerializeField] private float thrusterFuelRegenSpeed = 0.3f;
    [SerializeField] private float thrusterFuelAmount = 1f;

    public float GetThrusterFuelAmount()
    {
        return thrusterFuelAmount;
    }

    [Header("joint Options")]
    [SerializeField] private float jointSpring = 20f;
    [SerializeField] private float jointMaxForce = 50f;
    
    
    
    private PlayerMotor motor;
    private ConfigurableJoint joint;
    private Animator animator;

    private WeaponManager weaponManager;

    
    void Start()
    {
        
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();
        animator = GetComponent<Animator>();
        weaponManager = GetComponent<WeaponManager>();
        
        SetJointSettings(jointSpring);

    }

    private void Update()
    {
        if (PauseMenu.isOn)
        {
            
            if (Cursor.lockState != CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            
            motor.Move(Vector3.zero);
            motor.Rotate(Vector3.zero);
            motor.RotateCamera(0f);
            motor.ApplyThruster(Vector3.zero);
            
            return;
        }

        if (Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        RaycastHit _hit;
        if (Physics.Raycast(transform.position, Vector3.down, out _hit, 100f))
        {
            joint.targetPosition = new Vector3(0f, -_hit.point.y, 0f);
        }
        else
        {
            joint.targetPosition = new Vector3(0f, 0f, 0f);
        }
        
        
        float xMov = Input.GetAxis("Horizontal");
        float zMov = Input.GetAxis("Vertical");
        
        Vector3 movementHorizontal = transform.right * xMov;
        Vector3 movementVertical = transform.forward * zMov;
        
        Vector3 velocity = (movementHorizontal + movementVertical) * speed;
        
        animator.SetFloat("ForwardVelocity", -zMov);
        
        motor.Move(velocity);

        
        
        
        float xRot = Input.GetAxisRaw("Mouse X");
        
        Vector3 rotation = new Vector3(0,xRot,0) * MouseSensitivity;
        
        motor.Rotate(rotation);
        
        
        
        float yRot = Input.GetAxisRaw("Mouse Y");
        
        float camRotationX = yRot * MouseSensitivity;
        
        motor.RotateCamera(camRotationX);

        Vector3 thrusterVelocity = Vector3.zero;
        
        if (Input.GetButton("Jump") && thrusterFuelAmount > 0)
        {
            thrusterFuelAmount -= thrusterFuelBurnSpeed *  Time.deltaTime;

            if (thrusterFuelAmount >= 0.01f)
            {
            thrusterVelocity =  Vector3.up * thrusterForce;
            SetJointSettings(0f);
            }
        }
        else
        {
            SetJointSettings(jointSpring);
            
            thrusterFuelAmount += thrusterFuelRegenSpeed * Time.deltaTime;
        }

        thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount, 0f, 1f);
        
        motor.ApplyThruster(thrusterVelocity);

        
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        
        if (scrollWheel != 0)
        {
           weaponManager.changeWeapon();
        } 
    
        
    }

    private void SetJointSettings(float _jointSpring)
    {
        joint.yDrive = new JointDrive { positionSpring = _jointSpring, maximumForce = jointMaxForce };
    }
    
    
    
    
}
