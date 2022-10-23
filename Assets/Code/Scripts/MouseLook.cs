using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    // link camera to body
    [SerializeField] private Transform _orientation;
    
    
    // set mouse sensibility in X and Y axis
    [SerializeField] private float _xSensitivity = 100f;
    [SerializeField] private float _ySensitivity = 100f;
    [SerializeField] private float _sensitivity = 1f;
    
    private Transform _transform;

    private float _yRotation = 0f; // yaw movement variable
    private float _xRotation = 0f; // pitch movement variable


    private void Awake()
    {
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
        // get mouse input and proportionally modify the sensitivity
        float mouseX = Input.GetAxisRaw("Mouse X") * _xSensitivity * _sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * _ySensitivity * _sensitivity * Time.deltaTime;
        
        //calculate the rotation in both axis
        _yRotation += mouseX;
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);    // Clamping allows to block the rotation
        
        // rotate camera and orientation
        transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);
        _orientation.rotation = Quaternion.Euler(0, _yRotation, 0);
        
        
        /*
        //Rotate the entire object (body + camera) around the Y axis
        _playerBody.transform.Rotate(new Vector3(0f, mouseX, 0f));

        //Rotate the camera on X axis
         
        transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        */

    }
}
