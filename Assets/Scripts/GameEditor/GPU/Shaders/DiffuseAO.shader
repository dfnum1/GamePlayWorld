Shader "Hidden/Editor_DiffuseAO"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white"{}
		_AOStrength("Ambient Occlusion Strength",Range(0,10)) = 1 //光滑强度
		_AOMap("Ambient Occlusion Map (RGB)", 2D) = "white" {}
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
				o.pos.y = -o.pos.y;

				return o;
			}

			sampler2D _MainTex;
			sampler2D _AOMap;
			float _AOStrength;

			float4 frag(v2f i) : SV_Target
			{
				float3 diffuseColor = tex2D(_MainTex, i.uv).rgb;
				float occlusion = saturate(tex2D(_AOMap, i.uv.xy).r);
#if defined(UNITY_COLORSPACE_GAMMA) 
#else
				occlusion = GammaToLinearSpaceExact(occlusion);
#endif
				float3 indirectDiffuse = lerp(1.0, occlusion, _AOStrength) * diffuseColor;
				float3 diffuseAO = diffuseColor;// +indirectDiffuse;
#if defined(UNITY_COLORSPACE_GAMMA) 
#else
				diffuseAO.rgb = GammaToLinearSpace(diffuseAO.rgb);
#endif
				return float4(diffuseAO.r, diffuseAO.g, diffuseAO.b, occlusion);
			}
			ENDCG
		}

	}
	FallBack Off
}

