using System;
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

    //private float _yaw;
    private float _pitch = 0f;
  


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // get mouse input and proportionally modify the sensitivity
        float mouseX = Input.GetAxisRaw("Mouse X") * _xSensitivity * _sensitivity * Time.deltaTime;
        float mouseY = -Input.GetAxisRaw("Mouse Y") * _xSensitivity * _sensitivity * Time.deltaTime;
        
        //calculate the yaw (rotation around y) and the pitch (rotation around x)
        //_yaw += mouseX;
        _pitch -= mouseY;
        
        //Rotate the entire object (body + camera) around the Y axis
        _playerBody.transform.Rotate(new Vector3(0f, mouseX, 0f));

        //Rotate the camera on X axis
        _pitch = Mathf.Clamp(_pitch, -90f, 90f);    // Clamping allows to block the rotation 
        transform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        

    }
}
