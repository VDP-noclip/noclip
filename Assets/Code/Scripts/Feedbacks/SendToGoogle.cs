using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using POLIMIGameCollective;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SendToGoogle : MonoBehaviour
{


    [SerializeField] private bool _sendToProf = false;
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

    [Space] [Header("Feedback0")] 
    [SerializeField] private Transform _multipleChoices0_0;
    [SerializeField] private Transform _multipleChoices0_1;
    private GameObject[] _multipleChoicesArray0_0;
    private GameObject[] _multipleChoicesArray0_1;
    [SerializeField] [NotNull]private GameObject _buttonNext0;
    private string _multipleChoice0_0 = "null";
    private string _multipleChoice0_1 = "null";

    [Space]
    
    [Header("Feedback1")]
    [SerializeField] private Transform _multipleChoices1_0;
    private GameObject[] _multipleChoicesArray1_0;
    [SerializeField] private TMP_InputField _suggestions1_0;
    [SerializeField] [NotNull] private GameObject _buttonNext1;
    private string _multipleChoice1_0 = "null";
    private string _suggestion1_0 = "null"; 
    
    [Space]
    
    [Header("Feedback2")]
    [SerializeField] private Transform _multipleChoices2_0;
    [SerializeField] private Transform _multipleChoices2_1;
    private GameObject[] _multipleChoicesArray2_0;
    private GameObject[] _multipleChoicesArray2_1;
    [SerializeField] private TMP_InputField _suggestions2_0;
    [SerializeField] [NotNull] private GameObject _buttonNext2;
    private string _multipleChoice2_0 = "null";
    private string _multipleChoice2_1 = "null";
    private string _suggestion2_0 = "null";
    
    [Space]
    
    [Header("Feedback3")]
    [SerializeField] private Transform _multipleChoices3_0;
    private GameObject[] _multipleChoicesArray3_0;
    [SerializeField] private TMP_InputField _suggestions3_0;
    [SerializeField] [NotNull] private GameObject _buttonNext3;
    private string _multipleChoice3_0 = "null";
    private string _suggestion3_0 = "null";
    
    [Space]
    
    [Header("Feedback4")]
    [SerializeField] private TMP_InputField _suggestions4_0;
    [SerializeField] private TMP_InputField _notesOnBugs4;
    [SerializeField] [NotNull] private GameObject _buttonNext4;
    private string _multipleChoice4_0 = "null";
    private string _noteOnBugs4 = "null";
    private string _suggestion4_0 = "null";


    private string _checkpointArrived = "null";


    #region Unity Functions

    private void Awake()
    {
        // Page0
        _multipleChoicesArray0_0 = retrieveMultipleChoices(_multipleChoices0_0);
        _multipleChoicesArray0_1 = retrieveMultipleChoices(_multipleChoices0_1);

        foreach (GameObject var in _multipleChoicesArray0_0)
        {
            var.GetComponent<Button>().onClick.AddListener(delegate {_multipleChoice0_0 = SaveChoice(var.name, _multipleChoicesArray0_0); });
        }
        
        foreach (GameObject var in _multipleChoicesArray0_1)
        {
            var.GetComponent<Button>().onClick.AddListener(delegate {_multipleChoice0_1 = SaveChoice(var.name, _multipleChoicesArray0_1 ); });
        }
        
        _buttonNext0.GetComponent<Button>().onClick.AddListener(SendToGoogleFeedback0);
        
        //Page1
        _multipleChoicesArray1_0 = retrieveMultipleChoices(_multipleChoices1_0);
        
        foreach (GameObject var in _multipleChoicesArray1_0)
        {
            var.GetComponent<Button>().onClick.AddListener(delegate {_multipleChoice1_0 = SaveChoice(var.name, _multipleChoicesArray1_0 ); });
        }
        
        _buttonNext1.GetComponent<Button>().onClick.AddListener(SaveSuggestion1);
        _buttonNext1.GetComponent<Button>().onClick.AddListener(SendToGoogleFeedback1);
       
        
        //Page2
        _multipleChoicesArray2_0 = retrieveMultipleChoices(_multipleChoices2_0);
        _multipleChoicesArray2_1 = retrieveMultipleChoices(_multipleChoices2_1);
        
        foreach (GameObject var in _multipleChoicesArray2_0)
        {
            var.GetComponent<Button>().onClick.AddListener(delegate {_multipleChoice2_0 = SaveChoice(var.name, _multipleChoicesArray2_0 ); });
        }
        
        foreach (GameObject var in _multipleChoicesArray2_1)
        {
            var.GetComponent<Button>().onClick.AddListener(delegate {_multipleChoice2_1 = SaveChoice(var.name, _multipleChoicesArray2_1 ); });
        }
        
        _buttonNext2.GetComponent<Button>().onClick.AddListener(SaveSuggestion2);
        _buttonNext2.GetComponent<Button>().onClick.AddListener(SendToGoogleFeedback2);
        
        //Page3
        _multipleChoicesArray3_0 = retrieveMultipleChoices(_multipleChoices3_0);
        foreach (GameObject var in _multipleChoicesArray3_0)
        {
            var.GetComponent<Button>().onClick.AddListener(delegate {_multipleChoice3_0 = SaveChoice(var.name, _multipleChoicesArray3_0); });
        }
        
        _buttonNext3.GetComponent<Button>().onClick.AddListener(SaveSuggestion3);
        _buttonNext3.GetComponent<Button>().onClick.AddListener(SendToGoogleFeedback3);
        
        
        //Page4
        
        _buttonNext4.GetComponent<Button>().onClick.AddListener(SaveSuggestion4);
        _buttonNext4.GetComponent<Button>().onClick.AddListener(SendToGoogleFeedback4);
        _buttonNext4.GetComponent<Button>().onClick.AddListener(SendFeedback);
        
        //Add checkpoint listener
        EventManager.StartListening("save_checkpoint_feedback", SaveCheckpoint);
    }


    #endregion
    
    #region Public Functions
    
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
        _noteOnBugs4 = _notesOnBugs4.text;
    }

    #endregion
    
    
    #region Private Functions

    private void SendToGoogleFeedback0()
    {
        StartCoroutine(SendToGoogleFeedback0Coroutine());
    }
    
    private void SendToGoogleFeedback1()
    {
        StartCoroutine(SendToGoogleFeedback1Coroutine());
    }
    private void SendToGoogleFeedback2()
    {
        StartCoroutine(SendToGoogleFeedback2Coroutine());
    }
    private void SendToGoogleFeedback3()
    {
        StartCoroutine(SendToGoogleFeedback3Coroutine());
    }
    private void SendToGoogleFeedback4()
    {
        StartCoroutine(SendToGoogleFeedback4Coroutine());
    }
    
    private void SendFeedback()
    {
        if (_sendToProf)
        {
            string feedback = _suggestion1_0 + _suggestion2_0 + _suggestion3_0 + _suggestion4_0;
            StartCoroutine(PostFeedback(_videogamesNames[(int) VideogameName],feedback));
        }
        StartCoroutine(PostFeedbackToUs(_videogamesNames[(int) VideogameName]));
    }

    private void SaveCheckpoint(string checkpoint)
    {
        EventManager.StopListening("save_checkpoint_feedback", SaveCheckpoint);
        _checkpointArrived = checkpoint;
        PlayerPrefs.SetString("saveCheckpointFeedback", _checkpointArrived);
        EventManager.StartListening("save_checkpoint_feedback", SaveCheckpoint);
    }
    private string SaveChoice(string choice, GameObject[] list)
    {

        foreach (GameObject button in list)
        {
            if (button.name != choice)
            {
                button.GetComponent<Button>().image.color = Color.gray;
            }
            else
            {
                button.GetComponent<Button>().image.color = Color.cyan;
            }
        }
        
        Debug.Log(choice);
        return choice;
    }

    
    private GameObject[] retrieveMultipleChoices(Transform trans)
    {
        List<GameObject> list = new();
        foreach (Transform child in trans)
        {
            if (child.name != "Question")
            {
                list.Add(child.gameObject);
            }
        }
        Debug.Log(list.ToArray()[2].name);
        return list.ToArray();
    }

    #endregion

    #region Coroutine Functions
    
    private IEnumerator PostFeedbackToUs(string videogame_name)
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
        
    }
    private IEnumerator PostFeedback(string videogame_name, string feedback) 
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
    }

    private IEnumerator SendToGoogleFeedback0Coroutine()
    {
        if (_checkpointArrived == "null")
        {
            _checkpointArrived = PlayerPrefs.GetString("saveCheckpointFeedback");
        }
        
        string URL = "https://docs.google.com/forms/d/e/1FAIpQLSef5l-ZUoHC7c_VLqy09NA2VTbu1VCwNw7NZLjB8mOI2iipUA/formResponse";
        
        WWWForm form = new WWWForm();

        form.AddField("entry.643657764", _multipleChoice0_0);
        form.AddField("entry.1301141610", _multipleChoice0_1);
        form.AddField("entry.235040056", _checkpointArrived);

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
    }
    
    private IEnumerator SendToGoogleFeedback1Coroutine()
    {
        if (_checkpointArrived == "null")
        {
            _checkpointArrived = PlayerPrefs.GetString("saveCheckpointFeedback");
        }
        
        string URL = "https://docs.google.com/forms/d/e/1FAIpQLSfY6mCvNvumzwpGpiYJAbaeqnomloZ_hs5NXIG9f1EolvsJ4g/formResponse";
        
        WWWForm form = new WWWForm();

        form.AddField("entry.1689264014", _multipleChoice1_0);
        form.AddField("entry.1795851765", _suggestion1_0);
        form.AddField("entry.2089839631", _checkpointArrived);

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
    }
    private IEnumerator SendToGoogleFeedback2Coroutine()
    {
        if (_checkpointArrived == "null")
        {
            _checkpointArrived = PlayerPrefs.GetString("saveCheckpointFeedback");
        }
        
        string URL = "https://docs.google.com/forms/d/e/1FAIpQLSccb7D7IJbgEmdUOC8XBd1oQH3VmwDGbFygk7C4qQETN2eBHA/formResponse";
        
        WWWForm form = new WWWForm();

        form.AddField("entry.2064137350", _multipleChoice2_0);
        form.AddField("entry.608431837", _multipleChoice2_1);
        form.AddField("entry.122820910", _suggestion2_0);
        form.AddField("entry.969916236", _checkpointArrived);

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
    }
    
    private IEnumerator SendToGoogleFeedback3Coroutine()
    {
        if (_checkpointArrived == "null")
        {
            _checkpointArrived = PlayerPrefs.GetString("saveCheckpointFeedback");
        }
        
        string URL = "https://docs.google.com/forms/d/e/1FAIpQLSeTdMlbw04t4Dpq-FFakdZd9FWxaDCraGnGCy-tYT5EDhmXaw/formResponse";
        
        WWWForm form = new WWWForm();

        form.AddField("entry.2031543232", _multipleChoice3_0);
        form.AddField("entry.1198441079", _suggestion3_0);
        form.AddField("entry.1958101121", _checkpointArrived);

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
    }
    
    private IEnumerator SendToGoogleFeedback4Coroutine()
    {
        if (_checkpointArrived == "null")
        {
            _checkpointArrived = PlayerPrefs.GetString("saveCheckpointFeedback");
        }
        
        string URL = "https://docs.google.com/forms/d/e/1FAIpQLSfL8axq46a4vJCcQ3L6dCv8VasmyL26Xd9wkJ963MYld-7COw/formResponse";
        
        WWWForm form = new WWWForm();

        form.AddField("entry.2103217825", _suggestion4_0);
        form.AddField("entry.885548568", _noteOnBugs4);
        form.AddField("entry.1185853159", _checkpointArrived);

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
    }
    

    #endregion
    
}