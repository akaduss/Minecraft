using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Perlin3D_Grapher : MonoBehaviour
{
    private Vector3 dimensions = new(10, 10, 10);

    [Range(0.001f, 0.99f)]
    public float scale = 0.5f;
    public int octaves = 3;
    public float heightScale = 1f;
    public float heightOffset = 0f;
    [Range(0.0f, 10f)]
    public float DrawCutOff = 1f;

    void CreateCubes()
    {
        for (int x = 0; x < dimensions.x; x++)
        {
            for (int y=0; y < dimensions.y; y++)
            {
                for (int z=0; z < dimensions.z; z++)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.parent = transform;
                    cube.transform.position = new Vector3(x, y, z);
                    cube.name = "Perlin_Cube(" + x + ", " + y + ", " + z + ")";
                }
            }
        }
    }

    void Graph()
    {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        if(renderers.Length == 0)
        {
            CreateCubes();
        }

        if (renderers.Length == 0) return;

        for (int x = 0; x < dimensions.x; x++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                for (int z = 0; z < dimensions.z; z++)
                {
                    float noise = MeshUtils.FractialBrownianMotion3D(x, y, z, octaves, scale, heightScale, heightOffset);
                    if(noise < DrawCutOff)
                    {
                        //renderers[x * (int)dimensions.y * (int)dimensions.z + y * (int)dimensions.z + z].enabled = false;
                        renderers[x + (int)dimensions.x * y + (int)dimensions.x * (int)dimensions.y * z].enabled = false;
                    }
                    else
                    {
                        renderers[x * (int)dimensions.y * (int)dimensions.z + y * (int)dimensions.z + z].enabled = true;
                        //renderers[x + (int)dimensions.x * y + (int)dimensions.x * (int)dimensions.y * z].enabled = true;

                    }
                }
            }
        }
    }

    private void OnValidate()
    {
        Graph();
    }

}
