// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MyShaders/UI/Boss Bar"
{
	Properties
	{
		_BossBar("Boss Bar", 2D) = "white" {}
		_BossBarSplatmap("Boss Bar Splatmap", 2D) = "white" {}
		_ArrowBoss("Arrow Boss", 2D) = "white" {}
		_Mist("Mist", 2D) = "white" {}
		_RedGradient("Red Gradient", 2D) = "white" {}
		_Flowmap("Flowmap", 2D) = "white" {}
		_BaseBarDarkness("Base Bar Darkness", Range( 0 , 1)) = 0
		_SmokeFlowmapIntensity("Smoke Flowmap Intensity", Range( 0 , 1)) = 0.2
		_SmokeIntensity("Smoke Intensity", Range( 0 , 1)) = 0
		_SmokeColor("Smoke Color", Color) = (1,0,0,0)
		_ArrowLTiling("Arrow L Tiling", Vector) = (0,0,0,0)
		_ArrowRTiling("Arrow R Tiling", Vector) = (0,0,0,0)
		_ArrowLOffset("Arrow L Offset", Vector) = (0,0,0,0)
		_ArrowROffset("Arrow R Offset", Vector) = (0,0,0,0)
		_BossLifeMaxOffset("BossLifeMaxOffset", Float) = 0
		_BossLifeBarGradient("Boss Life Bar Gradient", Float) = 10
		_BossLifePercentage("Boss Life Percentage", Range( 0 , 100)) = 100
		_ArrowBeatRatePercentage("Arrow BeatRate Percentage", Range( 0 , 100)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha , SrcAlpha OneMinusSrcAlpha
		BlendOp Add , Add
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform sampler2D _BossBarSplatmap;
		uniform sampler2D _Flowmap;
		uniform float _SmokeFlowmapIntensity;
		uniform float _SmokeIntensity;
		uniform float4 _SmokeColor;
		uniform sampler2D _BossBar;
		uniform float4 _BossBar_ST;
		uniform float _BaseBarDarkness;
		uniform float4 _BossBarSplatmap_ST;
		uniform float _BossLifeBarGradient;
		uniform float _BossLifePercentage;
		uniform float _BossLifeMaxOffset;
		uniform sampler2D _ArrowBoss;
		uniform float2 _ArrowRTiling;
		uniform float2 _ArrowROffset;
		uniform float _ArrowBeatRatePercentage;
		uniform float2 _ArrowLTiling;
		uniform float2 _ArrowLOffset;
		uniform sampler2D _RedGradient;
		uniform sampler2D _Mist;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 panner12 = ( _Time.y * float2( 0.1,0.03 ) + i.uv_texcoord);
			float4 lerpResult10 = lerp( float4( i.uv_texcoord, 0.0 , 0.0 ) , tex2D( _Flowmap, panner12 ) , _SmokeFlowmapIntensity);
			float temp_output_25_0 = saturate( ( tex2D( _BossBarSplatmap, lerpResult10.rg ).b * _SmokeIntensity * (0.6 + (sin( _Time.y ) - -1.0) * (1.3 - 0.6) / (1.0 - -1.0)) ) );
			float4 SmokeColor28 = ( temp_output_25_0 * _SmokeColor );
			float2 uv_BossBar = i.uv_texcoord * _BossBar_ST.xy + _BossBar_ST.zw;
			float4 tex2DNode1 = tex2D( _BossBar, uv_BossBar );
			float BarBaseAlpha74 = tex2DNode1.a;
			float4 temp_cast_2 = (BarBaseAlpha74).xxxx;
			float3 appendResult35 = (float3(tex2DNode1.r , tex2DNode1.g , tex2DNode1.b));
			float3 temp_cast_3 = (_BaseBarDarkness).xxx;
			float3 BarBaseColor76 = ( ( appendResult35 * BarBaseAlpha74 ) - temp_cast_3 );
			float2 uv_BossBarSplatmap = i.uv_texcoord * _BossBarSplatmap_ST.xy + _BossBarSplatmap_ST.zw;
			float3 ase_worldPos = i.worldPos;
			float temp_output_148_0 = (0.0 + (_BossLifePercentage - 0.0) * (_BossLifeMaxOffset - 0.0) / (100.0 - 0.0));
			float temp_output_129_0 = ( tex2D( _BossBarSplatmap, uv_BossBarSplatmap ).g * ( saturate( ( ( ase_worldPos.x * _BossLifeBarGradient ) + temp_output_148_0 ) ) * saturate( ( ( ase_worldPos.x * -_BossLifeBarGradient ) + temp_output_148_0 ) ) ) );
			float BossLifeBarAlpha172 = temp_output_129_0;
			float3 temp_cast_4 = (BossLifeBarAlpha172).xxx;
			float temp_output_115_0 = (0.0 + (_ArrowBeatRatePercentage - 100.0) * (4.0 - 0.0) / (1.0 - 100.0));
			float lerpResult90 = lerp( 1.0 , 2.0 , saturate( temp_output_115_0 ));
			float lerpResult91 = lerp( lerpResult90 , 3.0 , saturate( ( temp_output_115_0 - 1.0 ) ));
			float lerpResult119 = lerp( lerpResult91 , 4.0 , saturate( ( temp_output_115_0 - 2.0 ) ));
			float lerpResult122 = lerp( lerpResult119 , 5.0 , saturate( ( temp_output_115_0 - 3.0 ) ));
			float mulTime50 = _Time.y * lerpResult122;
			float2 appendResult54 = (float2(( _ArrowROffset.x + (0.0 + (sin( mulTime50 ) - -1.0) * (-0.2 - 0.0) / (1.0 - -1.0)) ) , _ArrowROffset.y));
			float2 uv_TexCoord43 = i.uv_texcoord * _ArrowRTiling + appendResult54;
			float4 tex2DNode4 = tex2D( _ArrowBoss, uv_TexCoord43 );
			float3 appendResult41 = (float3(tex2DNode4.r , tex2DNode4.g , tex2DNode4.b));
			float mulTime56 = _Time.y * lerpResult122;
			float2 appendResult63 = (float2(( _ArrowLOffset.x + (0.0 + (sin( mulTime56 ) - -1.0) * (-0.2 - 0.0) / (1.0 - -1.0)) ) , _ArrowLOffset.y));
			float2 uv_TexCoord64 = i.uv_texcoord * _ArrowLTiling + appendResult63;
			float4 tex2DNode65 = tex2D( _ArrowBoss, uv_TexCoord64 );
			float3 appendResult66 = (float3(tex2DNode65.r , tex2DNode65.g , tex2DNode65.b));
			float3 ArrowColor69 = ( ( appendResult41 * tex2DNode4.a ) + ( appendResult66 * tex2DNode65.a ) );
			float2 panner184 = ( _Time.y * float2( 0.1,0.02 ) + i.uv_texcoord);
			float4 lerpResult187 = lerp( float4( i.uv_texcoord, 0.0 , 0.0 ) , tex2D( _Flowmap, panner184 ) , float4( 0.3333333,0.3333333,0.3333333,0 ));
			float2 temp_cast_9 = (saturate( ( tex2D( _Mist, lerpResult187.rg ).r + 0.07 ) )).xx;
			float4 BossLifeBarColor190 = ( temp_output_129_0 * tex2D( _RedGradient, temp_cast_9 ) );
			o.Emission = ( ( saturate( ( SmokeColor28 - temp_cast_2 ) ) + float4( saturate( ( BarBaseColor76 - temp_cast_4 ) ) , 0.0 ) ) + float4( ArrowColor69 , 0.0 ) + BossLifeBarColor190 ).rgb;
			float SmokeAlpha39 = temp_output_25_0;
			float ArrowAlpha71 = ( tex2DNode4.a + tex2DNode65.a );
			o.Alpha = saturate( ( BarBaseAlpha74 + SmokeAlpha39 + ArrowAlpha71 + BossLifeBarAlpha172 ) );
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
				surfIN.worldPos = worldPos;
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
Version=17200
-2;141;779;822;3289.848;368.4691;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;79;-3016.465,-304;Inherit;False;3624.537;855.8523;;44;90;122;125;119;91;123;124;115;110;121;118;120;116;95;93;71;69;68;72;67;42;41;66;65;4;64;43;63;44;62;54;61;55;60;53;59;52;58;45;49;57;50;56;207;Arrows;1,0,0,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;207;-2982.752,70.25572;Inherit;False;Property;_ArrowBeatRatePercentage;Arrow BeatRate Percentage;18;0;Create;True;0;0;False;0;0;62.65055;0;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;115;-2688,80;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;100;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;4;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;93;-2432,160;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;116;-2432,240;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;110;-2304,80;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;95;-2304,160;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;124;-2032,128;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;90;-2160,32;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;2;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;120;-2432,320;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;118;-2304,240;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;91;-2016,32;Inherit;False;3;0;FLOAT;2;False;1;FLOAT;3;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;123;-1888,128;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;80;-1600,-976;Inherit;False;2207.62;594.7974;;20;39;28;27;19;25;26;7;37;23;21;10;11;38;9;12;22;14;8;13;15;Smoke;0,0,1,1;0;0
Node;AmplifyShaderEditor.SaturateNode;121;-2304,320;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;125;-1744,128;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-1552,-576;Inherit;False;Constant;_SmokeTimeScale;Smoke Time Scale;5;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;119;-1872,32;Inherit;False;3;0;FLOAT;2;False;1;FLOAT;4;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;201;-1589.751,-1967.758;Inherit;False;2101.628;880.1169;;32;171;147;131;148;166;127;200;199;160;168;130;186;189;188;161;135;163;184;133;185;165;126;129;187;178;172;176;183;190;204;206;203;Boss Life Bar;0,1,0.02520466,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;171;-1440,-1504;Inherit;False;Property;_BossLifeMaxOffset;BossLifeMaxOffset;15;0;Create;True;0;0;False;0;0;1400;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;8;-1440,-832;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;13;-1408,-720;Inherit;False;Constant;_FlowmapSpeed;Flowmap Speed;5;0;Create;True;0;0;False;0;0.1,0.03;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;147;-1465.123,-1621.319;Inherit;False;Property;_BossLifePercentage;Boss Life Percentage;17;0;Create;True;0;0;False;0;100;100;0;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;14;-1328,-576;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;122;-1728,32;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;5;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;12;-1184,-736;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;50;-1552,32;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;148;-1193.123,-1605.319;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;100;False;3;FLOAT;0;False;4;FLOAT;7.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;131;-1193.123,-1685.319;Inherit;False;Property;_BossLifeBarGradient;Boss Life Bar Gradient;16;0;Create;True;0;0;False;0;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;56;-1552,368;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;22;-992,-512;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;166;-873.1224,-1573.319;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;189;-1491.751,-1309.641;Inherit;False;Constant;_LifeBarMistSpeed;LifeBarMist Speed;20;0;Create;True;0;0;False;0;0.1,0.02;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SinOpNode;57;-1376,368;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;9;-992,-768;Inherit;True;Property;_Flowmap;Flowmap;6;0;Create;True;0;0;False;0;-1;None;1903f5f8212195b4e9c128b41e6fd696;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;188;-1475.751,-1197.641;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;127;-937.1225,-1781.319;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SinOpNode;49;-1376,32;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;200;-742.1566,-1632.272;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;38;-720,-512;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-976,-576;Inherit;False;Property;_SmokeFlowmapIntensity;Smoke Flowmap Intensity;8;0;Create;True;0;0;False;0;0.2;0.057;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;186;-1539.751,-1421.641;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;160;-729.1224,-1605.319;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;10;-656,-832;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SinOpNode;21;-512,-656;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;130;-728.9272,-1713.237;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;59;-1232,368;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;-0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;81;-1591.874,-2418.749;Inherit;False;1288.603;300.6996;;7;82;76;36;74;35;1;83;Base Bar;0,1,1,1;0;0
Node;AmplifyShaderEditor.WireNode;199;-628.1566,-1634.272;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;52;-1232,32;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;-0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;168;-889.241,-1511.156;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;58;-1248,240;Inherit;False;Property;_ArrowLOffset;Arrow L Offset;13;0;Create;True;0;0;False;0;0,0;1.83,1.92;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;45;-1248,-96;Inherit;False;Property;_ArrowROffset;Arrow R Offset;14;0;Create;True;0;0;False;0;0,0;-10,-0.92;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;184;-1267.751,-1325.641;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WireNode;61;-944,304;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;60;-1040,240;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-1543.874,-2370.749;Inherit;True;Property;_BossBar;Boss Bar;1;0;Create;True;0;0;False;0;-1;c81aad8fa1c990940a49bf9178e62b96;c81aad8fa1c990940a49bf9178e62b96;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;7;-480,-928;Inherit;True;Property;_BossBarSplatmap;Boss Bar Splatmap;2;0;Create;True;0;0;False;0;-1;902f0af77c0ee3f4da1f20431809c401;902f0af77c0ee3f4da1f20431809c401;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;53;-1040,-96;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;161;-585.1224,-1605.319;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;23;-384,-656;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0.6;False;4;FLOAT;1.3;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;185;-1075.751,-1357.641;Inherit;True;Property;_TextureSample2;Texture Sample 2;6;0;Create;True;0;0;False;0;-1;None;1903f5f8212195b4e9c128b41e6fd696;True;0;False;white;Auto;False;Instance;9;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;37;-464,-736;Inherit;False;Property;_SmokeIntensity;Smoke Intensity;9;0;Create;True;0;0;False;0;0;0.65;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;135;-568.9271,-1713.237;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;55;-944,-32;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;187;-723.7521,-1421.641;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0.3333333,0.3333333,0.3333333,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;54;-912,-96;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;35;-1191.874,-2338.749;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector2Node;44;-944,-224;Inherit;False;Property;_ArrowRTiling;Arrow R Tiling;12;0;Create;True;0;0;False;0;0,0;12,3;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;74;-1223.874,-2226.749;Inherit;False;BarBaseAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-144,-704;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;62;-944,112;Inherit;False;Property;_ArrowLTiling;Arrow L Tiling;11;0;Create;True;0;0;False;0;0,0;-12,-3;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SaturateNode;133;-440.9272,-1713.237;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;63;-912,240;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;163;-457.1224,-1605.319;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;19;-64,-592;Inherit;False;Property;_SmokeColor;Smoke Color;10;0;Create;True;0;0;False;0;1,0,0,0;0.3773585,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;43;-752,-224;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;83;-983.8738,-2226.749;Inherit;False;Property;_BaseBarDarkness;Base Bar Darkness;7;0;Create;True;0;0;False;0;0;0.229;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-999.8737,-2338.749;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;64;-752,112;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;178;-499.7721,-1445.535;Inherit;True;Property;_Mist;Mist;4;0;Create;True;0;0;False;0;-1;None;6a03fae17dfcd914a819dcf8bdf8037c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;126;-584.585,-1917.758;Inherit;True;Property;_TextureSample1;Texture Sample 1;2;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Instance;7;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;25;0,-704;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;165;-265.1225,-1717.319;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;203;-384,-1248;Inherit;False;Constant;_MistFix;Mist Fix;19;0;Create;True;0;0;False;0;0.07;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;129;-105.9022,-1823.688;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;82;-695.8728,-2338.749;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;65;-496,80;Inherit;True;Property;_TextureSample0;Texture Sample 0;3;0;Create;True;0;0;False;0;-1;7ffed5d8f946f6a4db0b3cfa29e54572;7ffed5d8f946f6a4db0b3cfa29e54572;True;0;False;white;Auto;False;Instance;4;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;-512,-256;Inherit;True;Property;_ArrowBoss;Arrow Boss;3;0;Create;True;0;0;False;0;-1;7ffed5d8f946f6a4db0b3cfa29e54572;7ffed5d8f946f6a4db0b3cfa29e54572;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;208,-704;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;206;-192,-1408;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;28;352,-704;Inherit;False;SmokeColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;66;-144,128;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;76;-519.8705,-2338.749;Inherit;False;BarBaseColor;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;172;246.8775,-1829.319;Inherit;False;BossLifeBarAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;41;-160,-224;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;204;-66,-1407;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;194;800,-656;Inherit;False;172;BossLifeBarAlpha;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;29;846.1219,-879.1297;Inherit;False;28;SmokeColor;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;-16,-176;Inherit;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;72;-144,32;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;176;96,-1440;Inherit;True;Property;_RedGradient;Red Gradient;5;0;Create;True;0;0;False;0;-1;None;314676eb2b08d7c4d9914086acbfaed4;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;67;0,176;Inherit;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;77;827.5906,-736;Inherit;False;76;BarBaseColor;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;78;830.122,-799.1296;Inherit;False;74;BarBaseAlpha;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;39;208,-784;Inherit;False;SmokeAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;195;1056,-736;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;71;-16,32;Inherit;False;ArrowAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;183;118.8775,-1749.319;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;68;256,16;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;30;1072.843,-866.9315;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;69;384,16;Inherit;False;ArrowColor;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;75;1438.122,-575.1296;Inherit;False;74;BarBaseAlpha;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;40;1438.122,-511.1299;Inherit;False;39;SmokeAlpha;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;73;1438.122,-447.1299;Inherit;False;71;ArrowAlpha;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;192;1408,-368;Inherit;False;172;BossLifeBarAlpha;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;33;1230.122,-863.1296;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;190;246.8775,-1749.319;Inherit;False;BossLifeBarColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;197;1200,-736;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;70;1390.122,-767.1295;Inherit;False;69;ArrowColor;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;18;1662.122,-575.1296;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;198;1392,-704;Inherit;False;190;BossLifeBarColor;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;34;1390.122,-863.1296;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;191;1792,-576;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;46;1664,-864;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;3;2094.122,-767.1295;Float;False;True;2;ASEMaterialInspector;0;0;Unlit;MyShaders/UI/Boss Bar;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Transparent;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;2;5;False;-1;10;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;115;0;207;0
WireConnection;93;0;115;0
WireConnection;116;0;115;0
WireConnection;110;0;115;0
WireConnection;95;0;93;0
WireConnection;124;0;95;0
WireConnection;90;2;110;0
WireConnection;120;0;115;0
WireConnection;118;0;116;0
WireConnection;91;0;90;0
WireConnection;91;2;124;0
WireConnection;123;0;118;0
WireConnection;121;0;120;0
WireConnection;125;0;121;0
WireConnection;119;0;91;0
WireConnection;119;2;123;0
WireConnection;14;0;15;0
WireConnection;122;0;119;0
WireConnection;122;2;125;0
WireConnection;12;0;8;0
WireConnection;12;2;13;0
WireConnection;12;1;14;0
WireConnection;50;0;122;0
WireConnection;148;0;147;0
WireConnection;148;4;171;0
WireConnection;56;0;122;0
WireConnection;22;0;14;0
WireConnection;166;0;131;0
WireConnection;57;0;56;0
WireConnection;9;1;12;0
WireConnection;49;0;50;0
WireConnection;200;0;148;0
WireConnection;38;0;22;0
WireConnection;160;0;127;1
WireConnection;160;1;166;0
WireConnection;10;0;8;0
WireConnection;10;1;9;0
WireConnection;10;2;11;0
WireConnection;21;0;38;0
WireConnection;130;0;127;1
WireConnection;130;1;131;0
WireConnection;59;0;57;0
WireConnection;199;0;200;0
WireConnection;52;0;49;0
WireConnection;168;0;148;0
WireConnection;184;0;186;0
WireConnection;184;2;189;0
WireConnection;184;1;188;0
WireConnection;61;0;58;2
WireConnection;60;0;58;1
WireConnection;60;1;59;0
WireConnection;7;1;10;0
WireConnection;53;0;45;1
WireConnection;53;1;52;0
WireConnection;161;0;160;0
WireConnection;161;1;168;0
WireConnection;23;0;21;0
WireConnection;185;1;184;0
WireConnection;135;0;130;0
WireConnection;135;1;199;0
WireConnection;55;0;45;2
WireConnection;187;0;186;0
WireConnection;187;1;185;0
WireConnection;54;0;53;0
WireConnection;54;1;55;0
WireConnection;35;0;1;1
WireConnection;35;1;1;2
WireConnection;35;2;1;3
WireConnection;74;0;1;4
WireConnection;26;0;7;3
WireConnection;26;1;37;0
WireConnection;26;2;23;0
WireConnection;133;0;135;0
WireConnection;63;0;60;0
WireConnection;63;1;61;0
WireConnection;163;0;161;0
WireConnection;43;0;44;0
WireConnection;43;1;54;0
WireConnection;36;0;35;0
WireConnection;36;1;74;0
WireConnection;64;0;62;0
WireConnection;64;1;63;0
WireConnection;178;1;187;0
WireConnection;25;0;26;0
WireConnection;165;0;133;0
WireConnection;165;1;163;0
WireConnection;129;0;126;2
WireConnection;129;1;165;0
WireConnection;82;0;36;0
WireConnection;82;1;83;0
WireConnection;65;1;64;0
WireConnection;4;1;43;0
WireConnection;27;0;25;0
WireConnection;27;1;19;0
WireConnection;206;0;178;1
WireConnection;206;1;203;0
WireConnection;28;0;27;0
WireConnection;66;0;65;1
WireConnection;66;1;65;2
WireConnection;66;2;65;3
WireConnection;76;0;82;0
WireConnection;172;0;129;0
WireConnection;41;0;4;1
WireConnection;41;1;4;2
WireConnection;41;2;4;3
WireConnection;204;0;206;0
WireConnection;42;0;41;0
WireConnection;42;1;4;4
WireConnection;72;0;4;4
WireConnection;72;1;65;4
WireConnection;176;1;204;0
WireConnection;67;0;66;0
WireConnection;67;1;65;4
WireConnection;39;0;25;0
WireConnection;195;0;77;0
WireConnection;195;1;194;0
WireConnection;71;0;72;0
WireConnection;183;0;129;0
WireConnection;183;1;176;0
WireConnection;68;0;42;0
WireConnection;68;1;67;0
WireConnection;30;0;29;0
WireConnection;30;1;78;0
WireConnection;69;0;68;0
WireConnection;33;0;30;0
WireConnection;190;0;183;0
WireConnection;197;0;195;0
WireConnection;18;0;75;0
WireConnection;18;1;40;0
WireConnection;18;2;73;0
WireConnection;18;3;192;0
WireConnection;34;0;33;0
WireConnection;34;1;197;0
WireConnection;191;0;18;0
WireConnection;46;0;34;0
WireConnection;46;1;70;0
WireConnection;46;2;198;0
WireConnection;3;2;46;0
WireConnection;3;9;191;0
ASEEND*/
//CHKSM=76D8DE436530FD077E3B706AD4699082DD96084B