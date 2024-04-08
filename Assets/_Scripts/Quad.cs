using UnityEngine;

public class Quad
{
    public Mesh mesh;

    public Quad(MeshUtils.BlockSide blockSide, Vector3 offset, MeshUtils.BlockTypes blockType)
    {
        float vs = 0.5f; //(voxel size)

        Vector3[] vertices = new Vector3[4];
        int[] triangles = new int[] { 0, 3, 2, 2, 1, 0 };



        Vector2 uv00 = MeshUtils.BlockUVs[(int)blockType, 0];
        Vector2 uv10 = MeshUtils.BlockUVs[(int)blockType, 1];
        Vector2 uv01 = MeshUtils.BlockUVs[(int)blockType, 2];
        Vector2 uv11 = MeshUtils.BlockUVs[(int)blockType, 3];


        Vector2[] uvs = new Vector2[] { uv00, uv01, uv11, uv10 }; //1,01,0,10

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
                vertices = new Vector3[] { p0, p1, p2, p3 }; //clockwise
                break;

            case MeshUtils.BlockSide.Back:
                vertices = new Vector3[] { p7, p6, p5, p4 };
                break;

            case MeshUtils.BlockSide.Right:
                vertices = new Vector3[] { p3, p2, p6, p7 };
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


        mesh = new()
        {
            name = "ScriptedQuad",
            vertices = vertices,
            uv = uvs,
            triangles = triangles
        };

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

    }
}