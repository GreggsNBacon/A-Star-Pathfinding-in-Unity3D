using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour {

    Grid grid;

    public Transform seeker, target;

    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            FindPath(seeker.position, target.position); //finds the path from the seeking point to the target point.
            grid.generateVisiblePath(); //generates the visible path to be shown without the editor open.
        }
        
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos); //gets the starting node from the worldpoint
        Node targetNode = grid.NodeFromWorldPoint(targetPos); //gets the ending node from the worldpoint

        Heap<Node> openSet = new Heap<Node>(grid.MaxSize); //initialises the opening set as a heap as a form of optimization.
        HashSet<Node> closedSet = new HashSet<Node>(); //sets the closed set as a hashset as order is not important for the closed nodes

        openSet.Add(startNode); //adds the starting node to the openSet to begin

        while(openSet.Count > 0) //keeps the A Star algorithm running whilst there is a value in openSet
        {
            
            Node currentNode = openSet.RemoveFirst();
            
            closedSet.Add(currentNode);
            
            if (currentNode.gridX == targetNode.gridX && currentNode.gridY == targetNode.gridY)
            {
                
                Retrace(startNode, targetNode);
                return;
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour)) continue;

                int newMoveConstToNeigh = currentNode.gCost + GetDist(currentNode, neighbour) + neighbour.weight;

                if(newMoveConstToNeigh < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMoveConstToNeigh;
                    neighbour.hCost = GetDist(neighbour, targetNode);

                    neighbour.parent = currentNode;
                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                    else
                    {
                        openSet.UpdateItem(neighbour);
                    }
                }
            }
        }
    }

    void Retrace(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        grid.path = path;
    }

    int GetDist(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        int intToReturn = 0;

        if(distX > distY)
        {
            intToReturn = 14 * distY + 10 * (distX - distY);
        }
        else
        {
            intToReturn = 14 * distX + 10 * (distY - distX);
        }

        return intToReturn;
    }
}
