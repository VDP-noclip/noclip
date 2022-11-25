using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedNoclipMaterial : MonoBehaviour
{
    [SerializeField] private Material _noclipMaterial;
    private Material _realityMaterial;
    // Start is called before the first frame update
    void Start()
    {
        _realityMaterial = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetNoclipMaterial(Material noclipMaterial)
    {
        _noclipMaterial = noclipMaterial;
    }

    public void SwitchMaterial(bool noclip)
    {
        if (noclip)
        {
            GetComponent<Renderer>().material = _noclipMaterial;
        }
        else
        {
            GetComponent<Renderer>().material = _realityMaterial;
        }
    }
}
