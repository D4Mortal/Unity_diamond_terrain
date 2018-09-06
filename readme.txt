The terrain is generated using Diamond Square algorithm, implemented following the guide by Ather Omar https://www.youtube.com/watch?v=1HV8GbFnCik
DiamondSquare.cs contains the script used to generate the terrain. It first procedurally creates a flat plane using triangles and 
vertices and then generates a visually realistic terrain by randomly assigning height values using diamond square algorithm.

The coloring of the terrain is done through using the frgament shader, we initially used the surface shader to assign colors but decided to switch to fragment
shader considering that we also wanted phong illumination. The terrain shader has taken insights from https://docs.unity3d.com/Manual/SL-VertexFragmentShaderExamples.html
Phong illumination is taken from Lab5 and had its constants adjusted accordingly for the terrain, values such as specular highlight has been decreased to 
create a more realistic terrain.

The wave effect is achieved through creating an plane that cuts through the terrain, the wave script uses the sin function to simlulat the effects of a wave 
and has perlin noise added to it to add randomness in the waves for a more realistc effect. The idea of adding perlin noise is adapted from https://answers.unity.com/questions/443031/sinus-for-rolling-waves.html


