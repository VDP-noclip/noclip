using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// Contains a list of areas to be loaded sequentially. Each area will contain several puzzles and scenes.
/// This code reflects https://www.figma.com/file/VXmyoNeOfotAkpYbjbkXCT/noclip_ideas?node-id=423%3A1041
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] private List<string> _gameAreas;
    [SerializeField] private float _gravity;

    private int _currentGameAreaIndex;
    private int _finalGameAreaIndex;
    [SerializeField] private GameState _gameState = GameState.InitializeGame;
    private bool _mainStarted = false;
    //enum game states
    private enum GameState
    {
        InitializeGame,
        AreaFinished,
        NewArea,
        GameCompleted
    }
    
    private void Awake()
    {
        //Physics.gravity = new Vector3(0, 0, 0);
        //SceneManager.LoadScene(_gameAreas[_currentGameAreaIndex], LoadSceneMode.Additive);
        //_finalGameAreaIndex = _gameAreas.Count - 1;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
    }

    private void Update()
    {
        //if not mainstarted check if 0_Main is the only scene loaded
        if (!_mainStarted)
        {
            //get active scenes names
            List<string> activeScenes = new List<string>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                activeScenes.Add(SceneManager.GetSceneAt(i).name);
            }
            if (SceneManager.sceneCount == 1)
            {
                _mainStarted = true;
                Physics.gravity = new Vector3(0, 0, 0);
                //load scene if name is not in active scenes
                if (!activeScenes.Contains(_gameAreas[_currentGameAreaIndex]))
                {
                    SceneManager.LoadScene(_gameAreas[_currentGameAreaIndex], LoadSceneMode.Additive);
                    _finalGameAreaIndex = _gameAreas.Count - 1;
                }
            }
        }
        else
            //if area is finished load next area, complete the game if it is the last one
            if (_gameState == GameState.AreaFinished)
            {
                if (_currentGameAreaIndex == _finalGameAreaIndex)
                {
                    _gameState = GameState.GameCompleted;
                    Debug.Log("Congratulations, you have completed the game!");
                }
                else
                {
                    _gameState = GameState.NewArea;
                    CloseAllScenes();
                    //DestroyOtherGameObjects();
                    _currentGameAreaIndex += 1;
                    SceneManager.LoadScene(_gameAreas[_currentGameAreaIndex], LoadSceneMode.Additive);
                }
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

    public void SetAreaFinished()
    {
        _gameState = GameState.AreaFinished;
    }

    public void SetGravity(float gravity)
    {
        _gravity = gravity;
    }

    public float GetGravity()
    {
        return _gravity;
    }
}