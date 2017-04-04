using UnityEngine;
using System.Collections;

public class navigation : MonoBehaviour {
    // MoveTo.cs
       
       public Transform goal,car;
       
       void Start () {
          NavMeshAgent agent = GetComponent<NavMeshAgent>();
          agent.destination = goal.position; 
       }
    }

