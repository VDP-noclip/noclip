using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealityMovement : MonoBehaviour
{
    public CharacterController controller;

    [SerializeField] private float _speed = 12f;
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private float _jumpHeight = 3f;
    [SerializeField] private float _groundDistance = 0.4f;

    [SerializeField] private Transform _groundCheck;
    
    [SerializeField] private LayerMask _groundMask;
    
    private Vector3 _velocity;
    private Transform _transform;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        bool isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);

        if (isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = _transform.right * x + _transform.forward * z;

        controller.Move(move * (_speed * Time.deltaTime));

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
        }

        _velocity.y += _gravity * Time.deltaTime;

        controller.Move(_velocity * Time.deltaTime);
    }
}
