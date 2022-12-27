using System;
using Code.ScriptableObjects;
using POLIMIGameCollective;
using UnityEngine;

public class AnchorCheckpointController : MonoBehaviour
{
    [SerializeField] private GameObject _endAnchor;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioTracks _audioTracks;
    [SerializeField] private GameObject _noclipEnabler;
    
    [Header("If different from 0, this is the time limit to reach the new checkpoint")]
    [SerializeField] float _maxTimeToFinishPuzzle = 30f;
    
    private bool _alreadyHitThisCheckpoint;

    private void Start()
    {
        _noclipEnabler.SetActive(false);
    }

    public void ReactToPlayerCollision()
    {
        Debug.Log("You entered end anchor: " + gameObject.name);
        EventManager.TriggerEvent("SetNewTimeLimitConstraint", _maxTimeToFinishPuzzle.ToString());
        Physics.SyncTransforms();  // needed?
        if (_alreadyHitThisCheckpoint)
            return;
        _alreadyHitThisCheckpoint = true;
        _audioSource.PlayOneShot(_audioTracks.finishPuzzle);
        GameObject.Find("Puzzles").GetComponent<LevelManager>().LoadNextPuzzle();
        _noclipEnabler.SetActive(true);
        //find save object among children of parent and disable it
        transform.parent.Find("Save").gameObject.SetActive(false);

    }
}
