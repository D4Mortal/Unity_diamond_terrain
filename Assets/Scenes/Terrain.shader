Shader "Custom/terrain" {
	Properties{

		// properties for coloring the terrain
		_PeakColor("PeakColor", Color) = (0.6322535,0.754717,0.6016376,1)
		_PeakLevel("PeakLevel", Float) = 11

		_Level3Color("Level3Color", Color) = (0.660898,0.9716981,0.637104,1)
		_Level3("Level3", Float) = 8

		_Level2Color("Level2Color", Color) = (0.01610675,0.3962264,0,1)
		_Level2("Level2", Float) = 4

		_Level1Color("Level1Color", Color) = (0.5660378,0.4806644,0.05606978,1)
		_Level1("Level1", Float) = -4

		_GroundColor("SandColor", Color) = (0.6603774,0.4446357,0.07787468,1)
		_GroundLevel("SandLevel", Float) = -6

		_WaterColor("WaterColor", Color) = (0,0.4847451,0.6509434,1)
		_WaterLevel("WaterLevel", Float) = -8

		_Slope("Slope Fader", Range(0,1)) = 0
	}
		SubShader{

			CGPROGRAM

			// declare used functions
			#pragma surface surf Lambert 
			#pragma	vertex vert

			struct Input {
			float3 customColor;
			float3 worldPos;
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

			// initialize output parameter
			void vert(inout appdata_full v, out Input o) {
				UNITY_INITIALIZE_OUTPUT(Input, o);
				o.customColor = v.normal.y;
			}

			// use surface shader to implement colors for the terrain depending on the vertex height
			void surf(Input IN, inout SurfaceOutput o) {

				if (IN.worldPos.y >= _PeakLevel)
					o.Albedo = _PeakColor;

				if (IN.worldPos.y <= _PeakLevel)
					o.Albedo = lerp(_Level3Color, _PeakColor, (IN.worldPos.y - _Level3) / (_PeakLevel - _Level3));

				if (IN.worldPos.y <= _Level3)
					o.Albedo = lerp(_Level2Color, _Level3Color, (IN.worldPos.y - _Level2) / (_Level3 - _Level2));

				if (IN.worldPos.y <= _Level2)
					o.Albedo = lerp(_Level1Color, _Level2Color, (IN.worldPos.y - _WaterLevel) / (_Level2 - _WaterLevel));

				if (IN.worldPos.y <= _Level1)
					o.Albedo = lerp(_GroundColor, _Level1Color, (IN.worldPos.y - _GroundLevel) / (_Level1 - _GroundLevel));

				if (IN.worldPos.y <= _GroundLevel)
					o.Albedo = lerp(_WaterColor, _GroundColor, (IN.worldPos.y - _WaterLevel) / (_Level1 - _WaterLevel));

				if (IN.worldPos.y <= _WaterLevel)
					o.Albedo = _WaterColor;

				o.Albedo *= saturate(IN.customColor + _Slope);
			}
			ENDCG
		}
		
}