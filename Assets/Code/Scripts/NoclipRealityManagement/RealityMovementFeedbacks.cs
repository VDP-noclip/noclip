using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealityMovementFeedbacks : MonoBehaviour
{
    [Header("Headbob parameters")]
    [SerializeField] private float _headbobMultiplier = 0.05f;
    private float _headBobTimer;

    [Header("Footstep parameters")]
    [SerializeField] private float _stepSpeed = 2f;
    [SerializeField] private float _speedAudioActivation = 2f;
    [SerializeField] private AudioSource _audio;
    [SerializeField] private AudioClip[] _footstepClips;
    [SerializeField] private AudioClip _landSound;
    private float _footstepTimer;

    private float _moveSpeed;

    private Vector3 _cameraPosition;

    [Space]
    
    [SerializeField] private Camera _camera;
    private RealityMovementCalibration _realityMovementCalibration;

    private RealityMovementCalibration.MovementState _lastState;
    
    private void Awake()
    {
        _realityMovementCalibration = GetComponent<RealityMovementCalibration>();
        _cameraPosition = _camera.transform.localPosition;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _moveSpeed = _realityMovementCalibration.GetVelocity();
        HandleHeadbob();
        HandleFootstep();
    }

    /// <summary>
    /// This function handles the headbob of the reality player
    /// </summary>
    private void HandleHeadbob()
    {
        if (_realityMovementCalibration.IsGrounded())
        {
            if (_moveSpeed > 1f)
            {
                // The time is incremented each time that the camera moves
                _headBobTimer += Time.deltaTime * _realityMovementCalibration.GetMaxVelocity();  // This speed changes related to the reality player state
                // The position of the camera change on the y axis in order to do an up and down. The maximum difference is managed by the multiplier
                _camera.transform.localPosition = _cameraPosition + (new Vector3(0, Mathf.Sin(_headBobTimer), 0) * _headbobMultiplier);
            }
        }
    }

    /// <summary>
    /// This function handles the footstep sound
    /// </summary>
    private void HandleFootstep()
    {
        // If the reality player is in contact with an object of the ground layer
        if (_realityMovementCalibration.GetState() != RealityMovementCalibration.MovementState.Air)
        {
            _footstepTimer -= Time.deltaTime * _realityMovementCalibration.GetMaxVelocity();
            if (_moveSpeed > _speedAudioActivation && _footstepTimer < 0)
            {
                _audio.PlayOneShot(_footstepClips[Random.Range(0, _footstepClips.Length - 1)]);
                _audio.volume = Random.Range(0.8f, 1);
                _footstepTimer = _stepSpeed;
            }
            
            // if the player go from a state of air to a state of ground. so this is the sound when the player land on the ground
            if (_lastState == RealityMovementCalibration.MovementState.Air) 
            {
                _audio.PlayOneShot(_landSound); 
                _audio.volume = 1.5f;
            }
            
        }
        else // if the reality player is in air
        {
            _audio.Stop();
        }
        
        _lastState = _realityMovementCalibration.GetState();
        
        
    }
}
