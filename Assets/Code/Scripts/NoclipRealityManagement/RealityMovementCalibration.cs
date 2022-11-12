using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
//using list
using System.Collections.Generic;
public class RealityMovementCalibration : MonoBehaviour
{
    private enum MovementState       // define player states
    {
        Walking,
        Sprinting,
        Crouching,
        Air
    }

    [Header("Speed")] 
    [Tooltip("Suggestion: Max Run Speed < Run Force Multiplier")]
    [SerializeField] private float _maxRunSpeed = 6f;
    [SerializeField] private float _runForceMultiplier = 10f;
    [SerializeField] private float _maxWalkSpeed = 3f;
    [SerializeField] private float _walkForceMultiplier = 3f;
    
    private float _moveSpeed;     // speed intensity
    private float _maxMoveSpeed;
    
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
    

    //original gravity
    private Vector3 _originalGravity;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _originalGravity = Physics.gravity;
        Physics.gravity *= _gravityMultiplier;   // Change the gravity
        _transform = GetComponent<Transform>();
        _noclipManager = FindObjectOfType<NoclipManager>();
    }

    // Start is called before the first frame update
    private void Start()
    {

        _rigidbody.freezeRotation = true;
        
        ResetJump(); // set _readyToJump to "true"

        _startYScale = _transform.localScale.y;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Update is called once per frame
    private void Update()
    {
        CalibrationMenu();

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
            if (_grounded && !_commitJump)
                _rigidbody.drag = _groundDrag;
            else
                _rigidbody.drag = 0;
        }
    }

    public void ResetSpeedOnRespawn()
    {
        _rigidbody.velocity = Vector3.zero;
    }

    
    private bool _commitJump = false;

    private void LateUpdate()
    {
        if (_commitJump)
        {
            _commitJump = false;
            Jump();
        }
    }

    private void UserInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
        
        // when to jump
        if (Input.GetKey(_jumpKey) && _readyToJump && _grounded)
        {
            _readyToJump = false;
            
            //set ground drag to zero
            _rigidbody.drag = 0;
            _commitJump = true;
            
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

    private float _groundSpeed = 0;
    private void MovePlayer()
    {
        // calculate movement direction
        _moveDirection = _orientation.forward * _verticalInput + _orientation.right * _horizontalInput;

        // onslope
        if (OnSlope() && !_exitingOnSlope)
        {
            // Add a force on the plane direction of the plane
            _rigidbody.AddForce(GetSlopeMoveDirection() * (_moveSpeed * _gravity), ForceMode.Force); 
            _forces.Add(GetSlopeMoveDirection() * (_moveSpeed * _gravity));
            
            // if _rigidbody.velocity.y != 0 and w a s or d pressed
            if (_rigidbody.velocity.y != 0 &&(_horizontalInput != 0 || _verticalInput != 0))
            {
                // Add a force that obliged the player to stay on the inclined plane. The force is perpendicular to the plane
                _rigidbody.AddForce(-_slopeHit.normal * (_gravity * _gravityMultiplier * _runForceMultiplier * 4), ForceMode.Force); 
                _forces.Add(-_slopeHit.normal * (_gravity * _gravityMultiplier * _runForceMultiplier * 4));
                //TODO check if new slope angle is greater than the previous, if it is don't apply this force so that the player can climb it, then apply this force again as always 
            }
        }
        // differentiate movement on the ground and in air
        if (_grounded){
            _groundSpeed = _rigidbody.velocity.magnitude;
            _rigidbody.AddForce(_moveSpeed * _gravity * _moveDirection.normalized, ForceMode.Force);
            _forces.Add(_moveSpeed * _gravity * _moveDirection.normalized);
        }
        else{
            _rigidbody.AddForce(_moveSpeed * _airMultiplier * _gravity * _moveDirection.normalized, ForceMode.Force);
            _forces.Add(_moveSpeed * _airMultiplier * _gravity * _moveDirection.normalized);
            //prject the velocity on the horizontal plane
            Vector3 horizontalVelocity = Vector3.ProjectOnPlane(_rigidbody.velocity, Vector3.up);
            if (horizontalVelocity.magnitude > Mathf.Max(_maxMoveSpeed, _groundSpeed))
            {
                _rigidbody.velocity = horizontalVelocity.normalized * _groundSpeed + Vector3.up * _rigidbody.velocity.y;
            }
        }

        _rigidbody.useGravity = !OnSlope();
        //if gravity is used add it to the forces
        if (_rigidbody.useGravity)
        {
            _forces.Add(Physics.gravity);
        }
    }

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
        _forces.Add(_transform.up * _jumpForce);
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

    //This function returns the vector3 that represents the plane on which the player is moving
    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(_moveDirection, _slopeHit.normal).normalized;
    }
    
    //slider gameobject
    private GameObject _speedSlider;
    private GameObject _jumpForceSlider;
    private GameObject _gravitySlider;
    private GameObject _dragSlider;
    private GameObject _airSlider;
    private GameObject _speedMonitor;
    private bool _gPressed = false;
    private bool _hPressed = false;
    private bool _calibrationMenu = true;

    private void InitCalibrationMenu(){
        _speedSlider = GameObject.Find("RunSpeed");
        _jumpForceSlider = GameObject.Find("JumpForce");
        _gravitySlider = GameObject.Find("Gravity");
        _dragSlider = GameObject.Find("Drag");
        _airSlider = GameObject.Find("AirMultiplier");
        _speedMonitor = GameObject.Find("SpeedMonitor");
    }

    ForceVisualizer _forceVisualizer;
    
    //variable size list vector3 of forces
    private List<Vector3> _forces;
    private void CalibrationMenu(){
        if(_speedSlider == null || _jumpForceSlider == null || _gravitySlider == null || _dragSlider == null){
            InitCalibrationMenu();
            //find gameobject ForceVisualizer
            _forceVisualizer = GameObject.Find("ForceVisualizer").GetComponent<ForceVisualizer>();
        }
        _forceVisualizer.UpdateForces(_forces);
        _forces = new List<Vector3>();
        //_jumpForce = _jumpForceSlider.GetComponent<Slider>().value;
        //_moveSpeed = _speedSlider.GetComponent<Slider>().value;
        _jumpForce = _jumpForceSlider.GetComponent<Slider>().value;
        _maxRunSpeed = _speedSlider.GetComponent<Slider>().value;
        _maxWalkSpeed = _maxRunSpeed / 2;
        _runForceMultiplier = _speedSlider.GetComponent<Slider>().value;
        _groundDrag = _dragSlider.GetComponent<Slider>().value;
        _gravityMultiplier = _gravitySlider.GetComponent<Slider>().value;
        _airMultiplier = _airSlider.GetComponent<Slider>().value;
        _speedMonitor.GetComponent<TextMeshProUGUI>().text = ((int)_rigidbody.velocity.magnitude).ToString();
        Physics.gravity = _originalGravity * _gravityMultiplier;
        //g button toggle
        if (Input.GetKeyDown(KeyCode.G) && !_gPressed)
        {
            _calibrationMenu = !_calibrationMenu;
            if(_calibrationMenu){
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else{
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            _gPressed = true;
        }
        if (!Input.GetKeyDown(KeyCode.G))
        {
            _gPressed = false;
        }

        
        if (Input.GetKeyDown(KeyCode.H) && !_hPressed)
        {
            _hPressed = true;
            //toggle sliders
            _speedSlider.SetActive(!_speedSlider.activeSelf);
            _jumpForceSlider.SetActive(!_jumpForceSlider.activeSelf);
            _gravitySlider.SetActive(!_gravitySlider.activeSelf);
            _dragSlider.SetActive(!_dragSlider.activeSelf);
            _airSlider.SetActive(!_airSlider.activeSelf);
        }
        if (!Input.GetKeyDown(KeyCode.G))
        {
            _hPressed = false;
        }
    }
}
