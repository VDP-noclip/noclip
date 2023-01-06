using System;
using System.Collections;
using System.Collections.Generic;
using POLIMIGameCollective;
using UnityEngine;

public class FovManagement : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private float fovDefaultValue = 90f;

    #region UnityMethods
    private void Update()
    {
        _camera.fieldOfView = PlayerPrefs.GetFloat("cameraFov", fovDefaultValue);
    }

    #endregion
}
