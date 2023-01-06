using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//NOCLIP OBJECT CAN'T FADE IN IF THEY USE SPATIALWIREFRAME MATERIAL

public class FadeIn2 : MonoBehaviour
{
    [SerializeField] private float _fadeSpeed = 0.02f; //透明化の速さ
    //show a tooltip saying material is in Resources folder
    [SerializeField, Tooltip("Material is in Resources folder")] private string _fadeMaterialPath = "Materials/RealityPlatform";
    private Texture[] _tex;
    private Color[] _col;
    //finished array
    private bool[] _finished;
    [SerializeField] private Material[] _originalMaterial;
    private float[] _originalAlpha;
    private float[] _prevAlpha;

    void Start()
    {
        try{
            //GetComponent<MeshRenderer>().materials variable
            Material[] materials = GetComponent<MeshRenderer>().materials;
            //_finished array length = materials length
            _tex = new Texture[materials.Length];
            _col = new Color[materials.Length];
            _finished = new bool[materials.Length];
            //if size of finished is zero throw exception
            if(_finished.Length == 0){
                throw new Exception("Size of finished array is zero");
            }
            _originalMaterial = new Material[materials.Length];
            _originalAlpha = new float[materials.Length];
            _prevAlpha = new float[materials.Length];
            for(int j = 0; j < _prevAlpha.Length; j++){
                _originalAlpha[j] = 1f;
                _prevAlpha[j] = 1f;
                _finished[j] = false;
            }
            int i = 0;
            foreach(Material material in GetComponent<MeshRenderer>().materials){
                //material has property _Color
                if(!material.HasProperty("_Color") || !material.HasProperty("_MainTex") || !material.HasProperty("_AlphaClip")){
                    //throw exception
                    throw new Exception("Material " + material.name + " does not have property _Color");
                }
                //store material texture into variable
                _tex[i] = material.mainTexture;
                //create copy of material
                _originalMaterial[i] = new Material(material);
                _originalAlpha[i] = _originalMaterial[i].color.a;
                _col[i] = material.color;
                //if object tag is Background set finished to true
                if (gameObject.tag == "Background")
                {
                    _finished[i] = true;
                }
                else{
                    ChangeMaterialTransparency(true, material);
                    material.color = new Color(material.color.r, material.color.g, material.color.b, 0f);
                    material.SetInt("_ZWrite", 1);
                }
                i++;
            }
        }
        catch(Exception e){
            //log remove fadein from this object
            Debug.Log("FadeIn incompatible with " + gameObject.name);
            //disable this script
            this.enabled = false;
        }
    }
    
    //fixedupdate
    void FixedUpdate()
    {
        int i = 0;
        //foreach material in GetComponent<MeshRenderer>().materials
        foreach(Material material in GetComponent<MeshRenderer>().materials){
            if(!_finished[i]){
                if (material.color.a < _originalAlpha[i])
                {
                    _prevAlpha[i] = material.color.a;
                    //make mesh more opaque
                    material.color = new Color(material.color.r, material.color.g, material.color.b, material.color.a + _fadeSpeed);
                }
                //else switch to ProBuilder_yellow
                else if (!_finished[i])
                {
                    _finished[i] = true;
                    if(Untampered(material, i)){
                        material.CopyPropertiesFromMaterial(_originalMaterial[i]);
                    }
                    //if father of object is named IntangibleNoclipObjectsHolder set alpha to NoclipIntangibleController GetNoclipMaterials
                    //if(transform.parent != null && transform.parent.name == "IntangibleNoclipObjectsHolder"){
                    //    material.color = new Color(material.color.r, material.color.g, material.color.b, GetComponent<NoclipIntangibleController>().GetNoclipMaterials()[0].color.a);
                    //}
                }
            }
            int j = 0;
            if(_finished[i]){
                j++;
                Material currentMaterial = material;
                if(currentMaterial.name == _originalMaterial[i].name && currentMaterial.color.a != _originalAlpha[i]){
                    //Debug.Log("Fixing fade in");
                    material.CopyPropertiesFromMaterial(_originalMaterial[i]);
                }
                //if j is equal to length of _finished array
                if(j == _finished.Length){
                    //disable this script
                    this.enabled = false;
                }
            }
            i++;
        }
    }

    private bool Untampered(Material material, int i){
        return material.color.a == _prevAlpha[i];
    }

    //some black magic to change material transparency at runtime

    private enum SurfaceType
        {
            Opaque,
            Transparent
        }
    private enum BlendMode
    {
        Alpha,
        Premultiply,
        Additive,
        Multiply
    }
    
    private void ChangeMaterialTransparency(bool transparent, Material wallMaterial)
    {
        if (transparent)
        {
            wallMaterial.SetFloat("_Surface", (float)SurfaceType.Transparent);
            wallMaterial.SetFloat("_Blend", (float)BlendMode.Alpha);
        }
        else
        {
            wallMaterial.SetFloat("_Surface", (float)SurfaceType.Opaque);
        }
        SetupMaterialBlendMode(wallMaterial);
    }
    void SetupMaterialBlendMode(Material material)
    {
        if (material == null)
            throw new ArgumentNullException("material");
        bool alphaClip = material.GetFloat("_AlphaClip") == 1;
        if (alphaClip)
            material.EnableKeyword("_ALPHATEST_ON");
        else
            material.DisableKeyword("_ALPHATEST_ON");
        SurfaceType surfaceType = (SurfaceType)material.GetFloat("_Surface");
        if (surfaceType == 0)
        {
            material.SetOverrideTag("RenderType", "");
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.SetInt("_ZWrite", 1);
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = -1;
            material.SetShaderPassEnabled("ShadowCaster", true);
        }
        else
        {
            BlendMode blendMode = (BlendMode)material.GetFloat("_Blend");
            switch (blendMode)
            {
                case BlendMode.Alpha:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    material.SetShaderPassEnabled("ShadowCaster", false);
                    break;
                case BlendMode.Premultiply:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    material.SetShaderPassEnabled("ShadowCaster", false);
                    break;
                case BlendMode.Additive:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    material.SetShaderPassEnabled("ShadowCaster", false);
                    break;
                case BlendMode.Multiply:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.DstColor);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    material.SetShaderPassEnabled("ShadowCaster", false);
                    break;
            }
        }
    }
}