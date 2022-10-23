using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner: MonoBehaviour
{
    private GameManager _gameManager;
    
    [SerializeField] private Transform player;
    
    /*
     *  PlayerSpawner handles where Player should spawn after hitting certain objects.
     *  For now, when the Player collides with a trigger object, he respawns in the last saved checkpoint.
     *  Note: checkpoints for now are volatile, meaning that closing the game will erase them.
     */
    
    void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        player.transform.position = _gameManager.lastCheckPointPos;
        Physics.SyncTransforms();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    
    private void OnTriggerEnter(Collider otherObject)
    {
        if (otherObject.gameObject.name == "RealityPlayer")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
