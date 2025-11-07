Shader "Hidden/Editor_NormalAOConvert"
{
	Properties
	{
		_NormalTex("NormalTex", 2D) = "bump"{}
		_AOTex("AOTex", 2D) = "back"{}
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

				o.uv = v.texcoord;// fmod(v.texcoord.xy, 1);
				o.pos = float4(o.uv * 2 - 1, 0, 1);
				//o.pos.y = -o.pos.y;

				return o;
			}

			sampler2D _NormalTex;
			sampler2D _AOTex;

			float4 frag(v2f i) : SV_Target
			{
				float3 normalTS = UnpackNormal(tex2D(_NormalTex, i.uv));
				float  ao = tex2D(_AOTex, i.uv).x;
#if defined(UNITY_COLORSPACE_GAMMA) 
				//ao = LinearToGammaSpaceExact(ao);
#else
				//ao = GammaToLinearSpaceExact(ao);
#endif
				float4 col = float4(normalTS.xy * 0.5 + 0.5, 0, 1.0);
#if defined(UNITY_COLORSPACE_GAMMA) 
				//col.rgb = LinearToGammaSpace(col.rbg);
#else
				col.rgb = GammaToLinearSpace(col.rgb);
#endif
				col.b = ao;
				return col;
			}

			ENDCG
		}
	}
}