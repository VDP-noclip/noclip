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
    
    public void SwitchCamera()
    {
        bool isNoclipEnabled = _noclipManager.IsNoclipEnabled();
        
        //Activate/disactivate the realPlayer and his camera
        _realPlayerCamera.SetActive(!isNoclipEnabled);
        _realMouseLook.ActivateMouseLook(!isNoclipEnabled);
        
        if (isNoclipEnabled) //When the switch from reality mode to noclip mode happened
        {
            
            _noclipMovement.SetPositionAndRotation(_realPlayerCamera.transform.position, _realPlayerCamera.transform.rotation); //Set the noclip position in the realBody position
            _noclipMouseLook.CopyRotationCoordinates(_realMouseLook);
            _realityMovement.toggleKinematic(isNoclipEnabled);
        }
        else
        {
            _realityMovement.toggleKinematic(isNoclipEnabled);
            // when going back to reality the camera should be oriented like the noclip camera
            // regardless, because now with the animation noclip camera orientation is already ok
            // right before the switch, and also if you noclip and look around without moving
            // you should still point in that direction when leaving noclip
            // 
            // it is difficult to achieve this result because the animation back to reality
            // does not use _xRotation and _yRotation, but it still manages to change the orientation
            // thanks to the untampered method that makes the MouseLook ignore them.
            // it works just because _realMouseLook in this case is never enforced from outside,
            // in fact symmetry would suggest to use Untampered() also above, but that way real mouse
            // will remain tampered forever. 

            //_realMouseLook.transform.rotation = _noclipMouseLook.transform.rotation;
            //if(_noclipMouseLook.Untampered())
            //if noclipcamera is close to realcamera
            
            //VERY dirty fix
            if (Vector3.Distance(_realPlayerCamera.transform.position, _noclipCamera.transform.position) < 0.01f)
                _realMouseLook.CopyRotationCoordinates(_noclipMouseLook);
        }
        
        //Activate/disactivate the noclipPlayer and his camera
        _noclipCamera.SetActive(isNoclipEnabled);
        _noclipMouseLook.ActivateMouseLook(isNoclipEnabled);

        
    }
}
