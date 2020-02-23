// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Myshaders/UI/Controls Layout"
{
	Properties
	{
		_ImageToBurn("Image To Burn", 2D) = "white" {}
		_Ramp("Ramp", 2D) = "white" {}
		_Dissolve("Dissolve", 2D) = "white" {}
		_DissolveAmount("Dissolve Amount", Range( 0 , 1)) = 0
		_Tiling("Tiling", Range( 0 , 2)) = 0.5
		_Offset("Offset", Range( 0 , 2)) = 0.5
		_DissolveRemapMinNew("Dissolve Remap Min New", Range( 0 , 1)) = 0
		_DissolveRemapMaxNew("Dissolve Remap Max New", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Background+0" "IsEmissive" = "true"  }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha , SrcAlpha OneMinusSrcAlpha
		
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _ImageToBurn;
		uniform float4 _ImageToBurn_ST;
		uniform sampler2D _Ramp;
		uniform float _DissolveAmount;
		uniform float _DissolveRemapMinNew;
		uniform float _DissolveRemapMaxNew;
		uniform sampler2D _Dissolve;
		uniform float _Tiling;
		uniform float _Offset;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_ImageToBurn = i.uv_texcoord * _ImageToBurn_ST.xy + _ImageToBurn_ST.zw;
			float2 appendResult50 = (float2(_Tiling , _Tiling));
			float2 appendResult71 = (float2(_Offset , _Offset));
			float2 uv_TexCoord52 = i.uv_texcoord * appendResult50 + appendResult71;
			float clampResult59 = clamp( (-4.0 + (( (-0.6 + (( 1.0 - (_DissolveRemapMinNew + (_DissolveAmount - 0.0) * (_DissolveRemapMaxNew - _DissolveRemapMinNew) / (1.0 - 0.0)) ) - 0.0) * (0.6 - -0.6) / (1.0 - 0.0)) + tex2D( _Dissolve, uv_TexCoord52 ).r ) - 0.0) * (4.0 - -4.0) / (1.0 - 0.0)) , 0.06 , 1.0 );
			float temp_output_60_0 = ( 1.0 - clampResult59 );
			float2 temp_cast_0 = (temp_output_60_0).xx;
			float temp_output_64_0 = saturate( temp_output_60_0 );
			o.Emission = ( tex2D( _ImageToBurn, uv_ImageToBurn ) * tex2D( _Ramp, temp_cast_0 ) * temp_output_64_0 ).rgb;
			o.Alpha = (0.0 + (temp_output_64_0 - 0.0) * (1.0 - 0.0) / (0.9 - 0.0));
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Unlit keepalpha fullforwardshadows 

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
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
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
202;73;564;651;403.1285;196.9871;1;False;False
Node;AmplifyShaderEditor.CommentaryNode;48;-2397.377,-50.6059;Float;False;1139.971;487.7531;Dissolve - Opacity Mask;13;67;57;55;54;53;52;51;50;49;70;71;72;73;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;49;-2382.265,239.3108;Float;False;Property;_Tiling;Tiling;5;0;Create;True;0;0;False;0;0.5;0.26;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;70;-2382.557,324.6834;Float;False;Property;_Offset;Offset;6;0;Create;True;0;0;False;0;0.5;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;51;-2382.557,20.68302;Float;False;Property;_DissolveAmount;Dissolve Amount;4;0;Create;True;0;0;False;0;0;1.002688;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;72;-2382.557,84.6832;Float;False;Property;_DissolveRemapMinNew;Dissolve Remap Min New;7;0;Create;True;0;0;False;0;0;0.03;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;73;-2382.557,148.6834;Float;False;Property;_DissolveRemapMaxNew;Dissolve Remap Max New;8;0;Create;True;0;0;False;0;0;0.7;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;71;-2110.557,324.6834;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;50;-2110.264,239.3108;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TFHCRemapNode;67;-2062.557,20.68302;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;53;-1888,16;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;52;-1982.265,239.3108;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;55;-1728,16;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-0.6;False;4;FLOAT;0.6;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;54;-1755.823,238.26;Float;True;Property;_Dissolve;Dissolve;3;0;Create;True;0;0;False;0;None;aaeea63170594744db4273ab27cac4f4;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;56;-1245.05,-52.27406;Float;False;866.597;387.2368;Burn Effect - Emission;7;87;62;60;59;82;58;61;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;57;-1408,240;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;58;-1214.649,138.078;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-4;False;4;FLOAT;4;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;59;-1003.892,188.9814;Float;False;3;0;FLOAT;0;False;1;FLOAT;0.06;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;60;-859.8922,236.9816;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;82;-689.3443,283.6826;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;87;-416.7139,285.4158;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;61;-670.6492,106.078;Float;True;Property;_Ramp;Ramp;2;0;Create;True;0;0;False;0;None;8239ead513c4cbb4a804df044f0ff074;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;1,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;62;-1003.892,-3.018613;Float;True;Property;_ImageToBurn;Image To Burn;1;0;Create;True;0;0;False;0;None;42fade425817e9e41983b3c71950f6a1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;64;-352,256;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;-144,0;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;88;-189.1285,255.0129;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.9;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;2;16,-48;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;Myshaders/UI/Controls Layout;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Transparent;;Background;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;2;5;False;-1;10;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;True;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;71;0;70;0
WireConnection;71;1;70;0
WireConnection;50;0;49;0
WireConnection;50;1;49;0
WireConnection;67;0;51;0
WireConnection;67;3;72;0
WireConnection;67;4;73;0
WireConnection;53;0;67;0
WireConnection;52;0;50;0
WireConnection;52;1;71;0
WireConnection;55;0;53;0
WireConnection;54;1;52;0
WireConnection;57;0;55;0
WireConnection;57;1;54;1
WireConnection;58;0;57;0
WireConnection;59;0;58;0
WireConnection;60;0;59;0
WireConnection;82;0;60;0
WireConnection;87;0;82;0
WireConnection;61;1;60;0
WireConnection;64;0;87;0
WireConnection;68;0;62;0
WireConnection;68;1;61;0
WireConnection;68;2;64;0
WireConnection;88;0;64;0
WireConnection;2;2;68;0
WireConnection;2;9;88;0
ASEEND*/
//CHKSM=85E128C16C571D765BD9935F03718080811EFA75