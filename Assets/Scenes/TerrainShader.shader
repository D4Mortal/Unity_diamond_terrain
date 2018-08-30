// taken insight from https://docs.unity3d.com/Manual/SL-VertexFragmentShaderExamples.html
Shader "Custom/TerrainShader"
{	
	Properties{

		// properties for coloring the terrain
		_MountainTopColor("MountainTopColor", Color) = (0.6322535,0.754717,0.6016376,1)
		_MountainTopHeight("MountainTopHeight", Float) = 20

		_MountainCrestColor("MountainCrestColor", Color) = (0.660898,0.9716981,0.637104,1)
		_MountainCrestHeight("MountainCrestHeight", Float) = 15

		_ForestColor("ForestColor", Color) = (0.01610675,0.3962264,0,1)
		_ForestHeight("ForestHeight", Float) = 3

		_ForestEdgeColor("ForestEdgeColor", Color) = (0.5660378,0.4806644,0.05606978,1)
		_ForestEdgeHeight("ForestEdgeHeight", Float) = -6

		_GroundColor("GroundColor", Color) = (0.6603774,0.4446357,0.07787468,1)
		_GroundLevel("GroundLevel", Float) = -7

		_WaterColor("WaterColor", Color) = (0,0.4847451,0.6509434,1)
		_WaterLevel("WaterLevel", Float) = -8

		_Saturation("Saturation level", Range(0,1)) = 0.25

		// values for the light source and Phong illumination
		_PointLightColor("Point Light Color", Color) = (1, 1, 1, 1)
		_PointLightPosition("Point Light Position", Vector) = (0.0, 25.0, 0.0)

		_specN("Specular highlight", Range(0,50)) = 5
		_Ks("Specular constant",Range(0,1)) = 0.25

	}
	SubShader
	{
		Pass
	{
		CGPROGRAM

		// declare the functions that will be used
		#pragma vertex vert
		#pragma fragment frag
#		include "UnityCG.cginc"

		// declare the structure that's used 
		// worldPos is used to retrieve vertex height to determine the color of the terrain
		// worldNormal is used to calculate phong shading
		// pos to inform GPU of the clip space position
		struct v2f {
		float3 worldPos : TEXCOORD0;
		half3 worldNormal : TEXCOORD1;
		float4 pos : SV_POSITION;
		};

		// variables for different colors
		float _MountainTopHeight;
		float4 _MountainTopColor;

		float _MountainCrestHeight;
		float4 _MountainCrestColor;

		float _ForestHeight;
		float4 _ForestColor;

		float _ForestEdgeHeight;
		float4 _ForestEdgeColor;

		float _GroundLevel;
		float4 _GroundColor;

		float _WaterLevel;
		float4 _WaterColor;

		float _Saturation;

		uniform float3 _PointLightColor;
		uniform float3 _PointLightPosition;
		float _specN;
		float _Ks;

		// nothing really happens here, just passing through the vertex informations
		v2f vert(float4 vertex : POSITION, float3 normal : NORMAL)
		{
			v2f o;
			o.pos = UnityObjectToClipPos(vertex);
			o.worldPos = mul(unity_ObjectToWorld, vertex).xyz;
			o.worldNormal = UnityObjectToWorldNormal(normal);
			return o;
		}

		// color the pixels using fragment shader depending on the height value of the vertices and apply phong shading
		fixed4 frag(v2f i) : SV_Target
		{	
			// assign pixel colors depending on the height of the vertex
			// use lerp to interpolate colors https://docs.unity3d.com/ScriptReference/Color.Lerp.html
			float4 color;
			if (i.worldPos.y >= _MountainTopHeight)
				color = _MountainTopColor;

			if (i.worldPos.y <= _MountainTopHeight)
				color = lerp(_MountainCrestColor, _MountainTopColor, (i.worldPos.y - _MountainCrestHeight) / (_MountainTopHeight - _MountainCrestHeight));

			if (i.worldPos.y <= _MountainCrestHeight)
				color = lerp(_ForestColor, _MountainCrestColor, (i.worldPos.y - _ForestHeight) / (_MountainCrestHeight - _ForestHeight));

			if (i.worldPos.y <= _ForestHeight)
				color = lerp(_ForestEdgeColor, _ForestColor, (i.worldPos.y - _ForestEdgeHeight) / (_ForestHeight - _ForestEdgeHeight));

			if (i.worldPos.y <= _ForestEdgeHeight)
				color = lerp(_GroundColor, _ForestEdgeColor, (i.worldPos.y - _GroundLevel) / (_ForestEdgeHeight - _GroundLevel));

			if (i.worldPos.y <= _GroundLevel)
				color = lerp(_WaterColor, _GroundColor, (i.worldPos.y - _WaterLevel) / (_ForestEdgeHeight - _WaterLevel));

			if (i.worldPos.y <= _WaterLevel)
				color = _WaterColor;

			// apply saturation to the colors
			color *= saturate(color + _Saturation);


			// add in phong shading, adapted from lab5,  so it applies phong illumination at fragment shader rather than vertex shader
			float4 phongColor = float4(0.0f, 0.0f, 0.0f, 0.0f);

			// Convert Vertex position and corresponding normal into world coords.
			// Note that we have to multiply the normal by the transposed inverse of the world 
			// transformation matrix (for cases where we have non-uniform scaling; we also don't
			// care about the "fourth" dimension, because translations don't affect the normal) 
			float3 worldNormal = normalize(i.worldNormal);

			// Calculate ambient RGB intensities
			float Ka = 1;

			// here rather than using v.color.rgb in lab5, we use the color that was determined previously by the height of the vertices
			float3 amb = color.rgb * UNITY_LIGHTMODEL_AMBIENT.rgb * Ka;

			// Calculate diffuse RBG reflections, we save the results of L.N because we will use it again
			// (when calculating the reflected ray in our specular component)
			float fAtt = 1;
			float Kd = 1;
			float3 L = normalize(_PointLightPosition - i.worldPos);

			float LdotN = dot(L, worldNormal.xyz);
			float3 dif = fAtt * _PointLightColor.rgb * Kd * color.rgb * saturate(LdotN);

			// Calculate specular reflections
			float3 V = normalize(_WorldSpaceCameraPos - i.worldPos.xyz);

			//float3 R = float3(0.0, 0.0, 0.0);
			float3 R = float3(2 * worldNormal * LdotN - L);
			float3 spe = fAtt * _PointLightColor.rgb * _Ks * pow(saturate(dot(V, R)), _specN);

			// Combine Phong illumination model components
			phongColor.rgb = amb.rgb + dif.rgb + spe.rgb;
			phongColor.a = color.a;

			return phongColor;
		}
		ENDCG
	}
	}
}