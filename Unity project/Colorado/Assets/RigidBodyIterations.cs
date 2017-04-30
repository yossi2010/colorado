using UnityEngine;
using System.Collections;

public class RigidBodyIterations : MonoBehaviour {
public int Iterations=15;
public int MaxRotSpeed=7;
	// Use this for initialization
	void Start () {
	GetComponent<Rigidbody>().solverIterations=Iterations;
	GetComponent<Rigidbody>().maxAngularVelocity=MaxRotSpeed;

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
