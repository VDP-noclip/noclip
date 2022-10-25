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
    
    [SerializeField] private GameObject _noClipPlayerCamera;
    private MouseLook _noClipMouseLook;


    [SerializeField] private KeyCode _activation = KeyCode.P;

    private bool _activeRealPlayer;
    private void Awake()
    {
        _realPlayerMovement = _realPlayer.GetComponent<RealityMovement>();

        _realMouseLook = _realPlayerCamera.GetComponent<MouseLook>();
        _noClipMouseLook = _noClipPlayerCamera.GetComponent<MouseLook>();

    }

    // Start is called before the first frame update
    void Start()
    {
        _realPlayerCamera.SetActive(true);
        _realPlayerMovement.ActivatePlayer(true);
        _realMouseLook.ActivateMouseLook(true);
        
        _noClipPlayerCamera.SetActive(false);
        _noClipMouseLook.ActivateMouseLook(false);
        
        _activeRealPlayer = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(_activation))
        {
            _realPlayerCamera.SetActive(!_activeRealPlayer);
            _realPlayerMovement.ActivatePlayer(!_activeRealPlayer);
            _realMouseLook.ActivateMouseLook(!_activeRealPlayer);
            
            _noClipPlayerCamera.SetActive(_activeRealPlayer);
            _noClipMouseLook.ActivateMouseLook(_activeRealPlayer);
            
            _activeRealPlayer = !_activeRealPlayer;
        }
    }
}
