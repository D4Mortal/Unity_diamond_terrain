using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewDiamondSquare : MonoBehaviour {


    // Note, implementation is based off Ather Omar, at https://www.youtube.com/watch?v=1HV8GbFnCik
    /*  -----Method for Diamond Square-----
        1. Initialize the corners to random values
        2. Set the center of the heightmap to the average of the corners
           plus a random value (the "diamond" step)
        3. For each diamond in the array, set the midpoint of that square
           to be the average of the four corners plus a random value. The
           "square" step.
        4. Repeat steps 2-4 on successively smaller chunks of the heightmap
           until you bottom out at 3x3 chunks   */

    /*  The functions we'll need:
        1. Create the Terrain
        2. Diamond Step
        3. Square Step (2 and 3 are combined into DiamondSquare step because
           implementing them together was easier)
        4. Will not need an update function except for Phong shading    */


    /*  The variables we'll need:
        1. the number of times we want to perform diamond/square step
        2. the size of the overall terrain
        3. the maximum height we'll let the terrain go to
        4. a certain amount of noise for the terrain color generation
        5. the array which stores vertexes needs to be declared globally*/

    GameObject cube;
    public int nDivisions;
    public float nSize;
    public float nHeight;
    Vector3[] allVerts;

    // now also add Shader and pointLight for Phong shading
    public PointLight pointLight;

    void Start ()
    {
        CreateTerrain();
    }

    // update function used to implement Phong shading
    void Update()
    {
        // Get renderer component (in order to pass params to shader)
        MeshRenderer renderer = this.gameObject.GetComponent<MeshRenderer>();

        // Pass updated light positions to shader
        renderer.material.SetColor("_PointLightColor", this.pointLight.color);
        renderer.material.SetVector("_PointLightPosition", this.pointLight.GetWorldPosition());
    }

    void CreateTerrain()
    {
        //////////////////////////////////////////////////////////////
        // we will create the terrain by storing each vertex within an array
        // The terrain will be a square, so I can just declare one sidelength

        // for a square of 16 faces (4x4), there are 25 unique vertices (5x5), so the total number
        // of vertices is...
        int numVertices = (nDivisions + 1) * (nDivisions + 1);

        // the number of UVs is the same as the vertices (2 points for the UV, 3 points for the
        // vertex, but the overall number of each is the same)
        int numUVs = (nDivisions + 1) * (nDivisions + 1);

        // now create the arrays to store the vertices, UVs, and the points for the triangle 
        allVerts = new Vector3[numVertices];
        Vector2[] allUVs = new Vector2[numUVs];
        int[] allTriangles;

        // the total number of squares is nDivisions * nDivisions, but each square is made up of 
        // two triangles, hence, 6 points
        allTriangles = new int[nDivisions * nDivisions * 6];
        

        //////////////////////////////////////////////////////////////
        // We'll first create a flat terrain (set all the appropriate
        // x and z values and triangles, and adjust the height later

        // create some variables just to make indexing easier
        float halfSize = nSize * 0.5f;
        float divisionSize = nSize / nDivisions;

        // store the number of triangles we've placed into the matrix so far
        int triOffset = 0;

        for (int i = 0; i <= nDivisions; i++)
        {
            for (int j = 0; j <= nDivisions; j++)
            {
                // this first step is to set all the relevant x and z values to be next to each other
                // the y value is set at 0.0f for now, but will be changed later
                allVerts[i * (nDivisions + 1) + j] = new Vector3(-halfSize + j * divisionSize,
                    0.0f, halfSize - i * divisionSize);
                allUVs[i * (nDivisions + 1) + j] = new Vector2((float)i / nDivisions,
                    (float)j / nDivisions);

                if (i < nDivisions && j < nDivisions)
                {
                    // get the vertex of the triangle we're building
                    int topLeft = i * (nDivisions + 1) + j;
                    int botLeft = (i + 1) * (nDivisions + 1) + j;

                    // now make the triangle (build in counterclockwise)
                    allTriangles[triOffset] = topLeft;
                    allTriangles[triOffset + 1] = topLeft + 1;
                    allTriangles[triOffset + 2] = botLeft + 1;

                    allTriangles[triOffset + 3] = topLeft;
                    allTriangles[triOffset + 4] = botLeft + 1;
                    allTriangles[triOffset + 5] = botLeft;

                    triOffset += 6;
                }
            }
        }


        //////////////////////////////////////////////////////////////
        // now we start building the terrain
        // first, grab the mesh
        Mesh mesh = new Mesh();
        mesh = GetComponent<MeshFilter>().mesh;
        

        // first perform step 1 (initialize the four corners to random variables).
        // Their x and z values won't be set yet, only the height is random
        // (since the triangles need to be side by side).

        // top left corner
        allVerts[0].y = Random.Range(-nHeight/5, nHeight);

        // top right corner
        allVerts[nDivisions].y = Random.Range(-nHeight/5, nHeight);

        // bottom right corner (is just size of our array - 1)
        allVerts[allVerts.Length - 1].y = Random.Range(-nHeight/5, nHeight);

        // bottom left corner (size of our array - 1, -nDivisions)
        allVerts[allVerts.Length - 1 - nDivisions].y = Random.Range(-nHeight/5, nHeight);


        //////////////////////////////////////////////////////////////
        // now perform diamond square (steps 2-4)

        // we start off with just 1 square. It's size is nDivisions x nDivisions
        int numSquares = 1;
        int squareSize = nDivisions;

        // also store the number of steps we'll take
        int stepSize = (int)Mathf.Log(nDivisions, 2);

        for (int i = 0; i < stepSize; i++)
        {
            int row = 0;

            // second iteration (first time, 2x2 squares, will make 4 squares)
            for (int j = 0; j < numSquares; j++)
            {
                int column = 0;
                for (int k = 0; k < numSquares; k++)
                {
                    diamondSquare(row, column, squareSize, nHeight);
                    column += squareSize;
                }

                // move on to the next row
                row += squareSize;
            }

            // we create four times as many squares, each with half the size
            // now we need to run diamond square on four times as many squares
            numSquares *= 2;
            squareSize /= 2;
            nHeight /= 2;
        }


        //////////////////////////////////////////////////////////////
        // finally, put it all together
        mesh.vertices = allVerts;
        mesh.uv = allUVs;
        mesh.triangles = allTriangles;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

		//Add mesh to meshcollider
		//http://answers.unity3d.com/questions/14465/how-to-assign-procedural-mesh-to-a-collider.html
		MeshCollider meshc = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
		meshc.sharedMesh = mesh; 
    }

    // this is step 2 and 3
    void diamondSquare(int row, int column, int stepSize, float heightRange)
    {
        //////////////////////////////////////////////////////////////
        // first step 2, get the four points
        int topLeft = row * (nDivisions + 1) + column;
        int topRight = topLeft + stepSize;
        int botLeft = (row + stepSize) * (nDivisions + 1) + column;
        int botRight = botLeft + stepSize;

        // use the midway point to get the middle index
        int halfSize = (int)(stepSize/2);
        int mid = (int)(row + halfSize) * (nDivisions + 1) + (int)(column + halfSize);

        // get the average, and add a random value
        allVerts[mid].y = (allVerts[topLeft].y + allVerts[topRight].y +
            allVerts[botLeft].y + allVerts[botRight].y) * 0.25f + Random.Range(-nHeight, nHeight);


        //////////////////////////////////////////////////////////////
        // now for step 3 (the midpoints between the corners
        // take the top left point of the square, take half the size of the square
        // and get the point by averaging out between topleft, topright, and mid
        int topPoint = topLeft + halfSize;
        int leftPoint = mid - halfSize;
        int rightPoint = mid + halfSize;
        int botPoint = botLeft + halfSize;

        // take the average of the two points (left and right or up and down).
        allVerts[topPoint].y = (allVerts[topLeft].y + allVerts[topRight].y)/2 + Random.Range(-nHeight, nHeight);
        allVerts[leftPoint].y = (allVerts[topLeft].y + allVerts[botLeft].y)/2 + Random.Range(-nHeight, nHeight);
        allVerts[rightPoint].y = (allVerts[topRight].y + allVerts[botRight].y)/2 + Random.Range(-nHeight, nHeight);
        allVerts[botPoint].y = (allVerts[botLeft].y + allVerts[botRight].y)/2 + Random.Range(-nHeight, nHeight);
    }


}

