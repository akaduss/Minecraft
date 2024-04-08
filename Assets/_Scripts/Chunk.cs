using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine.Rendering;

public class Chunk : MonoBehaviour
{
    [SerializeField] private Material blockAtlas;
    public int depth = 2;
    public int height = 2;
    public int width = 2;
    public Block[,,] blocks;
    public MeshUtils.BlockTypes[] chunkData;

    private void BuildChunkData()
    {
        int totalSize = width * height * depth;
        chunkData = new MeshUtils.BlockTypes[totalSize];
        for (int i = 0; i < totalSize; i++)
        {
            if(UnityEngine.Random.value > 0.5f)
                chunkData[i] = MeshUtils.BlockTypes.Dirt;
            else
                chunkData[i] = MeshUtils.BlockTypes.GrassTop;
        }
    }

    void Start()
    {
        MeshFilter mf = gameObject.AddComponent<MeshFilter>();
        MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();
        mr.material = blockAtlas;
        blocks = new Block[width, height, depth];
        print(blocks[0,0,0]);
        print(blocks[0,0,1]);
        BuildChunkData();

        List<Mesh> inputMeshes = new();
        int triangleStart = 0;
        int vertexStart = 0;
        int meshCount = width * height * depth;
        int m = 0;

        ProcessMeshDataJob jobs = new()
        {
            vertexStart = new NativeArray<int>(meshCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory),
            triangleStart = new NativeArray<int>(meshCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory)
        };

        for (int z = 0; z < depth; z++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    blocks[x,y,z] = new(new Vector3(x, y, z), chunkData[x + width * (y + depth * z) ], this );
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
            new VertexAttributeDescriptor(VertexAttribute.TexCoord0, stream: 2)
            );

        JobHandle jobHandle = jobs.Schedule(inputMeshes.Count, 4);

        Mesh newMesh = new()
        {
            name = "Chunk Mesh"
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

            NativeArray<Vector3> outputVertices = outputMeshData.GetVertexData<Vector3>();
            NativeArray<Vector3> outputNormals = outputMeshData.GetVertexData<Vector3>(stream: 1);
            NativeArray<Vector3> outputUVs = outputMeshData.GetVertexData<Vector3>(stream: 2);

            for (int i = 0; i < vertexCount; i++)
            {
                outputVertices[vStart + i] = vertices[i];
                outputNormals[vStart + i] = normals[i];
                outputUVs[vStart + i] = uvs[i];
            }

            vertices.Dispose();
            normals.Dispose();
            uvs.Dispose();

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

}
