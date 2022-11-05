using Unity.VisualScripting;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    /// <summary>
    /// Everything here is linked to how cursor input is processed and used.
    /// Rotation, orientation, sensitivity handling and eventual added preferences such as inverted vertical axis.
    /// </summary>

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
        // Checks whether there are actual sensitivity settings, and if there are
        // it applies them by simply multiplying sensitivity with the _localSensitivity multiplier.
        if (PlayerPrefs.HasKey("masterSensitivity"))
        {
            float localSensitivity = PlayerPrefs.GetFloat("masterSensitivity");
            _sensitivity *= localSensitivity;
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
            
            // Checks whether there's local information about vertical axis preference.
            if (PlayerPrefs.HasKey("masterInvertY"))
            {
                // The following rotation order is needed because if the object is the same,
                // the second assign value overwrites the values of the first one. If the object is different, the order is irrelevant.
                
                if (PlayerPrefs.GetInt("masterInvertY"))
                {
                    // invert camera only on the vertical axis
                    _orientation.rotation = Quaternion.Euler(0, _yRotation, 0);
                    _transform.rotation = Quaternion.Euler(_xRotation*(-1), _yRotation, 0);
                }
                else
                {
                    // rotate camera and orientation
                    _orientation.rotation = Quaternion.Euler(0, _yRotation, 0);
                    _transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);
                }
            }
            
        }
    }

    public void ActivateMouseLook(bool active)
    {
        _activeCurrently = active;
    }
}
