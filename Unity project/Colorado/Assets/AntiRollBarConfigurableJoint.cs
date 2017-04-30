using UnityEngine;
using System.Collections;

public class AntiRollBarConfigurableJoint : MonoBehaviour {

	public Transform WheelR,WheelL;
	public float  AntiRoll = 5000,initheight;
	Rigidbody rb;
	// Use this for initialization
	void Start () {
		initheight=WheelL.localPosition.y-0.1f;
	rb=GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	WheelHit hit;
    float travelL = 1;
    float travelR = 1;

    bool groundedL = Physics.Raycast(WheelL.position,Vector3.down,0.4f);
    if (groundedL)
        travelL = initheight-WheelL.localPosition.y;
 
    bool groundedR = Physics.Raycast(WheelR.position,Vector3.down,0.4f);
    if (groundedR)
        travelR = initheight-WheelR.localPosition.y;
 
    float antiRollForce = (travelL - travelR) * AntiRoll;
 
    if (groundedL)
        rb.AddForceAtPosition(WheelL.transform.up * -antiRollForce,
               WheelL.transform.position); 
    if (groundedR)
        rb.AddForceAtPosition(WheelR.transform.up * antiRollForce,
               WheelR.transform.position); 
	}
}
