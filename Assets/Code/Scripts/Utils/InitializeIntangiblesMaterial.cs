using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeIntangiblesMaterial : MonoBehaviour
{
    //Material
    [SerializeField] private Material _intangibleMaterial;
    // Start is called before the first frame update
    void Start()
    {
        //set the material to all children
        foreach (Transform child in transform)
        {
            //print setting intangible material
            Debug.Log("Setting intangible material");
            child.gameObject.GetComponent<Renderer>().material = _intangibleMaterial;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
