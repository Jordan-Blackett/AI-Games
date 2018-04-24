using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Node : IHeapItem<Node>
{
    public bool IsWalkable;
    public Vector3 NodesWorldPos;
    public int GridX;
    public int GridY;

    public int MovementPenalty;

    public int GCost;
    public int HCost;
    public Node Parent;
    int _HeapIndex;

    public Node(bool _IsWalkable, Vector3 _WorldPos, int _GridX, int _GridY, int _Penalty)
    {
        IsWalkable = _IsWalkable;
        NodesWorldPos = _WorldPos;
        GridX = _GridX;
        GridY = _GridY;
        MovementPenalty = _Penalty;
    }

    public int FCost
    {
        get
        {
            return GCost + HCost;
        }
    }

    public int HeapIndex
    {
        get
        {
            return _HeapIndex;
        }
        set
        {
            _HeapIndex = value;
        }
    }

    public int CompareTo(Node NodeToCompare)
    {
        int Compare = FCost.CompareTo(NodeToCompare.FCost);
        if (Compare == 0)
        {
            Compare = HCost.CompareTo(NodeToCompare.HCost);
        }
        return -Compare;
    }
}
