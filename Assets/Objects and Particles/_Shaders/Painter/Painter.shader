// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MyShaders/Painter"
{
	Properties
	{
		_Art_ALBDark("Art_ALB - Dark", 2D) = "white" {}
		_Art_ALBNature("Art_ALB - Nature", 2D) = "white" {}
		_Paint("Paint", Range( 0 , 1)) = 0
		_Art_NORM("Art_NORM", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Art_NORM;
		uniform float4 _Art_NORM_ST;
		uniform sampler2D _Art_ALBDark;
		uniform float4 _Art_ALBDark_ST;
		uniform sampler2D _Art_ALBNature;
		uniform float4 _Art_ALBNature_ST;
		uniform float _Paint;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Art_NORM = i.uv_texcoord * _Art_NORM_ST.xy + _Art_NORM_ST.zw;
			o.Normal = tex2D( _Art_NORM, uv_Art_NORM ).rgb;
			float2 uv_Art_ALBDark = i.uv_texcoord * _Art_ALBDark_ST.xy + _Art_ALBDark_ST.zw;
			float2 uv_Art_ALBNature = i.uv_texcoord * _Art_ALBNature_ST.xy + _Art_ALBNature_ST.zw;
			float4 lerpResult3 = lerp( tex2D( _Art_ALBDark, uv_Art_ALBDark ) , tex2D( _Art_ALBNature, uv_Art_ALBNature ) , ( 1.0 - _Paint ));
			o.Albedo = lerpResult3.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16900
675;73;496;656;1010.332;546.9618;1.64809;False;False
Node;AmplifyShaderEditor.RangedFloatNode;4;-910.6041,8.896193;Float;False;Property;_Paint;Paint;2;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-972.4137,-452.3852;Float;True;Property;_Art_ALBDark;Art_ALB - Dark;0;0;Create;True;0;0;False;0;4fe908e0002f7c840998b71521a7ad9f;4fe908e0002f7c840998b71521a7ad9f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-968.5133,-252.185;Float;True;Property;_Art_ALBNature;Art_ALB - Nature;1;0;Create;True;0;0;False;0;e0c9a47097e22604f9f6773e774a0f27;e0c9a47097e22604f9f6773e774a0f27;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;5;-610.7545,23.65471;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;3;-426.4616,-279.0887;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;6;-380.8118,9.89939;Float;True;Property;_Art_NORM;Art_NORM;3;0;Create;True;0;0;False;0;9e84020badfb50a479d2981c8caa0ea0;9e84020badfb50a479d2981c8caa0ea0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;MyShaders/Painter;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;5;0;4;0
WireConnection;3;0;1;0
WireConnection;3;1;2;0
WireConnection;3;2;5;0
WireConnection;0;0;3;0
WireConnection;0;1;6;0
ASEEND*/
//CHKSM=2FCB03F996CC5261AD3D9846831050935CC21606