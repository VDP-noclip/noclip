using UnityEngine;

/// <summary>
/// An object that is visible but intangible. It disappears when noclip mode is enabled.
/// </summary>
public class NoclipIntangibleController : BaseNoclipObjectController
{
    private Renderer _meshRender;
    [SerializeField] private Material _noclipMaterial;
    [SerializeField] private Material _intangibleMaterial;

    private void Awake()
    {
        _meshRender = GetComponent<Renderer>();
        DisableNoclip();
    }

    private void Start(){
    }

    public override void ActivateNoclip()
    {
        _meshRender.material = _noclipMaterial;
    }

    public override void DisableNoclip()
    {
        _meshRender.material = _intangibleMaterial;
    }
    
    //Getnoclipmaterial function
    public Material[] GetNoclipMaterials()
    {
        return new[] {_noclipMaterial};
    }
}