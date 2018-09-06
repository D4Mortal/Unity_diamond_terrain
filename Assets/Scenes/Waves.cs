using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waves : MonoBehaviour {

    public float scale = 0.1f;
    public float speed = 1.0f;
    public float noiseStrength = 1f;
    public float noiseWalk = 1f;

    private Vector3[] initialVertices;
    private MeshCollider meshCol;

    private void Start()
    {
        meshCol = this.gameObject.AddComponent<MeshCollider>();
    }

    // based on wave behaviour in lab4, added in perlin noise based on https://answers.unity.com/questions/443031/sinus-for-rolling-waves.html for more realistic waves
    void Update()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        //meshCol.sharedMesh = mesh;

        if (initialVertices == null)
        {
            initialVertices = mesh.vertices;
        }

        Vector3[] vertices = new Vector3[initialVertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = initialVertices[i];

            // create waves using the sin function relative to time
            vertex.y += Mathf.Sin(Time.time * speed + initialVertices[i].x + initialVertices[i].y + initialVertices[i].z) * scale;

            // add perlin noise to the height of the waves to make them seem more realistic
            vertex.y += Mathf.PerlinNoise(initialVertices[i].x + noiseWalk, initialVertices[i].y + Mathf.Sin(Time.time * 0.1f)) * noiseStrength;
            vertices[i] = vertex;
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }
}
