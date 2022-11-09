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
        DisableNoclip();
    }

    public override void ActivateNoclip()
    {
        _meshRender.enabled = false;
    }

    public override void DisableNoclip()
    {
        _meshRender.enabled = true;
    }
}