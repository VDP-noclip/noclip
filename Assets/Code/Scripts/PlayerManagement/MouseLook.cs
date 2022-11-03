using Unity.VisualScripting;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The script is currently active")]
    private bool _activeScript = true;
    
    // link camera to body
    [Tooltip("The script is currently active")]
    [SerializeField] private Transform _orientation;
    
    // set mouse sensibility in X and Y axis
    [Tooltip("Set the mouse sensitivity")]
    [SerializeField] private float _xSensitivity = 50f;
    [SerializeField] private float _ySensitivity = 50f;
    [SerializeField] private float _sensitivity = 1f;
    
        
    // It's true if the camera is active, false otherwise
    private bool _activeCurrently;
    private Transform _transform;

    private float _yRotation = 0f; // yaw movement variable
    private float _xRotation = 0f; // pitch movement variable
    
    private void Awake()
    {
        if (PlayerPrefs.HasKey("masterSensitivity"))
        {
            float _localSensitivity = PlayerPrefs.GetFloat("masterSensitivity");
            _sensitivity = _sensitivity * _localSensitivity;
        }
        _transform = GetComponent<Transform>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_activeScript)
        {
            return;
        }
        
        if (_activeCurrently)
        {
            // get mouse input and proportionally modify the sensitivity
            float mouseX = Input.GetAxisRaw("Mouse X") * _xSensitivity * _sensitivity * Time.deltaTime;
            float mouseY = Input.GetAxisRaw("Mouse Y") * _ySensitivity * _sensitivity * Time.deltaTime;
        
            //calculate the rotation in both axis
            _yRotation += mouseX;
            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);    // Clamping allows to block the rotation
        
            // rotate camera and orientation
            _transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);
            _orientation.rotation = Quaternion.Euler(0, _yRotation, 0);
        }
    }

    public void ActivateMouseLook(bool active)
    {
        _activeCurrently = active;
    }
}
