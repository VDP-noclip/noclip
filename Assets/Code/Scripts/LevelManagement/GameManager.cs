using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using System.Collections.Generic;

/// <summary>
/// THIS CODE REFLECTS https://www.figma.com/file/VXmyoNeOfotAkpYbjbkXCT/noclip_ideas?node-id=423%3A1041
/// </summary>
public class GameManager : MonoBehaviour
{
    [Tooltip("At least this script has a fancy icon :-)")]
    [SerializeField] private string _gameState = "INITIALIZE_GAME";
    //list of game areas
    [SerializeField] private List<string> _gameAreas = new List<string>();
    //index of current game area
    private int _currentGameAreaIndex = 0;
    
    [Tooltip("This logs a specific debug channel")]
    [SerializeField] private string _debugChannel = "unloadScenes";
    [SerializeField] private float _gravity = 0f;

    //enum game states
    private enum GameState
    {
        INITIALIZE_GAME,
        AREA_FINISHED,
        GAME_FINISHED
    }
    void Awake()
    {
    }

    /* possibly AreaManager code
    void Awake()
    {
        _rawCheckpointControllers = FindObjectsOfType<CheckpointController>();
    }

    void Start()
    {
        CloseAllScenes();
    }
    */
    void Start()
    {
    }

    // Update is called once per frame
    private void Update () {
        //if area is finished load next area
        if(_gameState == "AREA_FINISHED")
        {
            //increase index of current game area
            _currentGameAreaIndex += 1;
            CloseAllScenes();
            DestroyOtherGameObjects();
            //load scene with name of next game area
            SceneManager.LoadScene(_gameAreas[_currentGameAreaIndex], LoadSceneMode.Additive);
            SetGameState("NEW_AREA");
        }
    }

    public void CloseAllScenes()
    {
        //get all scenes
        Scene[] scenes = SceneManager.GetAllScenes();
        //loop through all scenes
        foreach (Scene scene in scenes)
        {
            //if scene is not this scene
            if (scene.name != SceneManager.GetActiveScene().name)
            {
                //unload scene
                SceneManager.UnloadScene(scene);
            }
        }
    }

    public void DestroyOtherGameObjects()
    {
        //get all game objects
        GameObject[] gameObjects = FindObjectsOfType<GameObject>();
        //loop through all game objects
        foreach (GameObject gameObject in gameObjects)
        {
            //if game object is not this game object
            if (gameObject.name != this.gameObject.name)
            {
                //destroy game object
                Destroy(gameObject);
            }
        }
    }

    public void SetGameState(string gameState)
    {
        _gameState = gameState;
    }

    public void SetGravity(float gravity)
    {
        _gravity = gravity;
    }
    public float GetGravity()
    {
        return _gravity;
    }
/* OLD GAMEMANAGER THAT ACTUALLY WAS LEVELMANAGER THAT NOW IS SOMETHING ELSE
    
    // Channel Logger
    private void Log(string message, string channel) {
        if(channel == _debugChannel) {
            Debug.Log(message);
        }
    }

    //function to get integers from the names of checkpointControllers
    private Dictionary<int, CheckpointController> GetCheckpointIndexes()
    {
        //list of integers
        List<int> indexes = new List<int>();
        Dictionary<int, CheckpointController> validCheckpoints = new Dictionary<int, CheckpointController>();
        for (int i = 0; i < _rawCheckpointControllers.Length; i++)
        {
            //string
            string checkpointName = _rawCheckpointControllers[i].gameObject.name;
            CheckpointController checkpoint = _rawCheckpointControllers[i];
            //remove all characters except numbers
            //declare new string
            string index = "";
            index = Regex.Replace(checkpointName, "[^0-9]", "");
            //convert string index to int
            //try except
            try
            {
                int indexInt = int.Parse(index);
                //append index to array
                indexes.Add(indexInt);
                //Debug.Log("Checkpoint number " + indexes[indexes.Count - 1] + " found");
                validCheckpoints.Add(indexInt, checkpoint);
            }
            catch
            {
                //Debug.Log(_rawCheckpointControllers[i].gameObject + "is not a valid checkpoint, please rename it to contain a number");
            }
        }

        return validCheckpoints;
    }

    public void ActivateNextCheckpoint()
    {
        try
        {
            _currentCheckpointIndex++;
            //activate next checkpoint
            //_checkpointIndexPointer[_currentCheckpointIndex].ActivateCheckpoint();
            //increment checkpoint index
            Debug.Log("Activating checkpoint " + _currentCheckpointIndex);
            _gameState = "CHECKPOINT_" + _currentCheckpointIndex;
        }
        catch
        {
            Debug.Log("No more checkpoints, area is finished");
            _gameState = "END_AREA";
        }
    }

    private GameObject InitPlayer()
    {
        //find gameobject with name AllPlayer
        GameObject player = GameObject.Find("RealityPlayer");
        return player;
    }


    
    
    [SerializeField] private int _currentCheckpointIndex = 0;
    private GameObject _player;
    private Vector3 _lastCheckPointPos;
    private CheckpointController[] _rawCheckpointControllers;

    private List<CheckpointController> _checkpointControllers;

    //dictionary of int and CheckpointController
    private Dictionary<int, CheckpointController> _checkpointIndexPointer = new();
    
*/
}