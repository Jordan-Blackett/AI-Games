using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie_Move : MonoBehaviour {

    [SerializeField]
    Transform destination;

    NavMeshAgent navMeshAgent;

	// Use this for initialization
	void Start () {
        navMeshAgent = this.GetComponent<NavMeshAgent>();

        if (navMeshAgent == null)
            Debug.LogError("Nav Mesh is null " + gameObject.name);
        //else
        //    SetDestination();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetDestination()
    {
        if(destination != null)
        {
            Vector3 targetVector = destination.transform.position;
            navMeshAgent.SetDestination(targetVector);
        }
    }

    public void PauseMovement(bool pause)
    {
        navMeshAgent.isStopped = pause;
    }   
}
