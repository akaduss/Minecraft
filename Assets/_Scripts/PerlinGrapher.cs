using System.ComponentModel;
using UnityEngine;

public struct PerlinSettings
{
    public readonly float scale;
    public readonly int octaves;
    public readonly float heightScale;
    public readonly float heightOffset;
    public readonly float probability;

    public PerlinSettings(float scale, int octaves, float heightScale, float heightOffset, float probability)
    {
        this.scale = scale;
        this.octaves = octaves;
        this.heightScale = heightScale;
        this.heightOffset = heightOffset;
        this.probability = probability;
    }
}

public struct Perlin3DSettings
{
    public readonly float scale;
    public readonly int octaves;
    public readonly float heightScale;
    public readonly float heightOffset;
    public readonly float drawCutOff;

    public Perlin3DSettings(float scale, int octaves, float heightScale, float heightOffset, float drawCutOff)
    {
        this.scale = scale;
        this.octaves = octaves;
        this.heightScale = heightScale;
        this.heightOffset = heightOffset;
        this.drawCutOff = drawCutOff;
    }
}

[ExecuteInEditMode]
public class PerlinGrapher : MonoBehaviour
{
    public LineRenderer lineRenderer;
    [Range(0.001f, 0.99f)]
    public float scale = 0.5f;
    public int octaves = 3;
    public float heightScale = 1f;
    public float heightOffset = 0f;
    [Range(0.0f, 1f)]
    public float probability = 0.5f;

    public int zValue = 11;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 100;
        Graph();
    }

    private void Graph()
    {
        //int z = 11;
        Vector3[] positions = new Vector3[lineRenderer.positionCount];

        for (int x = 0; x < lineRenderer.positionCount; x++)
        {
            float y = MeshUtils.FractialBrownianMotion(x,zValue, octaves, scale, heightScale, heightOffset);
            positions[x] = new Vector3(x, y, zValue);
        }

        lineRenderer.SetPositions(positions);
    }

    private void OnValidate()
    {
        Graph();
    }
}
