using System;
using UnityEngine;

public class NoclipMaterialHolder : MonoBehaviour
{
    [SerializeField] private Material[] _noclipMaterials;

    private void Start()
    {
        if (_noclipMaterials == null)
        {
            var name = gameObject.name;
            throw new Exception(
                $"Cannot have a NoclipMaterialHolder with a null _noclipMaterials. Problem in {name} object");
        }
            
    }

    public Material[] GetNoclipMaterials()
    {
        return _noclipMaterials;
    }

    public void SetMaterial(Material[] materials)
    {
        _noclipMaterials = materials;
    }
}
