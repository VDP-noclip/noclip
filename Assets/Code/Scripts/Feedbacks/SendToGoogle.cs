using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SendToGoogle : MonoBehaviour
{


    /*[SerializeField]*/ private bool _sendToProf = false;
    private string[] _videogamesNames = new string[26]
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
    
    [SerializeField] private VideoGamesName VideogameName;

    [Space]
    
    [Header("Feedback0")]
    [SerializeField] private GameObject[] _multipleChoices0_0;
    [SerializeField] private GameObject[] _multipleChoices0_1;
    [SerializeField] private Button _nextPage0_0;
    private string _multipleChoice0_0 = "null";
    private string _multipleChoice0_1 = "null";

    [Space]
    
    [Header("Feedback1")]
    [SerializeField] private GameObject[] _multipleChoices1_0;
    [SerializeField] private InputField _suggestions1_0;
    [SerializeField] private Button _nextPage1_0;
    private string _multipleChoice1_0 = "null";
    private string _suggestion1_0 = "null"; 
    
    [Space]
    
    [Header("Feedback2")]
    [SerializeField] private GameObject[] _multipleChoices2_0;
    [SerializeField] private GameObject[] _multipleChoices2_1;
    [SerializeField] private InputField _suggestions2_0;
    [SerializeField] private Button _nextPage2_0;
    private string _multipleChoice2_0 = "null";
    private string _multipleChoice2_1 = "null";
    private string _suggestion2_0 = "null";
    
    [Space]
    
    [Header("Feedback3")]
    [SerializeField] private GameObject[] _multipleChoices3_0;
    [SerializeField] private InputField _suggestions3_0;
    [SerializeField] private Button _nextPage3_0;
    private string _multipleChoice3_0 = "null";
    private string _suggestion3_0 = "null";
    
    [Space]
    
    [Header("Feedback4")]
    [SerializeField] private InputField _suggestions4_0;
    [SerializeField] private Button _nextPage4_0;
    private string _multipleChoice4_0 = "null";
    private string _suggestion4_0 = "null";


    private void Awake()
    {
        // Page0
        foreach (GameObject gameObject in _multipleChoices0_0)
        {
            gameObject.GetComponent<Button>().onClick.AddListener(delegate {_multipleChoice0_0 = SaveChoice(gameObject.name, _multipleChoices0_0); });
        }
        
        foreach (GameObject gameObject in _multipleChoices0_1)
        {
            gameObject.GetComponent<Button>().onClick.AddListener(delegate {_multipleChoice0_1 = SaveChoice(gameObject.name, _multipleChoices0_1 ); });
        }
        
        //Page1
        foreach (GameObject gameObject in _multipleChoices1_0)
        {
            gameObject.GetComponent<Button>().onClick.AddListener(delegate {_multipleChoice1_0 = SaveChoice(gameObject.name, _multipleChoices1_0 ); });
        }
        
        
        //Page2
        foreach (GameObject gameObject in _multipleChoices2_0)
        {
            gameObject.GetComponent<Button>().onClick.AddListener(delegate {_multipleChoice2_0 = SaveChoice(gameObject.name, _multipleChoices2_0 ); });
        }
        
        foreach (GameObject gameObject in _multipleChoices2_1)
        {
            gameObject.GetComponent<Button>().onClick.AddListener(delegate {_multipleChoice2_1 = SaveChoice(gameObject.name, _multipleChoices2_1 ); });
        }
        
        
        //Page3
        foreach (GameObject gameObject in _multipleChoices3_0)
        {
            gameObject.GetComponent<Button>().onClick.AddListener(delegate {_multipleChoice3_0 = SaveChoice(gameObject.name, _multipleChoices3_0); });
        }
        

    }

    public void SendFeedback()
    {
        
        if (_sendToProf)
        {
            string feedback = _suggestion1_0 + _suggestion2_0 + _suggestion3_0 + _suggestion4_0;
            StartCoroutine(PostFeedback(_videogamesNames[(int) VideogameName],feedback));
        }
        
        
        StartCoroutine(PostFeedbackToUs(_videogamesNames[(int) VideogameName]));
        
    }

    IEnumerator PostFeedbackToUs(string videogame_name)
    {
        string URL =
            "https://docs.google.com/forms/u/0/d/e/1FAIpQLScXqp9PAHN-KchAizf4vTC3u5GREqvhrbpjt_ar8Sv7aWlXRQ/formResponse";
        
        WWWForm form = new WWWForm();

        form.AddField("entry.2019945724", videogame_name);

        //Page 0
        form.AddField("entry.977593355", _multipleChoice0_0);
        form.AddField("entry.1727891243", _multipleChoice0_1);
        
        //Page 1
        form.AddField("entry.885016062", _multipleChoice1_0);
        form.AddField("entry.723035808", _suggestion1_0);
        
        //Page 2
        form.AddField("entry.1632853779", _multipleChoice2_0);
        form.AddField("entry.1594325674", _multipleChoice2_1);
        form.AddField("entry.995350686", _suggestion2_0);
        
        //Page 3
        form.AddField("entry.264692402", _multipleChoice3_0);
        form.AddField("entry.1358697629", _suggestion3_0);
        
        //Page 4
        form.AddField("entry.776490464", _suggestion4_0);

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
        _suggestion1_0 = _suggestions1_0.text;
    }
    
    public void SaveSuggestion2()
    {
        _suggestion2_0 = _suggestions2_0.text;
    }
    
    public void SaveSuggestion3()
    {
        _suggestion3_0 = _suggestions3_0.text;
    }
    
    public void SaveSuggestion4()
    {
        _suggestion4_0 = _suggestions4_0.text;
    }
}