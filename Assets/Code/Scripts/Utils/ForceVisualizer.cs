using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceVisualizer : MonoBehaviour
{
    //private Rigidbody
    private Rigidbody _rigidbody;
    private GameObject _forceVisualizer;
    private GameObject _endCone;
    //List<Vector3> forces
    private Vector3 _force;
    // Start is called before the first frame update
    void Start()
    {
        //get rigidbody of RealityPlayer
        _rigidbody = GameObject.Find("RealityPlayer").GetComponent<Rigidbody>();
        //assign a pole as a child of this object
        GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        //name pole "ForceVisualizerVector"
        pole.name = "ForceVisualizerVector";
        _forceVisualizer = pole;
        //remove collider of pole
        Destroy(pole.GetComponent<Collider>());
        //change color of pole
        pole.GetComponent<Renderer>().material.color = Color.red;

        _endCone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        //remove collider of _endCone
        Destroy(_endCone.GetComponent<Collider>());
        //change color of _endCone
        _endCone.GetComponent<Renderer>().material.color = Color.yellow;
        _endCone.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
    }

    // Update is called once per frame
    void Update()
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

    public void UpdateForces(List<Vector3> forces)
    {
        //set force to sum of forces
        _force = Vector3.zero;
        try
        {
            foreach (Vector3 force in forces)
            {
                _force += force;
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("no forces");
        }
    }
}
