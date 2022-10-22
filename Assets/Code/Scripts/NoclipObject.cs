using UnityEngine;

/// <summary>
/// This class represents the basic noclip object. An invisible object with a collider that, when ActivateNoClip is
/// called, it reveals its mesh.
/// </summary>
public class NoclipObject : MonoBehaviour
{
    private bool _noclipEnabled = false;
    private Renderer _meshRender;

    private void Awake()
    {
        _meshRender = GetComponent<Renderer>();
    }
    
    /// <summary>
    /// Activate the noclip mode. For this noclip object type, it means that it will be revealed.
    /// </summary>
    public void EnableNoClip()
    {
        _meshRender.enabled = true;
        _noclipEnabled = true;
    }
    
    /// <summary>
    /// Disable the noclip mode. For this noclip object type, it means that it will disappear.
    /// </summary>
    public void DisableNoClip()
    {
        _meshRender.enabled = false;
        _noclipEnabled = false;
    }

    public void Update()
    {
        if (Application.isEditor)
        {
            // For debug purposes
            if (Input.GetKeyDown(KeyCode.N))
            {
                if (_noclipEnabled)
                {
                    DisableNoClip();
                }
                else
                {
                    EnableNoClip();
                }
            }
        }
    }
}
