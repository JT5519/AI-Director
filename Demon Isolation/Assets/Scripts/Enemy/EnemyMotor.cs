using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMotor : MonoBehaviour
{
    NavMeshAgent agent;
   
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    public void MoveToPoint(Vector3 point)
    {
        agent.SetDestination(point);
    }
}
