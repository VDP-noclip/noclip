using UnityEngine;

/// <summary>
/// This class represents the basic noclip object. An invisible object with a collider that, when ActivateNoClip is
/// called, it reveals its mesh.
/// </summary>
public class NoclipObjController : MonoBehaviour
{
    private bool _noclipEnabled = false;
    private Renderer _meshRender;

    private void Awake()
    {
        _meshRender = GetComponent<Renderer>();
        GetComponent<MeshRenderer>().enabled = false;
    }
    
    /// <summary>
    /// Activate or deactivate the noclip mode. For this noclip object type, it means that it will be revealed.
    /// </summary>
    public void Noclip()
    {
        if (_noclipEnabled)
        {
            _meshRender.enabled = false;
            _noclipEnabled = false;
        }
        else
        {
            _meshRender.enabled = true;
            _noclipEnabled = true;
        }
    }

}
