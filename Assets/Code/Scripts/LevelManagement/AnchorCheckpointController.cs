using System;
using POLIMIGameCollective;
using UnityEngine;

public class AnchorCheckpointController : MonoBehaviour
{
    [SerializeField] GameObject _endAnchor;
    
    [Header("If different from 0, this is the time limit to reach the new checkpoint")]
    [SerializeField] float _maxTimeToFinishPuzzle = 30f;
    
    private Renderer _anchorRenderer;
    private bool _alreadyAskedForNextPuzzle;

    private void Start()
    {
        _anchorRenderer = _endAnchor.GetComponent<Renderer>();
    }

    public void ReactToPlayerCollision()
    {
        Debug.Log("You entered end anchor: " + gameObject.name);
        EventManager.TriggerEvent("SetNewTimeLimitConstraint", _maxTimeToFinishPuzzle.ToString());
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
