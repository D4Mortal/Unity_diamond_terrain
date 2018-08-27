// taken insight from https://docs.unity3d.com/Manual/SL-VertexFragmentShaderExamples.html
Shader "Custom/TerrainShader"
{	
	Properties{

		// properties for coloring the terrain
		_PeakColor("PeakColor", Color) = (0.6322535,0.754717,0.6016376,1)
		_PeakLevel("PeakLevel", Float) = 20

		_Level3Color("Level3Color", Color) = (0.660898,0.9716981,0.637104,1)
		_Level3("Level3", Float) = 15

		_Level2Color("Level2Color", Color) = (0.01610675,0.3962264,0,1)
		_Level2("Level2", Float) = 3

		_Level1Color("Level1Color", Color) = (0.5660378,0.4806644,0.05606978,1)
		_Level1("Level1", Float) = -4

		_GroundColor("SandColor", Color) = (0.6603774,0.4446357,0.07787468,1)
		_GroundLevel("SandLevel", Float) = -6

		_WaterColor("WaterColor", Color) = (0,0.4847451,0.6509434,1)
		_WaterLevel("WaterLevel", Float) = -8

		_Slope("Slope Fader", Range(0,1)) = 0
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


		struct v2f {
		float3 worldPos : TEXCOORD0;
		half3 worldNormal : TEXCOORD1;
		float4 pos : SV_POSITION;
		};

		// variables for different colors
		float _PeakLevel;
		float4 _PeakColor;

		float _Level3;
		float4 _Level3Color;

		float _Level2;
		float4 _Level2Color;

		float _Level1;
		float4 _Level1Color;

		float _GroundLevel;
		float4 _GroundColor;

		float _WaterLevel;
		float4 _WaterColor;

		float _Slope;

		// nothing really happens here, just passing through the vertex informations
		v2f vert(float4 vertex : POSITION, float3 normal : NORMAL)
		{
			v2f o;
			o.pos = UnityObjectToClipPos(vertex);
			o.worldPos = mul(unity_ObjectToWorld, vertex).xyz;
			o.worldNormal = UnityObjectToWorldNormal(normal);
			return o;
		}

		// color the pixels using fragment shader depending on the height value of the vertices
		fixed4 frag(v2f i) : SV_Target
		{
			float4 color;
			if (i.worldPos.y >= _PeakLevel)
				color = _PeakColor;

			if (i.worldPos.y <= _PeakLevel)
				color = lerp(_Level3Color, _PeakColor, (i.worldPos.y - _Level3) / (_PeakLevel - _Level3));

			if (i.worldPos.y <= _Level3)
				color = lerp(_Level2Color, _Level3Color, (i.worldPos.y - _Level2) / (_Level3 - _Level2));

			if (i.worldPos.y <= _Level2)
				color = lerp(_Level1Color, _Level2Color, (i.worldPos.y - _WaterLevel) / (_Level2 - _WaterLevel));

			if (i.worldPos.y <= _Level1)
				color = lerp(_GroundColor, _Level1Color, (i.worldPos.y - _GroundLevel) / (_Level1 - _GroundLevel));

			if (i.worldPos.y <= _GroundLevel)
				color = lerp(_WaterColor, _GroundColor, (i.worldPos.y - _WaterLevel) / (_Level1 - _WaterLevel));

			if (i.worldPos.y <= _WaterLevel)
				color = _WaterColor;

			color *= saturate(color + _Slope);

			return color;
		}
		ENDCG
	}
	}
}