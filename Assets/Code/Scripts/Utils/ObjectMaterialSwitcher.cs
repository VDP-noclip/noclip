using UnityEngine;
//using exceptions
using System;

namespace Code.Scripts.Utils
{
    /// <summary>
    /// Store the reference to the object renderer and its list of materials. It allows to switch between a new list
    /// of materials and get back to the previous ones.
    /// </summary>
    public class ObjectMaterialSwitcher
    {
        private readonly Material[] _originalMaterials;
        private readonly Material[] _noclipMaterials;
        private readonly Renderer _renderer;

        public ObjectMaterialSwitcher(GameObject obj, Material[] noclipMaterials)
        {
            if (noclipMaterials == null)
            {
                var name = obj.name;
                throw new Exception(
                    $"Cannot have a ObjectMaterialSwitcher with a null _noclipMaterials. Problem in {name} object");
            }
            _noclipMaterials = noclipMaterials;
            _renderer = obj.GetComponent<Renderer>();
            _originalMaterials = _renderer.materials;
        }

        public void SetNoclipMaterials()
        {
            _renderer.materials = _noclipMaterials;
        }
        
        public void SetOriginalMaterials()
        {
            _renderer.materials = _originalMaterials;
        }
    }
}