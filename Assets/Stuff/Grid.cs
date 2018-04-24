using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public bool DisplayGridGizmos;
    public LayerMask UnwalkableMask;
    public Vector2 GridWorldSize;
    public float NodeRadius;
    public TerrainType[] WalkableRegions;
    public int ObsticleProximetyPenalty = 10;
    LayerMask WalkableMask;
    Dictionary<int, int> WalkableRegionsDictionary = new Dictionary<int, int>();
    Node[,] grid;
    float NodeDiameter;
    int GridSizeX, GridSizeY;
    int PenaltyMin = int.MaxValue;
    int PenaltyMax = int.MinValue;

    void Awake()
    {
        //Works out the Diameter of a node
        NodeDiameter = NodeRadius * 2;
        //Works out how many nodes I can have depending on the world size
        //I use the Mathf.RoundToInt function as we cannnot have half a node
        GridSizeX = Mathf.RoundToInt(GridWorldSize.x / NodeDiameter);
        GridSizeY = Mathf.RoundToInt(GridWorldSize.y / NodeDiameter);

        foreach (TerrainType region in WalkableRegions)
        {
            WalkableMask.value = WalkableMask |= region.TerrainMask.value;
            WalkableRegionsDictionary.Add((int)Mathf.Log(region.TerrainMask.value, 2), region.TerrainPenalty);
        }

        CreateGrid();
    }

    public int MaxSize
    {
        get
        {
            return GridSizeX * GridSizeY;
        }
    }

    void CreateGrid()
    {
        grid = new Node[GridSizeX, GridSizeY];
        Vector3 WorldBottomLeft = transform.position - Vector3.right * GridWorldSize.x/2 - Vector3.forward * GridWorldSize.y/2;
        //Loops through the entire world space
        for (int x = 0; x < GridSizeX; x++)
        {
            for (int y = 0; y < GridSizeY; y++)
            {
                Vector3 WorldPoint = WorldBottomLeft + Vector3.right * (x * NodeDiameter + NodeRadius) + Vector3.forward * (y * NodeDiameter + NodeRadius);
                //Collision Check
                bool IsWalkable = !(Physics.CheckSphere(WorldPoint, NodeRadius, UnwalkableMask));
                int MovementPenalty = 0;

                Ray ray = new Ray(WorldPoint + Vector3.up * 75, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray,out hit, 150, WalkableMask))
                {
                    WalkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out MovementPenalty);
                }

                if (!IsWalkable)
                {
                    MovementPenalty += ObsticleProximetyPenalty;
                }
              
                grid[x, y] = new Node(IsWalkable, WorldPoint, x, y, MovementPenalty);
            }
        }

        BlurPenaltyMap(3);
    }

    void BlurPenaltyMap(int BlurSize)
    {
        int KernelSize = BlurSize * 2 + 1;
        int KernelExtents = KernelSize - 1 / 2;

        int[,] PenaltiesHorizontalPass = new int[GridSizeX, GridSizeY];
        int[,] PenaltiesVirticalPass = new int[GridSizeX, GridSizeY];

        for (int y = 0; y < GridSizeY; y++)
        {
            for (int x = -KernelExtents; x <= +KernelExtents; x++)
            {
                int SampleX = Mathf.Clamp(x, 0, KernelExtents);
                PenaltiesHorizontalPass[0, y] += grid[SampleX, y].MovementPenalty;
            }

            for (int x = 1; x < GridSizeX; x++)
            {
                int RemoveIndex = Mathf.Clamp(x - KernelExtents - 1, 0, GridSizeX);
                int AddIndex = Mathf.Clamp(x + KernelExtents, 0, GridSizeX - 1);

                PenaltiesHorizontalPass[x, y] = PenaltiesHorizontalPass[x - 1, y] - grid[RemoveIndex, y].MovementPenalty + grid[AddIndex, y].MovementPenalty;
            }
        }
        for (int x = 0; x < GridSizeX; x++)
        {
            for (int y = -KernelExtents; y <= +KernelExtents; y++)
            {
                int SampleY = Mathf.Clamp(y, 0, KernelExtents);
                PenaltiesVirticalPass[x, 0] += PenaltiesHorizontalPass[x, SampleY];
            }

            for (int y = 1; y < GridSizeY; y++)
            {
                int RemoveIndex = Mathf.Clamp(y - KernelExtents - 1, 0, GridSizeY);
                int AddIndex = Mathf.Clamp(y + KernelExtents, 0, GridSizeY - 1);

                PenaltiesVirticalPass[x, y] = PenaltiesVirticalPass[x, y - 1] - PenaltiesHorizontalPass[x, RemoveIndex] + PenaltiesHorizontalPass[x, AddIndex];

                int BlurredPenalty = Mathf.RoundToInt((float)PenaltiesVirticalPass[x,y] / (KernelSize * KernelSize));
                grid[x, y].MovementPenalty = BlurredPenalty;

                if (BlurredPenalty > PenaltyMax)
                {
                    PenaltyMax = BlurredPenalty;
                }
                if (BlurredPenalty < PenaltyMin)
                {
                    PenaltyMin = BlurredPenalty;
                }
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> Neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }
                int CheckX = node.GridX + x;
                int CheckY = node.GridY + y;

                if (CheckX >= 0 && CheckX < GridSizeX && CheckY >= 0 && CheckY < GridSizeY)
                {
                    Neighbours.Add(grid[CheckX, CheckY]);
                }
            }
        }

        return Neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 WorldPosition)
    {
        float PercentX = (WorldPosition.x + GridWorldSize.x / 2) / GridWorldSize.x;
        float PercentY = (WorldPosition.z + GridWorldSize.y / 2) / GridWorldSize.y;
        PercentX = Mathf.Clamp01(PercentX);
        PercentY = Mathf.Clamp01(PercentY);
        int x = Mathf.RoundToInt((GridSizeX - 1) * PercentX);
        int y = Mathf.RoundToInt((GridSizeY - 1) * PercentY);
        return grid[x, y];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(GridWorldSize.x, 1, GridWorldSize.y));

        {
            if (grid != null && DisplayGridGizmos)
            {
                foreach (Node N in grid)
                {
                    Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(PenaltyMin, PenaltyMax, N.MovementPenalty));

                    Gizmos.color = (N.IsWalkable) ? Gizmos.color:Color.red;
                    Gizmos.DrawCube(N.NodesWorldPos, Vector3.one * (NodeDiameter));
                }
            }
        }
    }
    [System.Serializable]
    public class TerrainType
    {
        public LayerMask TerrainMask;
        public int TerrainPenalty;
    }
}