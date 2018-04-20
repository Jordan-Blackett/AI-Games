using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint_Debug_Connected : MonoBehaviour {

    public float debugDrawRadius = 1.0f;

    [SerializeField]
    protected float connectivityRadius = 50f;

    public List<Waypoint_Debug_Connected> connections;

	// Use this for initialization
	void Start () {
        GameObject[] waypoints = GameObject.FindGameObjectsWithTag("Waypoints");

        connections = new List<Waypoint_Debug_Connected>();
        
        // Check if waypoints are connected
        for(int i = 0; i < waypoints.Length; i++)
        {
            Waypoint_Debug_Connected nextWaypoint = waypoints[i].GetComponent<Waypoint_Debug_Connected>();

            if (nextWaypoint != null)
            {
                if (Vector3.Distance(this.transform.position, nextWaypoint.transform.position) <= connectivityRadius && nextWaypoint != this)
                {
                    connections.Add(nextWaypoint);
                }
            }
        }
    }
	
	// Update is called once per frame
	void Update () {}

    public Waypoint_Debug_Connected NextWaypoint(Waypoint_Debug_Connected previousWaypoint)
    {
        if (connections.Count == 0)
        {
            Debug.LogError("No Waypoints");
            return null;
        }
        else if (connections.Count == 1 && connections.Contains(previousWaypoint))
        {
            // Dead end
            return previousWaypoint;
        }
        else
        {
            Waypoint_Debug_Connected nextWaypoint;
            int nextIndex = 0;

            do
            {
                nextIndex = UnityEngine.Random.Range(0, connections.Count);
                nextWaypoint = connections[nextIndex];
            }
            while (nextWaypoint == previousWaypoint);

            return nextWaypoint;
        }
    }

    public virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, debugDrawRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, connectivityRadius);
    }
}
