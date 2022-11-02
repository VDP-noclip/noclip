using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    
    [SerializeField] private GameObject _realPlayer;
    [SerializeField] private GameObject _realPlayerCamera;
    private RealityMovement _realPlayerMovement;
    private MouseLook _realMouseLook;
    
    [SerializeField] private GameObject _noclipPlayer;
    [SerializeField] private GameObject _noclipCamera;
    private NoclipMovement _noclipMovement;
    private MouseLook _noclipMouseLook;


    [SerializeField] private KeyCode _activation = KeyCode.P;

    //Thi boolean is true when the realPlayer is active, so the game is in the reality mode
    private bool _activeRealPlayer;
    private void Awake()
    {
        _realPlayerMovement = _realPlayer.GetComponent<RealityMovement>();
        _noclipMovement = _noclipPlayer.GetComponent<NoclipMovement>();

        _realMouseLook = _realPlayerCamera.GetComponent<MouseLook>();
        _noclipMouseLook = _noclipCamera.GetComponent<MouseLook>();

    }

    // Start is called before the first frame update
    void Start()
    {
        _realPlayerCamera.SetActive(true);
        _realPlayerMovement.ActivatePlayer(true);
        _realMouseLook.ActivateMouseLook(true);
        
        _noclipCamera.SetActive(false);
        _noclipMovement.ActivatePlayer(false);
        _noclipMouseLook.ActivateMouseLook(false);
        
        _activeRealPlayer = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(_activation))
        {
            SwitchCamera();
        }
    }
    public void SwitchCamera()
    {
        //Activate/disactivate the realPlayer and his camera
        _realPlayerCamera.SetActive(!_activeRealPlayer);
        _realPlayerMovement.ActivatePlayer(!_activeRealPlayer);
        _realMouseLook.ActivateMouseLook(!_activeRealPlayer);
        
        //Activate/disactivate the noclipPlayer and his camera
        _noclipCamera.SetActive(_activeRealPlayer);
        _noclipMovement.ActivatePlayer(_activeRealPlayer);
        _noclipMouseLook.ActivateMouseLook(_activeRealPlayer);

        if (_activeRealPlayer) //When the switch from reality mode to noclip mode happened
        {
            _noclipMovement.SetPositionAndRotation(_realPlayer.transform.position, _realPlayer.transform.eulerAngles, _realPlayerCamera.transform.eulerAngles); //Set the noclip position in the realBody position
        }

        _activeRealPlayer = !_activeRealPlayer;
        
    }
}
