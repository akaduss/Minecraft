using System.Collections.Generic;
using UnityEngine;
using static MeshUtils;

public class Block
{
    public Mesh mesh;
    private readonly Chunk parentChunk;

    public Block(Vector3 offset, BlockTypes blockType, Chunk chunk, BlockTypes crackType = BlockTypes.Crack1)
    {
        parentChunk = chunk;
        offset -= chunk.location;

        if(blockType == BlockTypes.Air) return;

        List<Quad> quads = new();


        if (HasSolidNeighbour((int)offset.x, (int)offset.y - 1, (int)offset.z ) == false)
        {
            if (blockType == BlockTypes.GrassSide)
            {
                quads.Add(new(BlockSide.Bottom, offset, BlockTypes.Dirt, crackType));
            }
            else
            {
                quads.Add(new(BlockSide.Bottom, offset, blockType, crackType));
            }
        }
        if (HasSolidNeighbour((int)offset.x, (int)offset.y + 1, (int)offset.z ) == false)
        {
            if(blockType == BlockTypes.GrassSide)
            {
                quads.Add(new(BlockSide.Top, offset, BlockTypes.GrassTop, crackType));
            }
            else
            {
                quads.Add(new(BlockSide.Top, offset, blockType, crackType));
            }
        }

        if (HasSolidNeighbour((int)offset.x - 1, (int)offset.y, (int)offset.z ) == false)
        {
            quads.Add(new(BlockSide.Left, offset, blockType, crackType));
        }
        if (HasSolidNeighbour((int)offset.x + 1, (int)offset.y, (int)offset.z ) == false)
        {
            quads.Add(new(BlockSide.Right, offset, blockType, crackType));
        }

        if (HasSolidNeighbour((int)offset.x, (int)offset.y, (int)offset.z + 1 ) == false)
        {
            quads.Add(new(BlockSide.Front, offset, blockType, crackType));
        }
        if (HasSolidNeighbour((int)offset.x, (int)offset.y, (int)offset.z - 1 ) == false)
        {
            quads.Add(new(BlockSide.Back, offset, blockType, crackType));
        }

        if (quads.Count == 0) return;

        Mesh[] sideMeshes = new Mesh[quads.Count];

        int m = 0;
        foreach (Quad q in quads)
        {
            sideMeshes[m] = q.mesh;
            m++;
        }

        mesh = MergeMeshes(sideMeshes);
    }

    public bool HasSolidNeighbour(int x, int y, int z)
    {
        if (x < 0 || x >= parentChunk.width || 
            y < 0 || y >= parentChunk.height || 
            z < 0 || z >= parentChunk.depth)
        {
            return false;
        }

        if (parentChunk.chunkData[x + parentChunk.width * (y + parentChunk.depth * z)] == BlockTypes.Air)
        {
            return false;
        }

        if (parentChunk.chunkData[x + parentChunk.width * (y + parentChunk.depth * z)] == BlockTypes.Water)
        {
            return false;
        }

        return true;
    }

}
