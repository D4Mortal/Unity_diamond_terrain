using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamonSquare : MonoBehaviour {

    public int edges;
    public float size;
    public float height;

    private Vector3[] vertices;
    private int numOfVertices;

	// Use this for initialization
	void Start () {
        int temp = 1;
        for (int i = 0; i < edges; i++)
        {
            temp *= 2;
        }
        edges = temp;
        CreateTerrain();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CreateTerrain()
    {   
        // there's n+1 vertices for n faces on each side
        numOfVertices = (edges + 1) * (edges + 1);

        // initializing matrices for vertices and its uv values
        vertices = new Vector3[numOfVertices];
        Vector2[] vertex_uv = new Vector2[numOfVertices];

        // an array for storing triangles, there are a total of (edges*edges*2) number of triangles, and each triangles requires 3 points 
        int[] triangles = new int[edges * edges * 2 * 3];

        float halfSize = size * 0.5f;
        float sizeOfEdges = size / edges;

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        int offSet = 0;

        // creating flat terrain
        for (int i = 0; i <= edges; i++)
        {
            for (int j = 0; j <= edges; j++)
            {
                // setup the triangle vertices from left to right  and normalize the uv vector
                // i * (edges + 1) + j is used because it represents an 2D array using an 1D representation
                vertices[i * (edges + 1) + j] = new Vector3(-halfSize + j * sizeOfEdges, 0.0f, halfSize - i * sizeOfEdges);
                vertex_uv[i * (edges + 1) + j] = new Vector2((float)i / edges, (float)j / edges);

                // create triangles when it's not at the corners
                if (i < edges && j < edges)
                {
                    // retreive the co-oridinate of the vertices of the triangle that's currently building
                    int topLeft = i * (edges + 1) + j;
                    int bottomLeft = (i + 1) * (edges + 1) + j;

                    // create the triangles square by square, anticlockwise
                    triangles[offSet] = topLeft;
                    triangles[offSet + 1] = topLeft + 1;
                    triangles[offSet + 2] = bottomLeft + 1;

                    triangles[offSet + 3] = topLeft;
                    triangles[offSet + 4] = bottomLeft + 1;
                    triangles[offSet + 5] = bottomLeft;

                    offSet += 6;
                }
            }
        }

        // random a height value for the vertices
        // top left
        vertices[0].y = Random.Range(-height, height);

        // top right
        vertices[edges].y = Random.Range(-height, height);

        // bottom right 
        vertices[vertices.Length - 1].y = Random.Range(-height, height);

        // bottom left
        vertices[vertices.Length - 1 - edges].y = Random.Range(-height, height);





        // start diamond square algorithm

        // the number of steps needed is the log of the number of edges
        int numOfSteps = (int)Mathf.Log(edges, 2);
        int squareSize = edges;
        int numSquares = 1;

        for (int i = 0; i < numOfSteps; i++)
        {
            int row = 0;

            for (int j = 0; j < numSquares; j++)
            {
                int col = 0;

                for (int k = 0; k < numSquares; k++)
                {
                    DiamondSquare(row, col, squareSize, height);
                    col += squareSize;
                }

                row += squareSize;
            }

            // for every step 4 times as much squares are created
            numSquares *= 2;
            squareSize /= 2;

            // decreasing the rate height decreases
            height *= 0.5f;
        }


        mesh.vertices = vertices;
        mesh.uv = vertex_uv;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

    }

    // diamond square algorithm performed on a single square
    public void DiamondSquare(int row, int col, int size, float heightRange)
    {
        // Diamond step
        int halfSize = (int)(size * 0.5f);

        // find the index of the corners
        int topLeft = row * (edges + 1) + col;
        int topRight = topLeft + size;
        int bottomLeft = (row + size) * (edges + 1) + col;
        int bottomRight = bottomLeft + size;

        int center = (int)(row + halfSize) * (edges + 1) + (int)(col + halfSize);

        // find the average height
        vertices[center].y = vertices[topLeft].y + vertices[bottomLeft].y + vertices[topRight].y + vertices[bottomRight].y;
        vertices[center].y *= 0.25f;

        vertices[center].y += Random.Range(-heightRange, heightRange);


        // Square step

        // find the corners of the square 
        int left = center - halfSize;
        int right = center + halfSize;
        int top = topLeft + halfSize;
        int bottom = bottomLeft + halfSize;

        // find the average for corners of the square by taking the center and 2 neighboring points
        vertices[top].y = (vertices[center].y + vertices[topLeft].y + vertices[topRight].y) / 3;
        vertices[bottom].y = (vertices[center].y + vertices[bottomLeft].y + vertices[bottomRight].y) / 3;
        vertices[left].y = (vertices[center].y + vertices[bottomLeft].y + vertices[topLeft].y) / 3;
        vertices[right].y = (vertices[center].y + vertices[topRight].y + vertices[bottomRight].y) / 3;

        // add an random value to the heights
        vertices[top].y += Random.Range(-heightRange, heightRange);
        vertices[bottom].y += Random.Range(-heightRange, heightRange);
        vertices[left].y += Random.Range(-heightRange, heightRange);
        vertices[right].y += Random.Range(-heightRange, heightRange);
    }
}
