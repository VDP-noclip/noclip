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

    [SerializeField]
    [Tooltip("Camera movement speed")]
    private float _movementSpeed = 10f;
    
    [SerializeField]
    [Tooltip("Speed of the quick camera movement when holding the 'Left Shift' key")]
    private float _boostedSpeed = 50f;
    
    [SerializeField]
    [Tooltip("Boost speed")]
    private KeyCode _boostSpeed = KeyCode.LeftShift;
    
    [SerializeField]
    [Tooltip("Move up")]
    private KeyCode _moveUp = KeyCode.E;

    [SerializeField]
    [Tooltip("Move down")]
    private KeyCode _moveDown = KeyCode.Q;
    
    [Space]

    [SerializeField]
    [Tooltip("Acceleration at camera movement is active")]
    private bool _enableSpeedAcceleration = true;

    [SerializeField]
    [Tooltip("Rate which is applied during camera movement")]
    private float _speedAccelerationFactor = 1.5f;
    
    private Transform _transform;
    private CameraManager _cameraManager;
    
    
    // These positions depends o the level 
    private Vector3 _initRotation;
    private Vector3 _initPosition;
    
    private float _currentIncrease = 1;
    private float _currentIncreaseMem = 0;
    
    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _cameraManager = GameObject.FindObjectOfType<CameraManager>();

    }

    private void Start()
    {
        _initRotation = _transform.eulerAngles;
        _initPosition = _transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_active)
            return;
        
        if (_enableMovement)
        {
            Vector3 deltaPosition = Vector3.zero;
            float currentSpeed = _movementSpeed;

            if (Input.GetKey(_boostSpeed))
                currentSpeed = _boostedSpeed;

            if (Input.GetKey(KeyCode.W))
                deltaPosition += _transform.forward;

            if (Input.GetKey(KeyCode.S))
                deltaPosition -= _transform.forward;

            if (Input.GetKey(KeyCode.A))
                deltaPosition -= _transform.right;

            if (Input.GetKey(KeyCode.D))
                deltaPosition += _transform.right;

            if (Input.GetKey(_moveUp))
                deltaPosition += transform.up;

            if (Input.GetKey(_moveDown))
                deltaPosition -= transform.up;
            
            // Calc acceleration
            CalculateCurrentIncrease(deltaPosition != Vector3.zero);

            transform.position += deltaPosition * currentSpeed * _currentIncrease;
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RealityPlayer"))
        {
            _cameraManager.SwitchCamera();
            _transform.position = _initPosition;
            _transform.eulerAngles = _initRotation;
        }
    }

    private void CalculateCurrentIncrease(bool moving)
    {
        _currentIncrease = Time.deltaTime;

        if (!_enableSpeedAcceleration || _enableSpeedAcceleration && !moving)
        {
            _currentIncreaseMem = 0;
            return;
        }

        _currentIncreaseMem += Time.deltaTime * (_speedAccelerationFactor - 1);
        _currentIncrease = Time.deltaTime + Mathf.Pow(_currentIncreaseMem, 3) * Time.deltaTime;
    }

}
