using System.Collections;
using System.Collections.Generic;
using POLIMIGameCollective;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private int _currentPuzzleIndex = 0;
    private int _puzzleAmount = 0;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Looking for puzzles");
        //for N find Puzzle_N until there are no more
        while (true)
        {
            try
            {
                //find puzzle in children
                GameObject puzzle = transform.Find("Puzzle_" + _currentPuzzleIndex).gameObject;
                if (puzzle == null)
                    break;
                puzzle.SetActive(false);
                _currentPuzzleIndex++;

                if(_currentPuzzleIndex > 100)
                {
                    Debug.LogError("Too many puzzles!");
                    break;
                }
            }
            catch
            {
                break;
            }
        }
        Debug.Log("Found " + _currentPuzzleIndex + " puzzles");
        _puzzleAmount = _currentPuzzleIndex;
        _currentPuzzleIndex = 0;
        transform.Find("Puzzle_" + _currentPuzzleIndex).gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        //if k is pressed find AllPlayer and move it to EndAnchor child of next puzzle
        if (Input.GetKeyDown(KeyCode.K))
        {
            CompleteCurrentPuzzle();
        }
    }

    public void LoadNextPuzzle()
    {
        if(_currentPuzzleIndex < _puzzleAmount - 1)
        {
            Debug.Log("LoadNextPuzzle");
            _currentPuzzleIndex++;
            transform.Find("Puzzle_" + _currentPuzzleIndex).gameObject.SetActive(true);
            GameObject.Find("RealityPlayer").GetComponent<NoclipManager>().GetReadyForPuzzle();
        }
        else
        {
            Debug.Log("Area finished!");
            try{
                GameObject.Find("GameManager").GetComponent<GameManager>().SetAreaFinished();
            }catch{
                
            }
        }
    }

    public void CompleteCurrentPuzzle()
    {
        //find AllPlayer
        GameObject player = GameObject.Find("RealityPlayer").gameObject;
        NoclipManager noclipManager = player.GetComponentInChildren<NoclipManager>();
        if (noclipManager.IsNoclipEnabled())
        {
            EventManager.TriggerEvent("DisplayHint","RETURN TO YOUR BODY, THEN PRESS K TO SKIP PUZZLE");
            return;
        }
        
        //find EndAnchor of next puzzle
        GameObject endAnchor = transform.Find("Puzzle_" + _currentPuzzleIndex).Find("Save").gameObject;
        //get Save child of endAnchor
        GameObject checkpoint = endAnchor.transform.Find("AnchorCheckpoint").gameObject;
        //position of checkpoint
        Vector3 checkpointPosition = checkpoint.transform.position;
        //move the player to the geometric center of save plus 1 meter
        player.transform.position = checkpointPosition + new Vector3(0, 1, 0);
        Debug.Log("Moving you to the end of the puzzle, shame on you!");
    }
}
