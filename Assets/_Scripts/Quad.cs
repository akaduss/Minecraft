using UnityEngine;

public class Quad
{
    public Mesh mesh;

    public Quad(MeshUtils.BlockSide blockSide, Vector3 offset, MeshUtils.BlockTypes blockType)
    {
        mesh = new Mesh();
        mesh.name = "QuadCS";
        float vs = 0.5f; //(voxel size)

        Vector3[] vertices = new Vector3[4];
        Vector3[] normals = new Vector3[4];

        int[] triangles = new int[] { 0, 3, 2, 2, 1, 0 };

        Vector2 uv00 = MeshUtils.BlockUVs[(int)blockType, 0];
        Vector2 uv10 = MeshUtils.BlockUVs[(int)blockType, 1];
        Vector2 uv01 = MeshUtils.BlockUVs[(int)blockType, 2];
        Vector2 uv11 = MeshUtils.BlockUVs[(int)blockType, 3];

        Vector2[] uvs = new Vector2[] { uv00, uv01, uv11, uv10 }; //1,01,0,10


        Vector3 p0 = new Vector3(-vs, -vs, vs) + offset;
        Vector3 p1 = new Vector3(-vs, vs, vs) + offset;
        Vector3 p2 = new Vector3(vs, vs, vs) + offset;
        Vector3 p3 = new Vector3(vs, -vs, vs) + offset;
        Vector3 p4 = new Vector3(-vs, -vs, -vs) + offset;
        Vector3 p5 = new Vector3(-vs, vs, -vs) + offset;
        Vector3 p6 = new Vector3(vs, vs, -vs) + offset;
        Vector3 p7 = new Vector3(vs, -vs, -vs) + offset;

        switch (blockSide)
        {
            case MeshUtils.BlockSide.Front:
                vertices = new Vector3[] { p2,p3,p0,p1 }; //clockwise
                normals = new Vector3[] { Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward };
                break;

            case MeshUtils.BlockSide.Back:
                vertices = new Vector3[] { p5, p4, p7, p6 };
                normals = new Vector3[] { Vector3.back, Vector3.back, Vector3.back, Vector3.back };
                break;

            case MeshUtils.BlockSide.Right:
                vertices = new Vector3[] { p6, p7, p3, p2 };
                normals = new Vector3[] { Vector3.right, Vector3.right, Vector3.right, Vector3.right };
                break;

            case MeshUtils.BlockSide.Left:
                vertices = new Vector3[] { p1, p0, p4, p5 };
                normals = new Vector3[] { Vector3.left, Vector3.left, Vector3.left, Vector3.left };
                break;

            case MeshUtils.BlockSide.Top:
                vertices = new Vector3[] { p5, p6, p2, p1 };
                normals = new Vector3[] { Vector3.up, Vector3.up, Vector3.up, Vector3.up };
                break;

            case MeshUtils.BlockSide.Bottom:
                vertices = new Vector3[] { p0, p3, p7, p4 };
                normals = new Vector3[] { Vector3.down, Vector3.down, Vector3.down, Vector3.down };
                break;
        }

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
        //mesh.RecalculateNormals();

    }
}
