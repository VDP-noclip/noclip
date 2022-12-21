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
    private Texture _tex;
    private Color _col;
    private bool _finished = false;
    private Material _originalMaterial;
    private float _originalAlpha = 1f;
    private float _prevAlpha = 1f;

    void Start()
    {
        //foreach material in GetComponent<Renderer>().materials
        foreach(Material material in GetComponent<Renderer>().materials){
            //store material texture into variable
            _tex = material.mainTexture;
            //create copy of material
            _originalMaterial = new Material(material);
            _originalAlpha = _originalMaterial.color.a;
            _col = material.color;
            //if object tag is Background set finished to true
            if (gameObject.tag == "Background")
            {
                _finished = true;
                return;
            }
            ChangeMaterialTransparency(true, material);
            material.color = new Color(material.color.r, material.color.g, material.color.b, 0f);
            material.SetInt("_ZWrite", 1);
        }
    }
    
    //fixedupdate
    void FixedUpdate()
    {
        //foreach material in GetComponent<Renderer>().materials
        foreach(Material material in GetComponent<Renderer>().materials){
            if(!_finished){
                if (material.color.a < _originalAlpha)
                {
                    _prevAlpha = material.color.a;
                    //make mesh more opaque
                    material.color = new Color(material.color.r, material.color.g, material.color.b, material.color.a + _fadeSpeed);
                }
                //else switch to ProBuilder_yellow
                else if (!_finished)
                {
                    _finished = true;
                    if(Untampered(material)){
                        material.CopyPropertiesFromMaterial(_originalMaterial);
                    }
                    //if father of object is named IntangibleNoclipObjectsHolder set alpha to NoclipIntangibleController GetNoclipMaterials
                    if(transform.parent != null && transform.parent.name == "IntangibleNoclipObjectsHolder"){
                        material.color = new Color(material.color.r, material.color.g, material.color.b, GetComponent<NoclipIntangibleController>().GetNoclipMaterials()[0].color.a);
                    }
                }
            }
            if(_finished){
                Material currentMaterial = material;
                if(currentMaterial.name == _originalMaterial.name && currentMaterial.color.a != _originalAlpha){
                    Debug.Log("Fixing fade in");
                    material.CopyPropertiesFromMaterial(_originalMaterial);
                }
            }
        }
    }

    private bool Untampered(Material material){
        return material.color.a == _prevAlpha;
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