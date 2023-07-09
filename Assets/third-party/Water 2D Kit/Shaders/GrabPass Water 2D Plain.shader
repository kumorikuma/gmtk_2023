Shader "Water/GrabPass/Water 2D Plain"
{
	Properties 
	{
		_MainTex ("Main Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,0.5)
		
		[Normal] _Refraction ("Refraction", 2D) = "bump" {}
		_Intensity ("Refraction Intensity", Float) = 0.02
		_Current ("Current Speed", Float) = -0.25
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
		
		GrabPass { }
		
		Pass 
		{
		CGPROGRAM
			#pragma vertex SpriteVert
			#pragma fragment PlainFrag
			#pragma target 2.0
			
			#pragma multi_compile __ DISABLE_REFRACTION
			#include "Water2D.cginc"
			
		#if DISABLE_REFRACTION
			// no grab sampler
		#else
			sampler2D _GrabTexture;
		#endif
			
			fixed4 PlainFrag(v2f IN) : SV_Target
			{
				IN.texcoord.x += _Current * _Time.y;
			
				fixed4 texColor = tex2D(_MainTex, TRANSFORM_TEX(IN.texcoord, _MainTex));
			
			#if DISABLE_REFRACTION
				fixed4 c = texColor * _Color;
			#else
				fixed4 sceneColor = refractionGP(_GrabTexture, _Refraction, _Refraction_ST, IN.texcoord, IN.screenPos, _Intensity);
				fixed4 c = fixed4(lerp(sceneColor.rgb, texColor.rgb * _Color.rgb, _Color.a), texColor.a);
			#endif
				
				c.rgb *= c.a;
				
				return c;
			}
		ENDCG
		}
	}
}
