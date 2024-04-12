using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class Chunk : MonoBehaviour
{
    [SerializeField] private Material blockAtlas;
    public int depth = 2;
    public int height = 2;
    public int width = 2;
    public Block[,,] blocks;
    public MeshUtils.BlockTypes[] chunkData;
    public MeshUtils.BlockTypes[] crackData;
    public MeshRenderer MeshRenderer;
    public NativeArray<Unity.Mathematics.Random> RandomArray { get; private set; }

    public Vector3Int location;

    // Flatten the 3D array into a 1D array
    // Flat[x + width * (y + depth * z)] = Original[x, y, z]
    // Flat to 3D:
    // x = i % width
    // y = (i / width) % height
    // z = i / (width * height)

    private const float valuableProbability = 0.05f;
    private const float CoalProbability = 0.03f;

    CalculateBlockTypeDataJob processBlockTypeDataJob;
    JobHandle processBlockTypeDataJobHandle;

    private void BuildChunkData()
    {
        int totalSize = width * height * depth;
        chunkData = new MeshUtils.BlockTypes[totalSize];
        crackData = new MeshUtils.BlockTypes[totalSize];
        NativeArray<MeshUtils.BlockTypes> chunkDataNative = new(chunkData, Allocator.Persistent);
        NativeArray<MeshUtils.BlockTypes> crackDataNative = new(crackData, Allocator.Persistent);
        NativeArray<Unity.Mathematics.Random> randomArray = new(totalSize, Allocator.Persistent);

        for (int i = 0; i < totalSize; i++)
        {
            randomArray[i] = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(0, int.MaxValue));
        }


        processBlockTypeDataJob = new()
        {
            chunkData = chunkDataNative,
            crackData = crackDataNative,
            width = width,
            height = height,
            location = location,
            randomArray = randomArray,
            surfacePerlinSettings = World.surfacePerlinSettings,
            stonePerlinSettings = World.stonePerlinSettings,
            ironPerlinSettings = World.ironPerlinSettings,
            valuablesPerlinSettings = World.valuablesPerlinSettings,
            diamondPerlinSettings = World.diamondPerlinSettings,
            bedrockPerlinSettings = World.bedrockPerlinSettings,
            cavePerlinSettings = World.cavePerlinSettings
        };

        processBlockTypeDataJobHandle = processBlockTypeDataJob.Schedule(chunkData.Length, 64);
        processBlockTypeDataJobHandle.Complete();

        processBlockTypeDataJob.chunkData.CopyTo(chunkData);
        processBlockTypeDataJob.crackData.CopyTo(crackData);
        chunkDataNative.Dispose();
        crackDataNative.Dispose();
        randomArray.Dispose();
        
    }

    private static Vector3 GetBlockPosition(int index, int width, int height, Vector3 location)
    {
        float x = index % width + location.x;
        float y = (index / width) % height + location.y;
        float z = index / (width * height) + location.z;
        return new Vector3(x, y, z);
    }

    private static MeshUtils.BlockTypes DetermineBlockType(Vector3 blockPosition, float random, int surfaceHeight,int stoneHeight,int ironHeight,int valuableHeight,int diamondHeight, int bedrockHeight, int digCave, float drawCutOff, float diamondProbability)
    {
        if (digCave < drawCutOff && blockPosition.y > bedrockHeight)
        {
            return MeshUtils.BlockTypes.Air;
        }

        if (blockPosition.y > surfaceHeight)
        {
            return MeshUtils.BlockTypes.Air;
        }
        else if (blockPosition.y == surfaceHeight)
        {
            return MeshUtils.BlockTypes.GrassSide;
        }
        else if (stoneHeight < blockPosition.y)
        {
            return MeshUtils.BlockTypes.Dirt;
        }
        else if (blockPosition.y > ironHeight)
        {
            if (random < CoalProbability)
            {
                return MeshUtils.BlockTypes.Coal;
            }
            else
            {
                return MeshUtils.BlockTypes.Stone;
            }
        }
        else if (blockPosition.y > valuableHeight)
        {
            if (random < 0.04f)
            {
                return MeshUtils.BlockTypes.Iron;
            }
            else if (random < valuableProbability)
            {
                return MeshUtils.BlockTypes.Coal;
            }
            else
            {
                return MeshUtils.BlockTypes.Stone;
            }
        }
        else if (blockPosition.y > diamondHeight)
        {
            if (random < valuableProbability)
            {
                return MeshUtils.BlockTypes.Coal;
            }
            else if (random < valuableProbability)
            {
                return MeshUtils.BlockTypes.Iron;
            }
            else if (random < valuableProbability)
            {
                return MeshUtils.BlockTypes.Gold;
            }
            else if (random < valuableProbability)
            {
                return MeshUtils.BlockTypes.Redstone;
            }
            else if (random < valuableProbability)
            {
                return MeshUtils.BlockTypes.Lapis;
            }
            else
            {
                return MeshUtils.BlockTypes.Stone;
            }
        }
        else if (blockPosition.y > bedrockHeight)
        {
            if (random < diamondProbability)
            {
                return MeshUtils.BlockTypes.Diamond;
            }
            else if (random < CoalProbability)
            {
                return MeshUtils.BlockTypes.Coal;
            }
            else if (random < CoalProbability)
            {
                return MeshUtils.BlockTypes.Iron;
            }
            else if (random < valuableProbability)
            {
                return MeshUtils.BlockTypes.Gold;
            }
            else if (random < valuableProbability)
            {
                return MeshUtils.BlockTypes.Redstone;
            }
            else if (random < valuableProbability)
            {
                return MeshUtils.BlockTypes.Lapis;
            }
            else
            {
                return MeshUtils.BlockTypes.Stone;
            }
        }
        else
        {
            return MeshUtils.BlockTypes.Bedrock;
        }
    }
    public void CreateChunk(Vector3Int position, bool rebuild = true)
    {
        location = position;

        MeshFilter mf = gameObject.AddComponent<MeshFilter>();
        MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();
        mr.material = blockAtlas;
        MeshRenderer = mr;
        blocks = new Block[width, height, depth];
        if (rebuild)
        {
            BuildChunkData();
        }

        List<Mesh> inputMeshes = new();
        int triangleStart = 0;
        int vertexStart = 0;
        int meshCount = width * height * depth;
        int m = 0;

        ProcessMeshDataJob jobs = new();
        jobs.triangleStart = new NativeArray<int>(meshCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        jobs.vertexStart = new NativeArray<int>(meshCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

        for (int z = 0; z < depth; z++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    blocks[x,y,z] = new(new Vector3(x, y, z) + location, chunkData[x + width * (y + depth * z) ], this, crackData[x + width * (y + depth * z)]);
                    if (blocks[x, y, z].mesh == null) continue;

                    inputMeshes.Add(blocks[x, y, z].mesh);
                    int vertexCount = blocks[x, y, z].mesh.vertexCount;
                    int indexCount = (int)blocks[x, y, z].mesh.GetIndexCount(0);

                    jobs.vertexStart[m] = vertexStart;
                    jobs.triangleStart[m] = triangleStart;

                    vertexStart += vertexCount;
                    triangleStart += indexCount;
                    m++;
                }
            }
        }

        jobs.meshDataArray = Mesh.AcquireReadOnlyMeshData(inputMeshes);

        Mesh.MeshDataArray outputMeshData = Mesh.AllocateWritableMeshData(1);
       
        jobs.outputMeshData = outputMeshData[0];
        jobs.outputMeshData.SetIndexBufferParams(triangleStart, IndexFormat.UInt32);
        jobs.outputMeshData.SetVertexBufferParams(vertexStart, 
            new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32),
            new VertexAttributeDescriptor(VertexAttribute.Normal, stream: 1),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord0, stream: 2),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord1, stream: 3)
            );

        JobHandle jobHandle = jobs.Schedule(inputMeshes.Count, 4);

        Mesh newMesh = new()
        {
            #if UNITY_EDITOR
            name = $"Chunk_{location.x}_{location.y}_{location.z}"
            #endif
        };

        SubMeshDescriptor subMeshDescriptor = new(0, triangleStart, MeshTopology.Triangles)
        {
            firstVertex = 0,
            vertexCount = vertexStart
        };

        jobHandle.Complete();

        jobs.outputMeshData.subMeshCount = 1;
        jobs.outputMeshData.SetSubMesh(0, subMeshDescriptor);

        Mesh.ApplyAndDisposeWritableMeshData(outputMeshData, newMesh, MeshUpdateFlags.DontRecalculateBounds);
        jobs.meshDataArray.Dispose();
        jobs.vertexStart.Dispose();
        jobs.triangleStart.Dispose();

        newMesh.RecalculateBounds();
        mf.mesh = newMesh;
        gameObject.AddComponent<MeshCollider>().sharedMesh = mf.mesh;

    }

    [BurstCompile]
    struct ProcessMeshDataJob : IJobParallelFor
    {
        [ReadOnly]
        public Mesh.MeshDataArray meshDataArray;
        public Mesh.MeshData outputMeshData;
        public NativeArray<int> vertexStart;
        public NativeArray<int> triangleStart;

        public void Execute(int index)
        {
            Mesh.MeshData data = meshDataArray[index];
            var vertexCount = data.vertexCount;
            var vStart = vertexStart[index];
            
            NativeArray<float3> vertices = new(vertexCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            data.GetVertices(vertices.Reinterpret<Vector3>());

            NativeArray<float3> normals = new(vertexCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            data.GetNormals(normals.Reinterpret<Vector3>());

            NativeArray<float3> uvs = new(vertexCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            data.GetUVs(0, uvs.Reinterpret<Vector3>());

            NativeArray<float3> uv2s = new(vertexCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            data.GetUVs(1, uv2s.Reinterpret<Vector3>());

            NativeArray<Vector3> outputVertices = outputMeshData.GetVertexData<Vector3>();
            NativeArray<Vector3> outputNormals = outputMeshData.GetVertexData<Vector3>(stream: 1);
            NativeArray<Vector3> outputUVs = outputMeshData.GetVertexData<Vector3>(stream: 2);
            NativeArray<Vector3> outputUV2s = outputMeshData.GetVertexData<Vector3>(stream: 3);

            for (int i = 0; i < vertexCount; i++)
            {
                outputVertices[vStart + i] = vertices[i];
                outputNormals[vStart + i] = normals[i];
                outputUVs[vStart + i] = uvs[i];
                outputUV2s[vStart + i] = uv2s[i];
            }

            vertices.Dispose();
            normals.Dispose();
            uvs.Dispose();
            uv2s.Dispose();

            var triStart = triangleStart[index];
            var triCount = data.GetSubMesh(0).indexCount;
            var outputTris = outputMeshData.GetIndexData<int>();

            if (data.indexFormat == IndexFormat.UInt16)
            {
                NativeArray<ushort> tris = data.GetIndexData<ushort>();
                for (int i = 0; i < triCount; i++)
                {
                    outputTris[triStart + i] = vStart + tris[i];
                }

                tris.Dispose();
            }
            else
            {
                NativeArray<int> tris = data.GetIndexData<int>();
                for (int i = 0; i < triCount; i++)
                {
                    outputTris[triStart + i] = vStart + tris[i];
                }

                tris.Dispose();
            }

        }
    }

    [BurstCompile]
    struct CalculateBlockTypeDataJob : IJobParallelFor
    {
        public NativeArray<MeshUtils.BlockTypes> chunkData;
        public NativeArray<MeshUtils.BlockTypes> crackData;
        public int width;
        public int height;
        public Vector3 location;
        public NativeArray<Unity.Mathematics.Random> randomArray;
        public PerlinSettings surfacePerlinSettings;
        public PerlinSettings stonePerlinSettings;
        public PerlinSettings ironPerlinSettings;
        public PerlinSettings valuablesPerlinSettings;
        public PerlinSettings diamondPerlinSettings;
        public PerlinSettings bedrockPerlinSettings;
        public Perlin3DSettings cavePerlinSettings;


        public void Execute(int index)
        {
            Vector3 blockPosition = GetBlockPosition(index, width, height, location);
            int surfaceHeight = (int)MeshUtils.FractialBrownianMotion(blockPosition.x, blockPosition.z, surfacePerlinSettings.octaves, surfacePerlinSettings.scale, surfacePerlinSettings.heightScale, surfacePerlinSettings.heightOffset);
            int stoneHeight = (int)MeshUtils.FractialBrownianMotion(blockPosition.x, blockPosition.z, stonePerlinSettings.octaves, stonePerlinSettings.scale, stonePerlinSettings.heightScale, stonePerlinSettings.heightOffset);
            int ironHeight = (int)MeshUtils.FractialBrownianMotion(blockPosition.x, blockPosition.z, ironPerlinSettings.octaves, ironPerlinSettings.scale, ironPerlinSettings.heightScale, ironPerlinSettings.heightOffset);
            int valuableHeight = (int)MeshUtils.FractialBrownianMotion(blockPosition.x, blockPosition.z, valuablesPerlinSettings.octaves, valuablesPerlinSettings.scale, valuablesPerlinSettings.heightScale, valuablesPerlinSettings.heightOffset);
            int diamondHeight = (int)MeshUtils.FractialBrownianMotion(blockPosition.x, blockPosition.z, diamondPerlinSettings.octaves, diamondPerlinSettings.scale, diamondPerlinSettings.heightScale, diamondPerlinSettings.heightOffset);
            int bedrockHeight = (int)MeshUtils.FractialBrownianMotion(blockPosition.x, blockPosition.z, bedrockPerlinSettings.octaves, bedrockPerlinSettings.scale, bedrockPerlinSettings.heightScale, bedrockPerlinSettings.heightOffset);
            int digCave = (int)MeshUtils.FractialBrownianMotion3D(blockPosition.x, blockPosition.y, blockPosition.z, cavePerlinSettings.octaves, cavePerlinSettings.scale, cavePerlinSettings.heightScale, cavePerlinSettings.heightOffset);

            float random = randomArray[index].NextFloat(1);
            MeshUtils.BlockTypes blockType = DetermineBlockType(blockPosition, random, surfaceHeight, stoneHeight, ironHeight, valuableHeight, diamondHeight, bedrockHeight, digCave, cavePerlinSettings.drawCutOff, diamondPerlinSettings.probability);
            chunkData[index] = blockType;
            crackData[index] = MeshUtils.BlockTypes.Crack1;
        }
    }

}
