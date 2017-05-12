/*
Stereoscopic Shader for Innovator

CyberCANOE Virtual Reality API for Unity3D
(C) 2016 Ryan Theriot, Jason Leigh, Laboratory for Advanced Visualization & Applications, University of Hawaii at Manoa.
Version: 1.3, May 12th, 2017.
*/

Shader "CC_Shaders/CC_StereoShaderInnovator"
{
	Properties{ }

	SubShader
	{
		Pass
		{

			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"

			uniform float InterlaceValue;
			uniform sampler2D LeftTex;
			uniform sampler2D RightTex;

			float4 frag(v2f_img IN) : COLOR0
			{

				if (fmod(IN.uv.y * InterlaceValue, 2.0) < 1.0)
				{
					return tex2D(LeftTex, IN.uv);
				}
				else 
				{
					return tex2D(RightTex, IN.uv);
				}
			}

			ENDCG
		}
	}

	Fallback off

}
