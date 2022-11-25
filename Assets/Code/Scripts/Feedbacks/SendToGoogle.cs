using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SendToGoogle : MonoBehaviour
{


    /*[SerializeField]*/ private bool _sendToProf = false;
    private string[] _videogames_names = new string[26]
    {
        "AnotherD",
        "Arrow",
        "Be-Headed",
        "CloudDiver",
        "CursedHeroOfThePast",
        "DarkUnknown",
        "Don'TLetLukeFall",
        "DreamDiary",
        "FeedMe,Quack!",
        "Freshaliens",
        "HarmonyInDarkness",
        "HotPotato",
        "JourneyToAntartica",
        "JustTheTwoOfUs",
        "LaserKnight",
        "NoClip",
        "Paradox!",
        "ParkourNews",
        "PumpDownTheFlame",
        "Reset::Relive",
        "Shade",
        "SleepInvasion",
        "TheFelineParadox",
        "TiedRivals",
        "Zorball",
        ">>Test"
    };

    enum VideoGamesName
    {
        AnotherD=0,
        Arrow=1,
        BeHeaded=2,
        CloudDiver=3,
        CursedHeroOfThePast=4,
        DarkUnknown=5,
        DonTLetLukeFall=6,
        DreamDiary=7,
        FeedMeQuack=8,
        Freshaliens=9,
        HarmonyInDarkness=10,
        HotPotato=11,
        JourneyToAntartica=12,
        JustTheTwoOfUs=13,
        LaserKnight=14,
        NoClip=15,
        Paradox=16,
        ParkourNews=17,
        PumpDownTheFlame=18,
        ResetRelive=19,
        Shade=20,
        SleepInvasion=21,
        TheFelineParadox=22,
        TiedRivals=23,
        Zorball=24,
        Test=25
    };
    
    [SerializeField] private VideoGamesName Videogame;

    [Space]
    
    [Header("Feedback0")]
    [SerializeField] private GameObject[] _multipleChoices00;
    [SerializeField] private GameObject[] _multipleChoices01;
    private string _multipleChoice00 = "null";
    private string _multipleChoice01 = "null";

    [Space]
    
    [Header("Feedback1")]
    [SerializeField] private GameObject[] _multipleChoices1;
    [SerializeField] private InputField _suggestions1;
    [SerializeField] private Button _nextPage1;
    private string _multipleChoice1 = "null";
    private string _suggestion1 = "null"; 
    
    [Space]
    
    [Header("Feedback2")]
    [SerializeField] private GameObject[] _multipleChoices20;
    [SerializeField] private GameObject[] _multipleChoices21;
    [SerializeField] private InputField _suggestions2;
    [SerializeField] private Button _nextPage2;
    private string _multipleChoice20 = "null";
    private string _multipleChoice21 = "null";
    private string _suggestion2 = "null";
    
    [Space]
    
    [Header("Feedback3")]
    [SerializeField] private GameObject[] _multipleChoices3;
    [SerializeField] private InputField _suggestions3;
    [SerializeField] private Button _nextPage3;
    private string _multipleChoice3 = "null";
    private string _suggestion3 = "null";
    
    [Space]
    
    [Header("Feedback4")]
    [SerializeField] private InputField _suggestions4;
    [SerializeField] private Button _nextPage4;
    private string _multipleChoice4 = "null";
    private string _suggestion4 = "null";


    private void Awake()
    {
        // Page0
        foreach (GameObject gameObject in _multipleChoices00)
        {
            gameObject.GetComponent<Button>().onClick.AddListener(delegate {_multipleChoice00 = SaveChoice(gameObject.name, _multipleChoices00); });
        }
        
        foreach (GameObject gameObject in _multipleChoices01)
        {
            gameObject.GetComponent<Button>().onClick.AddListener(delegate {_multipleChoice01 = SaveChoice(gameObject.name, _multipleChoices01 ); });
        }
        
        //Page1
        foreach (GameObject gameObject in _multipleChoices1)
        {
            gameObject.GetComponent<Button>().onClick.AddListener(delegate {_multipleChoice1 = SaveChoice(gameObject.name, _multipleChoices1 ); });
        }
        
        _nextPage1.onClick.AddListener(SaveSuggestion1);
        
        //Page2
        foreach (GameObject gameObject in _multipleChoices20)
        {
            gameObject.GetComponent<Button>().onClick.AddListener(delegate {_multipleChoice20 = SaveChoice(gameObject.name, _multipleChoices20 ); });
        }
        
        foreach (GameObject gameObject in _multipleChoices21)
        {
            gameObject.GetComponent<Button>().onClick.AddListener(delegate {_multipleChoice21 = SaveChoice(gameObject.name, _multipleChoices21 ); });
        }
        
        _nextPage2.onClick.AddListener(SaveSuggestion2);
        
        //Page3
        foreach (GameObject gameObject in _multipleChoices3)
        {
            gameObject.GetComponent<Button>().onClick.AddListener(delegate {_multipleChoice3 = SaveChoice(gameObject.name, _multipleChoices3); });
        }
        
        _nextPage3.onClick.AddListener(SaveSuggestion3);

    }

    public void SendFeedback()
    {
        
        if (_sendToProf)
        {
            string feedback = _suggestion1 + _suggestion2 + _suggestion3 + _suggestion4;
            StartCoroutine(PostFeedback(_videogames_names[(int) Videogame],feedback));
        }
        
        
        StartCoroutine(PostFeedbackToUs(_videogames_names[(int) Videogame]));
        
    }

    IEnumerator PostFeedbackToUs(string videogame_name)
    {
        string URL =
            "https://docs.google.com/forms/u/0/d/e/1FAIpQLScXqp9PAHN-KchAizf4vTC3u5GREqvhrbpjt_ar8Sv7aWlXRQ/formResponse";
        
        WWWForm form = new WWWForm();

        form.AddField("entry.2019945724", videogame_name);

        //Page 0
        form.AddField("entry.977593355", _multipleChoice00);
        form.AddField("entry.1727891243", _multipleChoice01);
        
        //Page 1
        form.AddField("entry.885016062", _multipleChoice1);
        form.AddField("entry.723035808", _suggestion1);
        
        //Page 2
        form.AddField("entry.1632853779", _multipleChoice20);
        form.AddField("entry.1594325674", _multipleChoice21);
        form.AddField("entry.995350686", _suggestion2);
        
        //Page 3
        form.AddField("entry.264692402", _multipleChoice3);
        form.AddField("entry.1358697629", _suggestion3);
        
        //Page 4
        form.AddField("entry.776490464", _suggestion4);

        UnityWebRequest www = UnityWebRequest.Post(URL, form);

        yield return www.SendWebRequest();

        print(www.error);
        
        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
        }
        
        // at the end go back to the main menu
        MenuManager.Instance.OpenMainMenu();
    }
    IEnumerator PostFeedback(string videogame_name, string feedback) 
    {
        // https://docs.google.com/forms/d/e/1FAIpQLSdyQkpRLzqRzADYlLhlGJHwhbKZvKJILo6vGmMfSePJQqlZxA/viewform?usp=pp_url&entry.631493581=Simple+Game&entry.1313960569=Very%0AGood!

        string URL =
            "https://docs.google.com/forms/d/e/1FAIpQLSdyQkpRLzqRzADYlLhlGJHwhbKZvKJILo6vGmMfSePJQqlZxA/formResponse";
        
        WWWForm form = new WWWForm();

        form.AddField("entry.631493581", videogame_name);
        form.AddField("entry.1313960569", feedback);

        UnityWebRequest www = UnityWebRequest.Post(URL, form);

        yield return www.SendWebRequest();

        print(www.error);
        
        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
        }
        
        // at the end go back to the main menu
        MenuManager.Instance.OpenMainMenu();
    }

    public string SaveChoice(string choice, GameObject[] list)
    {
        
        
        foreach (GameObject button in list)
        {
            if (button.name != choice)
            {
                button.GetComponent<Button>().image.color = Color.white;
            }
            else
            {
                button.GetComponent<Button>().image.color = Color.green;
            }
        }

        return choice;
    }

    public void SaveSuggestion1()
    {
        _suggestion1 = _suggestions1.text;
    }
    
    public void SaveSuggestion2()
    {
        _suggestion2 = _suggestions2.text;
    }
    
    public void SaveSuggestion3()
    {
        _suggestion3 = _suggestions3.text;
    }
    
    public void SaveSuggestion4()
    {
        _suggestion4 = _suggestions4.text;
    }
}