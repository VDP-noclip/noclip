﻿

using UnityEngine;
using UnityEngine.UI;

namespace Code.POLIMIgameCollective.EventManager
{
    public class TutorialDialogObject
    {
        private readonly string _dialog;
        private readonly bool _slowDown;
        private readonly bool _highlightCrosshair;
        private readonly float _timePerLetter;
        private readonly GameObject _image;

        public TutorialDialogObject(string dialog, float timePerLetter, bool slowDown, bool highlightCrosshair, GameObject image)
        {
            _dialog = dialog;
            _slowDown = slowDown;
            _highlightCrosshair = highlightCrosshair;
            _timePerLetter = timePerLetter;
            _image = image;
        }

        public string GetDialog()
        {
            return _dialog;
        }

        public float GetTotalTime()
        {
            return _timePerLetter * _dialog.Length;
        }

        public float GetTimePerLetter()
        {
            return _timePerLetter;
        }

        public bool IsSlowDown()
        {
            return _slowDown;
        }

        public bool IsCrossHairHighlighted()
        {
            return _highlightCrosshair;
        }

        public GameObject GetImage()
        {
            return _image;
        }
    }
}