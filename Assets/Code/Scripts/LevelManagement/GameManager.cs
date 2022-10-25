using UnityEngine;

/// <summary>
/// Basic GameManager implementation where the last checkpoint coordinate is stored.
/// The istance of a GameManager is handled, even though it seems that Unity does this automatically.
/// </summary>
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    private Vector3 _lastCheckPointPos;

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

        _lastCheckPointPos = GameObject.FindGameObjectWithTag("SpawnPlatform").transform.position;
    }

    public void SetLastCheckpointPos(Vector3 pos)
    {
        _lastCheckPointPos = pos;
    }

    /// <summary>
    /// Get the position where the player needs to spawn. At the beginning of the level it corresponds with the spawn
    /// platform, but then it can be the last checkpoint.
    /// </summary>
    public Vector3 GetSpawningPosition()
    {
        return _lastCheckPointPos;
    }
    
}