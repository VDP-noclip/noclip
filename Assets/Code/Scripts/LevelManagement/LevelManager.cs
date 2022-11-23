using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private int _currentPuzzleIndex = 0;
    private int _puzzleAmount = 0;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Looking for puzzles");
        //for N find 00_Puzzle_N until there are no more
        while (true)
        {
            try
            {
                //find puzzle in children
                GameObject puzzle = transform.Find("00_Puzzle_" + _currentPuzzleIndex).gameObject;
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
        transform.Find("00_Puzzle_" + _currentPuzzleIndex).gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadNextPuzzle()
    {
        if(_currentPuzzleIndex < _puzzleAmount - 1)
        {
            Debug.Log("LoadNextPuzzle");
            _currentPuzzleIndex++;
            transform.Find("00_Puzzle_" + _currentPuzzleIndex).gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("Area finished!");
        }
    }
}
