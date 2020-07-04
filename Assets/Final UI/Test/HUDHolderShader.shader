// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MyShaders/TempHUDHolderShader"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
		_BaseTexture("Base Texture", 2D) = "white" {}
		_BeatRate("BeatRate", Range( 0 , 10)) = 0
		_BeatColorIntensity("BeatColorIntensity", Range( 0 , 1)) = 0
		_BeatColor("BeatColor", Color) = (1,0,0,0)
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_InnerGlow("Inner Glow", Range( 0 , 1)) = 0
		_Noise("Noise", 2D) = "white" {}
		_Flowmap("Flowmap", 2D) = "white" {}
		_WavesIntensity("Waves Intensity", Float) = 5
		_UVDistortion("UV Distortion", Range( 0 , 1)) = 0.8
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
		
		Stencil
		{
			Ref [_Stencil]
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
			CompFront [_StencilComp]
			PassFront [_StencilOp]
			FailFront Keep
			ZFailFront Keep
			CompBack Always
			PassBack Keep
			FailBack Keep
			ZFailBack Keep
		}


		Cull Off
		Lighting Off
		ZWrite Off
		ZTest LEqual
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		
		Pass
		{
			Name "Default"
		CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			#pragma multi_compile __ UNITY_UI_CLIP_RECT
			#pragma multi_compile __ UNITY_UI_ALPHACLIP
			
			#include "UnityShaderVariables.cginc"

			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				
			};
			
			uniform fixed4 _Color;
			uniform fixed4 _TextureSampleAdd;
			uniform float4 _ClipRect;
			uniform sampler2D _MainTex;
			uniform float _InnerGlow;
			uniform sampler2D _TextureSample0;
			uniform float4 _TextureSample0_ST;
			uniform sampler2D _BaseTexture;
			uniform float4 _BaseTexture_ST;
			uniform sampler2D _Noise;
			uniform sampler2D _Flowmap;
			uniform float4 _Flowmap_ST;
			uniform float _UVDistortion;
			uniform float _WavesIntensity;
			uniform float _BeatRate;
			uniform float4 _BeatColor;
			uniform float _BeatColorIntensity;
			
			v2f vert( appdata_t IN  )
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID( IN );
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
				OUT.worldPosition = IN.vertex;
				
				
				OUT.worldPosition.xyz +=  float3( 0, 0, 0 ) ;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				OUT.texcoord = IN.texcoord;
				
				OUT.color = IN.color * _Color;
				return OUT;
			}

			fixed4 frag(v2f IN  ) : SV_Target
			{
				float2 uv_TextureSample0 = IN.texcoord.xy * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
				float2 uv_BaseTexture = IN.texcoord.xy * _BaseTexture_ST.xy + _BaseTexture_ST.zw;
				float2 uv024 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 uv_Flowmap = IN.texcoord.xy * _Flowmap_ST.xy + _Flowmap_ST.zw;
				float4 lerpResult31 = lerp( float4( uv024, 0.0 , 0.0 ) , tex2D( _Flowmap, uv_Flowmap ) , _UVDistortion);
				float2 panner23 = ( 1.0 * _Time.y * float2( 0.15,0 ) + lerpResult31.rg);
				float4 temp_cast_2 = (saturate( pow( ( 1.0 - tex2D( _Noise, panner23 ).r ) , _WavesIntensity ) )).xxxx;
				float4 blendOpSrc34 = tex2D( _BaseTexture, uv_BaseTexture );
				float4 blendOpDest34 = temp_cast_2;
				float mulTime11 = _Time.y * _BeatRate;
				
				half4 color = saturate( ( ( _InnerGlow * tex2D( _TextureSample0, uv_TextureSample0 ) ) + ( saturate( ( blendOpSrc34 * blendOpDest34 ) )) + ( (0.0 + (sin( mulTime11 ) - -1.0) * (1.0 - 0.0) / (1.0 - -1.0)) * _BeatColor * _BeatColorIntensity ) ) );
				
				#ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif
				
				#ifdef UNITY_UI_ALPHACLIP
				clip (color.a - 0.001);
				#endif

				return color;
			}
		ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=16900
7;1;480;543;-194.3753;387.6446;1.38018;True;True
Node;AmplifyShaderEditor.TextureCoordinatesNode;24;-1215.615,-222.6005;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;32;-1244.399,109.3615;Float;False;Property;_UVDistortion;UV Distortion;9;0;Create;True;0;0;False;0;0.8;0.8;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;30;-1265.27,-91.43032;Float;True;Property;_Flowmap;Flowmap;7;0;Create;True;0;0;False;0;None;1903f5f8212195b4e9c128b41e6fd696;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;25;-873.3021,-29.34723;Float;False;Constant;_Vector0;Vector 0;8;0;Create;True;0;0;False;0;0.15,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.LerpOp;31;-948.3207,-179.5683;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.PannerNode;23;-660.4518,-64.03606;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-496,287;Float;False;Property;_BeatRate;BeatRate;1;0;Create;True;0;0;False;0;0;0;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;22;-424.2049,-89.65541;Float;True;Property;_Noise;Noise;6;0;Create;True;0;0;False;0;None;138ae4b08ba7f89438effbd67e97ccef;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;11;-220.0946,281.8518;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-139.8831,131.1809;Float;False;Property;_WavesIntensity;Waves Intensity;8;0;Create;True;0;0;False;0;5;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;26;-119.2963,-89.50471;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;10;-44.49225,281.7681;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;27;67.11679,-78.81876;Float;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;29;304.7969,-72.78281;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;6.596473,-296.1705;Float;True;Property;_BaseTexture;Base Texture;0;0;Create;True;0;0;False;0;None;850ce667e36b5844c8bf68b84373fec0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;15;107.9062,484.8523;Float;False;Property;_BeatColor;BeatColor;3;0;Create;True;0;0;False;0;1,0,0,0;1,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;16;48,656;Float;False;Property;_BeatColorIntensity;BeatColorIntensity;2;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-12.7863,-657.3914;Float;False;Property;_InnerGlow;Inner Glow;5;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;13;128,288;Float;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;19;-31.9341,-561.7031;Float;True;Property;_TextureSample0;Texture Sample 0;4;0;Create;True;0;0;False;0;None;ed3dd7842182f434486c188109f6711f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;358.5801,-567.4759;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;384,288;Float;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.BlendOpsNode;34;489.0439,-86.07538;Float;False;Multiply;True;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;17;768,-16;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;18;896,-16;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;4;1024,-32;Float;False;True;2;Float;ASEMaterialInspector;0;9;MyShaders/TempHUDHolderShader;5056123faa0c79b47ab6ad7e8bf059a4;True;Default;0;0;Default;2;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;True;2;False;-1;True;True;True;True;True;0;True;-9;True;True;0;True;-5;255;True;-8;255;True;-7;0;True;-4;0;True;-6;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;2;False;-1;True;0;False;-1;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;0;False;False;False;False;False;False;False;False;False;False;True;2;0;;0;0;Standard;0;0;1;True;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;31;0;24;0
WireConnection;31;1;30;0
WireConnection;31;2;32;0
WireConnection;23;0;31;0
WireConnection;23;2;25;0
WireConnection;22;1;23;0
WireConnection;11;0;12;0
WireConnection;26;0;22;1
WireConnection;10;0;11;0
WireConnection;27;0;26;0
WireConnection;27;1;28;0
WireConnection;29;0;27;0
WireConnection;13;0;10;0
WireConnection;21;0;20;0
WireConnection;21;1;19;0
WireConnection;14;0;13;0
WireConnection;14;1;15;0
WireConnection;14;2;16;0
WireConnection;34;0;1;0
WireConnection;34;1;29;0
WireConnection;17;0;21;0
WireConnection;17;1;34;0
WireConnection;17;2;14;0
WireConnection;18;0;17;0
WireConnection;4;0;18;0
ASEEND*/
//CHKSM=716E3C074881C26E585F442DE93D644D9ABD3659