using UnityEngine;
using System.Collections;

public class ColoradoDrive : MonoBehaviour {
	public float MaxSpeed=400,Speed,steeringangle=0;
	public float MaxSteer=0.6f;
	public HingeJoint[] joints;
	public ConfigurableJoint[] steering;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	steeringangle=-MaxSteer*Input.GetAxis("Horizontal");
	Speed=MaxSpeed*Input.GetAxis("Vertical");
	for (int i = 0; i<joints.Length; i++)
	{
		var tempmotor=joints[i].motor;
		tempmotor.targetVelocity=Speed;
		joints[i].motor=tempmotor;
	}
	for (int i = 0; i < steering.Length; i++)
	{
		var temp=steering[i].targetRotation;
		temp.x=steeringangle;
		steering[i].targetRotation=temp;
	}
	}
}
