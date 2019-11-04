// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "N_Shaders/MagicSphere"
{
	Properties
	{
		_EdgeLength ( "Edge length", Range( 2, 50 ) ) = 15
		_Power("Power", Range( 0 , 1)) = 0
		_COLOR_01("COLOR_01", Color) = (0,0.08320045,1,0)
		_NoiseMapStrength("NoiseMapStrength", Range( 0 , 1)) = 0
		_EmissiveIntensity("EmissiveIntensity", Range( 0 , 1)) = 0
		_RingPannerSpeed("RingPannerSpeed", Vector) = (0,0,0,0)
		_Coordinates_Value("Coordinates_Value", Range( 0 , 5)) = 0
		_NoiseMapSize("NoiseMapSize", Vector) = (512,512,0,0)
		_Bump("Bump", 2D) = "white" {}
		_NoiseMapPannerSpeed("NoiseMapPannerSpeed", Vector) = (0,0,0,0)
		_Speed("Speed", Range( 0 , 1)) = 0
		_ScrollSpeed("ScrollSpeed", Range( 0 , 1)) = 0
		_COLOR_02("COLOR_02", Color) = (1,1,1,0)
		_Texture0("Texture 0", 2D) = "white" {}
		_TextureSample1("Texture Sample 1", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "Tessellation.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 4.6
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			float2 uv_texcoord;
		};

		uniform float4 _COLOR_01;
		uniform float _Power;
		uniform float _EmissiveIntensity;
		uniform sampler2D _Bump;
		uniform float _ScrollSpeed;
		uniform float _Speed;
		uniform float _Coordinates_Value;
		uniform float4 _COLOR_02;
		uniform sampler2D _TextureSample1;
		uniform sampler2D _Texture0;
		uniform float2 _NoiseMapSize;
		uniform float2 _NoiseMapPannerSpeed;
		uniform float _NoiseMapStrength;
		uniform float2 _RingPannerSpeed;
		uniform float _EdgeLength;

		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			return UnityEdgeLengthBasedTess (v0.vertex, v1.vertex, v2.vertex, _EdgeLength);
		}

		void vertexDataFunc( inout appdata_full v )
		{
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV1 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode1 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV1, _Power ) );
			float2 temp_cast_0 = (_Speed).xx;
			float2 panner18 = ( ( _Time.y * _ScrollSpeed ) * temp_cast_0 + float2( 0,0 ));
			float2 uv_TexCoord6 = i.uv_texcoord + panner18;
			float4 tex2DNode9 = tex2D( _Bump, ( uv_TexCoord6 * _Coordinates_Value ) );
			float4 lerpResult10 = lerp( ( _COLOR_01 * fresnelNode1 * _EmissiveIntensity ) , ( tex2DNode9 * _COLOR_02 ) , tex2DNode9);
			float2 temp_output_1_0_g3 = _NoiseMapSize;
			float2 appendResult10_g3 = (float2(( (temp_output_1_0_g3).x * i.uv_texcoord.x ) , ( i.uv_texcoord.y * (temp_output_1_0_g3).y )));
			float2 temp_output_11_0_g3 = _NoiseMapPannerSpeed;
			float2 panner18_g3 = ( ( (temp_output_11_0_g3).x * _Time.y ) * float2( 1,0 ) + i.uv_texcoord);
			float2 panner19_g3 = ( ( _Time.y * (temp_output_11_0_g3).y ) * float2( 0,1 ) + i.uv_texcoord);
			float2 appendResult24_g3 = (float2((panner18_g3).x , (panner19_g3).y));
			float2 temp_output_47_0_g3 = _RingPannerSpeed;
			float2 uv_TexCoord78_g3 = i.uv_texcoord * float2( 2,2 );
			float2 temp_output_31_0_g3 = ( uv_TexCoord78_g3 - float2( 1,1 ) );
			float2 appendResult39_g3 = (float2(frac( ( atan2( (temp_output_31_0_g3).x , (temp_output_31_0_g3).y ) / 6.28318548202515 ) ) , length( temp_output_31_0_g3 )));
			float2 panner54_g3 = ( ( (temp_output_47_0_g3).x * _Time.y ) * float2( 1,0 ) + appendResult39_g3);
			float2 panner55_g3 = ( ( _Time.y * (temp_output_47_0_g3).y ) * float2( 0,1 ) + appendResult39_g3);
			float2 appendResult58_g3 = (float2((panner54_g3).x , (panner55_g3).y));
			float4 color68 = IsGammaSpace() ? float4(0.5,0.1601067,0.1108491,0) : float4(0.2140411,0.02200714,0.01178977,0);
			o.Emission = ( lerpResult10 * ( ( 1.0 - tex2D( _TextureSample1, ( ( (tex2D( _Texture0, ( appendResult10_g3 + appendResult24_g3 ) )).rg * _NoiseMapStrength ) + ( float2( -2,2 ) * appendResult58_g3 ) ) ) ) + color68 ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows vertex:vertexDataFunc tessellate:tessFunction 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.6
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
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float3 worldNormal : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
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
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17000
332;73;706;646;1590.11;347.2828;1.808421;True;False
Node;AmplifyShaderEditor.RangedFloatNode;21;-2862.312,162.8943;Float;False;Property;_ScrollSpeed;ScrollSpeed;15;0;Create;True;0;0;False;0;0;0.441959;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;20;-2833.771,14.18519;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-2673.044,-59.41831;Float;False;Property;_Speed;Speed;14;0;Create;True;0;0;False;0;0;0.5671425;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;-2537.854,122.3372;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;18;-2325.802,-39.8908;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;64;-1806.289,304.5323;Float;False;Property;_NoiseMapSize;NoiseMapSize;11;0;Create;True;0;0;False;0;512,512;5,5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;61;-1847.655,433.839;Float;False;Property;_NoiseMapPannerSpeed;NoiseMapPannerSpeed;13;0;Create;True;0;0;False;0;0,0;1,-1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;7;-2046.595,-44.85788;Float;False;Property;_Coordinates_Value;Coordinates_Value;10;0;Create;True;0;0;False;0;0;0;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;6;-2052.267,-180.6066;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;65;-1837.327,555.3633;Float;False;Property;_NoiseMapStrength;NoiseMapStrength;7;0;Create;True;0;0;False;0;0;0.4141487;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;62;-1823.114,820.2756;Float;False;Property;_RingPannerSpeed;RingPannerSpeed;9;0;Create;True;0;0;False;0;0,0;1,-1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;63;-1816.228,639.6533;Float;False;Constant;_Vector2;Vector 2;7;0;Create;True;0;0;False;0;-2,2;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TexturePropertyNode;60;-1822.122,109.2846;Float;True;Property;_Texture0;Texture 0;17;0;Create;True;0;0;False;0;None;a29f600386831914fa2f2b7b0cf78a5b;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;2;-1876.129,-414.9111;Float;False;Property;_Power;Power;5;0;Create;True;0;0;False;0;0;0.7677189;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-1685.975,-104.9036;Float;True;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;58;-1404.187,368.5874;Float;True;RadialUVDistortion;-1;;3;051d65e7699b41a4c800363fd0e822b2;0;7;60;SAMPLER2D;0.0;False;1;FLOAT2;1,1;False;11;FLOAT2;0,0;False;65;FLOAT;1;False;68;FLOAT2;1,1;False;47;FLOAT2;1,1;False;29;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;3;-1424.128,-727.9111;Float;False;Property;_COLOR_01;COLOR_01;6;0;Create;True;0;0;False;0;0,0.08320045,1,0;0,0.08320036,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;9;-1431.46,-171.019;Float;True;Property;_Bump;Bump;12;0;Create;True;0;0;False;0;179c8e1059e91e84bae3f32c3d5d178f;62539d06374c5e04795ab6e69e466531;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FresnelNode;1;-1547.066,-535.2601;Float;True;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;66;-948.6677,252.6844;Float;True;Property;_TextureSample1;Texture Sample 1;18;0;Create;True;0;0;False;0;None;1d7055aa4cb811f42a8663b78ec1b57c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;5;-1428.935,-301.2393;Float;False;Property;_EmissiveIntensity;EmissiveIntensity;8;0;Create;True;0;0;False;0;0;0.8221716;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;55;-1279.424,99.10374;Float;False;Property;_COLOR_02;COLOR_02;16;0;Create;True;0;0;False;0;1,1,1,0;0.1251334,0.6211949,0.7169812,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;-992.0493,-45.77247;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-1034.072,-536.3149;Float;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;70;-642.7999,262.0841;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;68;-765.7579,518.6323;Float;False;Constant;_COLOR_05;COLOR_05;14;0;Create;True;0;0;False;0;0.5,0.1601067,0.1108491,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;74;-448.565,306.1288;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;10;-655.8247,-190.2144;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;67;-282.1445,-141.7839;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2,0;Float;False;True;6;Float;ASEMaterialInspector;0;0;Standard;N_Shaders/MagicSphere;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;0;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;22;0;20;0
WireConnection;22;1;21;0
WireConnection;18;2;19;0
WireConnection;18;1;22;0
WireConnection;6;1;18;0
WireConnection;8;0;6;0
WireConnection;8;1;7;0
WireConnection;58;60;60;0
WireConnection;58;1;64;0
WireConnection;58;11;61;0
WireConnection;58;65;65;0
WireConnection;58;68;63;0
WireConnection;58;47;62;0
WireConnection;9;1;8;0
WireConnection;1;3;2;0
WireConnection;66;1;58;0
WireConnection;56;0;9;0
WireConnection;56;1;55;0
WireConnection;4;0;3;0
WireConnection;4;1;1;0
WireConnection;4;2;5;0
WireConnection;70;0;66;0
WireConnection;74;0;70;0
WireConnection;74;1;68;0
WireConnection;10;0;4;0
WireConnection;10;1;56;0
WireConnection;10;2;9;0
WireConnection;67;0;10;0
WireConnection;67;1;74;0
WireConnection;0;2;67;0
ASEEND*/
//CHKSM=E9F25AC45534B3B586C17E9FE0860B8FF782467C