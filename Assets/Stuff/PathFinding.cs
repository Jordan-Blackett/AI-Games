using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

public class PathFinding : MonoBehaviour
{
    PathRequestManager RequestManager;
    Grid grid;

    private void Awake()
    {
        RequestManager = GetComponent<PathRequestManager>();
        grid = GetComponent<Grid>();
    }
    public void StartFindPath(Vector3 StartPos, Vector3 TargetPos)
    {
        StartCoroutine(FindPath(StartPos, TargetPos));
    }

    IEnumerator FindPath(Vector3 StartPos, Vector3 TargetPos)
    {

        Stopwatch MyStopwatch = new Stopwatch();

        MyStopwatch.Start();

        Vector3[] Waypoints = new Vector3[0];
        bool PathSuccess = false;

        Node StartNode = grid.NodeFromWorldPoint(StartPos);
        Node TargetNode = grid.NodeFromWorldPoint(TargetPos);

        if(StartNode.IsWalkable && TargetNode.IsWalkable)
        { 

            Heap<Node> OpenSet = new Heap<Node>(grid.MaxSize);

            HashSet<Node> ClosedSet = new HashSet<Node>();

            OpenSet.Add(StartNode);

            while(OpenSet.Count > 0)
            {
                Node CurrentNode = OpenSet.RemoveFirstItem();
                ClosedSet.Add(CurrentNode);

                if(CurrentNode == TargetNode)
                {
                    MyStopwatch.Stop();
                    print("Path Found " + MyStopwatch.ElapsedMilliseconds + " Milliseconds");
                    PathSuccess = true;
                    break;
                }

                foreach (Node Neighbour in grid.GetNeighbours(CurrentNode))
                {
                    if (!Neighbour.IsWalkable || ClosedSet.Contains(Neighbour))
                    {
                        continue;
                    }

                    int NewMovementCostToNeighbour = CurrentNode.GCost + GetDistance(CurrentNode, Neighbour) + Neighbour.MovementPenalty;
                    if (NewMovementCostToNeighbour < Neighbour.GCost || !OpenSet.Contains(Neighbour))
                    {
                        Neighbour.GCost = NewMovementCostToNeighbour;
                        Neighbour.HCost = GetDistance(Neighbour, TargetNode);
                        Neighbour.Parent = CurrentNode;

                        if (!OpenSet.Contains(Neighbour))
                        {
                            OpenSet.Add(Neighbour);
                            OpenSet.UpdateItem(Neighbour);
                        }
                        else
                        {
                            OpenSet.UpdateItem(Neighbour);
                        }
                    }

                }
            }
        }
        yield return null;
        if (PathSuccess)
        {
            Waypoints = RetracePath(StartNode, TargetNode);
        }
        RequestManager.FinishedProcessingPath(Waypoints, PathSuccess);
    }

    Vector3[] RetracePath(Node StartNode, Node EndNode)
    {
        List<Node> Path = new List<Node>();
        Node CurrentNode = EndNode;

        while (CurrentNode != StartNode)
        {
            Path.Add(CurrentNode);
            CurrentNode = CurrentNode.Parent;
        }
        Vector3[] Waypoints =  SimplifyPath(Path);
        Array.Reverse(Waypoints);
        //Waypoints.Reverse();
        return Waypoints;
    }

    Vector3[] SimplifyPath(List<Node> Path)
    {
        List<Vector3> Waypoints = new List<Vector3>();
        Vector2 DirectionOld = Vector2.zero;
        for (int i = 1; i < Path.Count; i ++)
        {
            Vector2 DirectionNew = new Vector2(Path[i - 1].GridX - Path[i].GridX, Path[i - 1].GridY - Path[i].GridY);
            if (DirectionNew != DirectionOld)
            {
                Waypoints.Add(Path[i].NodesWorldPos);
            }
            DirectionOld = DirectionNew;
        }
        return Waypoints.ToArray();
    }

    int GetDistance(Node NodeA, Node NodeB)
    {
        int DistanceX = Mathf.Abs(NodeA.GridX - NodeB.GridX);
        int DistanceY = Mathf.Abs(NodeA.GridY - NodeB.GridY);

        if (DistanceX > DistanceY)
        {
            return 14 * DistanceY + 10 * (DistanceX - DistanceY);
        }
        else
        {
            return 14 * DistanceX + 10 * (DistanceY - DistanceX);
        }
    }
}
