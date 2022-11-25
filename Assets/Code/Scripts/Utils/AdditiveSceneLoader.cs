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

    private void Start()
    {
        //PuzzleManager puzzleManager = GameObject.Find("PuzzleManager").GetComponent<PuzzleManager>();
        //if (puzzleManager)
        //    puzzleManager.LoadNextPuzzle();
        //else
        //    Debug.LogError("Did not find a puzzle manager, are you sure on the Scene setup?");
    }
}