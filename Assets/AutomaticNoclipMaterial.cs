using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticNoclipMaterial : MonoBehaviour
{
    [SerializeField] private Material _noclipMaterial;
    // Start is called before the first frame update
    void Start()
    {
        //add NoclipMaterialHolder to all children
        foreach (Transform child in transform)
        {
            child.gameObject.AddComponent<FixedNoclipMaterial>();
            child.gameObject.GetComponent<FixedNoclipMaterial>().SetNoclipMaterial(_noclipMaterial);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchMaterials(bool noclip)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.GetComponent<FixedNoclipMaterial>().SwitchMaterial(noclip);
        }
    }
}
