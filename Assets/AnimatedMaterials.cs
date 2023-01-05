using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedMaterials : MonoBehaviour
{
    // serialize material field list
    [SerializeField] private List<Material> _animatedMaterials = new List<Material>();
    [SerializeField] private float _noclipSlowdownFactor = 10f;
    [SerializeField] private string _TODO = "Find object by tag instead of children";
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //get material list
    public List<Material> GetMaterialList(){
        return _animatedMaterials;
    }

    //get _noclipSlowdownFactor
    public float GetNoclipSlowdownFactor(){
        return _noclipSlowdownFactor;
    }
}
