// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Templates/Legacy/PostProcess"
{
	Properties
	{
		_MainTex ( "Screen", 2D ) = "black" {}
		_Gradient("Gradient", 2D) = "white" {}
		_FocusIntensity("Focus Intensity", Range( 0 , 1)) = 0
		_Noise("Noise", 2D) = "white" {}
		_GradientFix("Gradient Fix", Float) = 0.75
		_NoiseIntensity("Noise Intensity", Float) = 0.03
		_HitFXIntensity("Hit FX Intensity", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		
		
		ZTest Always
		Cull Off
		ZWrite Off

		
		Pass
		{ 
			CGPROGRAM 

			

			#pragma vertex vert_img_custom 
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"
			

			struct appdata_img_custom
			{
				float4 vertex : POSITION;
				half2 texcoord : TEXCOORD0;
				
			};

			struct v2f_img_custom
			{
				float4 pos : SV_POSITION;
				half2 uv   : TEXCOORD0;
				half2 stereoUV : TEXCOORD2;
		#if UNITY_UV_STARTS_AT_TOP
				half4 uv2 : TEXCOORD1;
				half4 stereoUV2 : TEXCOORD3;
		#endif
				
			};

			uniform sampler2D _MainTex;
			uniform half4 _MainTex_TexelSize;
			uniform half4 _MainTex_ST;
			
			uniform sampler2D _Noise;
			uniform float4 _Noise_ST;
			uniform sampler2D _Gradient;
			uniform float4 _Gradient_ST;
			uniform float _GradientFix;
			uniform float _NoiseIntensity;
			uniform float _HitFXIntensity;
			uniform float _FocusIntensity;

			v2f_img_custom vert_img_custom ( appdata_img_custom v  )
			{
				v2f_img_custom o;
				
				o.pos = UnityObjectToClipPos ( v.vertex );
				o.uv = float4( v.texcoord.xy, 1, 1 );

				#if UNITY_UV_STARTS_AT_TOP
					o.uv2 = float4( v.texcoord.xy, 1, 1 );
					o.stereoUV2 = UnityStereoScreenSpaceUVAdjust ( o.uv2, _MainTex_ST );

					if ( _MainTex_TexelSize.y < 0.0 )
						o.uv.y = 1.0 - o.uv.y;
				#endif
				o.stereoUV = UnityStereoScreenSpaceUVAdjust ( o.uv, _MainTex_ST );
				return o;
			}

			half4 frag ( v2f_img_custom i ) : SV_Target
			{
				#ifdef UNITY_UV_STARTS_AT_TOP
					half2 uv = i.uv2;
					half2 stereoUV = i.stereoUV2;
				#else
					half2 uv = i.uv;
					half2 stereoUV = i.stereoUV;
				#endif	
				
				half4 finalColor;

				// ase common template code
				float2 uv_MainTex = i.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float4 Camera30 = tex2D( _MainTex, uv_MainTex );
				float2 uv_Noise = i.uv.xy * _Noise_ST.xy + _Noise_ST.zw;
				float2 uv_Gradient = i.uv.xy * _Gradient_ST.xy + _Gradient_ST.zw;
				float temp_output_44_0 = saturate( ( tex2D( _Noise, uv_Noise ).b * ( ( 1.0 - tex2D( _Gradient, uv_Gradient ).r ) - _GradientFix ) * _NoiseIntensity ) );
				float4 color24 = IsGammaSpace() ? float4(0.4056604,0,0,0) : float4(0.13687,0,0,0);
				float4 lerpResult50 = lerp( Camera30 , ( temp_output_44_0 * color24 ) , ( temp_output_44_0 * _HitFXIntensity ));
				float lerpResult47 = lerp( 1.0 , (0.0 + (tex2D( _Gradient, uv_Gradient ).g - 0.0) * (0.85 - 0.0) / (1.0 - 0.0)) , _FocusIntensity);
				

				finalColor = ( lerpResult50 * lerpResult47 );

				return finalColor;
			} 
			ENDCG 
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=16900
751;73;885;656;485.6266;1222.441;3.214792;False;False
Node;AmplifyShaderEditor.CommentaryNode;15;-576,-320;Float;False;1327.711;451.2196;Hit Effect;14;24;44;41;38;43;39;42;40;45;49;50;54;58;57;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;38;-557.1196,-64.74612;Float;True;Property;_Gradient;Gradient;0;0;Create;True;0;0;False;0;6d138cf9083634449b8572f1be95a399;6d138cf9083634449b8572f1be95a399;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;39;-259.1196,-48.75647;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;43;-254.1196,28.24353;Float;False;Property;_GradientFix;Gradient Fix;3;0;Create;True;0;0;False;0;0.75;0.37;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;14;-571.7048,155.3775;Float;False;907;421;Focus Effect;6;8;12;11;10;28;47;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;28;-521.7048,205.3775;Float;True;Property;_TextureSample1;Texture Sample 1;0;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Instance;38;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;11;-505.7048,461.3774;Float;False;Constant;_FocusMaximumwhite;Focus Maximum white;1;0;Create;True;0;0;False;0;0.85;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-505.7048,397.3774;Float;False;Constant;_FocusMaximumblack;Focus Maximum black;1;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;54;-192,-128;Float;False;Property;_NoiseIntensity;Noise Intensity;4;0;Create;True;0;0;False;0;0.03;1.31;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;1;-583.454,-581.5986;Float;False;0;0;_MainTex;Shader;0;5;SAMPLER2D;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;40;-563.1196,-268.7565;Float;True;Property;_Noise;Noise;2;0;Create;True;0;0;False;0;None;adabe9ddbe5480a4191c8823f4785a22;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;42;-74.27576,-49.60947;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;8;-137.7048,237.3775;Float;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-140.9176,469.6661;Float;False;Property;_FocusIntensity;Focus Intensity;1;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;80,-192;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;3;-455.4538,-581.5986;Float;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;47;178.6125,234.141;Float;False;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;44;208,-96;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;30;-160,-576;Float;False;Camera;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;24;96,-32;Float;False;Constant;_HitEffectColor;Hit Effect Color;0;0;Create;True;0;0;False;0;0.4056604,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;58;288,48;Float;False;Property;_HitFXIntensity;Hit FX Intensity;5;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;57;576,32;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;352,-48;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;49;256,-240;Float;False;30;Camera;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;52;753.8157,262.7769;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;50;560,-144;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;51;777.5851,244.2895;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;880,80;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;1024,80;Float;False;True;2;Float;ASEMaterialInspector;0;7;Hidden/Templates/Legacy/PostProcess;c71b220b631b6344493ea3cf87110c93;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;1;False;False;False;True;2;False;-1;False;False;True;2;False;-1;True;7;False;-1;False;True;0;False;0;False;False;False;False;False;False;False;False;False;False;True;2;0;;0;0;Standard;0;0;1;True;False;1;0;FLOAT4;0,0,0,0;False;0
WireConnection;39;0;38;1
WireConnection;42;0;39;0
WireConnection;42;1;43;0
WireConnection;8;0;28;2
WireConnection;8;3;10;0
WireConnection;8;4;11;0
WireConnection;41;0;40;3
WireConnection;41;1;42;0
WireConnection;41;2;54;0
WireConnection;3;0;1;0
WireConnection;47;1;8;0
WireConnection;47;2;12;0
WireConnection;44;0;41;0
WireConnection;30;0;3;0
WireConnection;57;0;44;0
WireConnection;57;1;58;0
WireConnection;45;0;44;0
WireConnection;45;1;24;0
WireConnection;52;0;47;0
WireConnection;50;0;49;0
WireConnection;50;1;45;0
WireConnection;50;2;57;0
WireConnection;51;0;52;0
WireConnection;9;0;50;0
WireConnection;9;1;51;0
WireConnection;0;0;9;0
ASEEND*/
//CHKSM=AAD562D6CAE6825A7B98228F9A5A1751E4E03C63