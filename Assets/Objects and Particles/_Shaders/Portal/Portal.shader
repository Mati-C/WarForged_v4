// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MyShaders/Portal"
{
	Properties
	{
		_EdgeLength ( "Edge length", Range( 2, 50 ) ) = 35
		_NoiseMapStrength("NoiseMapStrength", Range( 0 , 1)) = 0
		_NoiseLines("Noise Lines", 2D) = "white" {}
		_NoiseBreak("Noise Break", 2D) = "white" {}
		_Noise("Noise", 2D) = "white" {}
		_PortalNoise("Portal Noise", 2D) = "white" {}
		_Flowmap("Flowmap", 2D) = "white" {}
		_PortalNoiseColor1("Portal Noise Color 1", Color) = (0.4481132,0.4935628,1,0)
		_PortalNoiseColor2("Portal Noise Color 2", Color) = (0.06711894,0,0.2735849,0)
		_PortalLinesColor("Portal Lines Color", Color) = (0,0,0,0)
		_EmissionMultiplier("Emission Multiplier", Float) = 0
		_Opacity("Opacity", Range( 0 , 1)) = 0
		_MaxOpacity("Max Opacity", Float) = 0
		_NoiseBaseWhite("Noise Base White", Range( 0 , 1)) = 0
		_NoiseAmount("Noise Amount", Float) = 2.54
		_NoiseMapSize("NoiseMapSize", Range( 0.5 , 2)) = 0
		_NoiseMapPannerSpeed("NoiseMapPannerSpeed", Range( 0 , 0.5)) = 0.1
		_DepthFadeDistance("Depth Fade Distance", Float) = 0
		_DepthFadeGradient("Depth Fade Gradient", Float) = 0
		_OpacityDepthFadeDistance("Opacity Depth Fade Distance", Range( 0 , 1)) = 0.001
		_PannerSpeed("Panner Speed", Range( 0 , 1)) = 0
		_RingPannerSpeed("RingPannerSpeed", Float) = 0
		_TesselAmount("Tessel Amount", Range( -0.3 , 0.3)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Tessellation.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 4.6
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPos;
		};

		uniform float4 _PortalNoiseColor1;
		uniform float4 _PortalNoiseColor2;
		uniform sampler2D _PortalNoise;
		uniform sampler2D _Flowmap;
		uniform float _PannerSpeed;
		uniform float _TesselAmount;
		uniform float4 _PortalLinesColor;
		uniform float _NoiseBaseWhite;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _DepthFadeDistance;
		uniform float _DepthFadeGradient;
		uniform sampler2D _NoiseLines;
		uniform sampler2D _Noise;
		uniform float _NoiseMapSize;
		uniform float _NoiseMapPannerSpeed;
		uniform float _NoiseMapStrength;
		uniform float _RingPannerSpeed;
		uniform sampler2D _NoiseBreak;
		uniform float _NoiseAmount;
		uniform float _EmissionMultiplier;
		uniform float _Opacity;
		uniform float _OpacityDepthFadeDistance;
		uniform float _MaxOpacity;
		uniform float _EdgeLength;

		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			return UnityEdgeLengthBasedTess (v0.vertex, v1.vertex, v2.vertex, _EdgeLength);
		}

		void vertexDataFunc( inout appdata_full v )
		{
			float2 uv_TexCoord94 = v.texcoord.xy * float2( 2,2 );
			float2 appendResult98 = (float2(_PannerSpeed , _PannerSpeed));
			float2 panner93 = ( 1.0 * _Time.y * appendResult98 + uv_TexCoord94);
			float4 lerpResult95 = lerp( float4( uv_TexCoord94, 0.0 , 0.0 ) , tex2Dlod( _Flowmap, float4( panner93, 0, 0.0) ) , 0.3529412);
			float4 lerpResult88 = lerp( _PortalNoiseColor1 , _PortalNoiseColor2 , tex2Dlod( _PortalNoise, float4( lerpResult95.rg, 0, 0.0) ).r);
			float4 PortalNoiseColor139 = lerpResult88;
			float3 appendResult168 = (float3(0.0 , _TesselAmount , 0.0));
			float4 VertexOffset184 = ( PortalNoiseColor139 * float4( appendResult168 , 0.0 ) );
			v.vertex.xyz += VertexOffset184.rgb;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_TexCoord94 = i.uv_texcoord * float2( 2,2 );
			float2 appendResult98 = (float2(_PannerSpeed , _PannerSpeed));
			float2 panner93 = ( 1.0 * _Time.y * appendResult98 + uv_TexCoord94);
			float4 lerpResult95 = lerp( float4( uv_TexCoord94, 0.0 , 0.0 ) , tex2D( _Flowmap, panner93 ) , 0.3529412);
			float4 lerpResult88 = lerp( _PortalNoiseColor1 , _PortalNoiseColor2 , tex2D( _PortalNoise, lerpResult95.rg ).r);
			float4 PortalNoiseColor139 = lerpResult88;
			float4 PortalLinesColor140 = _PortalLinesColor;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth50 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture,UNITY_PROJ_COORD( ase_screenPos )));
			float distanceDepth50 = abs( ( screenDepth50 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _DepthFadeDistance ) );
			float2 appendResult35 = (float2(_NoiseMapSize , _NoiseMapSize));
			float2 temp_output_1_0_g1 = appendResult35;
			float2 appendResult10_g1 = (float2(( (temp_output_1_0_g1).x * i.uv_texcoord.x ) , ( i.uv_texcoord.y * (temp_output_1_0_g1).y )));
			float2 appendResult38 = (float2(_NoiseMapPannerSpeed , _NoiseMapPannerSpeed));
			float2 temp_output_11_0_g1 = appendResult38;
			float2 panner18_g1 = ( ( (temp_output_11_0_g1).x * _Time.y ) * float2( 1,0 ) + i.uv_texcoord);
			float2 panner19_g1 = ( ( _Time.y * (temp_output_11_0_g1).y ) * float2( 0,1 ) + i.uv_texcoord);
			float2 appendResult24_g1 = (float2((panner18_g1).x , (panner19_g1).y));
			float2 appendResult188 = (float2(-_RingPannerSpeed , _RingPannerSpeed));
			float2 temp_output_47_0_g1 = appendResult188;
			float2 uv_TexCoord78_g1 = i.uv_texcoord * float2( 2,2 );
			float2 temp_output_31_0_g1 = ( uv_TexCoord78_g1 - float2( 1,1 ) );
			float2 appendResult39_g1 = (float2(frac( ( atan2( (temp_output_31_0_g1).x , (temp_output_31_0_g1).y ) / 6.28318548202515 ) ) , length( temp_output_31_0_g1 )));
			float2 panner54_g1 = ( ( (temp_output_47_0_g1).x * _Time.y ) * float2( 1,0 ) + appendResult39_g1);
			float2 panner55_g1 = ( ( _Time.y * (temp_output_47_0_g1).y ) * float2( 0,1 ) + appendResult39_g1);
			float2 appendResult58_g1 = (float2((panner54_g1).x , (panner55_g1).y));
			float2 temp_output_27_0 = ( ( (tex2D( _Noise, ( appendResult10_g1 + appendResult24_g1 ) )).rg * _NoiseMapStrength ) + ( float2( -1,1 ) * appendResult58_g1 ) );
			float cos73 = cos( 0.0001 * _Time.y );
			float sin73 = sin( 0.0001 * _Time.y );
			float2 rotator73 = mul( temp_output_27_0 - float2( 0.5,0.5 ) , float2x2( cos73 , -sin73 , sin73 , cos73 )) + float2( 0.5,0.5 );
			float cos74 = cos( -0.0002 * _Time.y );
			float sin74 = sin( -0.0002 * _Time.y );
			float2 rotator74 = mul( temp_output_27_0 - float2( 0.75,0.75 ) , float2x2( cos74 , -sin74 , sin74 , cos74 )) + float2( 0.75,0.75 );
			float cos78 = cos( -0.00035 * _Time.y );
			float sin78 = sin( -0.00035 * _Time.y );
			float2 rotator78 = mul( temp_output_27_0 - float2( 0.5,0.5 ) , float2x2( cos78 , -sin78 , sin78 , cos78 )) + float2( 0.5,0.5 );
			float PortalMask144 = saturate( ( _NoiseBaseWhite + ( saturate( ( 1.0 - ( distanceDepth50 * _DepthFadeGradient ) ) ) + tex2D( _NoiseLines, temp_output_27_0 ).r + floor( ( tex2D( _NoiseBreak, temp_output_27_0 ).r * _NoiseAmount ) ) + floor( ( tex2D( _NoiseBreak, rotator73 ).r * _NoiseAmount ) ) + floor( ( tex2D( _NoiseBreak, rotator74 ).r * _NoiseAmount ) ) + floor( ( tex2D( _NoiseBreak, rotator78 ).r * _NoiseAmount ) ) ) ) );
			float4 lerpResult31 = lerp( PortalNoiseColor139 , PortalLinesColor140 , PortalMask144);
			o.Emission = saturate( ( lerpResult31 * _EmissionMultiplier ) ).rgb;
			float screenDepth128 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture,UNITY_PROJ_COORD( ase_screenPos )));
			float distanceDepth128 = abs( ( screenDepth128 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _OpacityDepthFadeDistance ) );
			float FinalOpacity123 = (0.0 + (( _Opacity * distanceDepth128 ) - 0.0) * (_MaxOpacity - 0.0) / (1.0 - 0.0));
			o.Alpha = saturate( FinalOpacity123 );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows vertex:vertexDataFunc tessellate:tessFunction 

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
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float4 screenPos : TEXCOORD3;
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
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
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
				surfIN.screenPos = IN.screenPos;
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
744;73;853;656;2351.898;275.9079;1.772915;True;False
Node;AmplifyShaderEditor.CommentaryNode;161;-2908.338,-177.3087;Float;False;2549;1253;Portal Mask;56;27;73;74;78;146;49;148;75;70;79;147;30;65;149;150;151;80;71;66;76;77;72;81;68;153;154;152;155;52;51;50;57;56;53;55;82;84;85;144;37;36;34;38;35;23;25;160;159;158;157;156;188;187;189;190;191;Portal Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;187;-2848,624;Float;False;Property;_RingPannerSpeed;RingPannerSpeed;25;0;Create;True;0;0;False;0;0;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;190;-2664.367,609.2028;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;191;-2737.367,596.2028;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;36;-2858.338,176.6913;Float;False;Property;_NoiseMapSize;NoiseMapSize;19;0;Create;True;0;0;False;0;0;0.5;0.5;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;37;-2858.338,256.6913;Float;False;Property;_NoiseMapPannerSpeed;NoiseMapPannerSpeed;20;0;Create;True;0;0;False;0;0.1;0;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;34;-2666.338,-15.30868;Float;True;Property;_Noise;Noise;8;0;Create;True;0;0;False;0;None;eb58c36f965ff024cb94d9bba46e33e7;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.Vector2Node;25;-2682.338,416.6913;Float;False;Constant;_TextureCoordination;Texture Coordination;7;0;Create;True;0;0;False;0;-1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.DynamicAppendNode;35;-2586.338,160.6913;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;38;-2586.338,240.6913;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;23;-2714.338,336.6913;Float;False;Property;_NoiseMapStrength;NoiseMapStrength;5;0;Create;True;0;0;False;0;0;0.1308148;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;189;-2713,544;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;188;-2585,544;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WireNode;157;-2400.553,461.9693;Float;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WireNode;156;-2386.553,440.9692;Float;False;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.WireNode;158;-2417.553,482.9693;Float;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WireNode;160;-2445.553,516.9694;Float;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WireNode;159;-2432.553,499.9693;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;27;-2362.338,432.6914;Float;True;RadialUVDistortion;-1;;1;051d65e7699b41a4c800363fd0e822b2;0;7;60;SAMPLER2D;0.0;False;1;FLOAT2;1,1;False;11;FLOAT2;0,0;False;65;FLOAT;1;False;68;FLOAT2;1,1;False;47;FLOAT2;1,1;False;29;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;51;-1946.338,-95.30868;Float;False;Property;_DepthFadeDistance;Depth Fade Distance;21;0;Create;True;0;0;False;0;0;0.002;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;146;-1830.745,313.348;Float;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RotatorNode;73;-1834.338,432.6914;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;0.0001;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;141;-2290,-1106;Float;False;1957;797.6258;Portal Color;14;97;94;98;93;92;96;95;86;28;89;114;88;139;140;Portal Color;1,0,0,1;0;0
Node;AmplifyShaderEditor.RotatorNode;74;-1834.338,608.6915;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.75,0.75;False;2;FLOAT;-0.0002;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;65;-1578.338,960.6915;Float;False;Property;_NoiseAmount;Noise Amount;18;0;Create;True;0;0;False;0;2.54;2.03;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;78;-1834.338,784.6915;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;-0.00035;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;75;-1658.338,592.6915;Float;True;Property;_NoiseBreak3;Noise Break 3;7;0;Create;True;0;0;False;0;None;604971e1d9055324498a4589d01325ce;True;0;False;white;Auto;False;Instance;49;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;97;-2240,-608;Float;False;Property;_PannerSpeed;Panner Speed;24;0;Create;True;0;0;False;0;0;0.08065257;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;70;-1658.338,416.6913;Float;True;Property;_NoiseBreak2;Noise Break 2;7;0;Create;True;0;0;False;0;None;604971e1d9055324498a4589d01325ce;True;0;False;white;Auto;False;Instance;49;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;57;-1946.338,-15.30868;Float;False;Property;_DepthFadeGradient;Depth Fade Gradient;22;0;Create;True;0;0;False;0;0;0.001;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;150;-1360.667,691.7375;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;151;-1350.106,869.1277;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;50;-1690.338,-127.3087;Float;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;49;-1658.338,240.6913;Float;True;Property;_NoiseBreak;Noise Break;7;0;Create;True;0;0;False;0;None;604971e1d9055324498a4589d01325ce;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;148;-1395.397,334.1275;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;149;-1374.922,510.4326;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;79;-1658.338,768.6915;Float;True;Property;_NoiseBreak4;Noise Break 4;7;0;Create;True;0;0;False;0;None;604971e1d9055324498a4589d01325ce;True;0;False;white;Auto;False;Instance;49;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;94;-2048,-720;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;2,2;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;71;-1322.338,448.6915;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;-1434.338,-31.30868;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;98;-1968,-608;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;76;-1322.338,624.6915;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;80;-1322.338,800.6915;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;66;-1322.338,272.6913;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FloorOpNode;77;-1178.338,624.6915;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;147;-1834.222,169.0504;Float;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FloorOpNode;81;-1178.338,800.6915;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FloorOpNode;68;-1178.338,272.6913;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FloorOpNode;72;-1178.338,448.6915;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;53;-1306.338,-31.30868;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;93;-1808,-656;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;96;-1616,-464;Float;False;Constant;_UVDistortion;UV Distortion;15;0;Create;True;0;0;False;0;0.3529412;0.3529412;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;152;-1086.338,176.6913;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;92;-1632,-656;Float;True;Property;_Flowmap;Flowmap;10;0;Create;True;0;0;False;0;None;1903f5f8212195b4e9c128b41e6fd696;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;155;-1028.337,218.6913;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;55;-1162.338,-31.30868;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;154;-1042.337,204.6913;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;30;-1658.338,64.69131;Float;True;Property;_NoiseLines;Noise Lines;6;0;Create;True;0;0;False;0;None;138ae4b08ba7f89438effbd67e97ccef;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;153;-1062.337,187.6913;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;121;-1420.906,-1646.877;Float;False;1078;347.5928;Opacity;7;32;129;128;127;137;138;123;Opacity;0,0.8460016,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;95;-1296,-720;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;52;-986.3375,80.69131;Float;False;6;6;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;82;-1290.338,-111.3087;Float;False;Property;_NoiseBaseWhite;Noise Base White;17;0;Create;True;0;0;False;0;0;0.13;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;86;-1136,-720;Float;True;Property;_PortalNoise;Portal Noise;9;0;Create;True;0;0;False;0;None;3b0290eb3a0bb884990fd384e3373469;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;84;-858.3374,64.69131;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;89;-1376,-896;Float;False;Property;_PortalNoiseColor2;Portal Noise Color 2;12;0;Create;True;0;0;False;0;0.06711894,0,0.2735849,0;0.06711851,0,0.2735843,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;127;-1398.854,-1526.074;Float;False;Property;_OpacityDepthFadeDistance;Opacity Depth Fade Distance;23;0;Create;True;0;0;False;0;0.001;0.472;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;28;-1376,-1056;Float;False;Property;_PortalNoiseColor1;Portal Noise Color 1;11;0;Create;True;0;0;False;0;0.4481132,0.4935628,1,0;0.5960784,0.8235295,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;114;-840.9952,-515.3742;Float;False;Property;_PortalLinesColor;Portal Lines Color;13;0;Create;True;0;0;False;0;0,0,0,0;0.03529412,0.08675843,0.2078423,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;85;-746.3374,64.69131;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;186;-1238.683,-2138.691;Float;False;890.5378;282.1484;Vertex Offset;5;167;168;183;182;184;Vertex Offset;0.1115192,1,0,1;0;0
Node;AmplifyShaderEditor.LerpOp;88;-832,-736;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DepthFade;128;-1094.854,-1526.074;Float;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-1382.854,-1606.074;Float;False;Property;_Opacity;Opacity;15;0;Create;True;0;0;False;0;0;0.75;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;144;-602.3374,64.69131;Float;False;PortalMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;138;-1019.854,-1433.074;Float;False;Property;_MaxOpacity;Max Opacity;16;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;129;-854.8541,-1606.074;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;140;-592,-512;Float;False;PortalLinesColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;139;-608,-736;Float;False;PortalNoiseColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;167;-1188.683,-1996.544;Float;False;Property;_TesselAmount;Tessel Amount;26;0;Create;True;0;0;False;0;1;0.118;-0.3;0.3;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;142;32,32;Float;False;139;PortalNoiseColor;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;145;64,160;Float;False;144;PortalMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;143;32,96;Float;False;140;PortalLinesColor;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;183;-991.7813,-2088.691;Float;False;139;PortalNoiseColor;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;168;-900.6829,-2012.544;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TFHCRemapNode;137;-726.8541,-1574.074;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;103;304,192;Float;False;Property;_EmissionMultiplier;Emission Multiplier;14;0;Create;True;0;0;False;0;0;2.44;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;31;384,32;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;123;-550.8541,-1606.074;Float;False;FinalOpacity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;182;-735.145,-2043.598;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;184;-591.145,-2043.598;Float;False;VertexOffset;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;124;560,176;Float;False;123;FinalOpacity;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;102;560,32;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;185;688,256;Float;False;184;VertexOffset;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;104;688,32;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;162;752,176;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;896,-16;Float;False;True;6;Float;ASEMaterialInspector;0;0;Standard;MyShaders/Portal;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;2;35;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;0;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;190;0;187;0
WireConnection;191;0;190;0
WireConnection;35;0;36;0
WireConnection;35;1;36;0
WireConnection;38;0;37;0
WireConnection;38;1;37;0
WireConnection;189;0;191;0
WireConnection;188;0;189;0
WireConnection;188;1;187;0
WireConnection;157;0;35;0
WireConnection;156;0;34;0
WireConnection;158;0;38;0
WireConnection;160;0;25;0
WireConnection;159;0;23;0
WireConnection;27;60;156;0
WireConnection;27;1;157;0
WireConnection;27;11;158;0
WireConnection;27;65;159;0
WireConnection;27;68;160;0
WireConnection;27;47;188;0
WireConnection;146;0;27;0
WireConnection;73;0;27;0
WireConnection;74;0;27;0
WireConnection;78;0;27;0
WireConnection;75;1;74;0
WireConnection;70;1;73;0
WireConnection;150;0;65;0
WireConnection;151;0;65;0
WireConnection;50;0;51;0
WireConnection;49;1;146;0
WireConnection;148;0;65;0
WireConnection;149;0;65;0
WireConnection;79;1;78;0
WireConnection;71;0;70;1
WireConnection;71;1;149;0
WireConnection;56;0;50;0
WireConnection;56;1;57;0
WireConnection;98;0;97;0
WireConnection;98;1;97;0
WireConnection;76;0;75;1
WireConnection;76;1;150;0
WireConnection;80;0;79;1
WireConnection;80;1;151;0
WireConnection;66;0;49;1
WireConnection;66;1;148;0
WireConnection;77;0;76;0
WireConnection;147;0;27;0
WireConnection;81;0;80;0
WireConnection;68;0;66;0
WireConnection;72;0;71;0
WireConnection;53;0;56;0
WireConnection;93;0;94;0
WireConnection;93;2;98;0
WireConnection;152;0;68;0
WireConnection;92;1;93;0
WireConnection;155;0;81;0
WireConnection;55;0;53;0
WireConnection;154;0;77;0
WireConnection;30;1;147;0
WireConnection;153;0;72;0
WireConnection;95;0;94;0
WireConnection;95;1;92;0
WireConnection;95;2;96;0
WireConnection;52;0;55;0
WireConnection;52;1;30;1
WireConnection;52;2;152;0
WireConnection;52;3;153;0
WireConnection;52;4;154;0
WireConnection;52;5;155;0
WireConnection;86;1;95;0
WireConnection;84;0;82;0
WireConnection;84;1;52;0
WireConnection;85;0;84;0
WireConnection;88;0;28;0
WireConnection;88;1;89;0
WireConnection;88;2;86;1
WireConnection;128;0;127;0
WireConnection;144;0;85;0
WireConnection;129;0;32;0
WireConnection;129;1;128;0
WireConnection;140;0;114;0
WireConnection;139;0;88;0
WireConnection;168;1;167;0
WireConnection;137;0;129;0
WireConnection;137;4;138;0
WireConnection;31;0;142;0
WireConnection;31;1;143;0
WireConnection;31;2;145;0
WireConnection;123;0;137;0
WireConnection;182;0;183;0
WireConnection;182;1;168;0
WireConnection;184;0;182;0
WireConnection;102;0;31;0
WireConnection;102;1;103;0
WireConnection;104;0;102;0
WireConnection;162;0;124;0
WireConnection;0;2;104;0
WireConnection;0;9;162;0
WireConnection;0;11;185;0
ASEEND*/
//CHKSM=67806A147B32B9D1C8EED41D134AF43EAFAF2917