using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
namespace UnityStandardAssets.Vehicles.Car{
[RequireComponent(typeof(CarController))]
public class CarTargetControl : MonoBehaviour {

        private CarController m_Car; // the car controller we want to use
		float Throttle=0;
		Rigidbody rb;
		public navigation Navigator;
		Transform myref;
		public Transform Target;
        public float SteerSpeed = 0.1f,TargetSpeed=0,DesiredSpeed,TargetDistance=3;
        float Steer=0;
        private void Awake()
        {
			rb=GetComponent<Rigidbody>();
			myref=transform;
            // get the car controller
            m_Car = GetComponent<CarController>();
        }


        private void FixedUpdate()
        {
			Vector3 loc=myref.InverseTransformPoint(Target.position);
			float dist=loc.magnitude;
			float ang=Vector3.Angle(Vector3.forward,new Vector3(loc.x,0,loc.z));
			Steer=loc.x>0?ang/25:-ang/25;
			float Velocity=myref.InverseTransformVector(rb.velocity).z;
			// Debug.Log(Velocity);
			TargetSpeed=DesiredSpeed+dist-TargetDistance;
			Throttle=Mathf.Clamp((TargetDistance/3)*(TargetSpeed-Velocity),-0.05f,1);
			Navigator.Vel=Mathf.Clamp(DesiredSpeed-dist+TargetDistance,0,DesiredSpeed+3);
            // pass the input to the car!

            m_Car.Move(Steer, Throttle, Throttle, 0);
        }
    }
}
