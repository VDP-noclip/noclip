using UnityEngine;
using UnityEngine.SceneManagement;

public class AdditiveSceneLoader : MonoBehaviour
{

    [SerializeField] private string[] _scenesToLoad;

    [SerializeField] private float _ambientIntensity = 0.7f;
    [SerializeField] private float _reflectionIntensity = 1f;
    [SerializeField] private float _fogStartDistance = 100f;
    [SerializeField] private float _fogEndDistance = 400f;
    [SerializeField] private Color _fogColor = new Color(0f, 0.65f, 1f);
    [SerializeField] private FogMode _fogMode = FogMode.Linear;
    [SerializeField] private float _fogDensity = 3f;
    [SerializeField] private bool _fog = true;

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
        //set this objects scene to active
        SceneManager.SetActiveScene(gameObject.scene);
    }
}