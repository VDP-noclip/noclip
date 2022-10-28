//unity gameobject
using UnityEngine;
//full pipeline unity gameobject
public class ScriptTemplate : MonoBehaviour {

    [Header("Header")]
    [Tooltip("This variable has a tooltip")]
    [SerializeField] private string _debugChannel = "tutorial";

    [Space]
    [Space(3)]

    [TextArea]
    [Tooltip("This variable has a nicer tooltip")]
    public string Notes = "This is a script template with several useful features";
 
    [Header("Header1")]
    [Tooltip("This is a powerful variable")]
    [SerializeField] private float _floatValue = 1f;
    [SerializeField] private bool _boolValue = true;
    [SerializeField] private GameObject _gameObject;
    
    private float _moveSpeed;     // speed intensity
    
    [Header("Header2")]
    [Tooltip("Let me out of here")]
    [SerializeField] private KeyCode _button = KeyCode.Space;  
    [SerializeField] private Enumeration _enum;     // current player state

    private enum Enumeration       // define player states
    {
        value1,
        value2,
        value3
    }


    private void OnEnable() {
        Log("OnEnable", "tutorial");
        //called when the behaviour becomes enabled and active
    }
    private void Awake() {
        Log("Awaken " + gameObject.name, "tutorial");
        //called when the script instance is being loaded
        //show the mouse cursor
        Cursor.visible = true;
    }
    // Use this for initialization
    private void Start () {
        Log("Start", "tutorial");
        //called on the frame when a script is enabled just before any of the Update methods is called the first time
    }
    // Update is called once per frame
    private void Update () {
        //Debug.Log("Update");
        //called every frame, if the MonoBehaviour is enabled
    }

    private void FixedUpdate() {
        //Debug.Log("FixedUpdate");
        //called every fixed framerate frame
    }

    private void LateUpdate() {
        //Debug.Log("LateUpdate");
        //called every frame, if the Behaviour is enabled
    }

    private void OnGUI() {
        //Debug.Log("OnGUI");
        //called for rendering and handling GUI events
    }

    private void OnDisable() {
        Log("OnDisable", "tutorial");
        //called when the behaviour becomes disabled or inactive
    }

    private void OnDestroy() {
        Log("OnDestroy", "tutorial");
        //called when the script is destroyed
    }

    private void OnTriggerEnter(Collider other) {
        Log("OnTriggerEnter", "tutorial");
        //called when the collider other enters the trigger
    }

    private void OnTriggerExit(Collider other) {
        Log("OnTriggerExit", "tutorial");
        //called when the collider other has stopped touching the trigger
    }

    private void OnTriggerStay(Collider other) {
        //Debug.Log("OnTriggerStay");
        //called once per frame for every collider other that is touching the trigger
    }

    private void OnCollisionEnter(Collision collision) {
        Log("OnCollisionEnter", "tutorial");
        //called when this collider/rigidbody has begun touching another rigidbody/collider
    }

    private void OnCollisionExit(Collision collision) {
        Log("OnCollisionExit", "tutorial");
        //called when this collider/rigidbody has stopped touching another rigidbody/collider
    }

    private void OnCollisionStay(Collision collision) {
        //Debug.Log("OnCollisionStay");
        //called once per frame for every collider/rigidbody that is touching rigidbody/collider
    }

    private void OnMouseEnter() {
        Log("OnMouseEnter", "tutorial");
        //called when the mouse entered the GUIElement or Collider
    }

    private void OnMouseExit() {
        Log("OnMouseExit", "tutorial");
        //called when the mouse is not any longer over the GUIElement or Collider
    }

    private void OnMouseOver() {
        //Debug.Log("OnMouseOver");
        //called every frame while the mouse is over the GUIElement or Collider
    }

    private void OnMouseDown() {
        Log("OnMouseDown", "tutorial");
        //called on the frame when the mouse is pressed
    }

    private void OnMouseUp() {
        Log("OnMouseUp", "tutorial");
        //called on the frame when the mouse is released
    }

    private void OnMouseDrag() {
        //Debug.Log("OnMouseDrag");
        //called when the user has clicked on a GUIElement or Collider and is still holding down the mouse
    }

    private void Log(string message, string channel) {
        if(channel == _debugChannel) {
            Debug.Log(message);
        }
    }
}