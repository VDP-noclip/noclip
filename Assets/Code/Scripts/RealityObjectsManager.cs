using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealityObjectsManager : MonoBehaviour
{
    private void Awake() {
        //set tag of all children to "RealityObject"
        foreach (Transform child in transform) {
            child.tag = "RealityObject";
        }
        //set layer of all children to "Ground"
        foreach (Transform child in transform) {
            child.gameObject.layer = LayerMask.NameToLayer("Ground");
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
