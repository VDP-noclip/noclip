using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealityMovementFeedbacks : MonoBehaviour
{
    [Header("Headbob parameters")]
    [SerializeField] private float _walkBobSpeed = 12f;
    [SerializeField] private float _walkBobAmount = 0.05f;
    private float _headBobTimer;

    [Header("Footstep parameters")]
    [SerializeField] private float _stepSpeed = 2f;
    [SerializeField] private float _speedAudioActivation = 2f;
    [SerializeField] private AudioSource _audio;
    [SerializeField] private AudioClip[] _footstepClips;
    private float _footstepTimer;
    
    [Space]
    
    [SerializeField] private Camera _camera;
    private RealityMovement _realityMovement;

    private RealityMovement.MovementState _lastState;
    
    private void Awake()
    {
        _realityMovement = GetComponent<RealityMovement>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleHeadbob();
        HandleFootstep();
    }

    private void HandleHeadbob()
    {
        if (_realityMovement.IsGrounded())
        {
            if (_realityMovement.GetVelocity() > 1f)
            {
                _headBobTimer += Time.deltaTime * _walkBobSpeed;  // it could be differentiate 
                _camera.transform.localPosition = _camera.transform.localPosition + (new Vector3(0, Mathf.Sin(_headBobTimer), 0) * _walkBobAmount);
            }
        }
    }

    private void HandleFootstep()
    {
         
        if (_realityMovement.GetState() != RealityMovement.MovementState.Air)
        {
            _footstepTimer -= Time.deltaTime;
            if (_realityMovement.GetVelocity() > _speedAudioActivation && _footstepTimer < 0)
            {
                _audio.PlayOneShot(_footstepClips[Random.Range(0, _footstepClips.Length - 1)]);
                _audio.volume = Random.Range(0.8f, 1);
                _footstepTimer = _stepSpeed;
            }
            
        }
        else
        {
            _audio.Stop();
            if (_lastState == RealityMovement.MovementState.Air)
            {
                _audio.PlayOneShot(_footstepClips[Random.Range(0, _footstepClips.Length - 1)]); 
                _audio.volume = Random.Range(0.8f, 1);
                _footstepTimer = _stepSpeed;
            }
        }
        
        _lastState = _realityMovement.GetState();
        
        
    }
}
