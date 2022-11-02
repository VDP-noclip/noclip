using UnityEngine;
using System.Text.RegularExpressions;
//import list
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// Basic GameManager implementation where the last checkpoint coordinate is stored.
/// The istance of a GameManager is handled, even though it seems that Unity does this automatically.
/// </summary>
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    private GameObject _player;
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(_instance);
        }
        else
        {
            Destroy(gameObject);
        }
        
        // Important: use the position of RealityPlayer and not the SpawnPlatform!
        _lastCheckPointPos = GameObject.FindGameObjectWithTag("RealityPlayer").transform.position;
        _rawCheckpointControllers = FindObjectsOfType<CheckpointController>();
    }

    void Start()
    {
        _player = InitPlayer();
        Debug.Log(_player + " player found");
        GetCheckpointIndexes();
        _checkpointIndexPointer = GetCheckpointIndexes();
        //print all checkpoints
        foreach (var checkpoint in _checkpointIndexPointer)
        {
            //Debug.Log(checkpoint);
        }
        ActivateNextCheckpoint();
    }
    //Checkpoint managing

    private Vector3 _lastCheckPointPos;
    private CheckpointController[] _rawCheckpointControllers;
    private List<CheckpointController> _checkpointControllers;
    //dictionary of int and CheckpointController
    private Dictionary<int, CheckpointController> _checkpointIndexPointer = new Dictionary<int, CheckpointController>();
    [SerializeField] private int _currentCheckpointIndex = 0;
    [SerializeField] private string _gameState = "BEGIN_AREA";
    public void SetLastCheckpointPos(Vector3 pos)
    {
        Debug.Log("Setting new checkpoint");
        _lastCheckPointPos = pos;
        Debug.Log($"Checkpoint position: {_lastCheckPointPos}");
    }

    public Vector3 GetLastCheckpointPos()
    {
        return _lastCheckPointPos;
    }

    //function to get integers from the names of checkpointControllers
    private Dictionary<int, CheckpointController> GetCheckpointIndexes(){
        //list of integers
        List<int> indexes = new List<int>();
        Dictionary<int, CheckpointController> validCheckpoints = new Dictionary<int, CheckpointController>();
        for(int i = 0; i < _rawCheckpointControllers.Length; i++){
            //string
            string checkpointName = _rawCheckpointControllers[i].gameObject.name;
            CheckpointController checkpoint = _rawCheckpointControllers[i];
            //remove all characters except numbers
            //declare new string
            string index = "";
            index = Regex.Replace(checkpointName, "[^0-9]", "");
            //convert string index to int
            //try except
            try{
                int indexInt = int.Parse(index);
            //append index to array
                indexes.Add(indexInt);
                //Debug.Log("Checkpoint number " + indexes[indexes.Count - 1] + " found");
                validCheckpoints.Add(indexInt, checkpoint);
            }
            catch{
                //Debug.Log(_rawCheckpointControllers[i].gameObject + "is not a valid checkpoint, please rename it to contain a number");
            }
        }
        return validCheckpoints;
    }

    public void ActivateNextCheckpoint(){
        try{
            _currentCheckpointIndex++;
            //activate next checkpoint
            _checkpointIndexPointer[_currentCheckpointIndex].ActivateCheckpoint();
            //increment checkpoint index
            Debug.Log("Activating checkpoint " + _currentCheckpointIndex);
            _gameState = "CHECKPOINT_"+_currentCheckpointIndex;
        }
        catch{
            Debug.Log("No more checkpoints, area is finished");
            _gameState = "END_AREA";
        }
    }

    private GameObject InitPlayer(){
        //find gameobject with name AllPlayer
        GameObject player = GameObject.Find("RealityPlayer");
        return player;
    }

    //Spawn managing

    /// <summary>
    /// Get the position where the player needs to spawn. At the beginning of the level it corresponds with the spawn
    /// platform, but then it can be the last checkpoint.
    /// </summary>
    public Vector3 GetSpawningPosition()
    {
        return _lastCheckPointPos;
    }

    public void RespawnPlayer(){
        //set player position to last checkpoint
        _player.transform.position = _lastCheckPointPos;
    }
    
}