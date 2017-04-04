using UnityEngine;
using System.Collections;

public class AntiRollBar : MonoBehaviour {
	public WheelCollider WheelR,WheelL;
	public float  AntiRoll = 5000;
	Rigidbody rb;
	// Use this for initialization
	void Start () {
	rb=GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	WheelHit hit;
    float travelL = 1;
    float travelR = 1;
 
    bool groundedL = WheelL.GetGroundHit(out hit);
    if (groundedL)
        travelL = (-WheelL.transform.InverseTransformPoint(hit.point).y - WheelL.radius) / WheelL.suspensionDistance;
 
    bool groundedR = WheelR.GetGroundHit(out hit);
    if (groundedR)
        travelR = (-WheelR.transform.InverseTransformPoint(hit.point).y - WheelR.radius) / WheelR.suspensionDistance;
 
    float antiRollForce = (travelL - travelR) * AntiRoll;
 
    if (groundedL)
        rb.AddForceAtPosition(WheelL.transform.up * -antiRollForce,
               WheelL.transform.position); 
    if (groundedR)
        rb.AddForceAtPosition(WheelR.transform.up * antiRollForce,
               WheelR.transform.position); 
	}
}
