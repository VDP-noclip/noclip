using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    
    Vector3 velocity;
    bool isGrounded;
    
    // Start is called before the first frame update
    void Start()
    {
            GameObject.Find("Camera1").transform.Find("Canvas").transform.Find("NoClip").gameObject.SetActive(!GameObject.Find("Camera1").transform.Find("Canvas").transform.Find("NoClip").gameObject.activeSelf);
        
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        /*
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(60);
        }
        //velocity.y = 0 on button jump release
        if(Input.GetButtonUp("Jump"))
        {
            velocity.y = 0;
        }
        //on shift release set velocity to 0
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            velocity.y = 0;
        }
        if(Input.GetKey(KeyCode.LeftShift))
        {
            velocity.y = Mathf.Sqrt(-60);
        }
        */

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
        //if position y of controller is less than 3 set position y to 3   
        if (controller.transform.position.y < 3)
        {
            controller.transform.position = new Vector3((float)0.34, (float)60.2, (float)-4.19);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            GameObject[] noclipObjects = GameObject.FindGameObjectsWithTag("noclipObject");
            foreach (GameObject noclipObject in noclipObjects)
            {
                noclipObject.GetComponent<MeshRenderer>().enabled = !noclipObject.GetComponent<MeshRenderer>().enabled;
            }
            GameObject[] clipObjects = GameObject.FindGameObjectsWithTag("clipObject");
            foreach (GameObject clipObject in clipObjects)
            {
                clipObject.GetComponent<MeshRenderer>().enabled = !clipObject.GetComponent<MeshRenderer>().enabled;
            }
            //toggle show of text of NoClip in Camera1 canvas
            GameObject.Find("Camera1").transform.Find("Canvas").transform.Find("NoClip").gameObject.SetActive(!GameObject.Find("Camera1").transform.Find("Canvas").transform.Find("NoClip").gameObject.activeSelf);
        }
    }
}
