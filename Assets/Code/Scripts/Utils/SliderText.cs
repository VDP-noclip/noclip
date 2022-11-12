using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class SliderText : MonoBehaviour
{
    //textmeshpro gameobject
    [SerializeField] private GameObject textMeshPro;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //when slider value changes set its value to the text mesh pro
        textMeshPro.GetComponent<TextMeshProUGUI>().text = GetComponent<Slider>().value.ToString();
    }
}
