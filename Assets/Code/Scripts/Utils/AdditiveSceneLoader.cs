using UnityEngine;
using UnityEngine.SceneManagement;

public class AdditiveSceneLoader : MonoBehaviour
{

    [SerializeField] private string[] _scenesToLoad;

    private void Awake()
    {
        try{
            //if GameManager exists
            if (GameObject.Find("GameManager"))
            {
                //pass

            }
            else
            {
                //load 0_Main
                SceneManager.LoadScene("0_Main", LoadSceneMode.Additive);
            }
        }
        catch (System.Exception)
        {
            //SceneManager.LoadScene("0_Main", LoadSceneMode.Additive);
        }
        //PuzzleManager puzzleManager = GameObject.Find("PuzzleManager").GetComponent<PuzzleManager>();
        //if (puzzleManager)
        //    puzzleManager.LoadNextPuzzle();
        //else
        //    Debug.LogError("Did not find a puzzle manager, are you sure on the Scene setup?");
        
        foreach (var scene in _scenesToLoad)
        {
            SceneManager.LoadScene(scene, LoadSceneMode.Additive);
        }
    }

    private void Start()
    {
    }
}