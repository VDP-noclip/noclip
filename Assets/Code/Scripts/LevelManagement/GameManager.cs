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
    [SerializeField] private GameState _gameState = GameState.InitializeGame;

    //enum game states
    private enum GameState
    {
        InitializeGame,
        AreaFinished,
        NewArea
    }

    // Update is called once per frame
    private void Update()
    {
        //if area is finished load next area
        if (_gameState == GameState.AreaFinished)
        {
            _currentGameAreaIndex += 1;
            CloseAllScenes();
            DestroyOtherGameObjects();
            SceneManager.LoadScene(_gameAreas[_currentGameAreaIndex], LoadSceneMode.Additive);
            _gameState = GameState.NewArea;
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