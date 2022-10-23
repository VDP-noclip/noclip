using UnityEngine;

public class RealityMovement : MonoBehaviour
{
    [Header("Speed")]
    public float walkSpeed;
    public float sprintSpeed;
    
    private float _moveSpeed;     // speed intensity
    
    [Header("Drag")]
    public float groundDrag;    // ground drag
    
    [Header("Jump")]
    public float jumpForce;     // set jump upward force
    public float jumpCooldown;      // set jump cooldown
    public float airMultiplier;     // set air movement limitation
    private bool _readyToJump;      //

    [Header("Crouch")]
    public float crouchSpeed;
    public float crouchYScale;
    private float _startYScale;
    
    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;     // set the jump key
    public KeyCode sprintKey = KeyCode.LeftShift;   // set the sprint key
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    // set the ground check
    public float playerHeight;
    public LayerMask ground;
    private bool _grounded;
    
    public Transform orientation;

    private float _horizontalInput;
    private float _verticalInput;

    private Vector3 _moveDirection;

    private Rigidbody _rb;      // set the rigidbody

    // player states
    public MovementState state;     // current player state
    public enum MovementState       // define player states
    {
        Walking,
        Sprinting,
        Crouching,
        Air
    }


    // Start is called before the first frame update
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
        
        ResetJump(); // set _readyToJump to "true"

        _startYScale = transform.localScale.y;
    }

    // Update is called once per frame
    private void Update()
    {
        _grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, ground);

        MyInput();
        StateHandler();
        SpeedControl();

        // handle drag
        if (_grounded)
            _rb.drag = groundDrag;
        else
            _rb.drag = 0;
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
            
            Invoke(nameof(ResetJump), jumpCooldown); // continues to jump if space remains pressed
        }
        
        // start crouch
        if (_grounded && Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);   // reduce scale
            _rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);     // push down
        }
        
        // stop crouch
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, _startYScale, transform.localScale.z);
        }
    }


    private void StateHandler()
    {
        // mode - Crouching
        if (_grounded && Input.GetKey(crouchKey))
        {
            state = MovementState.Crouching;
            _moveSpeed = crouchSpeed;
        }
        
        // mode - Sprinting
        else if (_grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.Sprinting;
            _moveSpeed = sprintSpeed;
        }
        
        // mode - Walking
        else if (_grounded)
        {
            state = MovementState.Walking;
            _moveSpeed = walkSpeed;
        }

        // mode - Air
        else
        {
            state = MovementState.Air;
        }
    }

    //
    private void MovePlayer()
    {
        // calculate movement direction
        _moveDirection = orientation.forward * _verticalInput + orientation.right * _horizontalInput;
        
        // differentiate movement on the ground and in air
        if(_grounded)
            _rb.AddForce(_moveSpeed * 10f * _moveDirection.normalized, ForceMode.Force);
        else if(!_grounded)
            _rb.AddForce(_moveSpeed * airMultiplier * 10f * _moveDirection.normalized, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        
        // limit velocity if needed
        if (flatVel.magnitude > _moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * _moveSpeed;
            _rb.velocity = new Vector3(limitedVel.x, _rb.velocity.y, limitedVel.z); // apply the new velocity to rb
        }
    }

    private void Jump()
    {
        // reset y velocity
        _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        
        // create an upward impulse force
        _rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        _readyToJump = true;
    }
}
