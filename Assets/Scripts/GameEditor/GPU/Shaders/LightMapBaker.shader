Shader "Hidden/Eitor_LightMapBaker"
{
	Properties
	{
		_DiffuseTex("DiffuseTex", 2D)	= "white"{}
		_FinalTex("FinalTex", 2D)		= "white"{}
		_LightPowerScale("LightPowerScale",Range(0,5)) = 1
	}

	SubShader
	{
		ZTest Off
		Cull Off
		ZWrite Off
		Blend Off
		Pass
		{
			HLSLPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform float _LUTCurveScale;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata v)
			{
				v2f o;

				o.uv = v.texcoord;
				o.pos = float4(o.uv * 2 - 1, 0, 1);
				o.pos.y = -o.pos.y;

				return o;
			}

			sampler2D _DiffuseTex;
			sampler2D _FinalTex;

			CBUFFER_START(UnityPerMaterial)
			float _LightPowerScale;
			CBUFFER_END
			float4 frag(v2f i) : SV_Target
			{
				float4 diffCol = tex2D(_DiffuseTex, i.uv);
				float4 finalCol = tex2D(_FinalTex, i.uv);

				float3 rt = (finalCol - diffCol)*0.5 + 0.5;

				return float4(rt, 1);
			}

			ENDHLSL
		}

	}
	FallBack Off
}

