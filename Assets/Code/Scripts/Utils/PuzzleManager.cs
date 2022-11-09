using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _puzzlePrefabs;

    private int _currentPuzzleIndex = 0;

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
    }

    public void LoadNextPuzzle()
    {
        GameObject.Instantiate(_puzzlePrefabs[_currentPuzzleIndex]);
        _currentPuzzleIndex += 1;
        GameObject.Find("RealityPlayer").GetComponent<NoclipManager>().FindNoClipObjControllers();
    }
}
