Shader "Hidden/Editor_NormalRoughnessAOConvert"
{
	Properties
	{
		_NormalTex("NormalTex", 2D) = "bump"{}
		_RoughnessTex("RoughnessTex", 2D) = "white"{}
		_AOTex("AOTex", 2D) = "white"{}
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
				o.pos.y = -o.pos.y;

				return o;
			}

			sampler2D _NormalTex;
			sampler2D _RoughnessTex;
			sampler2D _AOTex;

			float4 frag(v2f i) : SV_Target
			{
				float3 normalTS = UnpackNormal(tex2D(_NormalTex, i.uv));
				float  roughness = tex2D(_RoughnessTex, i.uv).r;
				float  ao = tex2D(_AOTex, i.uv).r;

#if defined(UNITY_COLORSPACE_GAMMA) 
#else
				ao = GammaToLinearSpaceExact(ao);
#endif
				float4 col = float4(normalTS.xy * 0.5 + 0.5, roughness, ao);

#if defined(UNITY_COLORSPACE_GAMMA) 
#else
				col.rgb = GammaToLinearSpace(col.rgb);
#endif
				return col;
			}

			ENDCG
		}
	}
}