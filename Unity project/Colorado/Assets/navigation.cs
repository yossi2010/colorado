using UnityEngine;
using System.Collections;

public class navigation : MonoBehaviour {
    // MoveTo.cs
       
       public Transform goal,myref;
       RaycastHit Down, Left, Right;
       Vector3 LeftNorm,RightNorm,RightAdj,LeftAdj ,Dir;
       public float Vel=10,DesiredHeight=1,RotSpeed=0.2f;
       void Start () {
           myref=transform;
        
        //   NavMeshAgent agent = GetComponent<NavMeshAgent>();
        //   agent.destination = goal.position; 
       }
       void FixedUpdate(){
        Physics.Raycast(myref.position,-myref.up, out Down,10);
        Debug.DrawLine(transform.position, Down.point, Color.red);
        if(!Down.collider) UnityEditor.EditorApplication.isPaused = true;
        Physics.Raycast(myref.position,myref.right, out Right);
        Physics.Raycast(myref.position,-myref.right, out Left);
        Debug.DrawLine(transform.position, Right.point, Color.red);
        Debug.DrawLine(transform.position, Left.point, Color.red);
        RightNorm=new Vector3(Right.normal.x,0,Right.normal.z).normalized;
        LeftNorm=new Vector3(Left.normal.x,0,Left.normal.z).normalized;
        RightAdj=Vector3.Cross(RightNorm,-myref.up);
        LeftAdj=Vector3.Cross(LeftNorm,myref.up);
        Dir=(RightAdj+LeftAdj).normalized;
        Vector3 newDir = Vector3.RotateTowards(myref.forward, Dir, RotSpeed*Time.fixedDeltaTime, 0.0F);
        transform.rotation = Quaternion.LookRotation(newDir);
        // if(Right.distance<Left.distance)
        myref.Translate((Right.distance-Left.distance)*Time.fixedDeltaTime,(DesiredHeight-Down.distance),Vel*Time.fixedDeltaTime);
        // else myref.Translate((Left.distance-Right.distance)*Time.fixedDeltaTime,(DesiredHeight-Down.distance),Vel*Time.fixedDeltaTime);
       }
    }

