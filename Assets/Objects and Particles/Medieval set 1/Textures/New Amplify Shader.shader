// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MyShaders/Medieval Set 1"
{
	Properties
	{
		_Medievalset1_MedievalSet1_AlbedoTR("Medieval set 1_Medieval Set 1_AlbedoTR", 2D) = "white" {}
		_Medievalset1_MedievalSet1_MetallicSmth("Medieval set 1_Medieval Set 1_MetallicSmth", 2D) = "white" {}
		_Medievalset1_MedievalSet1_Normal("Medieval set 1_Medieval Set 1_Normal", 2D) = "bump" {}
		_Color("Color", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Off
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Medievalset1_MedievalSet1_Normal;
		uniform float4 _Medievalset1_MedievalSet1_Normal_ST;
		uniform sampler2D _Medievalset1_MedievalSet1_AlbedoTR;
		uniform float4 _Medievalset1_MedievalSet1_AlbedoTR_ST;
		uniform float4 _Color;
		uniform sampler2D _Medievalset1_MedievalSet1_MetallicSmth;
		uniform float4 _Medievalset1_MedievalSet1_MetallicSmth_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Medievalset1_MedievalSet1_Normal = i.uv_texcoord * _Medievalset1_MedievalSet1_Normal_ST.xy + _Medievalset1_MedievalSet1_Normal_ST.zw;
			o.Normal = UnpackNormal( tex2D( _Medievalset1_MedievalSet1_Normal, uv_Medievalset1_MedievalSet1_Normal ) );
			float2 uv_Medievalset1_MedievalSet1_AlbedoTR = i.uv_texcoord * _Medievalset1_MedievalSet1_AlbedoTR_ST.xy + _Medievalset1_MedievalSet1_AlbedoTR_ST.zw;
			o.Albedo = ( tex2D( _Medievalset1_MedievalSet1_AlbedoTR, uv_Medievalset1_MedievalSet1_AlbedoTR ) * _Color ).rgb;
			float2 uv_Medievalset1_MedievalSet1_MetallicSmth = i.uv_texcoord * _Medievalset1_MedievalSet1_MetallicSmth_ST.xy + _Medievalset1_MedievalSet1_MetallicSmth_ST.zw;
			o.Metallic = tex2D( _Medievalset1_MedievalSet1_MetallicSmth, uv_Medievalset1_MedievalSet1_MetallicSmth ).r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16900
118;407;1235;370;1364.659;-75.84077;1.480152;True;False
Node;AmplifyShaderEditor.SamplerNode;1;-428.0165,-64.29775;Float;True;Property;_Medievalset1_MedievalSet1_AlbedoTR;Medieval set 1_Medieval Set 1_AlbedoTR;0;0;Create;True;0;0;False;0;None;dfe4014189bd02d4c8dd39d4f076e6e2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;6;-346.34,340.0703;Float;False;Property;_Color;Color;4;0;Create;True;0;0;False;0;0,0,0,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-292.6012,482.7936;Float;True;Property;_Medievalset1_MedievalSet1_MetallicSmth;Medieval set 1_Medieval Set 1_MetallicSmth;2;0;Create;True;0;0;False;0;None;b63e31894c144814581001893c99a8ea;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;-297.2014,678.5122;Float;True;Property;_Medievalset1_MedievalSet1_Normal;Medieval set 1_Medieval Set 1_Normal;3;0;Create;True;0;0;False;0;None;81b9a24a94cf2904ba67547e1743dfb6;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-292.8548,100.9257;Float;True;Property;_Medievalset1_MedievalSet1_Height;Medieval set 1_Medieval Set 1_Height;1;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;51.73318,223.0776;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;336.1925,305.2545;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;MyShaders/Medieval Set 1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;5;0;1;0
WireConnection;5;1;6;0
WireConnection;0;0;5;0
WireConnection;0;1;4;0
WireConnection;0;3;3;1
ASEEND*/
//CHKSM=017C0819E9B90A85AB108D88091DBC18583B344C