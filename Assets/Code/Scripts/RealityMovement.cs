using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealityMovement : MonoBehaviour
{
    [SerializeField] private CharacterController controller;

    [SerializeField] private float _speed = 12f;
    [SerializeField] private float _playerWeight = 1f;
    [SerializeField] private float _jumpHeight = 3f;
    [SerializeField] private float _groundDistance = 0.4f;
    [SerializeField] private float _jumpMovementLimitationRate = 10;
    [SerializeField] private Transform _groundCheck; //This object is at the bottom of the player object, it's needed to check if the player is jumping or not
    [SerializeField] private LayerMask _groundMask;
    
    private float _gravity = -9.81f;
    private Vector3 _velocity;
    private Transform _transform;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        // isGrounded is true if there are any colliders overlapping the sphere defined by _groundCheck.position and _groundDistance in world coordinates
        bool isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);
        
        // This check stabilizes the player on the ground
        if (isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }
        
        // Take the input form the keyboard
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Vector that describe the movement relative to the player oriantation
        Vector3 move = _transform.right * x + _transform.forward * z;
        
        // The player jump with the space key
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity * _playerWeight);
        }
        
        // Calculate the velocity on y axis
        _velocity.y += _gravity * _playerWeight * Time.deltaTime;
        move.y = _velocity.y;
        
        // different type of movement at the ground and in the midair
        if (isGrounded)
        {
            controller.Move(move * (_speed * Time.deltaTime));
        }
        else
        {
            controller.Move(move/_jumpMovementLimitationRate * (_speed * Time.deltaTime));
        }

        
    }
}
