using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _puzzlePrefabs;

    private int _currentPuzzleIndex = 0;

    private GameObject _gameManager;
    private Vector3 _puzzlePosition = new Vector3(0, 0, 0);
    private void Awake()
    {
        LoadNextPuzzle();
    }

    void Update()
    {   
        // Todo: execute these 2 lines only when the player goes on the checkpoint
        //if (Application.isEditor && Input.GetKeyDown(KeyCode.X))
        //{
           // LoadNextPuzzle();
           // GameObject.Find("RealityPlayer").GetComponent<NoclipManager>().FindNoClipObjControllers();
        //}
        //if c is pressed, set gamestate to area_finished
    }

    public void LoadNextPuzzle()
    {
        //NOTE puzzles don't snap perfectly between anchors probably because of rotations
        try{
            GameObject newPuzzle = GameObject.Instantiate(_puzzlePrefabs[_currentPuzzleIndex]);
            _currentPuzzleIndex += 1;
            //rotate newPuzzle.transform.Find("BeginAnchor") to zero
            //newPuzzle.transform.Find("BeginAnchor").transform.rotation = Quaternion.Euler(0, 0, 0);
            //find absolute position of gameobject BeginAnchor in newPuzzle
            Vector3 beginAnchorPosition = newPuzzle.transform.Find("BeginAnchor").position;
            //difference between beginAnchorPosition and _puzzlePosition
            Vector3 difference = _puzzlePosition - beginAnchorPosition;
            //move newPuzzle by difference
            newPuzzle.transform.position = newPuzzle.transform.position + difference;
            //move newPuzzle by -2 on x and z
            newPuzzle.transform.position = newPuzzle.transform.position + new Vector3(2, 0, 2); //Why it doesn't work without this is beyond me
            //rotate EndAnchor of newPuzzle to zero
            //newPuzzle.transform.Find("EndAnchor").transform.rotation = Quaternion.Euler(0, 0, 0);
            //set _puzzlePosition to endAnchorPosition
            _puzzlePosition = newPuzzle.transform.Find("EndAnchor").position;
            GameObject.Find("RealityPlayer").GetComponent<NoclipManager>().FindNoClipObjControllers();
        }
        catch (IndexOutOfRangeException){
            Debug.Log("No more puzzles to load");
            GameObject.Find("GameManager").GetComponent<GameManager>().SetGameState("AREA_FINISHED");
            return;
            //GameObject.Find("GameManager").GetComponent<GameManager>().CloseAllScenes();
        }
    }
}
