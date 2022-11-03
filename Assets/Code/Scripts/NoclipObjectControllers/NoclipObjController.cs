using UnityEngine;

/// <summary>
/// An object that is invisible but tangible. It appears when noclip mode is enabled.
/// </summary>
// Todo: rename this class and related tag to "TangibleNoclipObject" or sth similar, upon agreement with the group
public class NoclipObjController : BaseNoclipObjectController
{
    private Renderer _meshRender;

    private void Awake()
    {
        _meshRender = GetComponent<Renderer>();
        disableNoclip();
    }

    protected override void activateNoclip()
    {
        _meshRender.enabled = true;
    }

    protected override void disableNoclip()
    {
        _meshRender.enabled = false;
    }
}