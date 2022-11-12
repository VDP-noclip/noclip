using System;
using Unity.VisualScripting;
using UnityEngine;

    /// <summary>
    /// Everything here is linked to how cursor input is processed and used.
    /// Rotation, orientation, sensitivity handling and eventual added preferences such as inverted vertical axis.
    /// </summary>
public class MouseLook3rdPerson : MonoBehaviour
{
    //camera of the parent
    private Transform _camera;
    //player
    private GameObject _player;
    void Start()
    {
        //initialize camera
        _camera = transform.parent.GetComponentInChildren<Camera>().transform;
        //initialize player to parent of parent
        _player = GameObject.Find("RealityPlayer");
    }
    void Update()
    {
        //move camera to the center of the player
        _camera.position = _player.transform.position;
        //get direction of the camera
        Vector3 direction = _camera.forward;
        //move camera back in direction
        _camera.position -= direction * 3;
        //rotate camera to look at player
        _camera.LookAt(_player.transform);
    }
}
