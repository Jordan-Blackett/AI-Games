using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathRequestManager : MonoBehaviour
{

    Queue<PathRequest> PathRequestQueue = new Queue<PathRequest>();
    PathRequest CurrentPathRequest;

    static PathRequestManager Instance;
    PathFinding pathFinding;

    bool IsProcessingPath;

    void Awake()
    {
        Instance = this;
        pathFinding = GetComponent<PathFinding>();
    }

    public static void RequestPath(Vector3 PathStart, Vector3 PathEnd, Action<Vector3[], bool> CallBack)
    {
        PathRequest NewRequest = new PathRequest(PathStart, PathEnd, CallBack);
        Instance.PathRequestQueue.Enqueue(NewRequest);
        Instance.TryProcessNext();
    }

    void TryProcessNext()
    {
        if(!IsProcessingPath && PathRequestQueue.Count > 0)
        {
            CurrentPathRequest = PathRequestQueue.Dequeue();
            IsProcessingPath = true;
            pathFinding.StartFindPath(CurrentPathRequest.PathStart, CurrentPathRequest.PathEnd);
        }
    }

    public void FinishedProcessingPath(Vector3[] Path, bool Success)
    {
        CurrentPathRequest.CallBack(Path, Success);
        IsProcessingPath = false;
        TryProcessNext();
    }

    struct PathRequest
    {
        public Vector3 PathStart;
        public Vector3 PathEnd;
        public Action<Vector3[], bool> CallBack;
        public PathRequest(Vector3 _Start, Vector3 _End, Action<Vector3[], bool> _CallBack)
        {
            PathStart = _Start;
            PathEnd = _End;
            CallBack = _CallBack;
        }
    }
}
