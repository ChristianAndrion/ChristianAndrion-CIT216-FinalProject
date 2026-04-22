using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    
    public HealthBarScript healthBar;
    public HealthBarScript staminaBar;

    public float moveSpeed = 5.0f;
    public float sprintSpeed = 7.0f;
    public float rotationSmoothTime = 0.12f;

    public float SpeedChangeRate = 10.0f;
    
    private GameObject _mainCamera;
    private CharacterController _controller;
    private PlayerInput _playerInput;

    private float _speed;
    private float _targetRotation;
    private float _rotationVelocity;

    //Input System
    private Vector2 _moveInput;
    private bool _sprinting = false;

    private UnityAction<int> DamageListener;

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
    }
    private void FixedUpdate()
    {
        Move();
    }

    private void OnEnable()
    {
        DamageListener = new UnityAction<int>(PlayerDamage);// Delegate points to function that handles event
        EventManager.StartListening("PlayerDamager", DamageListener); //Like and subscribe to an event
    }

    private void OnDisable()
    {
        EventManager.StopListening("PlayerDamager", DamageListener);
    }

    private void OnMove(InputValue moveVal)
    {
        _moveInput = moveVal.Get<Vector2>();
    }

    private void OnAttack(InputValue AttackVal)
    {
        //healthBar.UpdateBarValue(10);
    }

    private void Move()
    {
        //Code modified from Unity Starter 3D character movement script

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

        float targetSpeed = _sprinting ? sprintSpeed : moveSpeed;
         
        //Gather player input
        if (_moveInput == Vector2.zero) targetSpeed = 0.0f;
        
        _speed = targetSpeed;
        
        Vector3 inputDirection = new Vector3(_moveInput.x,0.0f, _moveInput.y).normalized;

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

    private void PlayerDamage(int amt)
    {
        healthBar.UpdateBarValue(amt);
    }

}
