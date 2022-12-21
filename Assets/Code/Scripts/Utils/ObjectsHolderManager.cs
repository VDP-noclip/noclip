using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



/// <summary>
/// Finds all the children of the given object and applies the selected tag and layer
/// </summary>
public class ObjectsHolderManager : MonoBehaviour
{
    [SerializeField] private string _tagToApply;
    [SerializeField] private string _layerToApply = null;

    private List<Transform> _childrenTransforms;
    
    private void Awake()
    {
        _childrenTransforms = GetAllChildrenTransforms(GetComponent<Transform>());
        
        if (_tagToApply != null)
            ApplyTagToChildren();

        if (_layerToApply != null)
            ApplyLayerToChildren();

        //disable mesh renderer of all children
        foreach (Transform child in _childrenTransforms)
        {
            //if tag is RealityObject
            if (child.tag == "RealityObject")
            {
                //if child doesn't have NoclipMaterialHolder add it
                if (!child.gameObject.GetComponent<NoclipMaterialHolder>())
                {
                    child.gameObject.AddComponent<NoclipMaterialHolder>();
                }
                child.gameObject.GetComponent<NoclipMaterialHolder>().SetMaterial(child.gameObject.GetComponent<MeshRenderer>().materials);
            }
            //add FadeIn script to all children
            child.gameObject.AddComponent<FadeIn2>();
        }
    }
    
    private List<Transform> GetAllChildrenTransforms(Transform _t)
    {
        List<Transform> ts = new List<Transform>();
 
        foreach (Transform t in _t)
        {
            ts.Add(t);
            if (t.childCount > 0)
                ts.AddRange(GetAllChildrenTransforms(t));
        }
        
        return ts;
    }

    private void ApplyTagToChildren()
    {
        #if UNITY_EDITOR
            var definedTags = UnityEditorInternal.InternalEditorUtility.tags;
            if (!definedTags.Contains(_tagToApply))
                throw new ArgumentException($"The tag '{_tagToApply}' is not in the tags list!");
        #endif
        
        //set tag of all children to _tagToApply
        foreach (Transform child in _childrenTransforms) {
            child.tag = _tagToApply;
        }
    }
    
    private void ApplyLayerToChildren()
    {
        #if UNITY_EDITOR
            var definedLayers = UnityEditorInternal.InternalEditorUtility.layers;
            if (!definedLayers.Contains(_layerToApply))
                throw new ArgumentException($"The tag '{_layerToApply}' is not in the layers list!");
        #endif
        
        //set layer of all children to _layerToApply
        foreach (Transform child in _childrenTransforms) {
            child.gameObject.layer = LayerMask.NameToLayer(_layerToApply);
        }
    }

}
