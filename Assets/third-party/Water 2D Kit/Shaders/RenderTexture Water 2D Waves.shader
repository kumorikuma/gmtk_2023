Shader "Water/RenderTexture/Water 2D Waves"
{
	Properties 
	{
		[NoScaleOffset] _TopTex ("Top Texture", 2D) = "white" {}
		
		_MainTex ("Main Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,0.5)
		
		[Normal] _Refraction ("Refraction", 2D) = "bump" {}
		_Intensity ("Refraction Intensity", Float) = 0.02
		_Current ("Current Speed", Float) = -0.25
		_Wave ("Wave", Vector) = (1, 0.2, 0.5, 0.7)
		_Level ("Level", Range (0, 1)) = 0.5
		_Trim ("Surface Trim", Range (0, 1)) = 0.9
		_GrabTex ("Grab Texture", 2D) = "white" {}
	}
	
	SubShader 
	{
		Tags
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		
		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
		
		Pass 
		{
		CGPROGRAM
			#pragma vertex SpriteVert
			#pragma fragment WavesFrag
			#pragma target 2.0
		
			#pragma multi_compile __ DISABLE_WAVES
			#pragma multi_compile __ DISABLE_REFRACTION
			#include "Water2D.cginc"
			
			uniform float4 _Wave;
			uniform float _Level;
			uniform sampler2D _TopTex;
			uniform float _Trim;
			
		#if DISABLE_REFRACTION
			// no grab sampler
		#else
			sampler2D _GrabTex;
			float4 _GrabTex_ST;
		#endif
			
			fixed4 WavesFrag(v2f IN) : SV_Target
			{
				float2 coord = IN.texcoord;
				IN.texcoord.x += _Current * _Time.y;
			
			#if DISABLE_WAVES
				// no waves
			#else
				float t = clamp((IN.texcoord.y - _Level)/(1 - _Level), 0, 1);
				IN.texcoord.y = lerp(IN.texcoord.y, sinTransform(IN.texcoord, _Wave), t*t);
			#endif
			
				fixed4 texColor = sampleCombined(_TopTex, _MainTex, _MainTex_ST, IN.texcoord, _Trim);
				
			#if DISABLE_REFRACTION
				fixed4 c = texColor * _Color;
			#else
				fixed4 sceneColor = refractionRT(_GrabTex, TRANSFORM_TEX(coord, _GrabTex), _Refraction, _Refraction_ST, IN.texcoord, _Intensity);
				fixed4 c = fixed4(lerp(sceneColor.rgb, texColor.rgb * _Color.rgb, _Color.a), texColor.a);
			#endif
				
				c.rgb *= c.a;
				
				return c;
			}
		ENDCG
		}
	}
}
