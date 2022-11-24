using System;
using UnityEngine;
//using list
using System.Collections.Generic;
//using slider
using UnityEngine.UI;

public class RealityMovement : MonoBehaviour
{
    [SerializeField] private bool _calibrationMenu = false;

    public enum MovementState       // define player states
    {
        Walking,
        Sprinting,
        Crouching,
        Air
    }

    [Header("Speed")] 
    [Tooltip("Suggestion: Max Run Speed < Run Force Multiplier")]
    [Range(0, 30)]
    [SerializeField] private float _maxRunSpeed = 21f;
    [Range(0, 30)]
    [SerializeField] private float _runForceMultiplier = 7f;
    [Range(0, 25)]
    [SerializeField] private float _maxWalkSpeed = 10f;
    [Range(0, 25)]
    [SerializeField] private float _walkForceMultiplier = 3f;
    
    private float _moveSpeed;     // speed intensity
    private float _maxMoveSpeed;
    
    [Header("Drag")]
    [Range(0, 15)]
    [SerializeField] private float _groundDrag = 20f;    // ground drag
    
    [Header("Jump")]
    [Range(0, 25)]
    [SerializeField] private float _jumpForce = 12f;     // set jump upward force
    [Range(0, 1)]
    [SerializeField] private float _jumpCooldown = 0.25f;      // set jump cooldown
    [Range(0, 1f)]
    [SerializeField] private float _airMultiplier = 0.3f;     // set air movement limitation
    private bool _readyToJump;      //

    [Header("Crouch")]
    [Range(0, 10)]
    [SerializeField] private float _crouchSpeed = 2f;
    [Range(0, 1)]
    [SerializeField] private float _crouchYScale = 0.5f;
    private float _startYScale;
    
    [Header("Slope handling")]
    [SerializeField] private float _maxSlopeAngle = 45;
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
    [SerializeField] private float _gravityMultiplier = 2f;
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
        /*GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        //if gravity magnitude is not 0
        if (gameManager.GetGravity() == 0)
        {
            //set gravity to gravity magnitude
            gameManager.SetGravity(Physics.gravity.magnitude);
        } */
        Physics.gravity *= _gravityMultiplier;
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

    //slider gameobject
    private GameObject _speedSlider;
    private GameObject _jumpForceSlider;
    private GameObject _gravitySlider;
    private GameObject _dragSlider;
    private GameObject _airSlider;
    private GameObject _slopeSlider;
    private GameObject _accSlider;
    private GameObject _speedMonitor;
    private GameObject _saveButton;
    private GameObject _sensitivitySlider;
    private MouseLook _mouseLook;
    private bool _gPressed = false;
    private bool _hPressed = false;

    private void InitCalibrationMenu(){
        _saveButton = GameObject.Find("SaveButton");
        _slopeSlider = GameObject.Find("MaxSlope");//
        _airSlider = GameObject.Find("AirAcceleration");
        _dragSlider = GameObject.Find("Drag");
        //_airSpeedSlider = GameObject.Find("AirSpeed");//
        _accSlider = GameObject.Find("RunAcceleration");//
        _speedSlider = GameObject.Find("RunSpeed"); //walk is half
        _jumpForceSlider = GameObject.Find("JumpForce");
        _gravitySlider = GameObject.Find("Gravity");
        _sensitivitySlider = GameObject.Find("Sensitivity");
        _mouseLook = GameObject.Find("RealityCamera").GetComponent<MouseLook>();

        _speedMonitor = GameObject.Find("SpeedMonitor");
        try{
            //load slider values from playerprefs
            _slopeSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("MaxSlope");
            _airSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("AirAcceleration");
            _dragSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("Drag");
            //_airSpeedSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("AirSpeed", 10);
            _accSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("RunAcceleration");
            _speedSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("RunSpeed");
            _jumpForceSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("JumpForce");
            _gravitySlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("Gravity");
            _sensitivitySlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("Sensitivity");
        }
        catch{
            Debug.Log("PlayerPrefs not found");
        }
    }

    private MultiForceVisualizer _forceVisualizer;

    //variable size list vector3 of forces
    private List<Vector3> _forces;

    private void ToggleCalibrationMenu(){
        if(!_calibrationMenu){
            GameObject.Find("CalibratePlayerGUI").SetActive(false);
            _calibrationMenu = false;
        }
        if(_calibrationMenu){
            GameObject.Find("CalibratePlayerGUI").SetActive(true);
            _calibrationMenu = true;
        }
    }
    private void CalibrationMenu(){
        if(!_calibrationMenu){
            if(_speedSlider == null || _jumpForceSlider == null || _gravitySlider == null || _dragSlider == null){
                InitCalibrationMenu();
                _forceVisualizer = GameObject.Find("ForceVisualizer").GetComponent<MultiForceVisualizer>();
            }
            _forceVisualizer.UpdateForces(_forces);
            _forces = new List<Vector3>();
            return;
        }
        else{
            if(_speedSlider == null || _jumpForceSlider == null || _gravitySlider == null || _dragSlider == null){
                InitCalibrationMenu();
                _forceVisualizer = GameObject.Find("ForceVisualizer").GetComponent<MultiForceVisualizer>();
            }
            _forceVisualizer.UpdateForces(_forces);
            _forces = new List<Vector3>();
            //_jumpForce = _jumpForceSlider.GetComponent<Slider>().value;
            //_moveSpeed = _speedSlider.GetComponent<Slider>().value;
            _jumpForce = _jumpForceSlider.GetComponent<Slider>().value;
            _maxRunSpeed = _speedSlider.GetComponent<Slider>().value;
            _maxWalkSpeed = _maxRunSpeed / 2;
            _groundDrag = _dragSlider.GetComponent<Slider>().value;
            _gravityMultiplier = _gravitySlider.GetComponent<Slider>().value;
            //cut airslider value to 2 decimal places
            _airSlider.GetComponent<Slider>().value = (float)Math.Round(_airSlider.GetComponent<Slider>().value, 2);
            _airMultiplier = _airSlider.GetComponent<Slider>().value;
            //_maxAirSpeed = _airSpeedSlider.GetComponent<Slider>().value;
            _maxSlopeAngle = _slopeSlider.GetComponent<Slider>().value;
            _runForceMultiplier = _accSlider.GetComponent<Slider>().value;
            _mouseLook.setSensitivity(_sensitivitySlider.GetComponent<Slider>().value);

            //set speed monitor value to current speed
            _speedMonitor.GetComponent<Slider>().value = _rigidbody.velocity.magnitude;

            Physics.gravity = new Vector3(0, -_gravity * _gravityMultiplier, 0);
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
                //find calibration menu
                GameObject calibrationMenu = GameObject.Find("CalibratePlayerGUI");
                //find canvas among children
                GameObject canvas = calibrationMenu.transform.Find("Canvas").gameObject;
                //find sliders among children
                GameObject sliders = canvas.transform.Find("Sliders").gameObject;
                //toggle sliders
                sliders.SetActive(!sliders.activeSelf);
            }
            if (!Input.GetKeyDown(KeyCode.G))
            {
                _hPressed = false;
            }
            
        }
    }

    void ApplyForce(Vector3 force)
    {
        _rigidbody.AddForce(force, ForceMode.Force);
        try{
        _forces.Add(force);
        }
        catch{
        }
    }


    public void SaveSettings(){
        //save settings
        Debug.Log("Saving settings");
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = this.transform.position + this.transform.forward * 2;
        cube.GetComponent<Renderer>().material.color = Color.blue;
        Destroy(cube, 2);
        PlayerPrefs.SetFloat("JumpForce", _jumpForce);
        PlayerPrefs.SetFloat("RunSpeed", _maxRunSpeed);
        PlayerPrefs.SetFloat("WalkSpeed", _maxWalkSpeed);
        PlayerPrefs.SetFloat("Drag", _groundDrag);
        PlayerPrefs.SetFloat("Gravity", _gravityMultiplier);
        PlayerPrefs.SetFloat("AirAcceleration", _airMultiplier);
        PlayerPrefs.SetFloat("MaxSlope", _maxSlopeAngle);
        PlayerPrefs.SetFloat("RunAcceleration", _runForceMultiplier);
        PlayerPrefs.SetFloat("Sensitivity", _sensitivitySlider.GetComponent<Slider>().value);
        PlayerPrefs.Save();
    }
}
