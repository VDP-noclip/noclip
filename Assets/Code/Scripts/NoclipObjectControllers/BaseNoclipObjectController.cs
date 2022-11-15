using UnityEngine;

/// <summary>
/// This class represents the basic noclip object. It exposes the DisableNoclip and ActivateNoclip methods.
/// </summary>
public abstract class BaseNoclipObjectController : MonoBehaviour
{

    public abstract void ActivateNoclip();

    public abstract void DisableNoclip();
}