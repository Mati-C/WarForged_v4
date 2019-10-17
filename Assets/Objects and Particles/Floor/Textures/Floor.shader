// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MyShaders/Floor"
{
	Properties
	{
		_FloorAlbedoTR("Floor AlbedoTR", 2D) = "white" {}
		_FloorNormal("Floor Normal", 2D) = "bump" {}
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 4.6
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _FloorNormal;
		uniform float4 _FloorNormal_ST;
		uniform sampler2D _FloorAlbedoTR;
		uniform float4 _FloorAlbedoTR_ST;
		uniform float _Metallic;
		uniform float _Smoothness;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_FloorNormal = i.uv_texcoord * _FloorNormal_ST.xy + _FloorNormal_ST.zw;
			o.Normal = UnpackNormal( tex2D( _FloorNormal, uv_FloorNormal ) );
			float2 uv_FloorAlbedoTR = i.uv_texcoord * _FloorAlbedoTR_ST.xy + _FloorAlbedoTR_ST.zw;
			o.Albedo = tex2D( _FloorAlbedoTR, uv_FloorAlbedoTR ).rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15301
811;116;1025;722;721.9646;-130.631;1;False;False
Node;AmplifyShaderEditor.RangedFloatNode;9;-429,696.5;Float;False;Property;_TesselIntensity;Tessel Intensity;5;0;Create;True;0;0;False;0;0;0.039;0;0.25;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;7;-485,365.5;Float;True;Property;_FloorHeight;Floor Height;2;0;Create;True;0;0;False;0;bb5a8b39cc505834e927e514435055c7;ef68cac9fc651b244866f8274f0c0c36;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector3Node;8;-391,553.5;Float;False;Constant;_TesselDirection;Tessel Direction;5;0;Create;True;0;0;False;0;0,0,0.1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WireNode;11;-166,518.5;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-458,-178.5;Float;True;Property;_FloorAlbedoTR;Floor AlbedoTR;0;0;Create;True;0;0;False;0;a39c29c2123fdef4ab3276cb13deaee5;2f999bd36a0e9c746b65c5328d0bc240;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-455,15.5;Float;True;Property;_FloorNormal;Floor Normal;1;0;Create;True;0;0;False;0;bc0bfca4abef0a742ada32e00075499a;bea31258aa70e6e48ae3eb79f2f48f21;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;4;-434,209.5;Float;False;Property;_Metallic;Metallic;3;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-434,282.5;Float;False;Property;_Smoothness;Smoothness;4;0;Create;True;0;0;False;0;0;0.35;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-143,368.5;Float;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;15,-3;Float;False;True;6;Float;ASEMaterialInspector;0;0;Standard;MyShaders/Floor;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;0;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;1;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;-1;False;-1;-1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;0;0;0;0;False;0;0;0;False;-1;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;11;0;9;0
WireConnection;10;0;7;0
WireConnection;10;1;8;0
WireConnection;10;2;11;0
WireConnection;0;0;1;0
WireConnection;0;1;3;0
WireConnection;0;3;4;0
WireConnection;0;4;6;0
ASEEND*/
//CHKSM=1EE82F132D250303F637101734FEFF11D15FFECF