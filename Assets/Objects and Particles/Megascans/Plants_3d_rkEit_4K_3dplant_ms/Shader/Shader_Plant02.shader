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
		uniform sampler2D _Billboard_4K_Translucency;
		uniform float4 _Billboard_4K_Translucency_ST;
		uniform sampler2D _Billboard_4K_AO;
		uniform float4 _Billboard_4K_AO_ST;
		uniform sampler2D _Billboard_4K_Opacity;
		uniform float4 _Billboard_4K_Opacity_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Billboard_4K_Normal = i.uv_texcoord * _Billboard_4K_Normal_ST.xy + _Billboard_4K_Normal_ST.zw;
			o.Normal = UnpackNormal( tex2D( _Billboard_4K_Normal, uv_Billboard_4K_Normal ) );
			float2 uv_Billboard_4K_Albedo = i.uv_texcoord * _Billboard_4K_Albedo_ST.xy + _Billboard_4K_Albedo_ST.zw;
			o.Albedo = tex2D( _Billboard_4K_Albedo, uv_Billboard_4K_Albedo ).rgb;
			float2 uv_Billboard_4K_Translucency = i.uv_texcoord * _Billboard_4K_Translucency_ST.xy + _Billboard_4K_Translucency_ST.zw;
			o.Emission = tex2D( _Billboard_4K_Translucency, uv_Billboard_4K_Translucency ).rgb;
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
346;291;1235;370;1339.125;537.3621;2.509888;True;False
Node;AmplifyShaderEditor.SamplerNode;1;-539.8735,-557.7021;Float;True;Property;_Billboard_4K_Albedo;Billboard_4K_Albedo;0;0;Create;True;0;0;False;0;44204e8735634fc49b4ccc83c23fa7ae;44204e8735634fc49b4ccc83c23fa7ae;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-505.8736,-349.7022;Float;True;Property;_Billboard_4K_Normal;Billboard_4K_Normal;1;0;Create;True;0;0;False;0;5b462dc988c8cc74abb4623c637b65b2;5b462dc988c8cc74abb4623c637b65b2;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;6;-495.5538,-105.7824;Float;True;Property;_Billboard_4K_Translucency;Billboard_4K_Translucency;3;0;Create;True;0;0;False;0;7e5884d03f737234590611f5a2c06464;7e5884d03f737234590611f5a2c06464;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;5;-495.9531,103.382;Float;True;Property;_Billboard_4K_AO;Billboard_4K_AO;2;0;Create;True;0;0;False;0;829523ffe2702fe41a629ab1216a71c9;829523ffe2702fe41a629ab1216a71c9;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;7;-298.0564,341.9415;Float;True;Property;_Billboard_4K_Opacity;Billboard_4K_Opacity;4;0;Create;True;0;0;False;0;0798e3f050f7b8a4cbbcf6c605ddb651;0798e3f050f7b8a4cbbcf6c605ddb651;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;4;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;MyShaders/ClimberPlant01;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;4;0;1;0
WireConnection;4;1;2;0
WireConnection;4;2;6;0
WireConnection;4;5;5;0
WireConnection;4;9;7;0
ASEEND*/
//CHKSM=A05E1162E6FCFC892D4504859DF94CEAFF608F82