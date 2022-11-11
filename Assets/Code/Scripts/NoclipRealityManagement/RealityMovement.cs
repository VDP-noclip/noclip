using System;
using UnityEngine;

public class RealityMovement : MonoBehaviour
{
    private enum MovementState       // define player states
    {
        Walking,
        Sprinting,
        Crouching,
        Air
    }
    
    [Header("Speed")]
    [SerializeField] private float _runSpeed = 6f;
    [SerializeField] private float _walkSpeed = 3f;
    
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
    
    [Header("Slope handling")]
    [SerializeField] private float _maxSlopeAngle;
    private RaycastHit _slopeHit;
    private bool _exitingOnSlope = false;
    
    [Header("Keybinds")]
    [SerializeField] private KeyCode _jumpKey = KeyCode.Space;     // set the jump key
    [SerializeField] private KeyCode _sprintKey = KeyCode.LeftShift;   // set the sprint key
    [SerializeField] private KeyCode _crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    // set the ground check
    
    // stefanofossati comment: I prefer to have a groundCheck object instead of the player height
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _playerHeight = 2f;  // Actually this isn't used
    [SerializeField] private LayerMask _ground;
    private bool _grounded;

    [SerializeField] private float _gravityMultiplier = 1f;
    [SerializeField] private Transform _orientation;

    [SerializeField] private float _gravity = 9.81f;  // This is used for the movement force //Questo peccato al cospetto di Dio va fixato
    private float _horizontalInput;
    private float _verticalInput;

    private Vector3 _moveDirection;

    private Transform _transform;
    private Rigidbody _rigidbody;      // set the rigidbody

    // It's true if is the realbody, it's false if is the noclip body
    private bool _currentPlayerBody  = true;

    // player states
    [SerializeField] private MovementState _state;     // current player state

    [SerializeField] private bool _OnSlope; //This seriazlized field is only use to debug from unity // TODO remove
    [SerializeField] private bool Grounded; //This seriazlized field is only use to debug from unity // TODO remove
    
    private void Awake()
    {
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        //if gravity magnitude is not 0
        if (gameManager.GetGravity() == 0)
        {
            //set gravity to gravity magnitude
            Physics.gravity *= _gravityMultiplier;
            gameManager.SetGravity(Physics.gravity.magnitude);
        }

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
        if (_currentPlayerBody)
        {
            //Alternative 1
            //_grounded = Physics.Raycast(_transform.position,  Vector3.down, _playerHeight * 0.5f + 0.3f, _ground);
        
            //Alternative 2
            // ATTENTION!!! the 0.48f value is based on the player capsule radius
            _grounded = Physics.CheckSphere(_groundCheck.position,  0.48f, _ground);
        
            // stefanofossati comment: I Think that this second alternative is a little more precise for the slopes
            _OnSlope = OnSlope(); // used to debug // TODO remove
            Grounded = _grounded; // used to debug // TODO remove
        
            UserInput();
            StateHandler();
            MovePlayer();
            SpeedControl();

            // handle drag
            if (_grounded)
                _rigidbody.drag = _groundDrag;
            else
                _rigidbody.drag = 0;
        }
    }

    public void ResetSpeedOnRespawn()
    {
        _rigidbody.velocity = Vector3.zero;
    }
    
    public void ActivatePlayer(bool active)
    {
        _currentPlayerBody = active;
    }

    // 
    private void UserInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
        
        // when to jump
        if (Input.GetKey(_jumpKey) && _readyToJump && _grounded)
        {
            _readyToJump = false;
            
            Jump();
            
            Invoke(nameof(ResetJump), _jumpCooldown); // continues to jump if space remains pressed
        }
        
        // start crouch
        if (_grounded && Input.GetKeyDown(_crouchKey))
        {
            _transform.localScale = new Vector3(_transform.localScale.x, _crouchYScale, _transform.localScale.z);   // reduce scale
            _rigidbody.AddForce(Vector3.down * 5f, ForceMode.Impulse);     // push down
        }
        
        // stop crouch
        if (Input.GetKeyUp(_crouchKey))
        { 
            _transform.localScale = new Vector3(_transform.localScale.x, _startYScale, _transform.localScale.z);
        }
    }


    private void StateHandler()
    {
        // mode - Crouching
        if (_grounded && Input.GetKey(_crouchKey))
        {
            _state = MovementState.Crouching;
            _moveSpeed = _crouchSpeed;
        }
        
        // mode - Sprinting
        else if (_grounded && Input.GetKey(_sprintKey))
        {
            _state = MovementState.Sprinting;
            _moveSpeed = _walkSpeed;
        }
        
        // mode - Walking
        else if (_grounded)
        {
            _state = MovementState.Walking;
            _moveSpeed = _runSpeed;
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

        // onslope
        if (OnSlope() && !_exitingOnSlope)
        {
            // Add a force on the plane direction of the plane
            _rigidbody.AddForce(GetSlopeMoveDirection() * (_moveSpeed * _gravity), ForceMode.Force); 
            
            // if _rigidbody.velocity.y != 0 and w a s or d pressed
            if (_rigidbody.velocity.y != 0 &&(_horizontalInput != 0 || _verticalInput != 0))
            {
                // Add a force that obliged the player to stay on the inclined plane 
                _rigidbody.AddForce(Vector3.down * (_gravity * _gravityMultiplier * 4), ForceMode.Force);  
            }
        }
        // differentiate movement on the ground and in air
        if (_grounded)
            _rigidbody.AddForce(_moveSpeed * _gravity * _moveDirection.normalized, ForceMode.Force);
        else
            _rigidbody.AddForce(_moveSpeed * _airMultiplier * _gravity * _moveDirection.normalized, ForceMode.Force);

        _rigidbody.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        if (OnSlope() && !_exitingOnSlope)
        {
            if (_rigidbody.velocity.magnitude > _moveSpeed)
            {
                _rigidbody.velocity = _rigidbody.velocity.normalized * _moveSpeed;
            }
        }
        else
        {
            Vector3 flatVel = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);
        
            // limit velocity if needed
            if (flatVel.magnitude > _moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * _moveSpeed;
                _rigidbody.velocity = new Vector3(limitedVel.x, _rigidbody.velocity.y, limitedVel.z); // apply the new velocity to rb
            }
        }
        
    }

    private void Jump()
    {
        
        _exitingOnSlope = true;
        // reset y velocity
        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);
        
        // create an upward impulse force
        _rigidbody.AddForce(_transform.up * _jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        _readyToJump = true;
        
        _exitingOnSlope = false;
    }

    //This function check if the plane is inclined
    private bool OnSlope()
    {
        if (Physics.Raycast(_groundCheck.position, Vector3.down, out _slopeHit, 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            return angle < _maxSlopeAngle && angle != 0 && _state!=MovementState.Air;
        }

        return false;
    }

    //This function retuns the vector3 that represents the plane on which the player is moving
    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(_moveDirection, _slopeHit.normal).normalized;
    }
}
