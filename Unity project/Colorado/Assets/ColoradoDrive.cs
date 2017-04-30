using UnityEngine;
using System.Collections;

public class ColoradoDrive : MonoBehaviour
{
    public float MaxTorque = 400, Torque, steeringangle = 0;
    public float MaxSteer = 0.4f, VehicleWidth = 2, VehicleLength = 3;
    public Rigidbody[] joints;
    Transform myref;
    Rigidbody rb;
    float[] Theta={0,0};
    public ConfigurableJoint[] steering;
    private float ThetaAckerman;
    private float xVel,Throttle;


    // Use this for initialization
    void Start()
    {
        myref=transform;
        rb=GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        xVel = myref.InverseTransformDirection(rb.velocity).z;
        steeringangle = -MaxSteer * Input.GetAxis("Horizontal");
        Throttle=Input.GetAxis("Vertical");
        
        Torque = MaxTorque * Throttle;
        for (int i = 0; i < joints.Length; i++)
        {
            // var tempmotor=joints[i].motor;
            // tempmotor.targetVelocity=Speed;
            // joints[i].motor=tempmotor;
            joints[i].AddRelativeTorque(new Vector3(Torque, 0, 0), ForceMode.Force);
        }
        if (steeringangle >= 0) //turning right
        {
            ThetaAckerman = Mathf.Atan(1 / ((1 / (Mathf.Tan(steeringangle)) + (VehicleWidth / VehicleLength))));
            Theta[0] = steeringangle;
            Theta[1] = ThetaAckerman;
        }
        else if (steeringangle < 0) //turning left
        {
            ThetaAckerman = Mathf.Atan(1 / ((1 / (Mathf.Tan(-steeringangle)) + (VehicleWidth / VehicleLength))));
            Theta[0] = -ThetaAckerman;
            Theta[1] = steeringangle;
        }
        for (int i = 0; i < steering.Length; i++)
        {
            var temp = steering[i].targetRotation;
            temp.x = Theta[i];
            steering[i].targetRotation = temp;
        }
    }
}
