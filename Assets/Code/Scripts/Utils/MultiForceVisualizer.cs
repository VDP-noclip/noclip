using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiForceVisualizer : MonoBehaviour
{
    //serialize player
    private GameObject _player;
    //private Rigidbody
    private Rigidbody _rigidbody;
    private List<GameObject> _forceVisualizers = new List<GameObject>();
    private GameObject _endCone;
    //List<Vector3> forces
    private List<Vector3> _forces;
    //list of 7 different color materials
    private Material _exceptionMaterial;
    private RealityMovementCalibration realityMovementCalibration;
    private Color[] _colors = new Color[7] { Color.red, Color.blue, Color.green, Color.yellow, Color.magenta, Color.cyan, Color.white };
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("RealityPlayer");
        //get rigidbody of RealityPlayer
        //_rigidbody = GameObject.Find("RealityPlayer").GetComponent<Rigidbody>();
        _rigidbody = _player.GetComponent<Rigidbody>();
        //get realityplayer realitymovementcalibration
        //RealityMovementCalibration realityMovementCalibration = GameObject.Find("RealityPlayer").GetComponent<RealityMovementCalibration>();
        realityMovementCalibration = _player.GetComponent<RealityMovementCalibration>();
        //for each color
        foreach(Color color in _colors){
            GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(pole.GetComponent<Collider>());
            //name pole "ForceVisualizerVector"
            pole.name = "ForceVisualizerVector";
            _forceVisualizers.Add(pole);
            //pole corresponding color from array of colors
            //pole.GetComponent<Renderer>().material.color = color;
            //disable shadow
            pole.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            //disable all shadows
            pole.GetComponent<Renderer>().receiveShadows = false;
            //new material clone of realitybody material
            // _materials.Add(new Material(GameObject.Find("RealityBody").GetComponent<MeshRenderer>().material));
            //_materials[_materials.Count - 1].color = color;
            //set material of pole
            //pole.GetComponent<MeshRenderer>().material = _materials[_materials.Count - 1];
            //set material of pole to material of realitybody
            pole.GetComponent<Renderer>().material = new Material(GameObject.Find("RealityBody").GetComponent<MeshRenderer>().material);
            pole.GetComponent<Renderer>().material.color = color;
            //add material to list of materials
        }
        _exceptionMaterial = new Material(GameObject.Find("RealityBody").GetComponent<MeshRenderer>().material);
        _exceptionMaterial.color = Color.red;
        
    }

    // single vector (sum)
    /*
    void FixedUpdate()
    {
        //set the position of the _forceVisualizer to the position of the RealityPlayer
        _forceVisualizer.transform.position = _rigidbody.transform.position;
        //get velocity vecor of the rigidbody
        Vector3 velocity = _force;
        //set the scale of the _forceVisualizer to the velocity vector
        _forceVisualizer.transform.localScale = new Vector3(0.1f, velocity.magnitude/100, 0.1f);
        //set the rotation of the _forceVisualizer to the velocity vector
        _forceVisualizer.transform.rotation = Quaternion.LookRotation(velocity);
        //rotate the _forceVisualizer by 90 degrees
        _forceVisualizer.transform.Rotate(90, 0, 0);
        //_forceVisualizer.transform.Rotate(0, 0, 90);
        //move the _forceVisualizer by half of its length in the direction Quaternion.LookRotation(velocity)
        //find one end of the _forceVisualizer
        Vector3 end = _forceVisualizer.transform.position + _forceVisualizer.transform.rotation * new Vector3(0, 0.5f * _forceVisualizer.transform.localScale.y, 0);
        //difference between the end of the end and the position of the RealityPlayer
        Vector3 difference = end - _rigidbody.transform.position;
        //move the _forceVisualizer by the difference
        _forceVisualizer.transform.position += difference;
        //find the other end of the _forceVisualizer
        end = _forceVisualizer.transform.position + _forceVisualizer.transform.rotation * new Vector3(0, 0.7f * _forceVisualizer.transform.localScale.y, 0);
        //put a cone at the end of the _forceVisualizer
        //set the position of the _endCone to the end of the _forceVisualizer
        _endCone.transform.position = end;
        //orient the cone to the _forceVisualizer
        _endCone.transform.rotation = _forceVisualizer.transform.rotation;
    }
    */
    
    //multiple vectors
    void Update()
    {

        //destroy all gameobjects in _forceVisualizers
        
        try{
            foreach (GameObject forceVisualizer in _forceVisualizers)
            {
                forceVisualizer.GetComponent<MeshRenderer>().enabled = false;
            }
            //for each force in the list of forces print ciao
            
            if(realityMovementCalibration.ShowForces()){
                foreach (Vector3 force in _forces)
                {
                    //set pole to corresponding element in _forceVisualizers
                    GameObject pole = _forceVisualizers[_forces.IndexOf(force)];
                    //remove collider of pole
                    //set the position of the pole to the position of the RealityPlayer
                    pole.transform.position = _rigidbody.transform.position;
                    //get velocity vecor of the rigidbody
                    Vector3 velocity = force;
                    //set the scale of the pole to the velocity vector
                    pole.transform.localScale = new Vector3(0.1f, velocity.magnitude/100, 0.1f);
                    //set the rotation of the pole to the velocity vector
                    if(velocity.magnitude != 0)
                        pole.transform.rotation = Quaternion.LookRotation(velocity);
                    //rotate the pole by 90 degrees
                    pole.transform.Rotate(90, 0, 0);
                    //pole.transform.Rotate(0, 0, 90);
                    //move the pole by half of its length in the direction Quaternion.LookRotation(velocity)
                    //find one end of the pole
                    Vector3 end = pole.transform.position + pole.transform.rotation * new Vector3(0, 0.5f * pole.transform.localScale.y, 0);
                    //difference between the end of the end and the position of the RealityPlayer
                    Vector3 difference = end - _rigidbody.transform.position;
                    //move the pole by the difference
                    pole.transform.position += difference;
                    //find the other end of the pole
                    end = pole.transform.position + pole.transform.rotation * new Vector3(0, 0.7f * pole.transform.localScale.y, 0);
                    //move pole up by 4
                    //pole.transform.position += new Vector3(0, 4, 0);
                    //show mesh of forceVisualizer
                    pole.GetComponent<MeshRenderer>().enabled = true;
                }
            }
        }
        catch{
            //spawn a big red cube
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //rename cube
            cube.name = "ErrorCube";
            cube.transform.position = _rigidbody.transform.position + new Vector3(0, 10, 0);
            cube.transform.localScale = new Vector3(10f, 10f, 10f);
            cube.GetComponent<Renderer>().material = _exceptionMaterial;
            //disable collider
            Destroy(cube.GetComponent<Collider>());
        }
    
        //spawn a big red cube
        //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //cube.transform.position = _rigidbody.transform.position;
        //cube.transform.localScale = new Vector3(10f, 10f, 10f);
        //cube.GetComponent<Renderer>().material.color = Color.red;
    }

    public void UpdateForces(List<Vector3> forces)
    {
        //set the _forces to the forces
        _forces = forces;
    }
}
