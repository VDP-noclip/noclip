using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using POLIMIGameCollective;

public class Credits : MonoBehaviour
{
    //allplayer gameobject
    private GameObject _realityBody;
    // Start is called before the first frame update
    void Start()
    {
        //_realityBody = GameObject.Find("RealityBody");
    }

    //triggerstay
    private void OnTriggerStay(Collider other)
    {
        //if _allPlayer collides with this object
        if (other.CompareTag("RealityPlayer"))
        {
            //debug log credits
            Debug.Log("Credits");
            EventManager.TriggerEvent("StartCredits");
        }
    }
    //if this object collides with _allPlayer
    /*private void OnTriggerEnter(Collider other)
    {
        //if _allPlayer collides with this object
        if (other.gameObject == _realityBody)
        {
            //debug log credits
            Debug.Log("CreditsOld");
            //EventManager.TriggerEvent("StartCredits");
        }
    }*/
}
