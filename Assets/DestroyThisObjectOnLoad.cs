using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyThisObjectOnLoad : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake(){
        //destroy this object
        Destroy(gameObject);
    }
}
