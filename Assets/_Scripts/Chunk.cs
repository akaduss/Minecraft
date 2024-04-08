using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;

public class Chunk : MonoBehaviour
{
    [SerializeField] private Material blockAtlas;
    public int depth = 2;
    public int height = 2;
    public int width = 2;
    public Block[,,] blocks;

    void Start()
    {
        MeshFilter mf = gameObject.AddComponent<MeshFilter>();
        MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();
        mr.material = blockAtlas;
        blocks = new Block[width,height, depth];

        int m = 0;
        int triangleStart = 0;
        int vertexStart = 0;
        int meshCount = width * height * depth;

        //var jobs = new ();
        var jobsvs = new NativeArray<int>(meshCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
        var jobsts = new NativeArray<int>(meshCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

        List<Mesh> meshes = new(meshCount);
        for (int z = 0; z < depth; z++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    blocks[x,y,z] = new(new Vector3(x, y, z), MeshUtils.BlockTypes.Dirt);
                    meshes.Add(blocks[x,y,z].mesh);

                    int vertexCount = blocks[x, y, z].mesh.vertexCount;
                    int indexCount = (int) blocks[x, y, z].mesh.GetIndexCount(0);

                    vertexStart += vertexCount;
                    triangleStart += indexCount;

                    m++;
                }
            }
        }

        mf.mesh = MeshUtils.MergeMeshes(meshes.ToArray());
        
    }



    [BurstCompile]
    struct ProcessMeshDataJob : IJobFor
    {
        [ReadOnly]
        public Mesh.MeshDataArray meshDataArray;
        public Mesh.MeshData outputMeshData;
        public NativeArray<int> vertexStart;
        public NativeArray<int> triangleStart;


        public void Execute(int index)
        {
            throw new System.NotImplementedException();
        }
    }

}
