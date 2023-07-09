
#ifndef UNITY_SPRITES_INCLUDED
#define UNITY_SPRITES_INCLUDED

#include "UnityCG.cginc"

// Material Color.
fixed4 _Color;

struct appdata_t
{
	float4 vertex   : POSITION;
	float2 texcoord : TEXCOORD0;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f
{
	float4 vertex   : SV_POSITION;
	float2 texcoord : TEXCOORD0;
	float4 screenPos : TEXCOORD5;
	UNITY_VERTEX_OUTPUT_STEREO
};

v2f SpriteVert(appdata_t IN)
{
	v2f OUT;

	UNITY_SETUP_INSTANCE_ID (IN);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

	OUT.vertex = UnityObjectToClipPos(IN.vertex);
	OUT.texcoord = IN.texcoord;
	OUT.screenPos = OUT.vertex;

	return OUT;
}

sampler2D _MainTex;
float4 _MainTex_ST;

float _Intensity;
sampler2D _Refraction; 
float4 _Refraction_ST;
float _Current;

fixed4 refractionGP(sampler2D grabTex, sampler2D normalMap, float4 normalMap_ST, float2 texcoord, float4 screenPos, float intensity)
{
	screenPos /= screenPos.w;
	screenPos.y *= _ProjectionParams.x;
	
	#if UNITY_UV_STARTS_AT_TOP
		screenPos.y *= -_ProjectionParams.x;
	#else
		screenPos.y *= _ProjectionParams.x;
	#endif
	
	return tex2D(grabTex, 0.5 + 0.5 * screenPos.xy + UnpackNormal(tex2D(normalMap, TRANSFORM_TEX(texcoord, normalMap))).xy * intensity);
}

fixed4 refractionRT(sampler2D grabTex, float2 grabTex_UV, sampler2D normalMap, float4 normalMap_ST, float2 texcoord, float intensity)
{
	return tex2D(grabTex, grabTex_UV + UnpackNormal(tex2D(normalMap, TRANSFORM_TEX(texcoord, normalMap))).xy * intensity);
}

float sinTransform(float2 texcoord, float4 wave)
{
	const float Pi2 = 2 * 3.14159;
	float w = 0.5 * (sin(Pi2 * (wave.x * texcoord.x + wave.z * _Time.y)) + 1);
	
	return texcoord.y + ((wave.w > 0 && wave.w < 1) ? wave.y * pow(w, wave.w) : wave.y * w);
}

fixed4 sampleCombined(sampler2D topTex, sampler2D mainTex, float4 mainTex_ST, float2 texcoord, float trim)
{
	float2 coord = float2(texcoord.x * mainTex_ST.x * mainTex_ST.y, texcoord.y * mainTex_ST.x + (1 - mainTex_ST.x));
	
	if (texcoord.y > 1 - 1/mainTex_ST.x)
		return (coord.y > trim) ? fixed4(0,0,0,0) : tex2D(topTex, coord);
	else
		return tex2D(mainTex, coord);
}

#endif // UNITY_SPRITES_INCLUDED
