using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /*
     * Basic GameManager implementation where the last checkpoint coordinate is stored.
     * The istance of a GameManager is handled, even though it seems that Unity does this automatically.
     */
    
    private static GameManager instance;
    public Vector3 lastCheckPointPos;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
}
