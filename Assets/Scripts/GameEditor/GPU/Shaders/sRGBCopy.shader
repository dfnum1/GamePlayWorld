Shader "Hidden/Editor_sRGBCopy"
{
	Properties
	{
		_MainTex("OrignTex", 2D) = "white"{}
		_ConvertGamma("ConvertGamma", Float) = 0.0
	}

	SubShader
	{
		ZTest Off
		Cull Off
		ZWrite Off
		Blend Off
		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata_full v)
			{
				v2f o;

				o.uv = v.texcoord;
				o.pos = float4(o.uv * 2 - 1, 0, 1);
				//o.pos.y = -o.pos.y;

				return o;
			}

			sampler2D _MainTex;

			float _ConvertGamma;

			float4 frag(v2f i) : SV_Target
			{
				float4 col = tex2D(_MainTex, i.uv);

#if defined(UNITY_COLORSPACE_GAMMA) 
#else
				float3 gammaCol = GammaToLinearSpace(col.xyz);
				col.rgb = lerp(col.rgb, gammaCol.rgb, _ConvertGamma);
#endif
				return col;
			}

			ENDCG
		}

	}
	FallBack Off
}

