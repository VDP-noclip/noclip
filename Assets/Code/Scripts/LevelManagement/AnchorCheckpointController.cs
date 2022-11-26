using System;
using UnityEngine;

public class AnchorCheckpointController : MonoBehaviour
{
    [SerializeField] GameObject _endAnchor;
    private Renderer _anchorRenderer;
    private bool _alreadyAskedForNextPuzzle;

    private void Start()
    {
        _anchorRenderer = _endAnchor.GetComponent<Renderer>();
    }

    public void ReactToPlayerCollision()
    {
        Debug.Log("You entered end anchor: " + gameObject.name);
        Physics.SyncTransforms();  // needed?
        if (_alreadyAskedForNextPuzzle)
            return;
        _anchorRenderer.material.color = Color.green;
        _alreadyAskedForNextPuzzle = true;
        
        GameObject.Find("Puzzles").GetComponent<LevelManager>().LoadNextPuzzle();
        //find save object among children of parent and disable it
        transform.parent.Find("Save").gameObject.SetActive(false);

    }
}
