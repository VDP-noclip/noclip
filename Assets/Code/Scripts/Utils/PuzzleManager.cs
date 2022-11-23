using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _puzzlePrefabs;

    private int _currentPuzzleIndex;
    private int _finalPuzzleIndex;

    private GameObject _gameManager;
    private Vector3 _puzzlePosition;
    
    private void Awake()
    {
        LoadCurrentPuzzle();
        _finalPuzzleIndex = _puzzlePrefabs.Length - 1;
    }
    
    /// <summary>
    /// Call this function when the puzzle is finished and you need to move to the next one. If the current puzzle is
    /// the last one, set the game state to AreaFinished
    /// </summary>
    public void LoadNextPuzzle()
    {
        if (_currentPuzzleIndex != _finalPuzzleIndex)
        {
            _currentPuzzleIndex += 1;
            LoadCurrentPuzzle();
            GameObject.Find("RealityPlayer").GetComponent<NoclipManager>().FindNoClipObjControllers();
        }
        else
        {
            Debug.Log("No more puzzles to load. Area finished!");
            GameObject.Find("GameManager").GetComponent<GameManager>().SetAreaFinished();
        }
    }

    private void LoadCurrentPuzzle()
    {
            //NOTE: puzzles don't snap perfectly between anchors probably because of rotations
            GameObject newPuzzle = Instantiate(_puzzlePrefabs[_currentPuzzleIndex]);
            
            //rotate newPuzzle.transform.Find("BeginAnchor") to zero
            //newPuzzle.transform.Find("BeginAnchor").transform.rotation = Quaternion.Euler(0, 0, 0);
            //find begin anchor
            GameObject beginAnchor = newPuzzle.transform.Find("BeginAnchor").gameObject;
            //find geometric center of begin anchor in world space
            Vector3 beginAnchorPosition = beginAnchor.GetComponent<Renderer>().bounds.center;
            //destroy begin anchor
            Destroy(beginAnchor);
            
            /*Check with visual debugging
            //spawn a red vertical pole at beginAnchorPosition
            GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pole.transform.position = beginAnchorPosition;
            pole.transform.localScale = new Vector3(0.1f, 0.2f, 0.1f);
            pole.GetComponent<Renderer>().material.color = Color.red;
            */

            //difference between beginAnchorPosition and _puzzlePosition
            Vector3 difference = _puzzlePosition - beginAnchorPosition;
            //move newPuzzle by difference
            newPuzzle.transform.position = newPuzzle.transform.position + difference;
            //move newPuzzle by -2 on x and z
            //find end anchor
            GameObject endAnchor = newPuzzle.transform.Find("EndAnchor").gameObject;
            //find geometric center of end anchor in world space
            _puzzlePosition = endAnchor.GetComponent<Renderer>().bounds.center;
            /*Check with visual debugging
            //spawn a green horizontal pole at beginAnchorPosition
            pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pole.transform.position = _puzzlePosition;
            pole.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            pole.GetComponent<Renderer>().material.color = Color.green;
            */
    }
}
