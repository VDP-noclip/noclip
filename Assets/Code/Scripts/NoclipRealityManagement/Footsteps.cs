using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Footsteps : MonoBehaviour
{
    [SerializeField] private float _audioVelocity = 1f;
    [SerializeField] private float _audioStart = 2f;
    private RealityMovement _realityMovement;
    private AudioSource _audioClip;

    private void Awake()
    {
        _realityMovement = GetComponent<RealityMovement>();
        _audioClip = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_realityMovement.IsGrounded() && _realityMovement.GetVelocity() > _audioStart && _audioClip.isPlaying == false)
        {
            _audioClip.pitch = _audioVelocity;
            _audioClip.volume = Random.Range(0.8f, 1f);
            _audioClip.Play();
        }
    }
}
