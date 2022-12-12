using System.Collections;
using System.Collections.Generic;
using POLIMIGameCollective;
using UnityEngine;

public class RespawningManager : MonoBehaviour
{
    private Transform _transform;

    private List<Transform> _childrenTransforms;
    
    private RealityMovementCalibration _realityMovement;

    private Vector3 _lastCheckPointPosition;
    private Quaternion _lastCheckPointRotation;
    private List<Vector3> _lastCheckPointChildrenPositions = new();
    private List<Quaternion> _lastCheckPointChildrenRotations = new();

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _childrenTransforms = GetAllChildrenTransforms(_transform);
        _realityMovement = _transform.GetComponentInChildren<RealityMovementCalibration>();

        Debug.Log($"Found {_childrenTransforms.Count} children!");

        UpdateCheckpointValues();
    }

    private void Update()
    {
        if (Application.isEditor)
        {
            // For debug, it should respawn everything!
            if (Input.GetButtonDown("DebugRespown"))
            {
                RespawnAllTransforms();
            }
        }
    }


    public void RespawnAllTransforms()
    {
        //find RealityPlayer and get component NoclipManager, call NoclipRespawnSequence
        GameObject realityPlayer = GameObject.Find("RealityPlayer");
        if (realityPlayer != null)
        {
            //Debug.LogError("Found RealityPlayer");
            NoclipManager noclipManager = realityPlayer.GetComponent<NoclipManager>();
            if (noclipManager != null)
            {
                //Debug.LogError("Found NoclipManager");
                noclipManager.NoclipRespawnSequence();
            }
        }
        _transform.position = _lastCheckPointPosition;
        _transform.rotation = _lastCheckPointRotation;

        for (int i = 0; i < _childrenTransforms.Count; i++)
        {
            //if _childrenTransforms[i] name is NoclipCamera skip
            if (_childrenTransforms[i].name == "NoclipCamera" || _childrenTransforms[i].name == "NoclipPlayer" || _childrenTransforms[i].name == "RealityCamera"){
                //Debug.LogError("Skipping NoclipCamera and player");
                continue;
            }
            _childrenTransforms[i].position = _lastCheckPointChildrenPositions[i];
            _childrenTransforms[i].rotation = _lastCheckPointChildrenRotations[i];
        }

        _realityMovement.ResetSpeedOnRespawn();
        EventManager.TriggerEvent("SetLastCheckpointRotation");
        EventManager.TriggerEvent("ResetTimeLimitConstraints");
        EventManager.TriggerEvent("StartTimeConstraintsTimer");
    }

    /// <summary>
    /// Store a copy of current position and rotations of the current game object and of all its children.
    /// </summary>
    public void UpdateCheckpointValues()
    {
        StartCoroutine(UpdateCpValuesCoroutine());
    }

    private IEnumerator UpdateCpValuesCoroutine()
    {
        _lastCheckPointPosition = _transform.position;
        _lastCheckPointRotation = _transform.rotation;

        _lastCheckPointChildrenRotations = new List<Quaternion>();
        _lastCheckPointChildrenPositions = new List<Vector3>();

        foreach (Transform childTransform in _childrenTransforms)
        {
            _lastCheckPointChildrenPositions.Add(childTransform.position);
            _lastCheckPointChildrenRotations.Add(childTransform.rotation);
        }

        EventManager.TriggerEvent("StoreCheckpointRotation");

        yield return null;
    }

    /// <summary>
    /// Get a list with all the transforms of the children of this gameobject.
    /// </summary>
    private List<Transform> GetAllChildrenTransforms(Transform _t)
    {
        List<Transform> ts = new List<Transform>();

        foreach (Transform t in _t)
        {
            ts.Add(t);
            if (t.childCount > 0)
                ts.AddRange(GetAllChildrenTransforms(t));
        }

        return ts;
    }
}