using UnityEngine;

public static class Utils
{
    const int atlasWidth = 16;
    const int atlasHeight = 16;

    /// <returns>
    /// The starting and ending position of the desired texture from the texture atlas.
    /// </returns>
    /// <param name="textureRow">0 based index of the desired texture</param>
    /// <param name="textureCollumn">0 based index of the desired texture</param>
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
