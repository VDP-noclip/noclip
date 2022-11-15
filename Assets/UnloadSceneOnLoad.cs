using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class UnloadSceneOnLoad : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        //unload this scene
    }

    // Update is called once per frame
    void Start()
    {
        SceneManager.UnloadSceneAsync(gameObject.scene);
    }
}
