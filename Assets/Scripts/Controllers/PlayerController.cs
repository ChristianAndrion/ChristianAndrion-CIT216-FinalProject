using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    
    public HealthBarScript healthBar;
    public HealthBarScript staminaBar;
    public GameObject hitbox;

    public Animator anim;

    public float moveSpeed = 5.0f;
    public float sprintSpeed = 7.0f;
    public float rotationSmoothTime = 0.12f;

    public float SpeedChangeRate = 10.0f;
    
    private GameObject _mainCamera;
    private CharacterController _controller;
    private PlayerInput _playerInput;
    private HurtboxScript hurtbox;
    

    private float _speed;
    private float _targetRotation;
    private float _rotationVelocity;

    //Input System
    private Vector2 _moveInput;
    private bool _sprinting = false;
    private bool _dashing = false;

    private UnityAction<int,GameObject> DamageListener;

    private void Awake()
    {
        // get a reference to our main camera
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();
        hurtbox = GetComponent<HurtboxScript>();
    }
    private void FixedUpdate()
    {
        Move();
    }

    private void OnEnable()
    {
        DamageListener = new UnityAction<int,GameObject>(PlayerDamage);// Delegate points to function that handles event
        EventManager.StartListening("Damager", DamageListener); //Like and subscribe to an event
    }

    private void OnDisable()
    {
        EventManager.StopListening("Damager", DamageListener);
    }

    //New input system move input
    private void OnMove(InputValue moveVal)
    {
        _moveInput = moveVal.Get<Vector2>();
    }

    //New Input system attack input
    private void OnAttack(InputValue AttackVal)
    {
        Debug.Log("Attacking");
        anim.SetTrigger("isAttacking");
        EnableHitbox();
    }

    void EnableHitbox()
    {
        hitbox.SetActive(true);
        Invoke("DisableHitbox", 0.5f);
    }

    void DisableHitbox()
    {
        hitbox.SetActive(false);
    }

    private void OnDash(InputValue DashVal)
    {
        if (staminaBar.BarValue > 30)
        {
            staminaBar.UpdateBarValue(30);
            Dashing();
        }
    }

    void Dashing()
    {
        _dashing = true;
        anim.SetBool("isDashing", _dashing);
        hurtbox.enabled = _dashing;
        Invoke("EndDash",0.75f);
    }

    void EndDash()
    {
        _dashing = false;
        anim.SetBool("isDashing", _dashing);
        hurtbox.enabled = _dashing;

    }

    //Handle player movement via modified Unity Starter 3D character movement script
    private void Move()
    {

        float isSprintHeld = _playerInput.actions["Sprint"].ReadValue<float>();

        // Handle sprint logic
        if ((isSprintHeld > 0) && (staminaBar.BarValue > 0))
        {
            staminaBar.UpdateBarValue(1);
            _sprinting = true;
        }
        else
        {
            if (staminaBar.BarValue < staminaBar.maxBarValue)
                staminaBar.UpdateBarValue(-1);
            _sprinting = false;
        }
        if (_dashing != true)
            anim.SetBool("isDashing",_sprinting);                   

        //Check if sprinting
        float targetSpeed = _sprinting ? sprintSpeed : moveSpeed;
         
        //Gather player input
        if (_moveInput == Vector2.zero) targetSpeed = 0.0f;
        
        _speed = targetSpeed;
        
        Vector3 inputDirection = new Vector3(_moveInput.x,0.0f, _moveInput.y).normalized;

        //No move input
        if(_moveInput != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                rotationSmoothTime);

            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);            
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;


        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime));
    }

    private void PlayerDamage(int amt, GameObject obj)
    {
        if (obj == gameObject)
        {
            healthBar.UpdateBarValue(amt);
            Debug.Log("Player Damaged, Health: " + healthBar.BarValue);
        }
    }

}
