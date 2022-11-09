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
    private NoclipManager _noclipManager;

    // These positions depends o the level 
    private Vector3 _initRotation;
    private Vector3 _initPosition;
    
    
    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _noclipManager = FindObjectOfType<NoclipManager>();
        _noclipCamera = GetComponent<Transform>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!_active)
            return;
        
        if (_enableMovement && _noclipManager.IsNoclipEnabled())
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
                deltaPosition += Vector3.up;

            if (Input.GetKey(_moveDown))
                deltaPosition -= Vector3.up;
            
            _transform.position += deltaPosition * (currentSpeed * Time.deltaTime);
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RealityPlayer") && _noclipManager.IsNoclipEnabled())
        {
            _noclipManager.SetPlayerCanDisableNoclip(true);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("RealityPlayer"))
        {
            _noclipManager.SetPlayerCanDisableNoclip(false);
        }
    }

    public void SetPositionAndRotation(Vector3 position, Quaternion cameraOrientation)
    {
        _transform.position = position;
        _noclipCamera.transform.rotation = cameraOrientation;
    }

}
