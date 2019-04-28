using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>{

    public bool walkable;
    public Vector3 worldPos;
    public int weight;

    public int gCost;
    public int hCost;

    public int gridX;
    public int gridY;

    public Node parent;

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    int heapIndex;

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }


    public Node(bool nWalkable, Vector3 nWorldPos, int nGridX, int nGridY, int nWeight)
    {
        walkable = nWalkable;
        worldPos = nWorldPos;
        weight = nWeight;
        gridX = nGridX;
        gridY = nGridY;
    }


    

    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if(compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }

        return -compare;
    }
}
