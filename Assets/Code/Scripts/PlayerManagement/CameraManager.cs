using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    
    [SerializeField] private GameObject _realPlayer;
    [SerializeField] private GameObject _realPlayerCamera;
    private MouseLook _realMouseLook;
    
    [SerializeField] private GameObject _noclipCamera;
    private NoclipMovement _noclipMovement;
    private MouseLook _noclipMouseLook;

    private NoclipManager _noclipManager;
    private void Awake()
    {
        _noclipMovement = _noclipCamera.GetComponent<NoclipMovement>();

        _realMouseLook = _realPlayerCamera.GetComponent<MouseLook>();
        _noclipMouseLook = _noclipCamera.GetComponent<MouseLook>();
        
        _noclipManager = FindObjectOfType<NoclipManager>();

    }

    // Start is called before the first frame update
    void Start()
    {
        bool isNoclipEnabled = _noclipManager.IsNoclipEnabled();
        
        _realPlayerCamera.SetActive(!isNoclipEnabled);
        _realMouseLook.ActivateMouseLook(!isNoclipEnabled);
        
        _noclipCamera.SetActive(isNoclipEnabled);
        _noclipMouseLook.ActivateMouseLook(isNoclipEnabled);
    }
    
    public void SwitchCamera()
    {
        bool isNoclipEnabled = _noclipManager.IsNoclipEnabled();
        
        //Activate/disactivate the realPlayer and his camera
        _realPlayerCamera.SetActive(!isNoclipEnabled);
        _realMouseLook.ActivateMouseLook(!isNoclipEnabled);
        
        if (isNoclipEnabled) //When the switch from reality mode to noclip mode happened
        {
            _noclipMovement.SetPositionAndRotation(_realPlayer.transform.position, _realPlayerCamera.transform.rotation); //Set the noclip position in the realBody position
        }
        
        //Activate/disactivate the noclipPlayer and his camera
        _noclipCamera.SetActive(isNoclipEnabled);
        _noclipMouseLook.ActivateMouseLook(isNoclipEnabled);

        
    }
}
