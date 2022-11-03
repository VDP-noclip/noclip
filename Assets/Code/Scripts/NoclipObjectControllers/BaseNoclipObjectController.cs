using UnityEngine;

/// <summary>
/// This class represents the basic noclip object. It exposes a Noclip method to enable/disable noclip.
/// This class must be implemented by child classes that should override the disableNoclip and activateNoclip methods.
/// </summary>
public abstract class BaseNoclipObjectController : MonoBehaviour
{
    private bool _noclipEnabled;

    /// <summary>
    /// Activate or deactivate the noclip mode.
    /// </summary>
    public void Noclip()
    {
        if (_noclipEnabled)
        {
            disableNoclip();
            _noclipEnabled = false;
        }
        else
        {
            activateNoclip();
            _noclipEnabled = true;
        }
    }

    protected abstract void activateNoclip();

    protected abstract void disableNoclip();
}