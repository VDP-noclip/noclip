using System;
using System.Collections;
using POLIMIGameCollective;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Everything here is linked to how cursor input is processed and used.
/// Rotation, orientation, sensitivity handling and eventual added preferences such as inverted vertical axis.
/// </summary>
public class MouseLook : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The script is currently active")]
    private bool _activeScript = true;
    
    // link camera to body
    // TODO check the right name for the orientation: in this way are a little confusing terms
    [Tooltip("Indicates the gameobject that has the orientation information: could be also the gameobject that own this script")]
    [SerializeField] private Transform _orientation;
    
    // set mouse sensibility in X and Y axis
    [Tooltip("Set the mouse sensitivity")]
    [SerializeField] private float _xSensitivity = 50f;
    [SerializeField] private float _ySensitivity = 50f;
    
    private float _sensitivity = 1f;
    
        
    // It's true if the camera is active, false otherwise
    private bool _activeCurrently;
    private Transform _transform;
    //_prevTransformRotation
    private Quaternion _prevTransformRotation;

    private float _yRotation; // yaw movement variable
    private float _yRotationCheckpoint;
    private float _xRotation; // pitch movement variable
    private float _xRotationCheckpoint;
    private void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    void Start()
    {
        EventManager.StartListening("setSensitivity", SetSensitivityFromPause);
        EventManager.StartListening("StoreCheckpointRotation", StoreCheckpointRotation);
        EventManager.StartListening("SetLastCheckpointRotation", SetLastCheckpointRotation);
        
        if (PlayerPrefs.HasKey("masterSensitivity"))
        {
            _sensitivity = PlayerPrefs.GetFloat("masterSensitivity");
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _prevTransformRotation = _transform.rotation;
        _yRotation = _orientation.eulerAngles.y;
    }
    
    void Update()
    {
        if (!_activeScript)
        {
            return;
        }
        
        if (_activeCurrently && Untampered())
        {
            // get mouse input and proportionally modify the sensitivity
            float mouseX = 0;
            float mouseY = 0;
            //if time is flowing
            if(Time.deltaTime > 0)
            {
                mouseX = Input.GetAxisRaw("Mouse X") * _xSensitivity * _sensitivity; //NB: here X refers to the axis of the screen
                mouseY = Input.GetAxisRaw("Mouse Y") * _ySensitivity * _sensitivity; //NB: here Y refers to the axis of the screen
            }
        
            // calculate the rotation in both axis
            _yRotation += mouseX; // the x screen axis corresponds to a rotation on the y axis of the camera 
            _xRotation -= mouseY; // the y screen axis corresponds to a rotation on the x axis of the camera 
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);    // Clamping allows to block the rotation
            UpdateRotation();
        }
    }

    private bool Untampered()
    {
        return _transform.rotation == _prevTransformRotation;
    }

    public void ActivateMouseLook(bool active)
    {
        _activeCurrently = active;
    }
    
    public void CopyRotationCoordinates(MouseLook mouseLook)
    {
        _xRotation = mouseLook.GetXRotation();
        _yRotation = mouseLook.GetYRotation();
        UpdateRotation();
    }

    public float GetXRotation()
    {
        return _xRotation;
    }
    
    public float GetYRotation()
    {
        return _yRotation;
    }

    private void SetSensitivityFromPause(string sensitivityPlaceholder)
    {
        EventManager.StopListening("setSensitivity", SetSensitivityFromPause);
        Debug.Log("SetSensitivityFromPause");
        _sensitivity = float.Parse(sensitivityPlaceholder);
        EventManager.StartListening("setSensitivity", SetSensitivityFromPause);
    }

    private void StoreCheckpointRotation()
    {
        _xRotationCheckpoint = _xRotation;
        _yRotationCheckpoint = _yRotation;
    }

    public void SetLastCheckpointRotation()
    {
        //log
        Debug.Log("SetLastCheckpointRotation");
        //find level manager of puzzles
        GameObject puzzles = GameObject.Find("Puzzles");
        //component
        LevelManager levelManager = puzzles.GetComponent<LevelManager>();
        //if name of this gameobject is not NoclipCamera
        if (gameObject.name != "NoclipCamera")
        {
            _yRotation = levelManager.NextCheckpointOrientation().y;
            _xRotation = 0;//levelManager.NextCheckpointOrientation().x; //this works but gives problems with some checkpoints
            UpdateRotation();
        }
    }

    private void UpdateRotation() //there is too much mess and redundancy in the rotations
    {
        // IMPORTANT: Don't change the order of the two following lines: if _orientation and _transform are the same gameobject it doesn't work due to value override.
        _orientation.rotation = Quaternion.Euler(0, _yRotation, 0);  // updates the orientation of the gameobject that has the orientation information of the camera on the y axis
        _transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);  // updates the camera orientation
        _prevTransformRotation = _transform.rotation;
    }

    public void SyncYRotation()
    {
        _yRotation = _orientation.eulerAngles.y;
        _prevTransformRotation = _transform.rotation;
    }
}