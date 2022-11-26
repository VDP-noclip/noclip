using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        try{
            GameObject player = GameObject.Find("AllPlayer").gameObject;
            //move the player to the spawn point plus 1 meter
            player.transform.position = transform.position + new Vector3(0, 1, 0);
            //rotate the player to the spawn point
            player.transform.rotation = transform.rotation;
            //disable this game object
            gameObject.SetActive(false);
        }
        catch{
            Debug.Log("Looking for player...");
        }
    }
}
