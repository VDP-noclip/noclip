using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections.Generic;

/// <summary>
/// TODO
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] private int _currentCheckpointIndex = 0;
    [SerializeField] private string _gameState = "BEGIN_AREA";
    
    private GameObject _player;
    private Vector3 _lastCheckPointPos;
    private CheckpointController[] _rawCheckpointControllers;

    private List<CheckpointController> _checkpointControllers;

    //dictionary of int and CheckpointController
    private Dictionary<int, CheckpointController> _checkpointIndexPointer = new();
    
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
            _checkpointIndexPointer[_currentCheckpointIndex].ActivateCheckpoint();
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
}