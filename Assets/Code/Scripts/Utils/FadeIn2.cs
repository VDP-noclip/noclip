using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIn2 : MonoBehaviour
{
    [SerializeField] private float _fadeSpeed = 0.02f; //透明化の速さ
    private Texture _tex;
    private Color _col;
    private bool _finished = false;

    void Start()
    {
        //store material texture into variable
        if (gameObject.tag != "NoclipObject")
        {
            _tex = GetComponent<Renderer>().material.mainTexture;
            Renderer renderer = GetComponent<Renderer>();
            Material material = renderer.material;
            _col = material.color;
            GetComponent<Renderer>().material.CopyPropertiesFromMaterial(Resources.Load("Materials/ProBuilder_yellow_transparent", typeof(Material)) as Material);
            GetComponent<Renderer>().material.mainTexture = _tex;
            GetComponent<Renderer>().material.color = _col;
            GetComponent<Renderer>().material.color = new Color(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g, GetComponent<Renderer>().material.color.b, 0f);
            //zwrite 0
            GetComponent<Renderer>().material.SetInt("_ZWrite", 1);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    //fixedupdate
    void FixedUpdate()
    {
        if (gameObject.tag != "NoclipObject")
        {
            if (GetComponent<Renderer>().material.color.a < 1)
            {
                //make mesh more opaque
                GetComponent<Renderer>().material.color = new Color(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g, GetComponent<Renderer>().material.color.b, GetComponent<Renderer>().material.color.a + _fadeSpeed);
            }
            //else switch to ProBuilder_yellow
            else if (!_finished)
            {
                GetComponent<Renderer>().material.CopyPropertiesFromMaterial(Resources.Load("Materials/ProBuilder_yellow", typeof(Material)) as Material);
                GetComponent<Renderer>().material.mainTexture = _tex;
                GetComponent<Renderer>().material.color = _col;
                _finished = true;
            }
        }
    }
}
