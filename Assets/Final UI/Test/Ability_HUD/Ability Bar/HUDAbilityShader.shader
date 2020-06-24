// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MyShaders/HUDAbilityGlow"
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
		_AbilityBar("AbilityBar", 2D) = "white" {}
		_BarColor("Bar Color", Color) = (0,0,0,0)
		_BeatRate("BeatRate", Range( 0 , 10)) = 0
		_Noise("Noise", 2D) = "white" {}
		_BlackExtraAmount("Black Extra Amount", Range( 0 , 1)) = 0
		_Flowmap("Flowmap", 2D) = "white" {}
		_UVDistortion("UV Distortion", Range( 0 , 1)) = 0
		_AbilityBar_InsideGlow("AbilityBar_InsideGlow", 2D) = "white" {}
		_InsideGlowColor("Inside Glow Color", Color) = (1,0,0,0)
		[IntRange]_InsideGlowOpacity("Inside Glow Opacity", Range( 0 , 1)) = 0
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
			uniform float4 _BarColor;
			uniform sampler2D _Noise;
			uniform sampler2D _Flowmap;
			uniform float4 _Flowmap_ST;
			uniform float _UVDistortion;
			uniform float _BlackExtraAmount;
			uniform sampler2D _AbilityBar_InsideGlow;
			uniform float4 _AbilityBar_InsideGlow_ST;
			uniform float _BeatRate;
			uniform float4 _InsideGlowColor;
			uniform float _InsideGlowOpacity;
			uniform sampler2D _AbilityBar;
			uniform float4 _AbilityBar_ST;
			
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
				float2 uv050 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 uv_Flowmap = IN.texcoord.xy * _Flowmap_ST.xy + _Flowmap_ST.zw;
				float4 lerpResult52 = lerp( float4( uv050, 0.0 , 0.0 ) , tex2D( _Flowmap, uv_Flowmap ) , _UVDistortion);
				float2 panner49 = ( 1.0 * _Time.y * float2( 0.01,0.2 ) + lerpResult52.rg);
				float4 lerpResult42 = lerp( _BarColor , float4( 0,0,0,0 ) , saturate( pow( tex2D( _Noise, panner49 ).r , (1.0 + (_BlackExtraAmount - 0.0) * (0.0 - 1.0) / (1.0 - 0.0)) ) ));
				float2 uv_AbilityBar_InsideGlow = IN.texcoord.xy * _AbilityBar_InsideGlow_ST.xy + _AbilityBar_InsideGlow_ST.zw;
				float mulTime60 = _Time.y * _BeatRate;
				float2 uv_AbilityBar = IN.texcoord.xy * _AbilityBar_ST.xy + _AbilityBar_ST.zw;
				float4 appendResult40 = (float4(( lerpResult42 + ( tex2D( _AbilityBar_InsideGlow, uv_AbilityBar_InsideGlow ).a * ( (0.0 + (sin( mulTime60 ) - -1.0) * (1.0 - 0.0) / (1.0 - -1.0)) * _InsideGlowColor ) * _InsideGlowOpacity ) ).rgb , tex2D( _AbilityBar, uv_AbilityBar ).a));
				
				half4 color = appendResult40;
				
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
653;133;734;549;756.9259;722.5527;2.397986;False;False
Node;AmplifyShaderEditor.RangedFloatNode;53;-1103.952,133.946;Float;False;Property;_UVDistortion;UV Distortion;6;0;Create;True;0;0;False;0;0;0.34;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;50;-1062.952,-180.054;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;51;-1121.952,-60.05405;Float;True;Property;_Flowmap;Flowmap;5;0;Create;True;0;0;False;0;None;8010c80413928474c9df9bddfbec2344;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;52;-780.9525,-93.05405;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector2Node;54;-777.9524,29.94595;Float;False;Constant;_Vector0;Vector 0;6;0;Create;True;0;0;False;0;0.01,0.2;0.18,0.12;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;59;-1143.982,591.0039;Float;False;Property;_BeatRate;BeatRate;2;0;Create;True;0;0;False;0;0;0;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;44;-438,63;Float;False;Property;_BlackExtraAmount;Black Extra Amount;4;0;Create;True;0;0;False;0;0;0.36;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;49;-592.9525,-140.054;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;60;-868.0768,585.8557;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;41;-300.6724,-134.6961;Float;True;Property;_Noise;Noise;3;0;Create;True;0;0;False;0;None;adabe9ddbe5480a4191c8823f4785a22;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SinOpNode;61;-692.4749,585.772;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;47;-140.9525,65.94595;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;64;-519.9822,592.0039;Float;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;58;-527.2395,795.1023;Float;False;Property;_InsideGlowColor;Inside Glow Color;8;0;Create;True;0;0;False;0;1,0,0,0;0.5754716,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;45;62.04755,-89.05405;Float;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;48;302,-90;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;36;201.9632,-302.7748;Float;False;Property;_BarColor;Bar Color;1;0;Create;True;0;0;False;0;0,0,0,0;0.443396,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;55;-700.5259,311.0661;Float;True;Property;_AbilityBar_InsideGlow;AbilityBar_InsideGlow;7;0;Create;True;0;0;False;0;6964863729d1ebd4c9dc5da6d4e375e4;c731af2f8590a5c4c8cd341bd52eb698;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;-263.9835,592.0039;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;69;-260.8194,811.7202;Float;False;Property;_InsideGlowOpacity;Inside Glow Opacity;9;1;[IntRange];Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;67;67.43553,591.7327;Float;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;42;502.0475,-199.054;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;35;501.6098,-406.019;Float;True;Property;_AbilityBar;AbilityBar;0;0;Create;True;0;0;False;0;ae3025498c072684dbc9c3734658dd7f;ae3025498c072684dbc9c3734658dd7f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;68;679.4167,5.082001;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;40;852.6018,-25.6329;Float;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;4;1106.827,-27.75243;Float;False;True;2;Float;ASEMaterialInspector;0;9;MyShaders/HUDAbilityGlow;5056123faa0c79b47ab6ad7e8bf059a4;True;Default;0;0;Default;2;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;True;2;False;-1;True;True;True;True;True;0;True;-9;True;True;0;True;-5;255;True;-8;255;True;-7;0;True;-4;0;True;-6;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;2;False;-1;True;0;False;-1;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;0;False;False;False;False;False;False;False;False;False;False;True;2;0;;0;0;Standard;0;0;1;True;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;52;0;50;0
WireConnection;52;1;51;0
WireConnection;52;2;53;0
WireConnection;49;0;52;0
WireConnection;49;2;54;0
WireConnection;60;0;59;0
WireConnection;41;1;49;0
WireConnection;61;0;60;0
WireConnection;47;0;44;0
WireConnection;64;0;61;0
WireConnection;45;0;41;1
WireConnection;45;1;47;0
WireConnection;48;0;45;0
WireConnection;65;0;64;0
WireConnection;65;1;58;0
WireConnection;67;0;55;4
WireConnection;67;1;65;0
WireConnection;67;2;69;0
WireConnection;42;0;36;0
WireConnection;42;2;48;0
WireConnection;68;0;42;0
WireConnection;68;1;67;0
WireConnection;40;0;68;0
WireConnection;40;3;35;4
WireConnection;4;0;40;0
ASEEND*/
//CHKSM=6698A27598029FA39CA30E98A7CF1AFDE2B77A4A