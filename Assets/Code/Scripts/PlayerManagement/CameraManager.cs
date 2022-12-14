using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    
    [SerializeField] private GameObject _realPlayer;
    [SerializeField] private GameObject _realPlayerCamera;
    private RealityMovementCalibration _realityMovement;
    private MouseLook _realMouseLook;
    
    [SerializeField] private GameObject _noclipCamera;
    private NoclipMovement _noclipMovement;
    private MouseLook _noclipMouseLook;

    private NoclipManager _noclipManager;
    private void Awake()
    {
        _noclipMovement = _noclipCamera.GetComponent<NoclipMovement>();
        _realityMovement = _realPlayer.GetComponent<RealityMovementCalibration>();
        
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
    
    public void SwitchCamera(bool? noclipEnabledOverrideValue=null)
    {
        bool isNoclipEnabled = _noclipManager.IsNoclipEnabled();
        if (noclipEnabledOverrideValue != null)
        {
            isNoclipEnabled = (bool) noclipEnabledOverrideValue;
        }
        
        
        //Activate/disactivate the realPlayer and his camera
        _realPlayerCamera.SetActive(!isNoclipEnabled);
        _realMouseLook.ActivateMouseLook(!isNoclipEnabled);
        
        if (isNoclipEnabled) //When the switch from reality mode to noclip mode happened
        {
            
            _noclipMovement.SetPositionAndRotation(_realPlayerCamera.transform.position, _realPlayerCamera.transform.rotation); //Set the noclip position in the realBody position
            _noclipMouseLook.CopyRotationCoordinates(_realMouseLook);
        }
        _realityMovement.toggleKinematic(isNoclipEnabled);

        
        //Activate/disactivate the noclipPlayer and his camera
        _noclipCamera.SetActive(isNoclipEnabled);
        _noclipMouseLook.ActivateMouseLook(isNoclipEnabled);
        
    }
    
}
