using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealityMovementFeedbacks : MonoBehaviour
{
    [Header("Headbob parameters")]
    [Tooltip("Indicates the variation of the headbob movement in walking mode, corresponds to the amplitude of the sine wave")]
    [SerializeField] private float _headbobVariationSprinting = 0.06f;
    [Tooltip("Indicates the variation of the headbob movement in sprinting mode, corresponds to the amplitude of the sine wave")]
    [SerializeField] private float _headbobVariationWalking = 0.06f;
    [Tooltip("It's the frequency of  the headbob movement, high value correspond to an high frequency")]
    [SerializeField] private float _headbobFrequency = 3f;
    private float _headbobVariation;
    private float _headBobTimer;

    [Header("Footstep parameters")]
    /*[Tooltip("Indicates the time of the reproduction of the sound, high value corresponds to slow speed")]
    [SerializeField]*/ private float reproductionTime = 1f;
    [Tooltip("Indicates the minimum speed of the player for activate the sound")]
    [SerializeField] private float _speedAudioActivation = 2f;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _footstepClips;
    [SerializeField] private AudioClip _landSound;
    private float _footstepTimer;
    private float _moveSpeed;
    private Vector3 _cameraPosition;

    [Space]
    
    [SerializeField] private Camera _camera;
    private RealityMovementCalibration _realityMovementCalibration;

    private MovementState _lastState;
    
    private void Awake()
    {
        _realityMovementCalibration = GetComponent<RealityMovementCalibration>();
        _cameraPosition = _camera.transform.localPosition;
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

        if (_realityMovementCalibration.GetState() == MovementState.Walking)
        {
            _headbobVariation = _headbobVariationWalking;
        }
        else
        {
            _headbobVariation = _headbobVariationSprinting;
        }
        
        if (_realityMovementCalibration.IsGrounded())
        {
            if (_moveSpeed > 1f)
            {
                // The time is incremented each time that the camera moves
                _headBobTimer += Time.deltaTime * _realityMovementCalibration.GetMaxVelocity() * _headbobFrequency;  // This speed changes related to the reality player state
                // The position of the camera change on the y axis in order to do an up and down. The maximum difference is managed by the multiplier
                _camera.transform.localPosition = _cameraPosition + (new Vector3(0, Mathf.Sin(_headBobTimer), 0) * _headbobVariation);
            }
        }
    }

    /// <summary>
    /// This function handles the footstep sound
    /// </summary>
    private void HandleFootstep()
    {
        // If the reality player is in contact with an object of the ground layer
        if (_realityMovementCalibration.GetState() != MovementState.Air)
        {
            _footstepTimer -= Time.deltaTime * _realityMovementCalibration.GetMaxVelocity();
            //checks the minimum speed of audio activation, if the audio is still playing and if the camera is at the low position of the headbob sine wave
            if (_moveSpeed > _speedAudioActivation && _footstepTimer < 0 && _camera.transform.localPosition.y < _cameraPosition.y - _headbobVariation*0.9) 
            {
                _audioSource.PlayOneShot(_footstepClips[Random.Range(0, _footstepClips.Length - 1)]);
                _audioSource.volume = Random.Range(0.8f, 1);
                _footstepTimer = reproductionTime;
            }
            
            // if the player go from a state of air to a state of ground. so this is the sound when the player land on the ground
            if (_lastState == MovementState.Air) 
            {
                _audioSource.PlayOneShot(_landSound); 
                _audioSource.volume = 1.5f;
            }
            
        }
        else // if the reality player is in air
        {
            _audioSource.Stop();
            _footstepTimer = reproductionTime;
        }
        
        _lastState = _realityMovementCalibration.GetState(); // Saves the last state of the player because it is used for the land sound
        
        
    }
}
