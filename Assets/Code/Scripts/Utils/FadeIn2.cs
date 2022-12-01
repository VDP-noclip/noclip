using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NOCLIP OBJECT CAN'T FADE IN IF THEY USE SPATIALWIREFRAME MATERIAL

public class FadeIn2 : MonoBehaviour
{
    [SerializeField] private float _fadeSpeed = 0.02f; //透明化の速さ
    //show a tooltip saying material is in Resources folder
    [SerializeField, Tooltip("Material is in Resources folder")] private string _fadeMaterialPath = "Materials/ProBuilder_yellow_transparent";
    private Texture _tex;
    private Color _col;
    private bool _finished = false;
    private Material _originalMaterial;
    private Material _transparentMaterial;
    private float _originalAlpha = 1f;
    private float _prevAlpha = 1f;

    void Start()
    {
        _transparentMaterial = Resources.Load(_fadeMaterialPath, typeof(Material)) as Material;
        //store material texture into variable
        _tex = GetComponent<Renderer>().material.mainTexture;
        Renderer renderer = GetComponent<Renderer>();
        Material material = renderer.material;
        //create copy of material
        _originalMaterial = new Material(material);
        _originalAlpha = _originalMaterial.color.a;
        _col = material.color;
        GetComponent<Renderer>().material.CopyPropertiesFromMaterial(_transparentMaterial);
        GetComponent<Renderer>().material.mainTexture = _tex;
        GetComponent<Renderer>().material.color = _col;
        GetComponent<Renderer>().material.color = new Color(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g, GetComponent<Renderer>().material.color.b, 0f);
        //zwrite 0
        GetComponent<Renderer>().material.SetInt("_ZWrite", 1);
    }

    public void Restart(){//not working
        _finished = false;
        GetComponent<Renderer>().material.CopyPropertiesFromMaterial(_transparentMaterial);
        GetComponent<Renderer>().material.mainTexture = _tex;
        GetComponent<Renderer>().material.color = _col;
        GetComponent<Renderer>().material.color = new Color(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g, GetComponent<Renderer>().material.color.b, 0f);
        GetComponent<Renderer>().material.SetInt("_ZWrite", 1);
    }
    
    //fixedupdate
    void FixedUpdate()
    {
        if(!_finished){
            if (GetComponent<Renderer>().material.color.a < _originalAlpha)
            {
                _prevAlpha = GetComponent<Renderer>().material.color.a;
                //make mesh more opaque
                GetComponent<Renderer>().material.color = new Color(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g, GetComponent<Renderer>().material.color.b, GetComponent<Renderer>().material.color.a + _fadeSpeed);
            }
            //else switch to ProBuilder_yellow
            else if (!_finished)
            {
                _finished = true;
                if(Untampered()){
                    GetComponent<Renderer>().material.CopyPropertiesFromMaterial(_originalMaterial);
                }
            }
        }
        if(_finished){
            Material currentMaterial = GetComponent<Renderer>().material;
            if(currentMaterial.name == _originalMaterial.name && currentMaterial.color.a != _originalAlpha){
                Debug.Log("Fixing fade in");
                GetComponent<Renderer>().material.CopyPropertiesFromMaterial(_originalMaterial);
            }
        }
    }

    private bool Untampered(){
        return GetComponent<Renderer>().material.color.a == _prevAlpha;
    }
}