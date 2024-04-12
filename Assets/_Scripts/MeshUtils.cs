using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VertexData = System.Tuple<UnityEngine.Vector3, UnityEngine.Vector3, UnityEngine.Vector2, UnityEngine.Vector2>; //vertex, normal, triangle

public static class MeshUtils
{
    public enum BlockTypes
    {
        None, GrassTop, GrassSide, Dirt, Stone, Water, Air, Coal, Iron, Gold, Diamond, Redstone, Lapis, Bedrock, Cobblestone, Wood, Plank, Crafting, Furnace, Glass, Brick, Obsidian, Crack1, Crack2, Crack3, Crack4, Crack5, Crack6, Crack7, Crack8, Crack9, Crack10
    }

    public enum BlockSide
    {
        Bottom, Left, Right, Top, Front, Back
    }


    //private static UvCoordinates sandUVcoords = Utils.CalculateAtlasPosition(1, 2);
    private static UvCoordinates grassTopUVcoords = Utils.CalculateAtlasPosition(10, 11);
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
    private static UvCoordinates cobblestoneUVCoords = Utils.CalculateAtlasPosition(1, 0);
    private static UvCoordinates woodUVCoords = Utils.CalculateAtlasPosition(1, 5);
    private static UvCoordinates plankUVCoords = Utils.CalculateAtlasPosition(0, 4);
    private static UvCoordinates craftingUVCoords = Utils.CalculateAtlasPosition(3, 11);
    private static UvCoordinates furnaceUVCoords = Utils.CalculateAtlasPosition(3, 12);
    private static UvCoordinates glassUVCoords = Utils.CalculateAtlasPosition(3, 1);
    private static UvCoordinates brickUVCoords = Utils.CalculateAtlasPosition(0, 7);
    private static UvCoordinates obsidianUVCoords = Utils.CalculateAtlasPosition(2, 5);

    private static UvCoordinates crack1UVCoords = Utils.CalculateAtlasPosition(15,0);
    private static UvCoordinates crack2UVCoords = Utils.CalculateAtlasPosition(15,1);
    private static UvCoordinates crack3UVCoords = Utils.CalculateAtlasPosition(15,2);
    private static UvCoordinates crack4UVCoords = Utils.CalculateAtlasPosition(15,3);
    private static UvCoordinates crack5UVCoords = Utils.CalculateAtlasPosition(15,4);
    private static UvCoordinates crack6UVCoords = Utils.CalculateAtlasPosition(15,5);
    private static UvCoordinates crack7UVCoords = Utils.CalculateAtlasPosition(15,6);
    private static UvCoordinates crack8UVCoords = Utils.CalculateAtlasPosition(15,7);
    private static UvCoordinates crack9UVCoords = Utils.CalculateAtlasPosition(15,8);
    private static UvCoordinates crack10UVCoords = Utils.CalculateAtlasPosition(15,9);



    public static Vector2[,] BlockUVs =
    {
        //{ sandUVcoords.uvBottomLeft,sandUVcoords.uvBottomRight,sandUVcoords.uvTopLeft,sandUVcoords.uvTopRight },
        { grassTopUVcoords.uvBottomLeft,grassTopUVcoords.uvBottomRight,grassTopUVcoords.uvTopLeft,grassTopUVcoords.uvTopRight },
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
        { bedrockUVCoords.uvBottomLeft, bedrockUVCoords.uvBottomRight, bedrockUVCoords.uvTopLeft, bedrockUVCoords.uvTopRight },
        { cobblestoneUVCoords.uvBottomLeft, cobblestoneUVCoords.uvBottomRight, cobblestoneUVCoords.uvTopLeft, cobblestoneUVCoords.uvTopRight },
        { woodUVCoords.uvBottomLeft, woodUVCoords.uvBottomRight, woodUVCoords.uvTopLeft, woodUVCoords.uvTopRight },
        { plankUVCoords.uvBottomLeft, plankUVCoords.uvBottomRight, plankUVCoords.uvTopLeft, plankUVCoords.uvTopRight },
        { craftingUVCoords.uvBottomLeft, craftingUVCoords.uvBottomRight, craftingUVCoords.uvTopLeft, craftingUVCoords.uvTopRight },
        { furnaceUVCoords.uvBottomLeft, furnaceUVCoords.uvBottomRight, furnaceUVCoords.uvTopLeft, furnaceUVCoords.uvTopRight },
        { glassUVCoords.uvBottomLeft, glassUVCoords.uvBottomRight, glassUVCoords.uvTopLeft, glassUVCoords.uvTopRight },
        { brickUVCoords.uvBottomLeft, brickUVCoords.uvBottomRight, brickUVCoords.uvTopLeft, brickUVCoords.uvTopRight },
        { obsidianUVCoords.uvBottomLeft, obsidianUVCoords.uvBottomRight, obsidianUVCoords.uvTopLeft, obsidianUVCoords.uvTopRight },

        // Crack UVs
        { crack1UVCoords.uvBottomLeft, crack1UVCoords.uvBottomRight, crack1UVCoords.uvTopLeft, crack1UVCoords.uvTopRight},
        { crack2UVCoords.uvBottomLeft, crack2UVCoords.uvBottomRight, crack2UVCoords.uvTopLeft, crack2UVCoords.uvTopRight },
        { crack3UVCoords.uvBottomLeft, crack3UVCoords.uvBottomRight, crack3UVCoords.uvTopLeft, crack3UVCoords.uvTopRight },
        { crack4UVCoords.uvBottomLeft, crack4UVCoords.uvBottomRight, crack4UVCoords.uvTopLeft, crack4UVCoords.uvTopRight },
        { crack5UVCoords.uvBottomLeft, crack5UVCoords.uvBottomRight, crack5UVCoords.uvTopLeft, crack5UVCoords.uvTopRight },
        { crack6UVCoords.uvBottomLeft, crack6UVCoords.uvBottomRight, crack6UVCoords.uvTopLeft, crack6UVCoords.uvTopRight },
        { crack7UVCoords.uvBottomLeft, crack7UVCoords.uvBottomRight, crack7UVCoords.uvTopLeft, crack7UVCoords.uvTopRight },
        { crack8UVCoords.uvBottomLeft, crack8UVCoords.uvBottomRight, crack8UVCoords.uvTopLeft, crack8UVCoords.uvTopRight },
        { crack9UVCoords.uvBottomLeft, crack9UVCoords.uvBottomRight, crack9UVCoords.uvTopLeft, crack9UVCoords.uvTopRight },
        { crack10UVCoords.uvBottomLeft, crack10UVCoords.uvBottomRight, crack10UVCoords.uvTopLeft, crack10UVCoords.uvTopRight }

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
                Vector2 uv2 = meshesArray[i].uv2[v];

                VertexData point = new(vert,norm,uv, uv2);

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
                Vector2 uv2 = meshesArray[i].uv[triPoint];

                VertexData pointData = new(vert,norm, uv, uv2);
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
        List<Vector2> uvs2 = new();

        foreach (VertexData i in points.Keys)
        {
            vertices.Add(i.Item1);
            normals.Add(i.Item2);
            uvs.Add(i.Item3);
            uvs2.Add(i.Item4);
        }

        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.uv2 = uvs2.ToArray();
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
        float xStartPosition = 1f / atlasWidth * textureCollumn;
        float xEndPosition = 1f / atlasWidth * (textureCollumn + 1);
        float yStartPosition = 1f - 1f / atlasHeight * textureRow;
        float yEndPosition = 1f - (1f / atlasHeight * (textureRow + 1));

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
