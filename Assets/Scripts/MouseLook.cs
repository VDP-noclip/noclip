using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    // set mouse sensibility in X and Y axis
    public float xSensitivity = 100f;
    public float ySensitivity = 100f;

    public Transform playerBody;

    float sensitivity = 1f;
    
    private float xRotation = 0f;
    // private float yRotation = 0f;
    
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * xSensitivity * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * ySensitivity * sensitivity * Time.deltaTime;

        // yRotation += mouseX;
        
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // rotate Camera1 and playerBody
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
