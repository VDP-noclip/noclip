using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    private bool _puzzleReached = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!_puzzleReached){
            //find gameobject Puzzles
            GameObject puzzles = GameObject.Find("Puzzles");
            //print objects colliding with this object
            foreach (Collider collider in Physics.OverlapBox(transform.position, transform.localScale / 2))
            {
                //if colliding object has layer ground
                if (collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    Debug.Log("Colliding with " + collider.gameObject.name);
                    _puzzleReached = true;
                }
            }
            try{
                if(_puzzleReached){
                    GameObject player = GameObject.Find("AllPlayer").gameObject;
                    //move the player to the spawn point plus 1 meter
                    player.transform.position = transform.position + new Vector3(0, 10, 0);
                    //rotate the player to the spawn point
                    player.transform.rotation = transform.rotation;
                    //update mouselook
                    player.GetComponentInChildren<MouseLook>().SyncYRotation();
                    //get respawningmanager in player and call update checkpoints
                    player.GetComponent<RespawningManager>().UpdateCheckpointValues();
                    gameObject.SetActive(false);
                }
                else{
                    //find level manager
                    LevelManager levelManager = puzzles.GetComponent<LevelManager>();
                    //unlock next puzzle
                    levelManager.LoadNextPuzzle();
                }
            }
            catch{
                Debug.Log("Looking for player...");
            }
        }
    }
}
