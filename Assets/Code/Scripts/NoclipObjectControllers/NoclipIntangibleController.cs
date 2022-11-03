using UnityEngine;

/// <summary>
/// An object that is visible but intangible. It disappears when noclip mode is enabled.
/// </summary>
public class NoclipIntangibleController : BaseNoclipObjectController
{
    private Renderer _meshRender;

    private void Awake()
    {
        _meshRender = GetComponent<Renderer>();
        disableNoclip();
    }

    protected override void activateNoclip()
    {
        _meshRender.enabled = false;
    }

    protected override void disableNoclip()
    {
        _meshRender.enabled = true;
    }
}