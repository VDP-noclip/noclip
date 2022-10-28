using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RespawningManager : MonoBehaviour
{
    private Transform _transform;
    private List<Transform> _childrenTransforms;

    private Vector3 _lastCheckPointPosition;
    private Quaternion _lastCheckPointRotation;
    private List<Vector3> _lastCheckPointChildrenPositions = new();
    private List<Quaternion> _lastCheckPointChildrenRotations = new();

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _childrenTransforms = GetAllChildrenTransforms(_transform);
        
        Debug.Log($"Found {_childrenTransforms.Count} children!");
        
        UpdateCheckpointValues();
    }

    private void Update()
    {
        if (Application.isEditor)
        {
            // For debug, it should respawn everything!
            if (Input.GetKeyDown(KeyCode.F))
            {
                RespawnAllTransforms();
            }
        }
    }


    public void RespawnAllTransforms()
    {
        _transform.position = _lastCheckPointPosition;
        _transform.rotation = _lastCheckPointRotation;
        
        for (int i = 0; i < _childrenTransforms.Count; i++)
        {
            _childrenTransforms[i].position = _lastCheckPointChildrenPositions[i];
            _childrenTransforms[i].rotation = _lastCheckPointChildrenRotations[i];
        }
    }

    /// <summary>
    /// Store a copy of current position and rotations of the current game object and of all its children.
    /// </summary>
    public void UpdateCheckpointValues()
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
