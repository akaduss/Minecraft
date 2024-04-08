using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VertexData = System.Tuple<UnityEngine.Vector3, UnityEngine.Vector3, UnityEngine.Vector2>; //vertex, normal, triangle

public static class MeshUtils
{
    public enum BlockTypes
    {
        Sand, GrassTop, GrassSide, Dirt, Water, Stone
    }

    public enum BlockSide
    {
        Bottom, Left, Right, Top, Front, Back
    }


    private static UvCoordinates sandUVcoords = Utils.CalculateAtlasPosition(1, 2);
    private static UvCoordinates grassTop = Utils.CalculateAtlasPosition(9, 3);
    private static UvCoordinates grassSideUVcoords = Utils.CalculateAtlasPosition(0, 3);
    private static UvCoordinates dirtUVcoords = Utils.CalculateAtlasPosition(0, 2);

    public static Vector2[,] BlockUVs =
    {
        { sandUVcoords.uvBottomLeft,sandUVcoords.uvBottomRight,sandUVcoords.uvTopLeft,sandUVcoords.uvTopRight },
        { grassTop.uvBottomLeft,grassTop.uvBottomRight,grassTop.uvTopLeft,grassTop.uvTopRight },
        { grassSideUVcoords.uvBottomLeft,grassSideUVcoords.uvBottomRight,grassSideUVcoords.uvTopLeft,grassSideUVcoords.uvTopRight },
        { dirtUVcoords.uvBottomLeft,dirtUVcoords.uvBottomRight,dirtUVcoords.uvTopLeft,dirtUVcoords.uvTopRight },
    };

    public static Mesh MergeMeshes(Mesh[] meshesArray)
    {
        Dictionary<VertexData, int> pointsOrder = new();
        HashSet<VertexData> pointsHash = new();
        List<int> triangles = new();

        int pointIndex = 0;
        for (int i = 0; i < meshesArray.Length; i++)
        {
            if (meshesArray[i] == null) continue;

            for (int v = 0; v <  meshesArray[i].vertices.Length; v++)
            {
                Vector3 vert = meshesArray[i].vertices[v];
                Vector3 norm = meshesArray[i].normals[v];
                Vector2 uv = meshesArray[i].uv[v];

                VertexData point = new(vert,norm,uv);

                if (pointsHash.Contains(point) == false)
                {
                    pointsOrder.Add(point, pointIndex);
                    pointsHash.Add(point);
                    pointIndex++;
                }
            }

            for(int t = 0; t < meshesArray[i].triangles.Length; t++)
            {
                int triPoint = meshesArray[i].triangles[t];
                Vector3 vert = meshesArray[i].vertices[triPoint];
                Vector3 norm = meshesArray[i].normals[triPoint];
                Vector2 uv = meshesArray[i].uv[triPoint];

                VertexData pointData = new(vert,norm, uv);
                pointsOrder.TryGetValue(pointData, out int index);
                triangles.Add(index);
            }

        }

        Mesh mesh = new();
        ExtractArrays(pointsOrder, mesh);
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateBounds();
        return mesh;

    }

    private static void ExtractArrays(Dictionary<VertexData, int> points, Mesh mesh)
    {
        List<Vector3> vertices = new();
        List<Vector3> normals = new();
        List<Vector2> uvs = new();

        foreach (VertexData i in points.Keys)
        {
            vertices.Add(i.Item1);
            normals.Add(i.Item2);
            uvs.Add(i.Item3);
        }

        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvs.ToArray();
    } 

}
