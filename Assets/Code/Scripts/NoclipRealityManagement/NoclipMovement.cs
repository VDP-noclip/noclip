using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoclipMovement : MonoBehaviour
{

    [SerializeField]
    [Tooltip("The script is currently active")]
    private bool _active = true;
    
    [Space]

    [SerializeField]
    [Tooltip("Camera movement by 'W','A','S','D','Q','E' keys is active")]
    private bool _enableMovement = true;

    [Header("Base")]
    [Tooltip("Camera movement speed")]
    [SerializeField] private float _baseSpeed = 50f;
    [SerializeField] private float _baseAcceleration = 100f;
    
    private float _maxSpeed = 50f;
    private float _acceleration = 50f;

    [Header("Boost")]
    [SerializeField]
    [Tooltip("Speed of the quick camera movement when holding the 'Left Shift' key")]
    private float _boostedSpeed = 100f;
    [SerializeField] private float _boostAcceleration = 200f;

    [Header("Smooth brake")]
    [Tooltip("MaxSpeed percentage where braking deceleration becomes nonlinear (smoother braking close to zero)")]
    [SerializeField] private float _smoothBrakeRange = 0.1f;
    [Tooltip("Higher is smoother")]
    [SerializeField] private float _smoothBrakeFactor = 0.9f;
    
    /*
    [Header("Controls")]
    [SerializeField]
    [Tooltip("Boost speed")]
    private KeyCode _boostSpeed = KeyCode.LeftShift;
    
    [SerializeField]
    [Tooltip("Move up")]
    private KeyCode _moveUp = KeyCode.E;

    [SerializeField]
    [Tooltip("Move down")]
    private KeyCode _moveDown = KeyCode.Q;
    */
    private Transform _transform;
    private CameraManager _cameraManager;
    private Transform _noclipCamera;
    private NoclipManager _noclipManager;

    // These positions depends o the level 
    private Vector3 _initRotation;
    private Vector3 _initPosition;
    
    private float _horizontalInput;
    private float _verticalInput;
    private float _upDownInput;

    private Vector3 _speed = Vector3.zero;
    
    private bool _insideRealityPlayer = true;
    
    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _noclipManager = FindObjectOfType<NoclipManager>();
        _noclipCamera = GetComponent<Transform>();
    }

    // Update is called once per frame
    /*void Update()
    {
        if (!_active)
            return;

        if (_enableMovement && _noclipManager.IsNoclipEnabled())
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");
            bool moveUp = Input.GetButton("NoclipMoveUp");
            bool moveDown = Input.GetButton("NoclipMoveDown");
            bool boost = Input.GetButton("BoostNoclip");
            _upDownInput = 0;
            if(moveUp){
                _upDownInput += 1;
            }
            if(moveDown){
                _upDownInput += -1;
            }

            if (boost)
            {
                _maxSpeed = _boostedSpeed;
                _acceleration = _boostAcceleration;
            }
            else
            {
                _maxSpeed = _baseSpeed;
                _acceleration = _baseAcceleration;
            }

            
            //_horizontalInput and _verticalInput move with linear acceleration in the direction of the camera
            if (_horizontalInput != 0 || _verticalInput != 0 || _upDownInput != 0)
            {
                //increase speed in direction of camera with acceleration
                _speed += (_transform.forward * _verticalInput).normalized * _acceleration * Time.deltaTime;
                _speed += (_transform.right * _horizontalInput).normalized * _acceleration * Time.deltaTime;
                _speed += (_transform.up * _upDownInput).normalized * _acceleration * Time.deltaTime;
                //limit speed
                _speed = _speed.normalized * Mathf.Min(_speed.magnitude, _maxSpeed);
            }
            else
            {
                Vector3 deltaSpeed = _speed.normalized * _acceleration * Time.deltaTime;
                if(_speed.magnitude > deltaSpeed.magnitude && _speed.magnitude > _maxSpeed * _smoothBrakeRange)
                {
                    _speed -= deltaSpeed;
                    _smoothBrake = false;
                }
                else
                {
                    _smoothBrake = true;
                }
            }
            //move in direction of speed
            transform.position += _speed * Time.deltaTime;
        }
        else
        {
            _speed = Vector3.zero;
        }
    }

    private bool _smoothBrake = false;

    private void FixedUpdate(){
        if(_smoothBrake){
            _speed *= _smoothBrakeFactor;
        }
    }*/

void Update()
    {
        //max between fixeddeltatime and (Time.deltaTime / Time.fixedDeltaTime)
        float timeCorrection = Mathf.Min(1f, Time.deltaTime / Time.fixedDeltaTime);
        if (!_active)
            return;

        if (_enableMovement && _noclipManager.IsNoclipEnabled())
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");
            bool moveUp = Input.GetButton("NoclipMoveUp");
            bool moveDown = Input.GetButton("NoclipMoveDown");
            bool boost = Input.GetButton("BoostNoclip");
            _upDownInput = 0;
            if(moveUp){
                _upDownInput += 1;
            }
            if(moveDown){
                _upDownInput += -1;
            }

            if (boost)
            {
                _maxSpeed = _boostedSpeed;
                _acceleration = _boostAcceleration;
            }
            else
            {
                _maxSpeed = _baseSpeed;
                _acceleration = _baseAcceleration;
            }

            
            //_horizontalInput and _verticalInput move with linear acceleration in the direction of the camera
            if (_horizontalInput != 0 || _verticalInput != 0 || _upDownInput != 0)
            {
                //increase speed in direction of camera with acceleration
                _speed += (_transform.forward * _verticalInput).normalized * _acceleration * timeCorrection;
                _speed += (_transform.right * _horizontalInput).normalized * _acceleration * timeCorrection;
                _speed += (_transform.up * _upDownInput).normalized * _acceleration * timeCorrection;
                //limit speed
                _speed = _speed.normalized * Mathf.Min(_speed.magnitude, _maxSpeed) * timeCorrection;
            }
            else
            {
                Vector3 deltaSpeed = _speed.normalized * _acceleration;
                if(_speed.magnitude > deltaSpeed.magnitude && _speed.magnitude > _maxSpeed * _smoothBrakeRange)
                {
                    _speed -= deltaSpeed * timeCorrection;
                    _smoothBrake = false;
                }
                else
                {
                    _smoothBrake = true;
                    if(_smoothBrake){
                        _speed *= _smoothBrakeFactor / timeCorrection;
                    }
                }
            }
            //move in direction of speed
            transform.position += _speed * timeCorrection;
        }
        else
        {
            _speed = Vector3.zero;
        }
    }

    private bool _smoothBrake = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RealityPlayer") && _noclipManager.IsNoclipEnabled())
        {
            _noclipManager.SetPlayerCanSwitchMode(true);
            if (!_insideRealityPlayer)
            {
                _noclipManager.NoClipReturnedToBody();
                _insideRealityPlayer = true;          
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("RealityPlayer"))
        {
            _insideRealityPlayer = false;
            _noclipManager.NoClipExitedToBody();
        }
    }

    public void SetPositionAndRotation(Vector3 position, Quaternion cameraOrientation)
    {
        _transform.position = position;
        _noclipCamera.transform.rotation = cameraOrientation;
    }

    public void SetEnableMovement(bool value)
    {
        _enableMovement = value;
    }
}
