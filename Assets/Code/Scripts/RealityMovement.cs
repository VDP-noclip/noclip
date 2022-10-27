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
    private bool _readyToJump;

    [Header("Crouch")]
    [SerializeField] private float _crouchSpeed = 2f;
    [SerializeField] private float _crouchYScale = 0.5f;
    private float _startYScale;
    
    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;     // set the jump key
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;   // set the sprint key
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground check")]
    // set the ground check
    [SerializeField] private float _playerHeight = 2f;
    [SerializeField] private LayerMask _ground;
    private bool _grounded;

    [Header("Slope handling")]
    [SerializeField] public float _maxSlopeAngle;
    private RaycastHit _slopeHit;
    private bool _exitingSlope;

    [SerializeField] private Transform _orientation;
    
    // It's true if is the realbody, it's false if is the noclip body
    private bool _currentPlayerBody = true;
    
    private float _horizontalInput;
    private float _verticalInput;

    private Vector3 _moveDirection;

    private Transform _transform;
    private Rigidbody _rigidbody;      // set the rigidbody
    private NoclipManager _noclipManager;
    private bool _touchingNoclipEnabler;

    // player states
    [SerializeField] private MovementState _state;     // current player state

    //sticking problem with walls was solved by removing collider friction alltogether and reliying only on RealityPlayer's drag, aka how it was probably intended from the beginning
    //TOREMOVE reality body capsule collider to change its material and fix sticking to walls
    //private CapsuleCollider _realityBodyCollider;

    //slope gravity vector3
    private Vector3 _slopeGravity = Vector3.zero;
    private bool _bodyOnSlope = false;
    private bool _groundedOnSlope= false;

    public bool Grounded;
    private enum MovementState       // define player states
    {
        Walking,
        Sprinting,
        Crouching,
        Air
    }
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.AddForce(_slopeGravity, ForceMode.Force);
        _transform = GetComponent<Transform>();
        _noclipManager = GetComponent<NoclipManager>();

        //TOREMOVE
        //_realityBodyCollider = GetComponentInChildren<CapsuleCollider>();
        //set reality body collider material to no_friction material in Assets/Physic Materials
        //_realityBodyCollider.material = (PhysicMaterial) Resources.Load("Physic Materials/no_friction");
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
        Grounded = _grounded;
        if (_currentPlayerBody)
        {
            _grounded = Physics.Raycast(_transform.position, Vector3.down, _playerHeight * 0.5f + 0.2f, _ground);

            UserInput();
            StateHandler();
            SpeedControl();

            // handle drag
            if (_grounded || _groundedOnSlope) //needed to make slopes higher than 45 degrees feel not slippery but to jump there UserInput should be modified
            //IMPORTANT slopes higher than about 80 degrees are fundamentally broken because they enable drag free movement but are still generating binding reaction (reazione vincolare), that is because the function that checks if player is on a slope doesn't return a normal vector if the slope angle is that high.
            //80 degrees slopes and walls are different because there is no way to exploit a wall to accelerate while in air
            {
                _rigidbody.drag = _groundDrag;
                //Frictionless(false);
            }
            else
            {
                _rigidbody.drag = 0;
                //Frictionless(true); //you still can't walk close to walls :-)
            }
            if (CanCallNoclip() && Input.GetKeyDown(KeyCode.E))
            {
                if (_noclipManager.NoclipEnabled)
                {
                    _noclipManager.DisableNoclip();
                }
                else
                {
                    _noclipManager.EnableNoclip();
                }
            }
        }
    }

    /*private void Frictionless(bool frictionless) //TOREMOVE
    {
        if (frictionless)
            _realityBodyCollider.material = (PhysicMaterial) Resources.Load("Physic Materials/no_friction");
        else
            _realityBodyCollider.material = (PhysicMaterial) Resources.Load("Physic Materials/default");
    }*/

    private void FixedUpdate()
    {
        MovePlayer();
        //Debug.Log(_moveSpeed);
    }
    
    public void ActivatePlayer(bool active)
    {
        _currentPlayerBody = active;
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("NoclipEnabler"))
        {
            _touchingNoclipEnabler = true;
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NoclipEnabler"))
        {
            _touchingNoclipEnabler = false;
        }
    }

    // 
    private void UserInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
        
        // when to jump
        if (Input.GetKey(jumpKey) && _readyToJump && _grounded) //_grounded is true up to 45 degrees, so _groundedOnSlope together with slope angle should be checked to jump at higher slopes
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
        
        /* Tizio - Stefano can you tell me how this works?
        // on slope
        if (OnSlope() && !_exitingSlope)
        {
            _rigidbody.AddForce(GetSlopeMoveDirection() * _moveSpeed * 20f, ForceMode.Force);

            if (_rigidbody.velocity.y > 0)
                _rigidbody.AddForce(Vector3.down * 80f, ForceMode.Force);
        }*/
        
        // differentiate movement on the ground and in air
        if (_grounded)
            _rigidbody.AddForce(_moveSpeed * 10f * _moveDirection.normalized, ForceMode.Force);
        else
            _rigidbody.AddForce(_moveSpeed * _airMultiplier * 10f * _moveDirection.normalized, ForceMode.Force);
        
        // turn gravity off while on slope
        //tizio: it is actually better to change the friction instead, to avoid side effects
        //_rigidbody.useGravity = !OnSlope();

        SlopeHandler();
        
    }

    private void SlopeHandler() //works up to 45 degrees because above something strange happens with cosine, ANYWAY player is not supposed to climb such slopes, he's not a steinbock
    {
        if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, _playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            _groundedOnSlope = true;
            //debug angle
            Debug.Log("slope angle: " + angle);
            if(angle > 0 && angle < _maxSlopeAngle && !_bodyOnSlope){
                _bodyOnSlope = true;
                _slopeGravity = _slopeHit.normal * Physics.gravity.magnitude;
                //Debug.Log("_slopeGravity: " + _slopeGravity.magnitude);
                _rigidbody.useGravity = false;
            }
            else if (!_bodyOnSlope){
                _bodyOnSlope = false;
                _slopeGravity = Vector3.zero;
                _rigidbody.useGravity = true;
            }
            //the lack of gravity makes the player slide more when the surface is more inclined
        }
        else{
            _groundedOnSlope = false;
            _bodyOnSlope = false;
            _slopeGravity = Vector3.zero;
            _rigidbody.useGravity = true;
            _groundDrag = 8;
        }
        //log _grounded
        Debug.Log("grounded: " + _grounded);
        Debug.Log("groundedonslope: " + _groundedOnSlope);
    }

    private void SpeedControl()
    {
        // limiting speed on slope
        if (OnSlope() && !_exitingSlope)
        {
            if (_rigidbody.velocity.magnitude > _moveSpeed)
                _rigidbody.velocity = _rigidbody.velocity.normalized * _moveSpeed;
        }
        // limiting speed on ground or in air
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
        _exitingSlope = true;
        
        // reset y velocity
        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);
        
        // create an upward impulse force
        _rigidbody.AddForce(_transform.up * _jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        _readyToJump = true;
        
        _exitingSlope = false;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, _playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            //debug angle
            //Debug.Log(angle);
            return angle < _maxSlopeAngle && angle != 0;
            //the lack of gravity makes the player slide more when the surface is more inclined
        }
        return false;
    }
    
    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(_moveDirection, _slopeHit.normal).normalized;
    }
    
    /// <summary>
    /// Check if we can call noclip method. If we are on the platform, we can call the method to enable/disable the
    /// noclip mode!
    /// </summary>
    private bool CanCallNoclip()
    {
        return _touchingNoclipEnabler;
    }
}
