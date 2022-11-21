using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIn2 : MonoBehaviour
{
    private float _fadeSpeed = 0.02f; //透明化の速さ
    // Awake is called when the script instance is being loaded.
    void Awake()
    {
        GetComponent<Renderer>().material.color = new Color(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g, GetComponent<Renderer>().material.color.b, 0f);
        GetComponent<Renderer>().material.CopyPropertiesFromMaterial(Resources.Load("Materials/ProBuilder_yellow_transparent", typeof(Material)) as Material);
        //zwrite 0
        GetComponent<Renderer>().material.SetInt("_ZWrite", 1);
        //no shadows ):
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
        if (GetComponent<Renderer>().material.color.a < 1)
        {
            //make mesh more opaque
            GetComponent<Renderer>().material.color = new Color(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g, GetComponent<Renderer>().material.color.b, GetComponent<Renderer>().material.color.a + _fadeSpeed);
        }
        //else switch to ProBuilder_yellow
        else
        {
            GetComponent<Renderer>().material.SetInt("_ZWrite", 1);
            GetComponent<Renderer>().material.CopyPropertiesFromMaterial(Resources.Load("Materials/ProBuilder_yellow", typeof(Material)) as Material);
        }
    }
}
