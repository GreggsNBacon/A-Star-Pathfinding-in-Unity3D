using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {
    public float nodeRadius;

    [SerializeField]
    bool enableGridRendering;

    [SerializeField]
    bool displayPath;

    [SerializeField]
    bool displayGrid;

    [SerializeField]
    bool displayWalkable;

    [SerializeField]
    bool displayBlocked;

    [SerializeField]
    Terrain ground;

    [SerializeField]
    float max = 2.5f, min = 1.5f;

    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    TerrainData terrainData;

    public List<Node> path;

    public LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        nodeDiameter = nodeRadius * 2;
        terrainData = ground.terrainData;
        gridSizeX = Mathf.RoundToInt(terrainData.size.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(terrainData.size.z / nodeDiameter);
        

        GridCreation();

    }

    public void generateVisiblePath()
    {
        lr.positionCount = path.Count;

        Vector3[] positions = new Vector3[path.Count];

        for (int i = 0; i < lr.positionCount; i++)
        {
            positions[i] = new Vector3(path[i].worldPos.x, path[i].worldPos.y +3, path[i].worldPos.z);
        }

        lr.SetPositions(positions);
    }

    void GridCreation()
    {
        grid = new Node[gridSizeX, gridSizeY];

        for(int x = 0; x<gridSizeX; x++)
        {
            for(int y = 0; y <gridSizeY; y++)
            {
                Vector3 worldPoint = Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = true;
                if(ground.SampleHeight(worldPoint) >= max || ground.SampleHeight(worldPoint) <= min)
                {
                    walkable = false;
                }
                int weight = 10;
                if (walkable)
                {
                    
                    if (GetMainTexture(worldPoint) == 1)
                    {
                        weight = 50;
                    }
                    if (GetMainTexture(worldPoint) == 2)
                    {
                        weight = 30;
                    }
                    if (GetMainTexture(worldPoint) == 3)
                    {
                        weight = 0;
                    }
                }
                grid[x, y] = new Node(walkable, worldPoint, x, y, weight); //if tile is walkable, where it is in the world, x coord in grid, y coord in grid and weight
            }
        }

        
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = worldPosition.x  / terrainData.size.x;
        float percentY = worldPosition.z / terrainData.size.z;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY -1) * percentY);

        return grid[x, y];
    }
    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(ground.terrainData.size.x, 1, ground.terrainData.size.z));
        if (grid != null && enableGridRendering)
        {

            foreach(Node n in grid)
            {
                if (displayGrid)
                {
                    Gizmos.color = Color.gray;
                    Gizmos.DrawWireCube(new Vector3(n.worldPos.x, ground.SampleHeight(n.worldPos), n.worldPos.z), Vector3.one * (nodeDiameter - 0.2f));
                }
                if (n.walkable && displayWalkable)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(new Vector3(n.worldPos.x, ground.SampleHeight(n.worldPos), n.worldPos.z), Vector3.one * (nodeDiameter - 0.2f));
                }
                else if(!n.walkable && displayBlocked)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(new Vector3(n.worldPos.x, ground.SampleHeight(n.worldPos), n.worldPos.z), Vector3.one * (nodeDiameter - 0.2f));
                }
                
                if(path!= null)
                {
                    if (path.Contains(n) && displayPath)
                    {
                        Gizmos.color = Color.blue;
                        Gizmos.DrawCube(new Vector3(n.worldPos.x, ground.SampleHeight(n.worldPos), n.worldPos.z), Vector3.one * (nodeDiameter - 0.2f));
                    }
                }
                
            }
            if(path != null)
            {
                Debug.Log(1);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++) 
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    private float[] GetTextureMix(Vector3 WorldPos)
    {
        /* returns mix of textures on terrain at exact worldpos.*/

        /*values in this array will be number of textures applied to terrain*/
        
        int mapX = (int)(((WorldPos.x - ground.transform.position.x) / terrainData.size.x) * terrainData.alphamapWidth);
        int mapZ = (int)(((WorldPos.z - ground.transform.position.z) / terrainData.size.z) * terrainData.alphamapHeight);

        // get the splat data for this cell as a 1x1xN 3d array (where N = number of textures)
        float[,,] splatmapData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);

        // extract the 3D array data to a 1D array:
        float[] cellMix = new float[splatmapData.GetUpperBound(2) + 1];

        for (int n = 0; n < cellMix.Length; n++)
        {
            cellMix[n] = splatmapData[0, 0, n];
        }
        return cellMix;
    }

    private int GetMainTexture(Vector3 WorldPos)
    {
        /* returns the index of the dominant texture at this position*/
        float[] mix = GetTextureMix(WorldPos);

        float maxMix = 0;
        int maxIndex = 0;

        // loop through each mix value and find the maximum
        for (int n = 0; n < mix.Length; n++)
        {
            if (mix[n] > maxMix)
            {
                maxIndex = n;
                maxMix = mix[n];
            }
        }
        
        return maxIndex;
    }
}
