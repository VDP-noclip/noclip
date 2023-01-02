using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//import application
using UnityEngine.UI;

public class BlackFadein : MonoBehaviour
{
    private int fadeIn = -1;
    private bool fading = false;
    [SerializeField] private float fadeTime = 1.0f;
    private UnityEngine.UI.Image image;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<UnityEngine.UI.Image>();
    }

    // Update is called once per frame
    void Update()
    {
        //if b is pressed toggle fade of this object
        if (Application.isEditor && Input.GetKeyDown(KeyCode.B))
        {
            fading = true;
            fadeIn = -fadeIn;
        }

        if (fading)
        {
            Color color = image.color;
            color.a += fadeIn * Time.deltaTime / fadeTime;
            if (color.a >= 1.0f)
            {
                color.a = 1.0f;
                fading = false;
            }
            else if (color.a <= 0.0f)
            {
                color.a = 0.0f;
                fading = false;
            }
            image.color = color;
        }
    }
}
