using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A sort of middleware class that allows the player to activate/deactivate noclip, to move to the next level, etc...
/// </summary>
public class NoclipManager : MonoBehaviour
{
    private List<NoclipObjController> _noclipObjControllers;
    public bool NoclipEnabled;

    void Awake()
    {
        GameObject[] noclipObjects = GameObject.FindGameObjectsWithTag("NoclipObject");
        _noclipObjControllers = noclipObjects.Select(
            obj => obj.GetComponent<NoclipObjController>()).ToList();
    }

    public void EnableNoclip()
    {
        NoclipEnabled = true;
        _noclipObjControllers.ForEach(obj => obj.Noclip());
    }
    
    public void DisableNoclip()
    {
        NoclipEnabled = false;
        _noclipObjControllers.ForEach(obj => obj.Noclip());
    }

    public void Update()
    {
        // For debug purposes
        if (Application.isEditor && Input.GetKeyDown(KeyCode.N))
        {
            if (NoclipEnabled) EnableNoclip();
            if (!NoclipEnabled) DisableNoclip();
        }
    }
}
