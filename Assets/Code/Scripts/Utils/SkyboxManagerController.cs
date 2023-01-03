using System;
using POLIMIGameCollective;
using UnityEngine;

namespace Code.Scripts.Utils
{
    public class SkyboxManagerController : MonoBehaviour
    {
        [SerializeField] private Material _realitySkybox;
        [SerializeField] private Material _noclipSkybox;

        private void Awake()
        {
            SetRealitySkybox();
        }

        private void Start()
        {
            EventManager.StartListening("SetRealitySkybox", SetRealitySkybox);
            EventManager.StartListening("SetNoclipSkybox", SetNoclipSkybox);
        }

        public void SetRealitySkybox()
        {
            RenderSettings.skybox = _realitySkybox;
        }

        public void SetNoclipSkybox()
        {
            RenderSettings.skybox = _noclipSkybox;
        }
    }
}