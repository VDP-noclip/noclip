using System.Collections;
using System.Collections.Generic;
using POLIMIGameCollective;
using UnityEngine;

public class RespawningManager : MonoBehaviour
{
    [SerializeField] private GameObject _realityCamera;
    [SerializeField] private GameObject _noclipCamera;

    private Transform _transform;
    private List<Transform> _childrenTransforms;
    private Vector3 _lastCheckPointPosition;
    private Quaternion _lastCheckPointRotation;
    private List<Vector3> _lastCheckPointChildrenPositions = new();
    private List<Quaternion> _lastCheckPointChildrenRotations = new();

    private RealityMovementCalibration _realityMovement;
    private Transform _noclipCameraTransform;
    private Transform _realityCameraTransform;
    private int _realityCameraTransformIndex;
    private CameraManager _cameraManager;
    private MouseLook _realityCameraMouselook;
    private readonly float _respawnAnimationDuration = 1f;
    private NoclipManager _noclipManager;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _childrenTransforms = GetAllChildrenTransforms(_transform);
        _realityMovement = _transform.GetComponentInChildren<RealityMovementCalibration>();
        _cameraManager = GetComponent<CameraManager>();
        _realityCameraMouselook = _realityCamera.GetComponent<MouseLook>();
        _realityCameraTransform = _realityCamera.transform;
        _noclipCameraTransform = _noclipCamera.transform;
        _noclipManager = FindObjectOfType<NoclipManager>();
        
        // Get _realityCameraTransformIndex
        for (int i = 0; i < _childrenTransforms.Count; i++)
        {
            if (_childrenTransforms[i] == _realityCameraTransform)
                _realityCameraTransformIndex = i;
        }
    }

    private void Start()
    {
        UpdateCheckpointValues();
    }

    private void Update()
    {
        if (Application.isEditor)
        {
            // For debug, it should respawn everything!
            if (Input.GetKeyDown(KeyCode.Z))
            {
                RespawnAllTransforms();
            }
        }
    }

    public void RespawnAllTransforms()
    {
        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        EventManager.TriggerEvent("ResetTimeLimitConstraints");
        _noclipManager.SetAcceptUserInput(false);
        // Switch to noclip camera before starting the respawn
        _cameraManager.SwitchCamera(true);

        // Update all the children apart from the cameras
        transform.position = _lastCheckPointPosition;
        transform.rotation = _lastCheckPointRotation;

        for (int i = 0; i < _childrenTransforms.Count; i++)
        {
            if (_childrenTransforms[i] == _noclipCameraTransform || _childrenTransforms[i] == _realityCameraTransform)
                continue;

            _childrenTransforms[i].position = _lastCheckPointChildrenPositions[i];
            _childrenTransforms[i].rotation = _lastCheckPointChildrenRotations[i];
        }
        
        // Get target position for noclip camera
        Vector3 targetPosition = _lastCheckPointChildrenPositions[_realityCameraTransformIndex];
        // We need an instantaneous change to get the proper targetangle => no rely on an event
        _realityCameraMouselook.SetLastCheckpointRotation();
        Quaternion targetAngle = _realityCameraTransform.rotation;
        
        // Noclip camera animation to return to the last CP position
        float timeElapsed = 0;
        Vector3 startPosition = _noclipCameraTransform.position;
        Quaternion startAngle = _noclipCameraTransform.rotation;
        while (timeElapsed < _respawnAnimationDuration)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / _respawnAnimationDuration;
            _noclipCameraTransform.position = Vector3.Lerp(startPosition,  targetPosition, t);
            _noclipCameraTransform.rotation = Quaternion.Lerp(startAngle, targetAngle, t);
            yield return new WaitForEndOfFrame();
        }

        // Switch to reality camera after the animation is complete
        _cameraManager.SwitchCamera(false);
        _realityMovement.ResetSpeedOnRespawn();
        EventManager.TriggerEvent("StartTimeConstraintsTimer");
        _noclipManager.SetAcceptUserInput(true);
        yield return null;
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