using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//import application
using UnityEngine.UI;

public class BlackFadein : MonoBehaviour
{
    private int _fadeIn = -1;
    private bool _fading = false;
    [SerializeField] private float _fadeTime = 1.0f;
    
    private RespawningManager _respawningManager;
    private UnityEngine.UI.Image _image;
    // Start is called before the first frame update
    void Start()
    {
        _image = GetComponent<UnityEngine.UI.Image>();
        //find AllPlayer gameobject
        GameObject allPlayer = GameObject.Find("AllPlayer");
        //get RespawningManager script from AllPlayer
        _respawningManager = allPlayer.GetComponent<RespawningManager>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //if b is pressed toggle fade of this object
        if (Application.isEditor && Input.GetKeyDown(KeyCode.B))
        {
            _fading = true;
            _fadeIn = 1;
        }

        if (_fading)
        {
            Color color = _image.color;
            color.a += _fadeIn * Time.deltaTime / _fadeTime;
            if (color.a >= 1.0f)
            {
                color.a = 1.0f;
                _fading = true;
                _fadeIn = -1;
                _respawningManager.IstantaneousRespawn();
            }
            else if (color.a <= 0.0f)
            {
                color.a = 0.0f;
                _fading = false;
            }
            _image.color = color;
        }
    }
}
