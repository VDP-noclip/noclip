using System;
using UnityEngine;

public class RealityMovement : MonoBehaviour
{

    [Header("Speed")] 
    [Tooltip("Suggestion: Max Run Speed < Run Force Multiplier")]
    [Range(0, 30)]
    [SerializeField] private float _maxRunSpeed = 6f;
    [Range(0, 30)]
    [SerializeField] private float _runForceMultiplier = 10f;
    [Range(0, 25)]
    [SerializeField] private float _maxWalkSpeed = 3f;
    [Range(0, 25)]
    [SerializeField] private float _walkForceMultiplier = 3f;
    
    private float _moveSpeed;     // speed intensity
    private float _maxMoveSpeed;
    
    [Header("Drag")]
    [Range(0, 15)]
    [SerializeField] private float _groundDrag = 4f;    // ground drag
    
    [Header("Jump")]
    [Range(0, 25)]
    [SerializeField] private float _jumpForce = 8f;     // set jump upward force
    [Range(0, 1)]
    [SerializeField] private float _jumpCooldown = 0.25f;      // set jump cooldown
    [Range(0, 1f)]
    [SerializeField] private float _airMultiplier = 0.4f;     // set air movement limitation
    private bool _readyToJump;      //

    [Header("Crouch")]
    [Range(0, 10)]
    [SerializeField] private float _crouchSpeed = 2f;
    [Range(0, 1)]
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

    [Range(0, 10)]
    [SerializeField] private float _gravityMultiplier = 1f;
    [SerializeField] private Transform _orientation;

    private float _gravity = 9.81f;  // This is used for the movement force 
    private float _horizontalInput;
    private float _verticalInput;

    private Vector3 _moveDirection;

    private Transform _transform;
    private Rigidbody _rigidbody;      // set the rigidbody
    private NoclipManager _noclipManager;

    // It's true if is the realbody, it's false if is the noclip body
    private bool _currentPlayerBody  = true;

    // player states
    [SerializeField] private MovementState _state;     // current player state

    [SerializeField] private bool _OnSlope; //This seriazlized field is only use to debug from unity // TODO remove
    [SerializeField] private bool Grounded; //This seriazlized field is only use to debug from unity // TODO remove
    
    private void Awake()
    {
        //if gravity magnitude is not 0 (Gamemanager zeroes it on game boot)
        if (Physics.gravity.magnitude != 0)
        {
            //set gravity to gravity magnitude
            Physics.gravity *= _gravityMultiplier;
        }
        _rigidbody = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();
        _noclipManager = FindObjectOfType<NoclipManager>();
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
        if (!_noclipManager.IsNoclipEnabled())
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

    /// <summary>
    /// This function is called in order to reset the real body velocity when it respawns
    /// </summary>
    public void ResetSpeedOnRespawn()
    {
        _rigidbody.velocity = Vector3.zero;
    }

    public bool IsGrounded()
    {
        return _grounded;
    }

    public float GetVelocity()
    {
        return _rigidbody.velocity.magnitude;
    }

    public MovementState GetState()
    {
        return _state;
    }

    /// <summary>
    /// This function manages the input of the user
    /// </summary>
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


    /// <summary>
    /// This function manages the state of the real body. The body has different movement properties in different states
    /// </summary>
    private void StateHandler()
    {
        // mode - Crouching
        if (_grounded && Input.GetKey(_crouchKey))
        {
            _state = MovementState.Crouching;
            _moveSpeed = _crouchSpeed;
        }
        
        // mode - Walking
        else if (_grounded && Input.GetKey(_sprintKey))
        {
            _state = MovementState.Walking;
            _moveSpeed = _walkForceMultiplier;
            _maxMoveSpeed = _maxWalkSpeed;
        }
        
        // mode - Sprinting
        else if (_grounded)
        {
            _state = MovementState.Sprinting;
            _moveSpeed = _runForceMultiplier;
            _maxMoveSpeed = _maxRunSpeed;
        }

        // mode - Air
        else
        {
            _state = MovementState.Air;
        }
    }

    /// <summary>
    /// This function manages the movement of the player in different situations
    /// </summary>
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
                // Add a force that obliged the player to stay on the inclined plane. The force is perpendicular to the plane
                _rigidbody.AddForce(-_slopeHit.normal * (_gravity * _gravityMultiplier * 4), ForceMode.Force);  
            }
        }
        // differentiate movement on the ground and in air
        if (_grounded)
            _rigidbody.AddForce(_moveSpeed * _gravity * _moveDirection.normalized, ForceMode.Force);
        else
            _rigidbody.AddForce(_moveSpeed * _airMultiplier * _gravity * _moveDirection.normalized, ForceMode.Force);

        _rigidbody.useGravity = !OnSlope();
    }

    /// <summary>
    /// This function manages the velocity of the reality player
    /// </summary>
    private void SpeedControl()
    {
        if (OnSlope() && !_exitingOnSlope)
        {
            if (_rigidbody.velocity.magnitude > _maxMoveSpeed)
            {
                _rigidbody.velocity = _rigidbody.velocity.normalized * _maxMoveSpeed;
            }
        }
        else
        {
            Vector3 flatVel = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);
        
            // limit velocity if needed
            if (flatVel.magnitude > _maxMoveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * _maxMoveSpeed;
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

    /// <summary>
    /// This function check if the plane on which is the reality player is inclined
    /// </summary>
    /// <returns>
    /// Returns a boolean that is true if the plane is inclined
    /// </returns>
    private bool OnSlope()
    {
        if (Physics.Raycast(_groundCheck.position, Vector3.down, out _slopeHit, 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            return angle < _maxSlopeAngle && angle != 0 && _state!=MovementState.Air;
        }

        return false;
    }

    /// <summary>
    /// This function calculates the direction of the slope plane
    /// </summary>
    /// <returns>
    /// eturns the vector3 that represents the plane on which the player is moving
    /// </returns>
    
    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(_moveDirection, _slopeHit.normal).normalized;
    }
}
