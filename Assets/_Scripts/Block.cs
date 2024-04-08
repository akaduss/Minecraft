using UnityEngine;
using static MeshUtils;

public class Block
{
    public Mesh mesh;
    public Block(Vector3 offset, BlockTypes blockType)
    {
        Mesh[] meshes = new Mesh[6];
        Quad[] quads = new Quad[6];

        quads[0] = new(BlockSide.Bottom,offset , blockType);
        quads[1]= new(BlockSide.Top, offset, blockType);
        quads[2]= new(BlockSide.Left, offset, blockType);
        quads[3]= new(BlockSide.Right, offset, blockType);
        quads[4] = new(BlockSide.Front, offset, blockType);
        quads[5]= new(BlockSide.Back, offset, blockType);

        meshes[0] = quads[0].mesh;
        meshes[1] = quads[1].mesh;
        meshes[2] = quads[2].mesh;
        meshes[3] = quads[3].mesh;
        meshes[4] = quads[4].mesh;
        meshes[5] = quads[5].mesh;
        
        mesh = MergeMeshes(meshes);
    }

}
