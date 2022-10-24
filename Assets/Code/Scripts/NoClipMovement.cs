using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoClipMovement : MonoBehaviour
{
    [SerializeField] private CharacterController _controller;
    [SerializeField] private Transform _orientation;

    [SerializeField] private float _speed = 12f;
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private float _groundDistance = 0.4f;

    [SerializeField] private Transform _groundCheck;
    
    [SerializeField] private LayerMask _groundMask;
    
    private float _horizontalInput;
    private float _verticalInput;

    private Vector3 _moveDirection;
    
    // It's true if is the realbody, it's false if it is the noclip body
    private bool _currentPlayerBody = true;
    
    private Vector3 _velocity;
    private Transform _transform;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentPlayerBody)
        {
            bool isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);

            if (isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }

            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");

            Vector3 move = _orientation.forward * _verticalInput + _orientation.right * _horizontalInput;

            _controller.Move(move * (_speed * Time.deltaTime));

            _velocity.y += _gravity * Time.deltaTime;

            _controller.Move(_velocity * Time.deltaTime);
        }

    }
    
    public void ActivatePlayer(bool active)
    {
        _currentPlayerBody = active;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RealityObject"))
        {
            Debug.Log("RealBody collieded");
        }
    }
}