using UnityEngine;

/// <summary>
/// An object that is invisible but tangible. It appears when noclip mode is enabled.
/// </summary>
public class NoclipInvisibleObjController : BaseNoclipObjectController
{
    private Renderer _meshRender;

    private void Awake()
    {
        _meshRender = GetComponent<Renderer>();
        DisableNoclip();
    }

    public override void ActivateNoclip()
    {
        _meshRender.enabled = true;
    }

    public override void DisableNoclip()
    {
        _meshRender.enabled = false;
    }
}