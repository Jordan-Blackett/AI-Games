using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie_Patrol : MonoBehaviour {

    [SerializeField]
    bool patrolWaiting;
    float totalWaitTime = 3f;

    [SerializeField]
    float switchProbability = 0.2f;

    NavMeshAgent navMeshAgent;
    public Waypoint_Debug_Connected currectWaypoint;
    Waypoint_Debug_Connected previousWaypoint;

    bool travelling;
    bool waiting;
    float waitTimer;
    int waypointsVisited;

    // Use this for initialization
    void Start () {
        navMeshAgent = this.GetComponent<NavMeshAgent>();

        if (navMeshAgent == null)
        {

        }

        SetDestination();
    }
	
	// Update is called once per frame
	void Update () {
        if (travelling && navMeshAgent.remainingDistance <= 1.0f)
        {
            travelling = false;
            waypointsVisited++;

            if (patrolWaiting)
            {
                waiting = true;
                waitTimer = 0f;
            }
            else
            {
                SetDestination();
            }
        }
    }

    public void SetDestination()
    {
        if (waypointsVisited > 0)
        {
            Waypoint_Debug_Connected nextWaypoint = currectWaypoint.NextWaypoint(previousWaypoint);
            previousWaypoint = currectWaypoint;
            currectWaypoint = nextWaypoint;
        }

        if (currectWaypoint != null)
        {
            Vector3 targetVector = currectWaypoint.transform.position;
            navMeshAgent.SetDestination(targetVector);
            travelling = true;
        }
    }
}
