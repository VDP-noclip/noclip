using System.Collections;
using System.Collections.Generic;
using POLIMIGameCollective;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private int _currentPuzzleIndex = 0;
    private int _puzzleAmount = 0;
    private Vector3 _checkpointOrientation;
    private Vector3 _spawnPlatformPosition;
    [SerializeField] private Material _previousNoclipMaterial;

    void Awake(){
        _spawnPlatformPosition = GameObject.Find("SpawnPlatform").transform.position;
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Looking for puzzles");
        //for N find Puzzle_N until there are no more
        while (true)
        {
            try
            {
                //find puzzle in children
                GameObject puzzle = transform.Find("Puzzle_" + _currentPuzzleIndex).gameObject;
                if (puzzle == null)
                    break;
                puzzle.SetActive(false);
                _currentPuzzleIndex++;

                if(_currentPuzzleIndex > 100)
                {
                    Debug.LogError("Too many puzzles!");
                    break;
                }
            }
            catch
            {
                break;
            }
        }
        Debug.Log("Found " + _currentPuzzleIndex + " puzzles");
        _puzzleAmount = _currentPuzzleIndex;
        _currentPuzzleIndex = 0;
        transform.Find("Puzzle_" + _currentPuzzleIndex).gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        //if k is pressed find AllPlayer and move it to EndAnchor child of next puzzle
        if (Input.GetButtonDown("NextPuzzle"))
        {
            CompleteCurrentPuzzle();
        }
    }

    public void LoadNextPuzzle()
    {
        if(_currentPuzzleIndex < _puzzleAmount - 1)
        {
            Debug.Log("LoadNextPuzzle");
            _currentPuzzleIndex++;
            transform.Find("Puzzle_" + _currentPuzzleIndex).gameObject.SetActive(true);
            
            EventManager.TriggerEvent("save_checkpoint_feedback", "Puzzle_" + _currentPuzzleIndex);  //Send to the feedback the checkpoint
            
            //EventManager.TriggerEvent("checkpoint_orientation", NextCheckpointOrientation());
            NextCheckpointOrientation();
            UpdatePreviousNoclipMaterials();
            GameObject.Find("RealityPlayer").GetComponent<NoclipManager>().GetReadyForPuzzle();
        }
        else
        {
            Debug.Log("Area finished!");
            try{
                GameObject.Find("GameManager").GetComponent<GameManager>().SetAreaFinished();
            }catch{
                
            }
        }
    }

    public Vector3 NextCheckpointOrientation()
    {
        try{
            GameObject nextSave = transform.Find("Puzzle_" + (_currentPuzzleIndex)).gameObject;
            //find Save in next puzzle
            GameObject save = nextSave.transform.Find("Save").gameObject;
            //find absolute position of save
            Vector3 savePosition = save.transform.position;
            //prev save position
            Vector3 prevSavePosition = transform.Find("Puzzle_" + (_currentPuzzleIndex - 1)).gameObject.transform.Find("Save").gameObject.transform.position;
            //get the direction from current save to next save
            Vector3 direction = savePosition - prevSavePosition;
            //convert direction to euler angles
            Vector3 euler = Quaternion.LookRotation(direction).eulerAngles;
            euler.z = 0;
            //log
            Debug.Log("Next checkpoint orientation: " + euler);
            //spawn a cube in position of prevsave + 10 in direction euler + 2 up
            //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //move cube to prevsave + 2 up
            //cube.transform.position = prevSavePosition + new Vector3(0, 2, 0);
            //move cube in direction 5 from player in direction euler
            //cube.transform.position += Quaternion.Euler(euler) * Vector3.forward * 5;
            return euler;
        }
        catch{
            GameObject nextSave = transform.Find("Puzzle_" + (_currentPuzzleIndex)).gameObject;
            //find Save in next puzzle
            GameObject save = nextSave.transform.Find("Save").gameObject;
            //find absolute position of save
            Vector3 savePosition = save.transform.position;
            //spawn platform position
            Vector3 spawnPlatformPosition = _spawnPlatformPosition;
            //direction save spawn
            Vector3 direction = savePosition - spawnPlatformPosition;
            Vector3 euler = Quaternion.LookRotation(direction).eulerAngles;
            euler.z = 0;
            Debug.Log("Next checkpoint orientation: " + euler);
            return euler;


            //SPAWN PLAYER ROTATION
            //vector 180 degrees front
            //return new Vector3(0, 180, 0);
        }
    }

    public void CompleteCurrentPuzzle()
    {
        //find AllPlayer
        GameObject player = GameObject.Find("RealityPlayer").gameObject;
        NoclipManager noclipManager = player.GetComponentInChildren<NoclipManager>();
        if (noclipManager.IsNoclipEnabled())
        {
            EventManager.TriggerEvent("DisplayHint","RETURN TO YOUR BODY, THEN PRESS K TO SKIP PUZZLE");
            return;
        }
        
        //find EndAnchor of next puzzle
        GameObject endAnchor = transform.Find("Puzzle_" + _currentPuzzleIndex).Find("Save").gameObject;
        //get Save child of endAnchor
        GameObject checkpoint = endAnchor.transform.Find("AnchorCheckpoint").gameObject;
        //position of checkpoint
        Vector3 checkpointPosition = checkpoint.transform.position;
        //move the player to the geometric center of save plus 1 meter
        player.transform.position = checkpointPosition + new Vector3(0, 1, 0);
        Debug.Log("Moving you to the end of the puzzle, shame on you!");
    }

    private void UpdatePreviousNoclipMaterials(){
        GameObject previousPuzzle = transform.Find("Puzzle_" + (_currentPuzzleIndex - 1)).gameObject;
        //for each children of RealityObjectsHolder add component NoclipMaterialHolder
        try{
            foreach (Transform child in previousPuzzle.transform.Find("RealityObjectsHolder"))
            {
                child.gameObject.AddComponent<NoclipMaterialHolder>();
                //list of one material containing _previousNoclipMaterial
                Material[] materials = new Material[1];
                materials[0] = _previousNoclipMaterial;
                //add material to NoclipMaterialHolder
                child.gameObject.GetComponent<NoclipMaterialHolder>().SetMaterial(materials);
            }
        }catch{
        }
        try{
            foreach (Transform child in previousPuzzle.transform.Find("IntangibleNoclipObjectsHolder"))
            {
                child.gameObject.AddComponent<NoclipMaterialHolder>();
                //list of one material containing _previousNoclipMaterial
                Material[] materials = new Material[1];
                //add a copy of previousNoclipMaterial to materials
                materials[0] = new Material(_previousNoclipMaterial);
                //set alpha of material to child's material alpha
                materials[0].color = new Color(materials[0].color.r, materials[0].color.g, materials[0].color.b, 0.2f);
                //add material to NoclipMaterialHolder
                child.gameObject.GetComponent<NoclipMaterialHolder>().SetMaterial(materials);
            }
        }catch{
        }
        try{
            foreach (Transform child in previousPuzzle.transform.Find("InvisibleNoclipObjectsHolder"))
            {
                child.gameObject.AddComponent<NoclipMaterialHolder>();
                //list of one material containing _previousNoclipMaterial
                Material[] materials = new Material[1];
                materials[0] = _previousNoclipMaterial;
                //add material to NoclipMaterialHolder
                child.gameObject.GetComponent<NoclipMaterialHolder>().SetMaterial(materials);
            }
        }catch{
        }
    }
}
