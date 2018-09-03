// taken insight from https://docs.unity3d.com/Manual/SL-VertexFragmentShaderExamples.html
Shader "Custom/TerrainShader"
{	
	Properties{

		// values for the light source and Phong illumination
		_PointLightColor("Point Light Color", Color) = (1, 1, 1, 1)
		_PointLightPosition("Point Light Position", Vector) = (0.0, 25.0, 0.0)

		_specN("Specular highlight", Range(0,50)) = 5
		_Ks("Specular constant",Range(0,1)) = 0.08
		_Ka("RGB intensity", Range(0,1)) = 1
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
		struct vertOut {
		float3 worldPos : TEXCOORD0;
		half3 worldNormal : TEXCOORD1;
		float4 pos : SV_POSITION;
		fixed4 color : COLOR;
		};

		// variables for different colors
		static float4 MountainTopColor = float4(0.6322535f, 0.754717f, 0.6016376f, 1.0f);
		static float MountainTopHeight = 20;

		static float4 MountainCrestColor = float4(0.660898f, 0.9716981f, 0.637104f, 1.0f);
		static float MountainCrestHeight = 15;

		static float4 ForestColor = float4(0.01610675f, 0.3962264f, 0.0f, 1.0f);
		static float ForestHeight = 3;

		static float4 ForestEdgeColor = float4(0.5660378f, 0.4806644f, 0.05606978f, 1.0f);
		static float ForestEdgeHeight = -6;

		static float4 GroundColor = float4(0.6603774f, 0.4446357f, 0.07787468f, 1.0f);
		static float GroundLevel = -7;

		static float4 WaterColor = float4(0.0f, 0.4847451f, 0.6509434f, 1.0f);
		static float WaterLevel = -8;

		uniform float3 _PointLightColor;
		uniform float3 _PointLightPosition;
		float _specN;
		float _Ks;
		float _Ka;

		// nothing really happens here, just passing through the vertex informations
		vertOut vert(float4 vertex : POSITION, float3 normal : NORMAL, fixed4 color : COLOR, float3 worldPos : TEXCOORD0)
		{
			vertOut o;
			o.pos = UnityObjectToClipPos(vertex);
			o.worldPos = mul(unity_ObjectToWorld, vertex).xyz;
			o.worldNormal = UnityObjectToWorldNormal(normal);
			o.color = color;
			
			return o;
		}

		// color the pixels using fragment shader depending on the height value of the vertices and apply phong shading
		// coloring was originally done in the vertex shader but switched the fragment shader as it provided better coloring
		fixed4 frag(vertOut i) : SV_Target
		{	
			// assign pixel colors depending on the height of the vertex
			// use lerp to linearly interpolate colors 
			if (i.worldPos.y >= MountainTopHeight) {
				i.color = MountainTopColor;
			}

			if (i.worldPos.y <= MountainTopHeight) {
				i.color = lerp(MountainCrestColor, MountainTopColor, (i.worldPos.y - MountainCrestHeight) / (MountainTopHeight - MountainCrestHeight));
			}
				
			if (i.worldPos.y <= MountainCrestHeight) {
				i.color = lerp(ForestColor, MountainCrestColor, (i.worldPos.y - ForestHeight) / (MountainCrestHeight - ForestHeight));
			}
	
			if (i.worldPos.y <= ForestHeight) {
				i.color = lerp(ForestEdgeColor, ForestColor, (i.worldPos.y - ForestEdgeHeight) / (ForestHeight - ForestEdgeHeight));
			}

			if (i.worldPos.y <= ForestEdgeHeight) {
				i.color = lerp(GroundColor, ForestEdgeColor, (i.worldPos.y - GroundLevel) / (ForestEdgeHeight - GroundLevel));
			}
	
			if (i.worldPos.y <= GroundLevel) {
				i.color = lerp(WaterColor, GroundColor, (i.worldPos.y - WaterLevel) / (GroundLevel - WaterLevel));
			}

			if (i.worldPos.y <= WaterLevel) {
				i.color = WaterColor;
			}

			// add in phong shading, adapted from lab5,  so it applies phong illumination at fragment shader rather than vertex shader
			float4 phongColor = float4(0.0f, 0.0f, 0.0f, 0.0f);

			// Convert Vertex position and corresponding normal into world coords.
			// Note that we have to multiply the normal by the transposed inverse of the world 
			// transformation matrix (for cases where we have non-uniform scaling; we also don't
			// care about the "fourth" dimension, because translations don't affect the normal) 
			float3 worldNormal = normalize(i.worldNormal);

			// Calculate ambient RGB intensities
			// here rather than using v.color.rgb in lab5, we use the color that was determined previously by the height of the vertices
			float3 amb = i.color.rgb * UNITY_LIGHTMODEL_AMBIENT.rgb * _Ka;

			// Calculate diffuse RBG reflections, we save the results of L.N because we will use it again
			// (when calculating the reflected ray in our specular component)
			float fAtt = 1;
			float Kd = 1;
			float3 L = normalize(_PointLightPosition - i.worldPos);

			float LdotN = dot(L, worldNormal.xyz);
			float3 dif = fAtt * _PointLightColor.rgb * Kd * i.color.rgb * saturate(LdotN);

			// Calculate specular reflections
			float3 V = normalize(_WorldSpaceCameraPos - i.worldPos.xyz);

			//float3 R = float3(0.0, 0.0, 0.0);
			float3 R = float3(2 * worldNormal * LdotN - L);
			float3 spe = fAtt * _PointLightColor.rgb * _Ks * pow(saturate(dot(V, R)), _specN);

			// Combine Phong illumination model components
			phongColor.rgb = amb.rgb + dif.rgb + spe.rgb;
			i.color.rgb = phongColor.rgb;

			return i.color;
		}
		ENDCG
	}
	}
}