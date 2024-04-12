using System.Collections.Generic;
using UnityEngine;

public class Quad
{
    public Mesh mesh;

    public Quad(MeshUtils.BlockSide blockSide, Vector3 offset, MeshUtils.BlockTypes blockType, MeshUtils.BlockTypes crackType)
    {
        float vs = 0.5f; //(voxel size)

        Vector3[] vertices = new Vector3[4];
        int[] triangles = new int[] { 0, 3, 2, 2, 1, 0 };



        Vector2 uv00 = MeshUtils.BlockUVs[(int)blockType, 0];
        Vector2 uv10 = MeshUtils.BlockUVs[(int)blockType, 1];
        Vector2 uv01 = MeshUtils.BlockUVs[(int)blockType, 2];
        Vector2 uv11 = MeshUtils.BlockUVs[(int)blockType, 3];


        Vector2[] uvs = new Vector2[] { uv00, uv01, uv11, uv10 }; //1,01,0,10

        List<Vector2> secondaryUVs = new()
        {
            MeshUtils.BlockUVs[(int)crackType, 3],
            MeshUtils.BlockUVs[(int)crackType, 2],
            MeshUtils.BlockUVs[(int)crackType, 0],
            MeshUtils.BlockUVs[(int)crackType, 1],
        };


        Vector3 p0 = new(-vs, -vs, vs);
        Vector3 p1 = new(-vs, vs, vs);
        Vector3 p2 = new(vs, vs, vs);
        Vector3 p3 = new(vs, -vs, vs);
        Vector3 p4 = new(-vs, -vs, -vs);
        Vector3 p5 = new(-vs, vs, -vs);
        Vector3 p6 = new(vs, vs, -vs);
        Vector3 p7 = new(vs, -vs, -vs);

        switch (blockSide)
        {
            case MeshUtils.BlockSide.Front:
                vertices = new Vector3[] { p2,p3,p0,p1 }; //clockwise
                break;

            case MeshUtils.BlockSide.Back:
                vertices = new Vector3[] { p5, p4, p7, p6 };
                break;

            case MeshUtils.BlockSide.Right:
                vertices = new Vector3[] { p6, p7, p3, p2 };
                break;

            case MeshUtils.BlockSide.Left:
                vertices = new Vector3[] { p1, p0, p4, p5 };
                break;

            case MeshUtils.BlockSide.Top:
                vertices = new Vector3[] { p5, p6, p2, p1 };
                break;

            case MeshUtils.BlockSide.Bottom:
                vertices = new Vector3[] { p0, p3, p7, p4 };
                break;
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] += offset;
        }

        Vector3[] normals = new Vector3[]
        {
            Vector3.forward,
            Vector3.forward,
            Vector3.forward,
            Vector3.forward
        };

        mesh = new()
        {
            name = "ScriptedQuad",
            vertices = vertices,
            normals = normals,
            uv = uvs,
            triangles = triangles
        };

        mesh.SetUVs(1, secondaryUVs);

        mesh.RecalculateBounds();
        //mesh.RecalculateNormals();
    }
}
