using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIn : MonoBehaviour
{
    private float _fadeSpeed = 0.01f; //透明化の速さ
    // Awake is called when the script instance is being loaded.
    void Awake()
    {
        //set material to ProBuilder_yellow_transparent
        GetComponent<Renderer>().material = Resources.Load("Materials/ProBuilder_yellow_transparent", typeof(Material)) as Material;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    //fixedupdate
    void FixedUpdate()
    {
        //if mesh is not fully opaque
        if (GetComponent<MeshRenderer>().material.color.a < 1)
        {
            //make mesh more opaque
            GetComponent<MeshRenderer>().material.color = new Color(GetComponent<MeshRenderer>().material.color.r, GetComponent<MeshRenderer>().material.color.g, GetComponent<MeshRenderer>().material.color.b, GetComponent<MeshRenderer>().material.color.a + _fadeSpeed);
        }
        //else switch to ProBuilder_yellow
        else
        {
            GetComponent<Renderer>().material = Resources.Load("Materials/ProBuilder_yellow", typeof(Material)) as Material;
        }
    }
}
