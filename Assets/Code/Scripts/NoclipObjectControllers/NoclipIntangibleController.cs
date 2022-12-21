using UnityEngine;
using System;

/// <summary>
/// An object that is visible but intangible. It disappears when noclip mode is enabled.
/// </summary>
public class NoclipIntangibleController : BaseNoclipObjectController
{
    private Renderer _meshRender;
    //material list
    [SerializeField] private Material[] _noclipMaterials;
    [SerializeField] private Material[] _intangibleMaterials;

    private void Awake()
    {
        _meshRender = GetComponent<Renderer>();
        //if _intangibleMaterials is null, fill it with the current materials
        if (_noclipMaterials == null)
        {
            _noclipMaterials = _meshRender.materials;
        }
        if (_intangibleMaterials == null)
        {
            //for each element in intangible materials set corresponding element in noclip materials to ChangeMaterialTransparency(false, element)
            _intangibleMaterials = new Material[_noclipMaterials.Length];
            for (int i = 0; i < _noclipMaterials.Length; i++)
            {
                //_noclipMaterials[i] = copy of _intangibleMaterials[i]
                _intangibleMaterials[i] = new Material(_noclipMaterials[i]);
                ChangeMaterialTransparency(false, _intangibleMaterials[i]);
                //log _intangibleMaterials[i].name
                _intangibleMaterials[i].name = _noclipMaterials[i].name + "_Opaque";
            }
        }
        DisableNoclip();
    }

    private void Start(){
    }

    public override void ActivateNoclip()
    {
        _meshRender.materials = _noclipMaterials;
    }

    public override void DisableNoclip()
    {
        _meshRender.materials = _intangibleMaterials;
    }
    
    //Getnoclipmaterial function
    public Material[] GetNoclipMaterials()
    {
        return _noclipMaterials;
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