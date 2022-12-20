using UnityEngine;

/// <summary>
/// An object that is visible but intangible. It disappears when noclip mode is enabled.
/// </summary>
public class NoclipIntangibleController : BaseNoclipObjectController
{
    private Renderer _meshRender;
    //material list
    [SerializeField] private Material[] _noclipMaterials;
    [SerializeField] private Material[] _intangibleMaterials;

    private void Awake()
    {
        _meshRender = GetComponent<Renderer>();
        DisableNoclip();
    }

    private void Start(){
    }

    public override void ActivateNoclip()
    {
        _meshRender.materials = _noclipMaterials;
    }

    public override void DisableNoclip()
    {
        _meshRender.materials = _intangibleMaterials;
    }
    
    //Getnoclipmaterial function
    public Material[] GetNoclipMaterials()
    {
        return _noclipMaterials;
    }

}