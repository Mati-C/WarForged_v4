// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "New Amplify Shader"
{
	Properties
	{
		_Rock_ALBBWv1K("Rock_ALB B&W v1K", 2D) = "white" {}
		_Rock_NORMv1K("Rock_NORM v1K", 2D) = "bump" {}
		_Rock_DISPv1K("Rock_DISP v1K", 2D) = "white" {}
		_Rock_AOv1K("Rock_AO v1K", 2D) = "white" {}
		_Tiling("Tiling", Float) = 1
		_HeightIntensity("Height Intensity", Float) = 0
		_OffsetHorizontal("Offset Horizontal", Range( 0 , 0.5)) = 0
		_OffsetVertical("Offset Vertical", Range( 0 , 0.5)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Rock_DISPv1K;
		uniform float _Tiling;
		uniform float _OffsetHorizontal;
		uniform float _OffsetVertical;
		uniform float _HeightIntensity;
		uniform sampler2D _Rock_NORMv1K;
		uniform sampler2D _Rock_ALBBWv1K;
		uniform sampler2D _Rock_AOv1K;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float2 appendResult13 = (float2(_Tiling , _Tiling));
			float2 appendResult20 = (float2(_OffsetHorizontal , _OffsetVertical));
			float2 uv_TexCoord11 = v.texcoord.xy * appendResult13 + appendResult20;
			float4 tex2DNode9 = tex2Dlod( _Rock_DISPv1K, float4( uv_TexCoord11, 0, 0.0) );
			float lerpResult15 = lerp( 0.0 , tex2DNode9.r , _HeightIntensity);
			float3 temp_cast_0 = (lerpResult15).xxx;
			v.vertex.xyz += temp_cast_0;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 appendResult13 = (float2(_Tiling , _Tiling));
			float2 appendResult20 = (float2(_OffsetHorizontal , _OffsetVertical));
			float2 uv_TexCoord11 = i.uv_texcoord * appendResult13 + appendResult20;
			o.Normal = UnpackNormal( tex2D( _Rock_NORMv1K, uv_TexCoord11 ) );
			float4 color6 = IsGammaSpace() ? float4(0.3679245,0.3343127,0.05380026,1) : float4(0.1114872,0.09139252,0.00428653,1);
			o.Albedo = ( color6 * tex2D( _Rock_ALBBWv1K, uv_TexCoord11 ) ).rgb;
			o.Occlusion = tex2D( _Rock_AOv1K, uv_TexCoord11 ).r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16900
1068;73;482;656;1694.391;-41.65949;1.190726;False;False
Node;AmplifyShaderEditor.RangedFloatNode;12;-1349.95,233.0386;Float;False;Property;_Tiling;Tiling;4;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-1449.104,394.1142;Float;False;Property;_OffsetVertical;Offset Vertical;7;0;Create;True;0;0;False;0;0;0;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-1449.104,326.2429;Float;False;Property;_OffsetHorizontal;Offset Horizontal;6;0;Create;True;0;0;False;0;0;0;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;13;-1199.95,231.0386;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;20;-1196.669,326.2429;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;11;-1004.747,178.039;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;5;-513.9216,21.68958;Float;True;Property;_Rock_ALBBWv1K;Rock_ALB B&W v1K;0;0;Create;True;0;0;False;0;c88478e0abf799a4190e54ddfffea5fa;c88478e0abf799a4190e54ddfffea5fa;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;6;-469.6343,-129.9906;Float;False;Constant;_Color0;Color 0;1;0;Create;True;0;0;False;0;0.3679245,0.3343127,0.05380026,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;9;-524.6343,414.0094;Float;True;Property;_Rock_DISPv1K;Rock_DISP v1K;2;0;Create;True;0;0;False;0;9729738d80bac9a4d977f5260cc8b42d;9729738d80bac9a4d977f5260cc8b42d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;16;-418.5778,903.959;Float;False;Property;_HeightIntensity;Height Intensity;5;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-191.6343,-18.99057;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;8;-524.6343,206.0094;Float;True;Property;_Rock_NORMv1K;Rock_NORM v1K;1;0;Create;True;0;0;False;0;8ebb3133c7416c041b0aa2fffe041bad;8ebb3133c7416c041b0aa2fffe041bad;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;10;-492.6343,685.0094;Float;True;Property;_Rock_AOv1K;Rock_AO v1K;3;0;Create;True;0;0;False;0;e65e4417544c672439c2f83084ba6e6c;e65e4417544c672439c2f83084ba6e6c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;15;-140.5778,659.959;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;New Amplify Shader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;13;0;12;0
WireConnection;13;1;12;0
WireConnection;20;0;18;0
WireConnection;20;1;19;0
WireConnection;11;0;13;0
WireConnection;11;1;20;0
WireConnection;5;1;11;0
WireConnection;9;1;11;0
WireConnection;7;0;6;0
WireConnection;7;1;5;0
WireConnection;8;1;11;0
WireConnection;10;1;11;0
WireConnection;15;1;9;1
WireConnection;15;2;16;0
WireConnection;0;0;7;0
WireConnection;0;1;8;0
WireConnection;0;5;10;1
WireConnection;0;11;15;0
ASEEND*/
//CHKSM=41733322E160D7147B58CDE06AB5AFEC4F1842CD