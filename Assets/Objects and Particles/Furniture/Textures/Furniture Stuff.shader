// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MyShaders/Furniture Stuff"
{
	Properties
	{
		_FurnitureStuff_AlbedoTR_2K("Furniture Stuff_AlbedoTR_2K", 2D) = "white" {}
		_FurnitureStuff_Normal_2K("Furniture Stuff_Normal_2K", 2D) = "bump" {}
		_NormalScale("Normal Scale", Range( 0 , 1)) = 0
		_FurnitureStuff_Emission_2K("Furniture Stuff_Emission_2K", 2D) = "white" {}
		_EmissionIntensity("Emission Intensity", Range( 0 , 5)) = 0
		_FurnitureStuff_AO_2K("Furniture Stuff_AO_2K", 2D) = "white" {}
		_FurnitureStuff_MetallicSM_2K("Furniture Stuff_MetallicSM_2K", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _NormalScale;
		uniform sampler2D _FurnitureStuff_Normal_2K;
		uniform float4 _FurnitureStuff_Normal_2K_ST;
		uniform sampler2D _FurnitureStuff_AlbedoTR_2K;
		uniform float4 _FurnitureStuff_AlbedoTR_2K_ST;
		uniform float _EmissionIntensity;
		uniform sampler2D _FurnitureStuff_Emission_2K;
		uniform float4 _FurnitureStuff_Emission_2K_ST;
		uniform sampler2D _FurnitureStuff_MetallicSM_2K;
		uniform float4 _FurnitureStuff_MetallicSM_2K_ST;
		uniform sampler2D _FurnitureStuff_AO_2K;
		uniform float4 _FurnitureStuff_AO_2K_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_FurnitureStuff_Normal_2K = i.uv_texcoord * _FurnitureStuff_Normal_2K_ST.xy + _FurnitureStuff_Normal_2K_ST.zw;
			o.Normal = UnpackScaleNormal( tex2D( _FurnitureStuff_Normal_2K, uv_FurnitureStuff_Normal_2K ), _NormalScale );
			float2 uv_FurnitureStuff_AlbedoTR_2K = i.uv_texcoord * _FurnitureStuff_AlbedoTR_2K_ST.xy + _FurnitureStuff_AlbedoTR_2K_ST.zw;
			float4 tex2DNode3 = tex2D( _FurnitureStuff_AlbedoTR_2K, uv_FurnitureStuff_AlbedoTR_2K );
			o.Albedo = tex2DNode3.rgb;
			float2 uv_FurnitureStuff_Emission_2K = i.uv_texcoord * _FurnitureStuff_Emission_2K_ST.xy + _FurnitureStuff_Emission_2K_ST.zw;
			float4 temp_output_11_0 = ( _EmissionIntensity * tex2D( _FurnitureStuff_Emission_2K, uv_FurnitureStuff_Emission_2K ) );
			o.Emission = ( ( tex2DNode3 * temp_output_11_0 ) + temp_output_11_0 ).rgb;
			float2 uv_FurnitureStuff_MetallicSM_2K = i.uv_texcoord * _FurnitureStuff_MetallicSM_2K_ST.xy + _FurnitureStuff_MetallicSM_2K_ST.zw;
			o.Metallic = tex2D( _FurnitureStuff_MetallicSM_2K, uv_FurnitureStuff_MetallicSM_2K ).r;
			float2 uv_FurnitureStuff_AO_2K = i.uv_texcoord * _FurnitureStuff_AO_2K_ST.xy + _FurnitureStuff_AO_2K_ST.zw;
			o.Occlusion = tex2D( _FurnitureStuff_AO_2K, uv_FurnitureStuff_AO_2K ).r;
			o.Alpha = tex2DNode3.a;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16900
1020;73;621;656;986.3232;514.3262;2.793665;False;False
Node;AmplifyShaderEditor.SamplerNode;5;-384,368;Float;True;Property;_FurnitureStuff_Emission_2K;Furniture Stuff_Emission_2K;4;0;Create;True;0;0;False;0;2a325b8b142c32e4f9645cb55feea609;2a325b8b142c32e4f9645cb55feea609;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;10;-645.3938,313.2578;Float;False;Property;_EmissionIntensity;Emission Intensity;5;0;Create;True;0;0;False;0;0;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;3;-384,-16;Float;True;Property;_FurnitureStuff_AlbedoTR_2K;Furniture Stuff_AlbedoTR_2K;1;0;Create;True;0;0;False;0;a1cbfb89eec76a9458a9eb8a38c5ed45;a1cbfb89eec76a9458a9eb8a38c5ed45;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-84.78748,317.3031;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-656,224;Float;False;Property;_NormalScale;Normal Scale;3;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;8;-384,560;Float;True;Property;_FurnitureStuff_MetallicSM_2K;Furniture Stuff_MetallicSM_2K;7;0;Create;True;0;0;False;0;37ce2728db75e7341aa3a847e1dec340;37ce2728db75e7341aa3a847e1dec340;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;-384,752;Float;True;Property;_FurnitureStuff_AO_2K;Furniture Stuff_AO_2K;6;0;Create;True;0;0;False;0;ac90404453047e4408009914cc4ffd8b;ac90404453047e4408009914cc4ffd8b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;48,240;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;7;-384,176;Float;True;Property;_FurnitureStuff_Normal_2K;Furniture Stuff_Normal_2K;2;0;Create;True;0;0;False;0;e69a58399f06f2149b9ee29a3b61c16f;e69a58399f06f2149b9ee29a3b61c16f;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;16;189.4,297.0999;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;17;323.0571,239.0386;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;18;317.0572,342.2386;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;19;338.8568,364.0385;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;416,160;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;MyShaders/Furniture Stuff;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;3;Custom;0.5;True;True;0;False;Transparent;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;5;False;-1;10;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;11;0;10;0
WireConnection;11;1;5;0
WireConnection;15;0;3;0
WireConnection;15;1;11;0
WireConnection;7;5;9;0
WireConnection;16;0;15;0
WireConnection;16;1;11;0
WireConnection;17;0;3;4
WireConnection;18;0;8;1
WireConnection;19;0;4;1
WireConnection;0;0;3;0
WireConnection;0;1;7;0
WireConnection;0;2;16;0
WireConnection;0;3;18;0
WireConnection;0;5;19;0
WireConnection;0;9;17;0
ASEEND*/
//CHKSM=ED9E33B39FAE7267D51BD603229BD20B1FED5E78