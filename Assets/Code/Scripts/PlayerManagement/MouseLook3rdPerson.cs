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
    private int _distance = 3;
    void Start()
    {
        //initialize camera
        _camera = transform.parent.GetComponentInChildren<Camera>().transform;
        //initialize player to parent of parent
        _player = GameObject.Find("RealityPlayer");
    }
    void Update()
    {
        //if wheel is scrolled
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            //if wheel is scrolled up
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                //if distance is greater than 1
                if (_distance > 10)
                    //set distance to 0
                    _distance-=10;
                else if (_distance > 1)
                    //decrease distance by 1
                    _distance--;
            }
            //if wheel is scrolled down
            else
            {
                if(_distance < 100)
                    //if distance is less than 10
                    if (_distance < 10)
                        //increase distance by 1
                        _distance++;
                    else
                        //increase distance by 10
                        _distance += 10;
            }
        }
        //move camera to the center of the player
        _camera.position = _player.transform.position;
        //get direction of the camera
        Vector3 direction = _camera.forward;
        //move camera back in direction
        _camera.position -= direction * _distance;
        //rotate camera to look at player
        _camera.LookAt(_player.transform);
    }
}
