using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoclipMovement : MonoBehaviour
{
    

    public CharacterController controller;

    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public Vector3 respawnPosition;
    public int currentLevel = 1;
    Vector3 velocity;
    bool isGrounded;
    public bool noclip;
    Vector3 noclipPos;
    float respawnLookAngle = 135;
    Vector3 playerToBlock = new Vector3((float)2.22, (float)52.6, (float)-20.76);
    Vector3 runOffset = new Vector3((float)-4.675042, (float)-51.669827, (float)18.08);
    Material originalSkybox;
    float goBackTime = 0;
    float checkRespawnInterval = 0.5f;  
    //list of floats
    List<float> respawnLookAngles = new List<float>();
    void switchLevel(int n)
    {
        if(!noclip && currentLevel == n){
            if (closeToPosition(blockToPlayer(GameObject.Find("Respawn"+(n+1)).GetComponent<Transform>().position)))//(new Vector3((float)18.85, (float)62, (float)-20.57)))
            {   
                currentLevel = currentLevel + 1;

                nextLevel();

                //change Cube (12) material to default 1
                //GameObject.Find("Cube (12)").GetComponent<MeshRenderer>().material = GameObject.Find("Default 1").GetComponent<MeshRenderer>().material;
                //disable Cube (4)
                //GameObject.Find("Cube (4)").GetComponent<MeshRenderer>().enabled = false;
                //change Cube (12) material to material of Cube (4)
                GameObject.Find("Respawn"+(n+1)).GetComponent<MeshRenderer>().material = GameObject.Find("Cube (10)").GetComponent<MeshRenderer>().material;
                //change material of Cube (4) material of Cube (5)
                GameObject.Find("Respawn"+n).GetComponent<MeshRenderer>().material = GameObject.Find("Cube (5)").GetComponent<MeshRenderer>().material;
                respawnPosition = blockToPlayer(GameObject.Find("Respawn"+(n+1)).GetComponent<Transform>().position);
                respawnPosition.y = respawnPosition.y + 1.5f;
                //respawnLookAngle = respawnLookAngles[n];
                respawnLookAngle = 180;
                //change material of all level1 objects to material of NoclipCube3
                //change tag of all level1 objects to noclipObject
                noclipLevel(n);
            }
        }
    }

    void nextLevel(){
        toggleLevel((currentLevel)+"");
        toggleLevel((currentLevel+1)+"a");
        toggleLevel((currentLevel-1)+"a");
    }

    void noclipLevel(int n){
        GameObject[] level1Objects = GameObject.FindGameObjectsWithTag("level"+n);
        foreach (GameObject level1Object in level1Objects)
        {
            level1Object.GetComponent<MeshRenderer>().material = GameObject.Find("NoclipCube3").GetComponent<MeshRenderer>().material;
            level1Object.tag = "noclipObject";
            //disable mesh renderer of level1 objects
            level1Object.GetComponent<MeshRenderer>().enabled = false;
        }
        if(n>1){
            GameObject[] level2Objects = GameObject.FindGameObjectsWithTag("level"+(n-1)+"a");
            foreach (GameObject level2Object in level2Objects)
            {
                level2Object.GetComponent<MeshRenderer>().material = GameObject.Find("NoclipCube3").GetComponent<MeshRenderer>().material;
                level2Object.tag = "noclipObject";
                //disable mesh renderer of level2 objects
                level2Object.GetComponent<MeshRenderer>().enabled = false;
            }
            //disable mesh renderer of objects with tag nonoClipObject
            GameObject[] nonoclipObjects = GameObject.FindGameObjectsWithTag("nonoClipObject"+n);
            foreach (GameObject nonoclipObject in nonoclipObjects)
            {
                nonoclipObject.GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }

    void toggleLevel(string num)
    {
            GameObject[] levelObjects = GameObject.FindGameObjectsWithTag("level"+num);
            foreach (GameObject levelObject in levelObjects)
            {
                levelObject.GetComponent<MeshRenderer>().enabled = !levelObject.GetComponent<MeshRenderer>().enabled;
            }
            //if num does not end with character a
            if(!num.EndsWith("a")){
                GameObject[] nonoclipObjects = GameObject.FindGameObjectsWithTag("nonoClipObject"+num);
                foreach (GameObject nonoclipObject in nonoclipObjects)
                {
                    nonoclipObject.GetComponent<MeshRenderer>().enabled = !nonoclipObject.GetComponent<MeshRenderer>().enabled;
                }
            }
            /*
            GameObject[] level2Objects = GameObject.FindGameObjectsWithTag("level"+(num-1)+"a");
            foreach (GameObject level2Object in level2Objects)
            {
                level2Object.GetComponent<MeshRenderer>().enabled = !level2Object.GetComponent<MeshRenderer>().enabled;
            }*/
    }
//0.3399999 -16.61 \ 60.2-7.6 \ -4.19-1.45
//-4.675042 \ -51.669827 \ 18.08
    bool closeToPosition(Vector3 pos)
    {
        //pos = pos + playerToBlock;
        return (controller.transform.position.y < pos.y+2 && controller.transform.position.y > pos.y-1  && controller.transform.position.x > pos.x-2 && controller.transform.position.x < pos.x+2  && controller.transform.position.z > pos.z-2 && controller.transform.position.z < pos.z+2);
    }

    Vector3 blockToPlayer(Vector3 pos){
        return (pos + playerToBlock + runOffset);
    }

    // Start is called before the first frame update
    void Start()
    {
        respawnLookAngles.Add(45);
        respawnLookAngles.Add(180);
        respawnLookAngles.Add(270);
        respawnLookAngles.Add(135);
        respawnLookAngles.Add(135);
        originalSkybox = RenderSettings.skybox;
        respawnPosition = new Vector3((float)0.34, (float)60.2, (float)-4.19);
        GameObject.Find("Camera1").transform.Find("Canvas").transform.Find("NoClip").gameObject.SetActive(!GameObject.Find("Camera1").transform.Find("Canvas").transform.Find("NoClip").gameObject.activeSelf);
        GameObject[] noclipObjects = GameObject.FindGameObjectsWithTag("noclipObject");
        foreach (GameObject noclipObject in noclipObjects)
        {
            noclipObject.GetComponent<MeshRenderer>().enabled = !noclipObject.GetComponent<MeshRenderer>().enabled;
        }
        toggleLevel(2+"");
        //try except
        int i = 3;
        try
        {
            while(i<99){
                toggleLevel(i+"");
                toggleLevel(i+"a");
                GameObject[] nonoclipObject = GameObject.FindGameObjectsWithTag("nonoClipObject"+i);
                foreach (GameObject nonoclipObjects in nonoclipObject)
                {
                    nonoclipObjects.GetComponent<MeshRenderer>().enabled = false;
                }
                i++;
            }

        }
        catch (System.Exception e)
        {
            print(e);
            print("there are "+(i-1)+" levels");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //set noclip to player noclip
        noclip = GameObject.Find("Player").GetComponent<NoclipMovement>().noclip;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        if(Input.GetButtonDown("Jump"))
        {
            //velocity.y = 15 if noclip, else Mathf.Sqrt(jumpHeight * -2f * gravity)
            if(noclip)
            {
                velocity.y = 15;
            }
            else
            {
                //if isGrounded
                if(isGrounded)
                {
                    velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                }
            }

        }
        //velocity.y = 0 on button jump release
        if(Input.GetButtonUp("Jump"))
        {
            //velocity.y = 0 if noclip, else unchanged
            if(noclip)
            {
                velocity.y = 0;
            }
        }
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            //velocity.y = 15 if noclip, else unchanged
            if(noclip)
            {
                velocity.y = -15;
            }
        }
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            //velocity.y = 0 if noclip, else unchanged
            if(noclip)
            {
                velocity.y = 0;
            }
        }
        //set velocity to 0 when shift key is pressed


        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
        //if position y of controller is less than 3 set position y to 3   
        if (controller.transform.position.y < 30)
        {
            controller.transform.position = respawnPosition;
            //set xrotation of camera1 mouselook to 0
            GameObject.Find("Camera1").GetComponent<MouseLook>().xRotation = 0;
            controller.transform.rotation = Quaternion.Euler(0, respawnLookAngle, 0);
        }
        if (controller.transform.position.y > 154.1)
        {
            controller.transform.position = respawnPosition;
            GameObject.Find("Camera1").GetComponent<MouseLook>().xRotation = 0;
            controller.transform.rotation = Quaternion.Euler(0, respawnLookAngle, 0);
        }
        // 
        /*
        if(!noclip && currentLevel == 1){
            if (closeToPosition(blockToPlayer(GameObject.Find("Respawn2").GetComponent<Transform>().position)))//(new Vector3((float)18.85, (float)62, (float)-20.57)))
            {   
                currentLevel = 2;

                //call toggleLevel(2) once and not next time
                toggleLevel(2);

                //change Cube (12) material to default 1
                //GameObject.Find("Cube (12)").GetComponent<MeshRenderer>().material = GameObject.Find("Default 1").GetComponent<MeshRenderer>().material;
                //disable Cube (4)
                //GameObject.Find("Cube (4)").GetComponent<MeshRenderer>().enabled = false;
                //change Cube (12) material to material of Cube (4)
                GameObject.Find("Respawn2").GetComponent<MeshRenderer>().material = GameObject.Find("Cube (10)").GetComponent<MeshRenderer>().material;
                //change material of Cube (4) material of Cube (5)
                GameObject.Find("Respawn1").GetComponent<MeshRenderer>().material = GameObject.Find("Cube (5)").GetComponent<MeshRenderer>().material;
                respawnPosition = blockToPlayer(GameObject.Find("Respawn2").GetComponent<Transform>().position);
                respawnPosition.y = respawnPosition.y + 1.5f;
                respawnLookAngle = 180;
                //change material of all level1 objects to material of NoclipCube3
                //change tag of all level1 objects to noclipObject
                GameObject[] level1Objects = GameObject.FindGameObjectsWithTag("level1");
                foreach (GameObject level1Object in level1Objects)
                {
                    level1Object.GetComponent<MeshRenderer>().material = GameObject.Find("NoclipCube3").GetComponent<MeshRenderer>().material;
                    level1Object.tag = "noclipObject";
                    //disable mesh renderer of level1 objects
                    level1Object.GetComponent<MeshRenderer>().enabled = false;
                }
            }
        }*/
        switchLevel(currentLevel);
        if (Input.GetKeyDown(KeyCode.T))
        {
            //change material of Stairs (3)
            //GameObject.Find("Stairs (3)").GetComponent<Renderer>().material.color = Color.red;
            //if not noclip save current position
            if (!noclip)
            {
                //save current position
                //noclipPos = controller.transform.position;
                gravity = 0;
                //disable mesh of Capsule component child of controller
                controller.GetComponentInChildren<MeshRenderer>().enabled = false;
                velocity.y = 0;
                //switch skybox material
                RenderSettings.skybox = GameObject.Find("NoclipCube3").GetComponent<MeshRenderer>().material;
            }
            else
            {
                print(controller.transform.position);
                //controller.transform.position = noclipPos;
                gravity = -30f;
                controller.GetComponentInChildren<MeshRenderer>().enabled = true;
                //change player rotation y to respawnLookAngle
                controller.transform.rotation = Quaternion.Euler(0, respawnLookAngle, 0);
                GameObject.Find("Camera1").GetComponent<MouseLook>().xRotation = 0;
                //change camera1 rotation x to 0
                //GameObject.Find("Camera1").transform.rotation = Quaternion.Euler(0, 0, 0);
                RenderSettings.skybox = originalSkybox;
                //move player to respawn position
                //get current time
                goBackTime = Time.time;
                print(respawnPosition);
                controller.transform.position = respawnPosition;
                print(controller.transform.position);
            }
            noclip = !noclip;
            GameObject[] noclipObjects = GameObject.FindGameObjectsWithTag("noclipObject");
            foreach (GameObject noclipObject in noclipObjects)
            {
                noclipObject.GetComponent<MeshRenderer>().enabled = !noclipObject.GetComponent<MeshRenderer>().enabled;
            }
            GameObject[] nonoclipObjects = GameObject.FindGameObjectsWithTag("nonoClipObject"+currentLevel);
            foreach (GameObject nonoclipObject in nonoclipObjects)
            {
                nonoclipObject.GetComponent<MeshRenderer>().enabled = !nonoclipObject.GetComponent<MeshRenderer>().enabled;
            }

            MeshCollider[] children = GameObject.Find("Level").GetComponentsInChildren<MeshCollider>();

            //set isTrigger of all children of Level to true
            foreach (MeshCollider child in children)
            {
                //if owner of this meshcollider tag is not nonoclipobject
                //for i to currentlevel  print ciao
                for (int i = 1; i <= currentLevel; i++)
                {
                    if (child.gameObject.tag != "nonoClipObject" + i)
                    {
                        child.enabled = !child.enabled;
                    }
                }
            }

            //if player touches Cube (12), load objects with tag level2
            //if player is distance 1 from  18.96 62.19 -19.45 print hello
            //if (controller.transform.position.y < 154.1 + 2) & (controller.transform.position.y > 154.1 - 2) print hello world

            //toggle show of text of NoClip in Camera1 canvas
            GameObject.Find("Camera1").transform.Find("Canvas").transform.Find("NoClip").gameObject.SetActive(!GameObject.Find("Camera1").transform.Find("Canvas").transform.Find("NoClip").gameObject.activeSelf);
            GameObject.Find("Camera1").transform.Find("Canvas").transform.Find("Clip").gameObject.SetActive(!GameObject.Find("Camera1").transform.Find("Canvas").transform.Find("Clip").gameObject.activeSelf);
        }
        if(goBackTime>0){
            if(Time.time - goBackTime < checkRespawnInterval){
                controller.transform.position = respawnPosition;
                //print "sbagliato" if respawn position - controller.transform.position = 0
                    print("fixando");
                if(respawnPosition - controller.transform.position != Vector3.zero){
                    print("sbagliato");
                }
            }
            else{
                goBackTime = 0;
            }
        }
    }
}
