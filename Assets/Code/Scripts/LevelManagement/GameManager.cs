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

    
    [Tooltip("This logs a specific debug channel")]
    [SerializeField] private string _debugChannel = "unloadScenes";

    
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
        _player = InitPlayer();
        Debug.Log(_player + " player found");
        GetCheckpointIndexes();
        _checkpointIndexPointer = GetCheckpointIndexes();
        ActivateNextCheckpoint();
    }
    */
    
    // Update is called once per frame
    private void Update () {
        //if c is pressed close this scene
        if (Input.GetKeyDown(KeyCode.C))
        {
            //close this scene
            int y = SceneManager.GetActiveScene().buildIndex;
            SceneManager.UnloadSceneAsync(y);
        }
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