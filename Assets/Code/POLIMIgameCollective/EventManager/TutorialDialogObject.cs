namespace Code.POLIMIgameCollective.EventManager
{
    public class TutorialDialogObject
    {
        private readonly string _dialog;
        private readonly bool _slowDown;
        private readonly bool _highlightCrosshair;
        private readonly float _timePerLetter;

        public TutorialDialogObject(string dialog, float timePerLetter, bool slowDown, bool highlightCrosshair)
        {
            _dialog = dialog;
            _slowDown = slowDown;
            _highlightCrosshair = highlightCrosshair;
            _timePerLetter = timePerLetter;
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
    }
}