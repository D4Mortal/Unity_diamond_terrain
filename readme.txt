The terrain is generated using Diamond Square algorithm, implemented following the guide by Ather Omar https://www.youtube.com/watch?v=1HV8GbFnCik
DiamondSquare.cs contains the script used to generate the terrain. It first procedurally creates a flat plane using triangles and 
vertices and then generates a visually realistic terrain by randomly assigning height values using diamond square algorithm.

The coloring of the terrain is done through using the frgament shader, we initially used the surface shader to assign colors but decided to switch to fragment
shader considering that we also wanted phong illumination. The terrain shader has taken insights from https://docs.unity3d.com/Manual/SL-VertexFragmentShaderExamples.html
Phong illumination is taken from Lab5 and had its constants adjusted accordingly for the terrain, values such as specular highlight has been decreased to 
create a more realistic terrain.

The wave effect is achieved through creating an plane that cuts through the terrain, the wave script uses the sin function to simlulat the effects of a wave 
and has perlin noise added to it to add randomness in the waves for a more realistc effect. 
The idea of adding perlin noise is adapted from https://answers.unity.com/questions/443031/sinus-for-rolling-waves.html

For the sun component, we first made created a sphere game object and added a shader which would simulate the effect of a more realistic
sun rising and setting. We then made the sun object rotate appropriately around the x-axis relative to the plane’s position. To simulate
the effect of changing light we manipulated the point light position variable within the Terrain shader by changing it’s vector position 
to be the same as the sun object within the SunOrbit script. By doing this, only the terrain changes its light intensity on its surface.
Most of the implementations done here was learned/adapted from the labs on manipulating shader light and object rotation. 
