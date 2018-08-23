// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'



// NOTE, CODE BASED ON http://answers.unity3d.com/questions/54313/shader-rgb-colour-based-on-y-value-vertex.html
// The method for implementation is something I did on my own, but the overall strategy was taken from the above
// (I used the fragment function to implement it. The user did not)

// NOTE, the Phong Shading component is based off of Lab 4 and adjusted appropriately to be able to take in
// the gradientColor parameter (instead of using color directly from the procedural generation script).

Shader "Custom/WorldPosShader" {
	// basic properties used to do the color shading (different levels for different colors)
	Properties{
		_PeakColor("PeakColor", Color) = (0.8,0.9,0.9,1)
		_PeakLevel("PeakLevel", Float) = 2500
		_Level4Color("Level4Color", Color) = (0.64,0.224,0.208,1)
		_Level4("Level4", Float) = 2000
		_Level3Color("Level3Color", Color) = (0.64,0.224,0.208,1)
		_Level3("Level3", Float) = 1000
		_Level2Color("Level2Color", Color) = (0,0.14,0.69,1)
		_Level2("Level2", Float) = 400
		_Level1Color("Level1Color", Color) = (0.65,0.86,0.63,1)
		_Level1("Level1", Float) = 300
		_SandLevel("SandLevel", Float) = 100
		_SandColor("SandColor", Color) = (0.37,0.78,0.92,1)
		_WaterLevel("WaterLevel", Float) = 0
		_WaterColor("WaterColor", Color) = (0.37,0.78,0.92,1)
		_Slope("Slope Fader", Range(0,1)) = 0
		_PointLightColor("Point Light Color", Color) = (0, 0, 0)
		_PointLightPosition("Point Light Position", Vector) = (0.0, 0.0, 0.0)
	}
	SubShader{
		Pass{
			CGPROGRAM

			// declare functions
			#pragma vertex vertexFunction
			#pragma fragment fragmentFunction

			#include "UnityCG.cginc"

			// declare the data we need from our script
			struct appdata {
				float4 vertex : POSITION;
				float4 normal : NORMAL;
			};

			// declare the struct which we'll use
			// needs to be able to take in position vertex as well as worldPos (for height shading,
			// and worldNormal for the phong shading)
			struct v2f {
				float4 position: SV_POSITION;
				float4 worldPosition : TEXCOORD0;
				float3 worldNormal : TEXCOORD1;
			};


			// now prepare the different property levels
			float _PeakLevel;
			float4 _PeakColor;
			float _Level4;
			float4 _Level4Color;
			float _Level3;
			float4 _Level3Color;
			float _Level2;
			float4 _Level2Color;
			float _Level1;
			float4 _Level1Color;
			float _Slope;
			float _SandLevel;
			float4 _SandColor;
			float _WaterLevel;
			float4 _WaterColor;
			uniform float3 _PointLightColor;
			uniform float3 _PointLightPosition;

			// reads in the data we need from the program
			v2f vertexFunction(appdata IN) {
				v2f OUT;

				OUT.position = UnityObjectToClipPos(IN.vertex);
				OUT.worldPosition = mul(unity_ObjectToWorld, IN.vertex);
				OUT.worldNormal = normalize(mul(transpose((float3x3)unity_WorldToObject), IN.normal.xyz));


				return OUT;
			}

			// depending on the actual height of the target, the color of it changes (use several if statements,
			// switch may have been more appropriate but the functionality is the same)
			fixed4 fragmentFunction(v2f IN) : SV_Target{
				float4 gradientColor;
				if (IN.worldPosition.y >= _PeakLevel)
					gradientColor = _PeakColor;
				if (IN.worldPosition.y <= _PeakLevel)
					gradientColor = lerp(_Level4Color, _PeakColor, (IN.worldPosition.y - _Level4) / (_PeakLevel - _Level4));
				if (IN.worldPosition.y <= _Level4)
					gradientColor = lerp(_Level3Color, _Level4Color, (IN.worldPosition.y - _Level3) / (_Level4 - _Level3));
				if (IN.worldPosition.y <= _Level3)
					gradientColor = lerp(_Level2Color, _Level3Color, (IN.worldPosition.y - _Level2) / (_Level3 - _Level2));
				if (IN.worldPosition.y <= _Level2)
					gradientColor = lerp(_Level1Color, _Level2Color, (IN.worldPosition.y - _Level1) / (_Level2 - _Level1));
				if (IN.worldPosition.y <= _Level1)
					gradientColor = lerp(_SandColor, _Level1Color, (IN.worldPosition.y - _SandLevel) / (_Level1 - _SandLevel));
				if (IN.worldPosition.y <= _SandLevel)
					gradientColor = _SandColor;
				if (IN.worldPosition.y <= _WaterLevel) 
					gradientColor = _WaterColor;
					gradientColor *= saturate(gradientColor + _Slope);
				
				// PHONG SHADING COMPONENT IS TAKEN FROM LAB4 and then adjusted to suit the current situation (e.g. since we are
				// finding the gradientColor ourselves (color not stated within the procedural mesh generation script), the color
				// of gradientColor has phong shading applied to it).
				// Our interpolated normal might not be of length 1
				float3 interpNormal = normalize(IN.worldNormal);

				// Calculate ambient RGB intensities
				float Ka = 1;
				float3 amb = gradientColor.rgb * UNITY_LIGHTMODEL_AMBIENT.rgb * Ka;

				// Calculate diffuse RBG reflections, we save the results of L.N because we will use it again
				// (when calculating the reflected ray in our specular component)
				float fAtt = 1;
				float Kd = 1;
				if (IN.worldPosition.y <= _WaterLevel) {
				Kd=0.5;
				}
				float3 L = normalize(_PointLightPosition - IN.worldPosition.xyz);
				float LdotN = dot(L, interpNormal);
				float3 dif = fAtt * _PointLightColor.rgb * Kd * gradientColor.rgb * saturate(LdotN);

				// Calculate specular reflections

//				float Ks = 1;
//				float specN = 5; // Values>>1 give tighter highlights
				float Ks = 0.5;
				if (IN.worldPosition.y <= _WaterLevel) {
				Ks=0.1;
				}
				float specN = 5; // Values>>1 give tighter highlights
				float3 V = normalize(_WorldSpaceCameraPos - IN.worldPosition.xyz);
				// Using classic reflection calculation:
				//float3 R = normalize((2.0 * LdotN * interpNormal) - L);
				//float3 spe = fAtt * _PointLightColor.rgb * Ks * pow(saturate(dot(V, R)), specN);
				// Using Blinn-Phong approximation:
				//specN = 25; // We usually need a higher specular power when using Blinn-Phong
				float3 H = normalize(V + L);
				float3 spe = fAtt * _PointLightColor.rgb * Ks * pow(saturate(dot(interpNormal, H)), specN);

				// Combine Phong illumination model components
				float4 returnColor = float4(0.0f, 0.0f, 0.0f, 0.0f);
				returnColor.rgb = amb.rgb + dif.rgb + spe.rgb;
								
				returnColor.a = gradientColor.a;

				return returnColor;
				//return gradientColor;
			}

			ENDCG
		}
	}
}
