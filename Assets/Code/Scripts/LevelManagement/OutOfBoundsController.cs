using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// PlayerSpawner handles where Player should spawn after hitting certain objects.
/// For now, when the Player collides with a trigger object, he respawns in the last saved checkpoint.
/// Note: checkpoints for now are volatile, meaning that closing the game will erase them.
/// </summary>
public class OutOfBoundsController: MonoBehaviour
{
    private GameManager _gameManager;
    [SerializeField] private GameObject _realityPlayer;

    private void Awake()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _realityPlayer = GameObject.FindGameObjectWithTag("RealityPlayer");
    }

    void Start()
    {
        _realityPlayer.transform.position = _gameManager.GetSpawningPosition();
        Physics.SyncTransforms();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    
    private void OnTriggerEnter(Collider otherObject)
    {
        RespawnPlayer();
    }

    private void RespawnPlayer()
    {
        //get parent of reality player
        GameObject allPlayer = _realityPlayer.transform.parent.gameObject;
        //and move it to the last checkpoint
        allPlayer.transform.position = _gameManager.GetSpawningPosition();
        Physics.SyncTransforms();
    }
}
