using System;
using System.Collections;
using System.Collections.Generic;
using POLIMIGameCollective;
using UnityEngine;

public class FovManagement : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    #region UnityMethods

    private void Awake()
    {
        EventManager.StartListening("UpdateFovFromPlayerPrefs", UpdateFovFromPlayerPrefs);
        UpdateFovFromPlayerPrefs();
    }

    private void OnEnable()
    {
        UpdateFovFromPlayerPrefs();
    }

    #endregion
    
    private void UpdateFovFromPlayerPrefs()
    {
        _camera.fieldOfView = PlayerPrefs.GetFloat("cameraFov");
    }


}
