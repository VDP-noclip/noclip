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
    //[SerializeField] private bool _calibration = true;
    [SerializeField] private bool _calibrationMenu = false;
    private bool _showForces = false;
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
        _transform = GetComponent<Transform>();
        _noclipManager = FindObjectOfType<NoclipManager>();
    }

    // Start is called before the first frame update
    private void Start()
    {

        _rigidbody.freezeRotation = true;
        
        ResetJump(); // set _readyToJump to "true"

        _startYScale = _transform.localScale.y;

        //if gravity magnitude is 0 (Gamemanager zeroes it on game boot)
        if (Physics.gravity.magnitude == 0)
        {
            //set gravity to gravity magnitude
            Physics.gravity = new Vector3(0, -_gravity, 0) * _gravityMultiplier;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        //IMPORTANT LESSON materials loaded at runtime should be instantiated
        //find ErrorCube and enable its mesh
        //GameObject errorCube = GameObject.Find("ErrorCube");
        //find realitybody
        //GameObject realityBody = GameObject.Find("RealityBody");
        //set material of errorCube to material of RealityBody
        //errorCube.GetComponent<MeshRenderer>().material = realityBody.GetComponent<MeshRenderer>().material;

        //if c is pressed toggle calibration menu
        /*if (Input.GetKeyDown(KeyCode.C))
        {
            _calibrationMenu = !_calibrationMenu;
            ToggleCalibrationMenu();
        }
        CalibrationMenu();*/

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
            if (_grounded && _horizontalInput == 0 && _verticalInput == 0 && !_commitJump)
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
            ApplyForce(GetSlopeMoveDirection() * (_moveSpeed * _gravity));
            // if _rigidbody.velocity.y != 0 and w a s or d pressed
            if (_rigidbody.velocity.y != 0 &&(_horizontalInput != 0 || _verticalInput != 0))
            {
                // Add a force that obliged the player to stay on the inclined plane. The force is perpendicular to the plane
                //ApplyForce(-_slopeHit.normal * (_gravity * _gravityMultiplier * _runForceMultiplier * 4));

                //TODO check if new slope angle is greater than the previous, if it is don't apply this force so that the player can climb it, then apply this force again as always 
            }
        }
        // differentiate movement on the ground and in air
        else if (_grounded){
            _groundSpeed = _rigidbody.velocity.magnitude;
            ApplyForce(_moveSpeed * _gravity * _moveDirection.normalized);
        }
        else{
            ApplyForce(_moveSpeed * _airMultiplier * _gravity * _moveDirection.normalized);
            //prject the velocity on the horizontal plane
            Vector3 horizontalVelocity = Vector3.ProjectOnPlane(_rigidbody.velocity, Vector3.up);
            float airMaxSpeed = Mathf.Max(_maxMoveSpeed, _groundSpeed); //in the air the max speed is the max between move speed and "sling speed" in case player was thrown away
            if (horizontalVelocity.magnitude > airMaxSpeed)
            {
                _rigidbody.velocity = horizontalVelocity.normalized * airMaxSpeed + Vector3.up * _rigidbody.velocity.y;
            }
        }

        _rigidbody.useGravity = !OnSlope();
        //if gravity is used add it to the forces
        if (_rigidbody.useGravity)
        {
            ApplyForce(Physics.gravity);
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
        //_rigidbody.AddForce(_transform.up * _jumpForce, ForceMode.Impulse);
        //_forces.Add(_transform.up * _jumpForce);
        ApplyForce(_transform.up * _jumpForce * 50); //impulse is force/dt, 1/dt is 50 (fixed update is 0.02)
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
            
            if (Input.GetKeyDown(KeyCode.T))
            {
                _showForces = !_showForces;
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

    public bool ShowForces()
    {
        return _showForces;
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
