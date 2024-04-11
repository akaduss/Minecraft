using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VertexData = System.Tuple<UnityEngine.Vector3, UnityEngine.Vector3, UnityEngine.Vector2>; //vertex, normal, triangle

public static class MeshUtils
{
    public enum BlockTypes
    {
        Sand, GrassTop, GrassSide, Dirt, Stone, Water, Air, Coal, Iron, Gold, Diamond, Redstone, Lapis, Bedrock
    }

    public enum BlockSide
    {
        Bottom, Left, Right, Top, Front, Back
    }


    private static UvCoordinates sandUVcoords = Utils.CalculateAtlasPosition(1, 2);
    private static UvCoordinates grassTop = Utils.CalculateAtlasPosition(9, 2);
    private static UvCoordinates grassSideUVcoords = Utils.CalculateAtlasPosition(0, 3);
    private static UvCoordinates dirtUVcoords = Utils.CalculateAtlasPosition(0, 2);
    private static UvCoordinates stoneUVCoords = Utils.CalculateAtlasPosition(0, 1);
    private static UvCoordinates waterUVCoords = Utils.CalculateAtlasPosition(0, 15);
    private static UvCoordinates coalUVCoords = Utils.CalculateAtlasPosition(2, 2);
    private static UvCoordinates ironUVCoords = Utils.CalculateAtlasPosition(2, 1);
    private static UvCoordinates goldUVCoords = Utils.CalculateAtlasPosition(2, 0);
    private static UvCoordinates diamondUVCoords = Utils.CalculateAtlasPosition(3, 2);
    private static UvCoordinates redstoneUVCoords = Utils.CalculateAtlasPosition(3, 3);
    private static UvCoordinates lapisUVCoords = Utils.CalculateAtlasPosition(10, 0);
    private static UvCoordinates bedrockUVCoords = Utils.CalculateAtlasPosition(1, 1);


    public static Vector2[,] BlockUVs =
    {
        { sandUVcoords.uvBottomLeft,sandUVcoords.uvBottomRight,sandUVcoords.uvTopLeft,sandUVcoords.uvTopRight },
        { grassTop.uvBottomLeft,grassTop.uvBottomRight,grassTop.uvTopLeft,grassTop.uvTopRight },
        { grassSideUVcoords.uvBottomLeft,grassSideUVcoords.uvBottomRight,grassSideUVcoords.uvTopLeft,grassSideUVcoords.uvTopRight },
        { dirtUVcoords.uvBottomLeft,dirtUVcoords.uvBottomRight,dirtUVcoords.uvTopLeft,dirtUVcoords.uvTopRight },
        { stoneUVCoords.uvBottomLeft,stoneUVCoords.uvBottomRight,stoneUVCoords.uvTopLeft,stoneUVCoords.uvTopRight },
        { waterUVCoords.uvBottomLeft, waterUVCoords.uvBottomRight, waterUVCoords.uvTopLeft, waterUVCoords.uvTopRight },
        { Vector2.zero,Vector2.zero,Vector2.zero,Vector2.zero },
        { coalUVCoords.uvBottomLeft, coalUVCoords.uvBottomRight, coalUVCoords.uvTopLeft, coalUVCoords.uvTopRight },
        { ironUVCoords.uvBottomLeft, ironUVCoords.uvBottomRight, ironUVCoords.uvTopLeft, ironUVCoords.uvTopRight },
        { goldUVCoords.uvBottomLeft, goldUVCoords.uvBottomRight, goldUVCoords.uvTopLeft, goldUVCoords.uvTopRight },
        { diamondUVCoords.uvBottomLeft, diamondUVCoords.uvBottomRight, diamondUVCoords.uvTopLeft, diamondUVCoords.uvTopRight },
        { redstoneUVCoords.uvBottomLeft, redstoneUVCoords.uvBottomRight, redstoneUVCoords.uvTopLeft, redstoneUVCoords.uvTopRight },
        { lapisUVCoords.uvBottomLeft, lapisUVCoords.uvBottomRight, lapisUVCoords.uvTopLeft, lapisUVCoords.uvTopRight },
        { bedrockUVCoords.uvBottomLeft, bedrockUVCoords.uvBottomRight, bedrockUVCoords.uvTopLeft, bedrockUVCoords.uvTopRight }
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

    public static float FractialBrownianMotion(float x, float z, int octaves, float scale, float heightScale, float heightOffset, float lacunarity = 2f)
    {
        float total = 0;
        float frequency = 1;

        for (int i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise(x * frequency * scale, z * frequency * scale) * heightScale;
            frequency *= lacunarity;
        }
        return total + heightOffset;
    }

    public static float FractialBrownianMotion3D(float x,float y, float z, int octaves, float scale, float heightScale, float heightOffset = 0f)
    {
        float XY = FractialBrownianMotion(x, y, octaves, scale, heightScale, heightOffset);
        float XZ = FractialBrownianMotion(x, z, octaves, scale, heightScale, heightOffset);
        float YZ = FractialBrownianMotion(y, z, octaves, scale, heightScale, heightOffset);
        float YX = FractialBrownianMotion(y, x, octaves, scale, heightScale, heightOffset);
        float ZX = FractialBrownianMotion(z, x, octaves, scale, heightScale, heightOffset);
        float ZY = FractialBrownianMotion(z, y, octaves, scale, heightScale, heightOffset);

        return (XY + XZ + YZ + YX + ZX + ZY) / 6.0f;
    }

}

public static class Utils
{
    const int atlasWidth = 16;
    const int atlasHeight = 16;

    public static UvCoordinates CalculateAtlasPosition(int textureRow, int textureCollumn)
    {
        float xStartPosition = (1f / atlasWidth) * textureCollumn;
        float xEndPosition = (1f / atlasWidth) * (textureCollumn + 1);
        float yStartPosition = 1f - (1f / atlasHeight) * textureRow;
        float yEndPosition = 1f - ((1f / atlasHeight) * (textureRow + 1));

        return new UvCoordinates(xStartPosition, xEndPosition, yStartPosition, yEndPosition);
    }
}

public class UvCoordinates
{
    public Vector2 uvBottomLeft;
    public Vector2 uvBottomRight;
    public Vector2 uvTopLeft;
    public Vector2 uvTopRight;

    public UvCoordinates(float xStart, float xEnd, float yStart, float yEnd)
    {
        uvBottomLeft = new Vector2(xStart, yStart);
        uvBottomRight = new Vector2(xEnd, yStart);
        uvTopLeft = new Vector2(xStart, yEnd);
        uvTopRight = new Vector2(xEnd, yEnd);
    }

}
