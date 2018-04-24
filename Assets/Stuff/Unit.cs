using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    public Transform Target;
    float Speed = 20;
    Vector3[] Path;
    int TargetIndex;

    private void Start()
    {
        PathRequestManager.RequestPath(transform.position, Target.position, OnPathFound);
    }

    public void OnPathFound(Vector3[] NewPath, bool PathSuccessful)
    {
        if(PathSuccessful)
        {
            Path = NewPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        Vector3 CurrentWaypoint = Path[0];

        while (true)
        {
            if (transform.position == CurrentWaypoint)
            {
                TargetIndex++;
                if (TargetIndex >= Path.Length)
                {
                    yield break;
                }
                CurrentWaypoint = Path[TargetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, CurrentWaypoint, Speed * Time.deltaTime);
            yield return null;
        }
    }

    public void OnDrawGizmos()
    {
        if (Path != null)
        {
            for (int i = TargetIndex; i < Path.Length; i ++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(Path[i], Vector3.one);

                if(i == TargetIndex)
                {
                    Gizmos.DrawLine(transform.position, Path[i]);
                }
                else
                {
                    Gizmos.DrawLine(Path[i - 1], Path[i]);
                }
            }
        }
    }
}
