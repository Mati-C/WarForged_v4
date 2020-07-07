// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MyShaders/ClimberPlant01"
{
	Properties
	{
		_Billboard_4K_Albedo("Billboard_4K_Albedo", 2D) = "white" {}
		_Billboard_4K_Normal("Billboard_4K_Normal", 2D) = "bump" {}
		_Billboard_4K_AO("Billboard_4K_AO", 2D) = "white" {}
		_Billboard_4K_Translucency("Billboard_4K_Translucency", 2D) = "white" {}
		_Billboard_4K_Opacity("Billboard_4K_Opacity", 2D) = "white" {}
		_Color("Color", Color) = (0.764151,0.764151,0.764151,0)
		_Intencity("Intencity", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard alpha:fade keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Billboard_4K_Normal;
		uniform float4 _Billboard_4K_Normal_ST;
		uniform sampler2D _Billboard_4K_Albedo;
		uniform float4 _Billboard_4K_Albedo_ST;
		uniform float4 _Color;
		uniform sampler2D _Billboard_4K_Translucency;
		uniform float4 _Billboard_4K_Translucency_ST;
		uniform float _Intencity;
		uniform sampler2D _Billboard_4K_AO;
		uniform float4 _Billboard_4K_AO_ST;
		uniform sampler2D _Billboard_4K_Opacity;
		uniform float4 _Billboard_4K_Opacity_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Billboard_4K_Normal = i.uv_texcoord * _Billboard_4K_Normal_ST.xy + _Billboard_4K_Normal_ST.zw;
			o.Normal = UnpackNormal( tex2D( _Billboard_4K_Normal, uv_Billboard_4K_Normal ) );
			float2 uv_Billboard_4K_Albedo = i.uv_texcoord * _Billboard_4K_Albedo_ST.xy + _Billboard_4K_Albedo_ST.zw;
			o.Albedo = ( tex2D( _Billboard_4K_Albedo, uv_Billboard_4K_Albedo ) * _Color ).rgb;
			float2 uv_Billboard_4K_Translucency = i.uv_texcoord * _Billboard_4K_Translucency_ST.xy + _Billboard_4K_Translucency_ST.zw;
			o.Emission = ( tex2D( _Billboard_4K_Translucency, uv_Billboard_4K_Translucency ) * _Intencity ).rgb;
			float2 uv_Billboard_4K_AO = i.uv_texcoord * _Billboard_4K_AO_ST.xy + _Billboard_4K_AO_ST.zw;
			o.Occlusion = tex2D( _Billboard_4K_AO, uv_Billboard_4K_AO ).r;
			float2 uv_Billboard_4K_Opacity = i.uv_texcoord * _Billboard_4K_Opacity_ST.xy + _Billboard_4K_Opacity_ST.zw;
			o.Alpha = tex2D( _Billboard_4K_Opacity, uv_Billboard_4K_Opacity ).r;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16900
67;415;1235;370;928.4396;157.5683;1;True;False
Node;AmplifyShaderEditor.SamplerNode;1;-579.043,-796.4563;Float;True;Property;_Billboard_4K_Albedo;Billboard_4K_Albedo;0;0;Create;True;0;0;False;0;None;579fae0d9ffe9b64b8a2c97b74045d3e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;9;-494.5226,-568.916;Float;False;Property;_Color;Color;5;0;Create;True;0;0;False;0;0.764151,0.764151,0.764151,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;6;-540.7532,-150.9817;Float;True;Property;_Billboard_4K_Translucency;Billboard_4K_Translucency;3;0;Create;True;0;0;False;0;None;1d84d4db8930b0b4b99c068251ef9382;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;14;-446.1082,51.43417;Float;False;Property;_Intencity;Intencity;6;0;Create;True;0;0;False;0;0;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-505.8736,-349.7022;Float;True;Property;_Billboard_4K_Normal;Billboard_4K_Normal;1;0;Create;True;0;0;False;0;None;3a6ec707fb3ac56449e278588724fe30;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;7;-298.0564,341.9415;Float;True;Property;_Billboard_4K_Opacity;Billboard_4K_Opacity;4;0;Create;True;0;0;False;0;None;50b88e8de5de417408a7bb9cf1598316;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-108.6393,-586.5509;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;5;-497.4597,268.7939;Float;True;Property;_Billboard_4K_AO;Billboard_4K_AO;2;0;Create;True;0;0;False;0;None;ca957be88cdac1a449c1cf945c541dfd;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-138.7529,-168.536;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;4;315.9534,-415.0236;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;MyShaders/ClimberPlant01;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;12;0;1;0
WireConnection;12;1;9;0
WireConnection;13;0;6;0
WireConnection;13;1;14;0
WireConnection;4;0;12;0
WireConnection;4;1;2;0
WireConnection;4;2;13;0
WireConnection;4;5;5;0
WireConnection;4;9;7;0
ASEEND*/
//CHKSM=F8A9354BD4E454E2D3EC2E148762FAD44952F9F0