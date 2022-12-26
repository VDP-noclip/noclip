using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "LoadingScene/Dialogs Options")]
public class LoadingSceneOption : ScriptableObject
{
    /// <summary>
    /// List of dialogs. It is used to set the dialogs of the loading scenes in a incremental order
    /// </summary>
    [SerializeField] public string[] _dialogs;

    /// <summary>
    /// This parameter is used to save the dialog index that will be loaded
    /// </summary>
    private int _currentScene = 0;

    /// <summary>
    /// Changes the index of the dialog
    /// </summary>
    public void ChangeSceneNumber()
    {
        _currentScene++;
    }

    /// <summary>
    /// Gets the index of the dialog 
    /// </summary>
    /// <returns></returns>
    public int GetSceneNumber()
    {
        return _currentScene;
    }
}
