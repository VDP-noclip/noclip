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

    private Transform _transform;
    private CameraManager _cameraManager;
    private Transform _noclipCamera;
    
    // It's true if is the noclipPlayer, it's false if is the reality body
    private bool _currentPlayer = false;
    
    // These positions depends o the level 
    private Vector3 _initRotation;
    private Vector3 _initPosition;
    
    
    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _cameraManager = GameObject.FindObjectOfType<CameraManager>();
        _noclipCamera = _transform.GetChild(0).GetComponent<Transform>();
        _initRotation = _transform.eulerAngles;
        _initPosition = _transform.position;

    }

    /*private void Start()
    {
        _initRotation = _transform.eulerAngles;
        _initPosition = _transform.position;
    }*/

    // Update is called once per frame
    void Update()
    {
        if (!_active)
            return;
        
        if (_enableMovement && _currentPlayer)
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
            
            _transform.position += deltaPosition * (currentSpeed * Time.deltaTime);
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RealityPlayer") && _currentPlayer)
        {
            _cameraManager.SwitchCamera();
        }
    }

    public void ActivatePlayer(bool active)
    {
        _currentPlayer = active;
    }

    public void SetPositionAndRotation(Vector3 position, Vector3 orientation, Vector3 cameraOrientation)
    {
        _transform.position = position;
        _transform.eulerAngles = orientation;
        _noclipCamera.transform.eulerAngles = cameraOrientation;
    }

}
