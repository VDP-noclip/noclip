using System;
using UnityEngine;

public class RealityMovement : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float _walkSpeed = 6f;
    [SerializeField] private float _sprintSpeed = 10f;
    
    private float _moveSpeed;     // speed intensity
    
    [Header("Drag")]
    [SerializeField] private float _groundDrag = 4f;    // ground drag
    
    [Header("Jump")]
    [SerializeField] private float _jumpForce = 8f;     // set jump upward force
    [SerializeField] private float _jumpCooldown = 0.25f;      // set jump cooldown
    [SerializeField] private float _airMultiplier = 0.4f;     // set air movement limitation
    private bool _readyToJump;      //

    [Header("Crouch")]
    [SerializeField] private float _crouchSpeed = 2f;
    [SerializeField] private float _crouchYScale = 0.5f;
    private float _startYScale;
    
    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;     // set the jump key
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;   // set the sprint key
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    // set the ground check
    [SerializeField] private float _playerHeight = 2f;
    [SerializeField] private LayerMask _ground;
    private bool _grounded;
    
    [SerializeField] private Transform _orientation;

    private float _horizontalInput;
    private float _verticalInput;

    private Vector3 _moveDirection;

    private Transform _transform;
    private Rigidbody _rigidbody;      // set the rigidbody

    // player states
    [SerializeField] private MovementState _state;     // current player state
    public enum MovementState       // define player states
    {
        Walking,
        Sprinting,
        Crouching,
        Air
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        _rigidbody.freezeRotation = true;
        
        ResetJump(); // set _readyToJump to "true"

        _startYScale = _transform.localScale.y;
    }

    // Update is called once per frame
    private void Update()
    {
        _grounded = Physics.Raycast(_transform.position, Vector3.down, _playerHeight * 0.5f + 0.2f, _ground);

        MyInput();
        StateHandler();
        SpeedControl();

        // handle drag
        if (_grounded)
            _rigidbody.drag = _groundDrag;
        else
            _rigidbody.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
        Debug.Log(_moveSpeed);
    }

    // 
    private void MyInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
        
        // when to jump
        if (Input.GetKey(jumpKey) && _readyToJump && _grounded)
        {
            _readyToJump = false;
            
            Jump();
            
            Invoke(nameof(ResetJump), _jumpCooldown); // continues to jump if space remains pressed
        }
        
        // start crouch
        if (_grounded && Input.GetKeyDown(crouchKey))
        {
            _transform.localScale = new Vector3(_transform.localScale.x, _crouchYScale, _transform.localScale.z);   // reduce scale
            _rigidbody.AddForce(Vector3.down * 5f, ForceMode.Impulse);     // push down
        }
        
        // stop crouch
        if (Input.GetKeyUp(crouchKey))
        { 
            _transform.localScale = new Vector3(_transform.localScale.x, _startYScale, _transform.localScale.z);
        }
    }


    private void StateHandler()
    {
        // mode - Crouching
        if (_grounded && Input.GetKey(crouchKey))
        {
            _state = MovementState.Crouching;
            _moveSpeed = _crouchSpeed;
        }
        
        // mode - Sprinting
        else if (_grounded && Input.GetKey(sprintKey))
        {
            _state = MovementState.Sprinting;
            _moveSpeed = _sprintSpeed;
        }
        
        // mode - Walking
        else if (_grounded)
        {
            _state = MovementState.Walking;
            _moveSpeed = _walkSpeed;
        }

        // mode - Air
        else
        {
            _state = MovementState.Air;
        }
    }

    //
    private void MovePlayer()
    {
        // calculate movement direction
        _moveDirection = _orientation.forward * _verticalInput + _orientation.right * _horizontalInput;
        
        // differentiate movement on the ground and in air
        if(_grounded)
            _rigidbody.AddForce(_moveSpeed * 10f * _moveDirection.normalized, ForceMode.Force);
        else if(!_grounded)
            _rigidbody.AddForce(_moveSpeed * _airMultiplier * 10f * _moveDirection.normalized, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);
        
        // limit velocity if needed
        if (flatVel.magnitude > _moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * _moveSpeed;
            _rigidbody.velocity = new Vector3(limitedVel.x, _rigidbody.velocity.y, limitedVel.z); // apply the new velocity to rb
        }
    }

    private void Jump()
    {
        // reset y velocity
        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);
        
        // create an upward impulse force
        _rigidbody.AddForce(_transform.up * _jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        _readyToJump = true;
    }
}
