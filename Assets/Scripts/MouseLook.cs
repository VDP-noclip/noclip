using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    // set mouse sensibility in X and Y axis
    [SerializeField] private float _xSensitivity = 100f;
    [SerializeField] private float _ySensitivity = 100f;
    [SerializeField] private float _sensitivity = 1f;
    
    [SerializeField] private Transform _playerBody;

    private float yaw;
    private float pitch;
    
    private float _xRotation = 0f;
    // private float _yRotation = 0f;
    
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        yaw = 0f;
        pitch = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        // get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * _xSensitivity * _sensitivity * Time.deltaTime;
        yaw += Input.GetAxisRaw("Mouse X") * _xSensitivity * _sensitivity * Time.deltaTime;
        pitch -= Input.GetAxisRaw("Mouse Y") * _ySensitivity * _sensitivity * Time.deltaTime;

        transform.eulerAngles = new Vector3(pitch, yaw, 0f);
        // _yRotation += mouseX;

        //_xRotation -= mouseY;
        //_xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        // rotate Camera1 and playerBody
        //transform.localRotation = Quaternion.Euler(0f, mouseX, 0f);
        _playerBody.Rotate(Vector3.up * mouseX);


    }
}
