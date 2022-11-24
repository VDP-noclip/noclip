using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoclipMaterialApplier : MonoBehaviour
{
    //serialize material
    [SerializeField] private Material _noclipMaterial;
    // Start is called before the first frame update
    void Start()
    {
        //for each child set material
        foreach (Transform child in transform)
        {
            child.GetComponent<Renderer>().material = _noclipMaterial;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
