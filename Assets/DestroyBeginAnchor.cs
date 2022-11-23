using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBeginAnchor : MonoBehaviour
{
    void Awake()
    {
        //destroy BeginAnchor among the children of this gameobject
        Destroy(transform.Find("BeginAnchor").gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
