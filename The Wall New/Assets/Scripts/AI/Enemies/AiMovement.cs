using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiMovement : MonoBehaviour
{
    public float wanderRadius;
    public float wanderTimer;
    private NavMeshAgent agent;
 
    void Start() 
    {
        agent = GetComponent<NavMeshAgent> ();
        InvokeRepeating("setNewRandom",0.05f,wanderTimer);
    }
 
    private void setNewRandom()
    {
        Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
        agent.SetDestination(newPos);//sets new destination
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask) 
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
 
        randDirection += origin;
 
        NavMeshHit navHit;
 
        NavMesh.SamplePosition (randDirection, out navHit, dist, layermask);
 
        return navHit.position;
    }
}
