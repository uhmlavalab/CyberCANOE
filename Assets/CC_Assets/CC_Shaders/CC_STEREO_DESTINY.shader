/*
Stereoscopic Shader for Destiny

CyberCANOE Virtual Reality API for Unity3D
(C) 2016 Ryan Theriot, Jason Leigh, Laboratory for Advanced Visualization & Applications, University of Hawaii at Manoa.
Version: October 26th, 2016.
*/

Shader "CC_Shaders/CC_StereoShaderDestiny" {

	Properties{ }

	SubShader
	{
		//Stereo
		Pass 
		{
			ZTest Always Cull off ZWrite off

			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"


			uniform float resX;
			uniform float resY;
			uniform sampler2D leftTopLeft;
			uniform sampler2D leftTopRight;
			uniform sampler2D leftBottomLeft;
			uniform sampler2D leftBottomRight;
			uniform sampler2D rightTopLeft;
			uniform sampler2D rightTopRight;
			uniform sampler2D rightBottomLeft;
			uniform sampler2D rightBottomRight;

			float4 frag(v2f_img IN) : COLOR0
			{

				if (fmod(IN.uv.x * resX, 2.0) < 1.0)
				{
					if (IN.uv.x <= 0.5 && IN.uv.y > 0.5) 
					{
						float2 uvCoord;
						uvCoord.x = IN.uv.x * 2;
						uvCoord.y = (IN.uv.y - 0.5f) * 2;
						return tex2D(rightTopLeft, uvCoord);
					}
					else if (IN.uv.x <= 0.5 && IN.uv.y <= 0.5)
					{
						float2 uvCoord;
						uvCoord.x = IN.uv.x * 2;
						uvCoord.y = IN.uv.y * 2;
						return tex2D(rightBottomLeft, uvCoord);
					}
					else if (IN.uv.x > 0.5 && IN.uv.y >= 0.5)
					{
						float2 uvCoord;
						uvCoord.x = (IN.uv.x - 0.5f) * 2;
						uvCoord.y = (IN.uv.y - 0.5f) * 2;
						return tex2D(rightTopRight, uvCoord);
					}
					else 
					{
						float2 uvCoord;
						uvCoord.x = (IN.uv.x - 0.5f) * 2;
						uvCoord.y = IN.uv.y * 2;
						return tex2D(rightBottomRight, uvCoord);
					}

				}
				else 
				{
					if (IN.uv.x <= 0.5 && IN.uv.y > 0.5)
					{
						float2 uvCoord;
						uvCoord.x = IN.uv.x * 2;
						uvCoord.y = (IN.uv.y - 0.5f) * 2;
						return tex2D(leftTopLeft, uvCoord);
					}
					else if (IN.uv.x <= 0.5 && IN.uv.y <= 0.5)
					{
						float2 uvCoord;
						uvCoord.x = IN.uv.x * 2;
						uvCoord.y = IN.uv.y * 2;
						return tex2D(leftBottomLeft, uvCoord);
					}
					else if (IN.uv.x > 0.5 && IN.uv.y > 0.5)
					{
						float2 uvCoord;
						uvCoord.x = (IN.uv.x - 0.5f) * 2;
						uvCoord.y = (IN.uv.y - 0.5f) * 2;
						return tex2D(leftTopRight, uvCoord);
					}
					else
					{
						float2 uvCoord;
						uvCoord.x = (IN.uv.x - 0.5f) * 2;
						uvCoord.y = IN.uv.y * 2;
						return tex2D(leftBottomRight, uvCoord);
					}
				}
			}
		    ENDCG
		}

	//Non Stereo
		Pass
		{
			ZTest Always Cull off ZWrite off

			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"

			uniform float resX;
			uniform float resY;
			uniform sampler2D centerTopLeft;
			uniform sampler2D centerTopRight;
			uniform sampler2D centerBottomLeft;
			uniform sampler2D centerBottomRight;

			float4 frag(v2f_img IN) : COLOR0
			{

				if (IN.uv.x <= 0.5 && IN.uv.y > 0.5) 
				{
					float2 uvCoord;
					uvCoord.x = IN.uv.x  * 2;
					uvCoord.y = (IN.uv.y - 0.5f) * 2;
					return tex2D(centerTopLeft, uvCoord);
				}
				else if (IN.uv.x <= 0.5 && IN.uv.y <= 0.5)
				{
					float2 uvCoord;
					uvCoord.x = IN.uv.x * 2;
					uvCoord.y = IN.uv.y * 2;
					return tex2D(centerBottomLeft, uvCoord);
				}
				else if (IN.uv.x > 0.5 && IN.uv.y >= 0.5)
				{
					float2 uvCoord;
					uvCoord.x = (IN.uv.x - 0.5f) * 2;
					uvCoord.y = (IN.uv.y - 0.5f) * 2;
					return tex2D(centerTopRight, uvCoord);
				}
				else 
				{
					float2 uvCoord;
					uvCoord.x = (IN.uv.x - 0.5f) * 2;
					uvCoord.y = IN.uv.y * 2;
					return tex2D(centerBottomRight, uvCoord);
				}

			}
			ENDCG
		}
	}

	Fallback off

}
