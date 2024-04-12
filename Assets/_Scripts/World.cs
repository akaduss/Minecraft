using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class World : MonoBehaviour
{
    public static Vector3Int WorldDimensions = new(3, 9, 3);
    public static Vector3Int ExtraWorldDimensions = new(1, 9, 1);
    public static Vector3Int ChunkDimensions = new(10, 10, 10);
    public GameObject ChunkPrefab;
    public GameObject PlayerPrefab;
    public Slider LoadingBarSlider;
    
    private int playerHeightOffset = 1;
    private int drawRadius = 3;
    private HashSet<Vector3Int> chunkPositions = new();
    private HashSet<Vector2Int> chunkColumns = new();
    private Dictionary<Vector3Int, Chunk> chunks = new();
    private Queue<IEnumerator> chunksQueue = new();
    private Vector3Int lastBuildPosition = new();
    private WaitForSeconds wait = new(0.5f);

    //3D Perlin Settings
    [Space(10)]
    [Header("3D Perlin Settings")]
    public static Perlin3DSettings cavePerlinSettings;
    public Perlin3D_Grapher caveGraph;

    //2D Perlin Settings
    [Space(10)]
    [Header("2D Perlin Settings")]
    public static PerlinSettings surfacePerlinSettings;
    public PerlinGrapher surfaceGraph;

    public static PerlinSettings stonePerlinSettings;
    public PerlinGrapher stoneGraph;

    public static PerlinSettings ironPerlinSettings;
    public PerlinGrapher ironGraph;

    public static PerlinSettings valuablesPerlinSettings;
    public PerlinGrapher valuablesGraph;

    public static PerlinSettings diamondPerlinSettings;
    public PerlinGrapher diamondGraph;

    public static PerlinSettings bedrockPerlinSettings;
    public PerlinGrapher bedrockGraph;

    private void Start()
    {
        LoadingBarSlider.maxValue = WorldDimensions.x * WorldDimensions.z;

        surfacePerlinSettings = new PerlinSettings(surfaceGraph.scale,surfaceGraph.octaves, surfaceGraph.heightScale, surfaceGraph.heightOffset, surfaceGraph.probability);
        stonePerlinSettings = new PerlinSettings(stoneGraph.scale, stoneGraph.octaves, stoneGraph.heightScale, stoneGraph.heightOffset, stoneGraph.probability);
        cavePerlinSettings = new Perlin3DSettings(caveGraph.scale, caveGraph.octaves, caveGraph.heightScale, caveGraph.heightOffset, caveGraph.DrawCutOff);
        ironPerlinSettings = new PerlinSettings(ironGraph.scale, ironGraph.octaves, ironGraph.heightScale, ironGraph.heightOffset, ironGraph.probability);
        valuablesPerlinSettings = new PerlinSettings(valuablesGraph.scale, valuablesGraph.octaves, valuablesGraph.heightScale, valuablesGraph.heightOffset, valuablesGraph.probability);
        diamondPerlinSettings = new PerlinSettings(diamondGraph.scale, diamondGraph.octaves, diamondGraph.heightScale, diamondGraph.heightOffset, diamondGraph.probability);
        bedrockPerlinSettings = new PerlinSettings(bedrockGraph.scale, bedrockGraph.octaves, bedrockGraph.heightScale, bedrockGraph.heightOffset, bedrockGraph.probability);

        StartCoroutine(BuildWorld());
    }

    private IEnumerator UpdateWorld()
    {
        while (true)
        {
            if((lastBuildPosition - PlayerPrefab.transform.position).magnitude > ChunkDimensions.x)
            {
                lastBuildPosition = Vector3Int.CeilToInt(PlayerPrefab.transform.position);
                int posX = Mathf.FloorToInt(PlayerPrefab.transform.position.x / ChunkDimensions.x) * ChunkDimensions.x;
                int posZ = Mathf.FloorToInt(PlayerPrefab.transform.position.z / ChunkDimensions.z) * ChunkDimensions.z;
                chunksQueue.Enqueue(BuildRecursiveWorld(posX, posZ, drawRadius));
                chunksQueue.Enqueue(HideColumns(posX, posZ));
            }
            yield return wait;
        }
    }

    private void 

    private IEnumerator BuildQ()
    {
        while (true)
        {
            while (chunksQueue.Count > 0)
            {
                yield return StartCoroutine(chunksQueue.Dequeue());
            }
            yield return null;
        }
    }

    private IEnumerator BuildRecursiveWorld(int x, int z, int radius)
    {
        int nextRadius = radius - 1;
        if (nextRadius <= 0) yield break;

        BuildChunkColumn(x, z + ChunkDimensions.z);
        chunksQueue.Enqueue(BuildRecursiveWorld( x, z + ChunkDimensions.z, nextRadius));
        yield return null;

        BuildChunkColumn(x, z - ChunkDimensions.z);
        chunksQueue.Enqueue(BuildRecursiveWorld(x, z - ChunkDimensions.z, nextRadius));
        yield return null;

        BuildChunkColumn(x + ChunkDimensions.x, z);
        chunksQueue.Enqueue(BuildRecursiveWorld(x + ChunkDimensions.x, z, nextRadius));
        yield return null;

        BuildChunkColumn(x - ChunkDimensions.x, z);
        chunksQueue.Enqueue(BuildRecursiveWorld(x - ChunkDimensions.x, z, nextRadius));
        yield return null;
    }

    private void BuildChunkColumn(int x, int z, bool meshEnabled = true)
    {
        for(int y = 0; y < WorldDimensions.y; y++)
        {
            Vector3Int position = new(x, y * ChunkDimensions.y, z);
            if(!chunkPositions.Contains(position))
            {
                GameObject chunk = Instantiate(ChunkPrefab);
                chunk.name = $"Chunk({position.x},{position.y},{position.z})";
                chunk.transform.position = position;
                Chunk c = chunk.GetComponent<Chunk>();
                c.CreateChunk(position);
                chunks.Add(position, c);
                chunkPositions.Add(position);
            }
            chunks[position].MeshRenderer.enabled = meshEnabled;

        }

        chunkColumns.Add(new Vector2Int(x, z));

    }

    private IEnumerator BuildWorld()
    {
        for (int z = 0; z < WorldDimensions.z; z++)
        {
            for (int x = 0; x < WorldDimensions.x; x++)
            {
                BuildChunkColumn(x * ChunkDimensions.x, z * ChunkDimensions.z);
                LoadingBarSlider.value++;
                yield return null;
            }
        }

        if (LoadingBarSlider.value == LoadingBarSlider.maxValue)
        {
            PlayerPrefab.SetActive(true);
            LoadingBarSlider.gameObject.SetActive(false);

        }

        int xpos = WorldDimensions.x * ChunkDimensions.x / 2;
        int zpos = WorldDimensions.z * ChunkDimensions.z / 2;

        var c = surfacePerlinSettings;

        int ypos = (int) MeshUtils.FractialBrownianMotion(xpos,zpos, c.octaves, c.scale, c.heightScale, c.heightOffset) + playerHeightOffset;

        PlayerPrefab.transform.position = new Vector3Int(xpos, ypos, zpos);

        lastBuildPosition = Vector3Int.CeilToInt(PlayerPrefab.transform.position);
        StartCoroutine(BuildQ());
        StartCoroutine(UpdateWorld());
        StartCoroutine(BuildExtraWorld());
    }

    private IEnumerator BuildExtraWorld()
    {
        int xEnd = WorldDimensions.x + ExtraWorldDimensions.x;
        int xStart = WorldDimensions.x;
        int zEnd = WorldDimensions.z + ExtraWorldDimensions.z;
        int zStart = WorldDimensions.z;

        for (int z = zStart; z < zEnd; z++)
        {
            for (int x = 0; x < xEnd; x++)
            {
                BuildChunkColumn(x * ChunkDimensions.x, z * ChunkDimensions.z, false);
                yield return null;
            }
        }

        for (int z = 0; z < zEnd; z++)
        {
            for (int x = xStart; x < xEnd; x++)
            {
                BuildChunkColumn(x * ChunkDimensions.x, z * ChunkDimensions.z, false);
                yield return null;
            }
        }
    }

    private IEnumerator HideColumns(int x, int z)
    {
        Vector2Int playerPos = new(x,z);
        foreach(Vector2Int pos in chunkColumns)
        {
            if(Vector2Int.Distance(playerPos, pos) >= drawRadius * ChunkDimensions.x)
            {
                HideChunkColumn(pos.x, pos.y);
            }
        }
        yield return null;
    }

    public void HideChunkColumn(int x, int z)
    {
        for(int y = 0; y < WorldDimensions.y; y++)
        {
            Vector3Int position = new(x, y * ChunkDimensions.y, z);
            if(chunkPositions.Contains(position))
            {
                chunks[position].MeshRenderer.enabled = false;
            }
        }
    }


}
