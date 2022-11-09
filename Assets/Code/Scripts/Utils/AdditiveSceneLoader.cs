using UnityEngine;
using UnityEngine.SceneManagement;

public class AdditiveSceneLoader : MonoBehaviour
{

    [SerializeField] private string[] _scenesToLoad;
    
    private void Awake()
    {
        foreach (var scene in _scenesToLoad)
        {
            SceneManager.LoadScene(scene, LoadSceneMode.Additive);
        }
    }
}