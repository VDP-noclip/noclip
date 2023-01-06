using System;
using System.Collections;
using System.Collections.Generic;
using POLIMIGameCollective;
using UnityEngine;

public class FovManagement : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    #region UnityMethods
    private void Update()
    {
       _camera.fieldOfView = PlayerPrefs.GetFloat("cameraFov");
    }

    #endregion
}
