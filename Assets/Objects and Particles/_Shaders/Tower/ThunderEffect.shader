// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "N_Shaders/ThunderEffect"
{
	Properties
	{
		_Speed("Speed", Range( 0 , 1)) = 0
		_Power("Power", Range( 0 , 1)) = 0
		_Tessellation("Tessellation", Range( 0 , 5)) = 0
		_Thunder_COLOR("Thunder_COLOR", Color) = (0.5115387,0.2896938,0.5849056,0)
		_Thunder_01("Thunder_01", 2D) = "white" {}
		_Thunder_02("Thunder_02", 2D) = "white" {}
		_PowerTime("PowerTime", Range( 0 , 2)) = 0
		_Scale_Time("Scale_Time", Range( 0 , 4)) = 0
		_Waves_Height("Waves_Height", 2D) = "white" {}
		_Float6("Float 6", Range( 0 , 5)) = 5
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
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
		};

		uniform sampler2D _Waves_Height;
		uniform float4 _Waves_Height_ST;
		uniform float _Float6;
		uniform float4 _Thunder_COLOR;
		uniform sampler2D _Thunder_01;
		uniform float4 _Thunder_01_ST;
		uniform sampler2D _Thunder_02;
		uniform float4 _Thunder_02_ST;
		uniform float _PowerTime;
		uniform float _Power;
		uniform float _Scale_Time;
		uniform float _Speed;
		uniform float _Tessellation;

		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			float4 temp_cast_6 = (_Tessellation).xxxx;
			return temp_cast_6;
		}

		void vertexDataFunc( inout appdata_full v )
		{
			float3 ase_vertexNormal = v.normal.xyz;
			float2 uv_Waves_Height = v.texcoord * _Waves_Height_ST.xy + _Waves_Height_ST.zw;
			float2 temp_cast_1 = (0.53).xx;
			float2 temp_cast_2 = (0.14).xx;
			float2 uv_TexCoord71 = v.texcoord.xy * temp_cast_1 + temp_cast_2;
			float2 temp_cast_3 = (0.88).xx;
			float cos67 = cos( _Time.y );
			float sin67 = sin( _Time.y );
			float2 rotator67 = mul( uv_TexCoord71 - temp_cast_3 , float2x2( cos67 , -sin67 , sin67 , cos67 )) + temp_cast_3;
			v.vertex.xyz += ( float4( ase_vertexNormal , 0.0 ) * ( tex2Dlod( _Waves_Height, float4( uv_Waves_Height, 0, 0.0) ) * _Float6 ) * float4( rotator67, 0.0 , 0.0 ) * 0.1 ).rgb;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Thunder_01 = i.uv_texcoord * _Thunder_01_ST.xy + _Thunder_01_ST.zw;
			float2 uv_Thunder_02 = i.uv_texcoord * _Thunder_02_ST.xy + _Thunder_02_ST.zw;
			float4 lerpResult16 = lerp( tex2D( _Thunder_01, uv_Thunder_01 ) , tex2D( _Thunder_02, uv_Thunder_02 ) , saturate( ( (-2.0 + (exp( sin( ( _Time.y * 5.0 ) ) ) - 0.0) * (2.0 - -2.0) / (1.0 - 0.0)) * _PowerTime ) ));
			float4 color13 = IsGammaSpace() ? float4(0.2761192,0,1,0) : float4(0.06196645,0,1,0);
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV11 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode11 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV11, (1.0 + (sin( ( _Time.y * _Power ) ) - 0.0) * (1.5 - 1.0) / (1.0 - 0.0)) ) );
			float2 temp_cast_0 = (_Speed).xx;
			float2 panner4 = ( sin( ( _Time.y * _Scale_Time ) ) * temp_cast_0 + float2( 0,0 ));
			float4 lerpResult52 = lerp( ( color13 * fresnelNode11 * 0.437474 ) , float4( 0,0,0,0 ) , float4( panner4, 0.0 , 0.0 ));
			o.Emission = ( ( _Thunder_COLOR * lerpResult16 ) + lerpResult52 ).rgb;
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
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
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
Version=16900
575;266;1235;753;1606.381;3519.277;3.508987;True;False
Node;AmplifyShaderEditor.RangedFloatNode;42;-1810.464,-1496.567;Float;False;Constant;_Scale;Scale;7;0;Create;True;0;0;False;0;5;0;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;45;-1718.669,-1655.137;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;46;-1530.936,-1579.363;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;44;-1368.393,-1578.877;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-1298.065,-267.5347;Float;False;Property;_Power;Power;1;0;Create;True;0;0;False;0;0;0.731;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;59;-1230.134,-443.8623;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ExpOpNode;47;-1179.1,-1565.135;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-1063.565,-1299.504;Float;False;Constant;_Float1;Float 1;7;0;Create;True;0;0;False;0;-2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;-958.6831,-370.4873;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-1059.149,-1185.889;Float;False;Constant;_Float0;Float 0;7;0;Create;True;0;0;False;0;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;29;-849.6352,-1384.385;Float;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;60;-754.8796,-281.6272;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;49;-693.1775,-917.2402;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;37;-799.6234,-1160.313;Float;False;Property;_PowerTime;PowerTime;6;0;Create;True;0;0;False;0;0;1.102;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;50;-931.825,-798.5845;Float;False;Property;_Scale_Time;Scale_Time;7;0;Create;True;0;0;False;0;0;3;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;56;-814.7501,-154.8777;Float;False;Constant;_Float2;Float 2;7;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;57;-810.246,-46.80079;Float;False;Constant;_Float3;Float 3;7;0;Create;True;0;0;False;0;1.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-398.7226,-1350.257;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;55;-616.905,-218.7676;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-503.9814,-872.8156;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-395.165,-1035.094;Float;False;Property;_Speed;Speed;0;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;83;355.5975,-2374.12;Float;False;Constant;_Float7;Float 7;10;0;Create;True;0;0;False;0;0.53;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;51;-227.092,-861.4078;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;72;389.0342,-2264.343;Float;False;Constant;_Float4;Float 4;8;0;Create;True;0;0;False;0;0.14;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;11;-360.5004,-344.6134;Float;True;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;39;-147.7246,-1453.907;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;13;-672.9836,-587.8264;Float;False;Constant;_COLOR_01;COLOR_01;5;0;Create;True;0;0;False;0;0.2761192,0,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;17;-668.2092,-1810.077;Float;True;Property;_Thunder_02;Thunder_02;5;0;Create;True;0;0;False;0;86c775c92700fb74bbf0caf655318598;8804ac111ca8c9844bb1a9ca7b98fe51;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;15;-224.4528,-76.08578;Float;False;Constant;_Intensity_Emmission;Intensity_Emmission;5;0;Create;True;0;0;False;0;0.437474;0;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;10;-661.3378,-2030.645;Float;True;Property;_Thunder_01;Thunder_01;4;0;Create;True;0;0;False;0;2818d1cfa9c5eed4b9ffd0d404ab4311;54794fc18fdc2fe4e82ab4048aab642c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;94.87674,-567.2803;Float;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;73;649.0679,-2112.225;Float;False;Constant;_Float5;Float 5;8;0;Create;True;0;0;False;0;0.88;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;71;595.2427,-2389.401;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;79;265.7248,-2880.151;Float;True;Property;_Waves_Height;Waves_Height;8;0;Create;True;0;0;False;0;a29f600386831914fa2f2b7b0cf78a5b;a29f600386831914fa2f2b7b0cf78a5b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;16;27.68574,-1668.265;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleTimeNode;74;654.3954,-2001.389;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;80;306.713,-2569.357;Float;False;Property;_Float6;Float 6;9;0;Create;True;0;0;False;0;5;1.427325;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;23;165.5647,-1920.413;Float;False;Property;_Thunder_COLOR;Thunder_COLOR;3;0;Create;True;0;0;False;0;0.5115387,0.2896938,0.5849056,0;0.7466669,0,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;4;268.0132,-1119.772;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NormalVertexDataNode;62;847.9304,-2850.549;Float;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;52;679.7138,-1009.969;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;618.0529,-1724.717;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RotatorNode;67;931.9769,-2176.792;Float;True;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;81;639.4924,-2651.489;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;66;929.5194,-2450.933;Float;False;Constant;_NoiseEffect;NoiseEffect;7;0;Create;True;0;0;False;0;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;24;1106.149,-1364.08;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;61;1172.192,-1619.718;Float;False;Property;_Tessellation;Tessellation;2;0;Create;True;0;0;False;0;0;2.317972;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;63;1183.105,-2537.297;Float;True;4;4;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT2;0,0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1698.679,-1920.937;Float;False;True;6;Float;ASEMaterialInspector;0;0;Standard;N_Shaders/ThunderEffect;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;46;0;45;0
WireConnection;46;1;42;0
WireConnection;44;0;46;0
WireConnection;47;0;44;0
WireConnection;58;0;59;0
WireConnection;58;1;12;0
WireConnection;29;0;47;0
WireConnection;29;3;32;0
WireConnection;29;4;31;0
WireConnection;60;0;58;0
WireConnection;38;0;29;0
WireConnection;38;1;37;0
WireConnection;55;0;60;0
WireConnection;55;3;56;0
WireConnection;55;4;57;0
WireConnection;8;0;49;0
WireConnection;8;1;50;0
WireConnection;51;0;8;0
WireConnection;11;3;55;0
WireConnection;39;0;38;0
WireConnection;14;0;13;0
WireConnection;14;1;11;0
WireConnection;14;2;15;0
WireConnection;71;0;83;0
WireConnection;71;1;72;0
WireConnection;16;0;10;0
WireConnection;16;1;17;0
WireConnection;16;2;39;0
WireConnection;4;2;5;0
WireConnection;4;1;51;0
WireConnection;52;0;14;0
WireConnection;52;2;4;0
WireConnection;22;0;23;0
WireConnection;22;1;16;0
WireConnection;67;0;71;0
WireConnection;67;1;73;0
WireConnection;67;2;74;0
WireConnection;81;0;79;0
WireConnection;81;1;80;0
WireConnection;24;0;22;0
WireConnection;24;1;52;0
WireConnection;63;0;62;0
WireConnection;63;1;81;0
WireConnection;63;2;67;0
WireConnection;63;3;66;0
WireConnection;0;2;24;0
WireConnection;0;11;63;0
WireConnection;0;14;61;0
ASEEND*/
//CHKSM=E17C07882270D6AC56922F58E63D08EDBF6F6FA7