// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MyShaders/Bars01"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Cuttingperfection("Cutting perfection", Float) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_Position("Position", Float) = 0
		_Barsv2("Bars v2", 2D) = "white" {}
		_OpacityCutout("Opacity Cutout", Float) = 2
		_CastleDoorIronBars_02Default_AO("Castle Door (Iron Bars)_02 - Default_AO", 2D) = "white" {}
		_CastleDoorIronBars_02Default_Normal("Castle Door (Iron Bars)_02 - Default_Normal", 2D) = "bump" {}
		_CastleDoorIronBars_02Default_MetallicSmth("Castle Door (Iron Bars)_02 - Default_MetallicSmth", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Geometry+0" }
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma target 4.6
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform sampler2D _CastleDoorIronBars_02Default_Normal;
		uniform float4 _CastleDoorIronBars_02Default_Normal_ST;
		uniform float _Cuttingperfection;
		uniform float _Position;
		uniform sampler2D _Barsv2;
		uniform float4 _Barsv2_ST;
		uniform sampler2D _CastleDoorIronBars_02Default_MetallicSmth;
		uniform float4 _CastleDoorIronBars_02Default_MetallicSmth_ST;
		uniform float _Smoothness;
		uniform sampler2D _CastleDoorIronBars_02Default_AO;
		uniform float4 _CastleDoorIronBars_02Default_AO_ST;
		uniform float _OpacityCutout;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_CastleDoorIronBars_02Default_Normal = i.uv_texcoord * _CastleDoorIronBars_02Default_Normal_ST.xy + _CastleDoorIronBars_02Default_Normal_ST.zw;
			float3 NORMAL33 = UnpackNormal( tex2D( _CastleDoorIronBars_02Default_Normal, uv_CastleDoorIronBars_02Default_Normal ) );
			o.Normal = NORMAL33;
			float3 ase_worldPos = i.worldPos;
			float temp_output_24_0 = saturate( ( ( ase_worldPos.y * _Cuttingperfection ) + _Position ) );
			float2 uv_Barsv2 = i.uv_texcoord * _Barsv2_ST.xy + _Barsv2_ST.zw;
			float4 tex2DNode26 = tex2D( _Barsv2, uv_Barsv2 );
			o.Albedo = ( temp_output_24_0 * tex2DNode26 ).rgb;
			float2 uv_CastleDoorIronBars_02Default_MetallicSmth = i.uv_texcoord * _CastleDoorIronBars_02Default_MetallicSmth_ST.xy + _CastleDoorIronBars_02Default_MetallicSmth_ST.zw;
			float4 METALIC35 = tex2D( _CastleDoorIronBars_02Default_MetallicSmth, uv_CastleDoorIronBars_02Default_MetallicSmth );
			o.Metallic = METALIC35.r;
			o.Smoothness = _Smoothness;
			float2 uv_CastleDoorIronBars_02Default_AO = i.uv_texcoord * _CastleDoorIronBars_02Default_AO_ST.xy + _CastleDoorIronBars_02Default_AO_ST.zw;
			float4 OCC31 = tex2D( _CastleDoorIronBars_02Default_AO, uv_CastleDoorIronBars_02Default_AO );
			o.Occlusion = OCC31.r;
			o.Alpha = 1;
			clip( saturate( ( tex2DNode26.r * _OpacityCutout * temp_output_24_0 ) ) - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16900
685;130;1235;746;496.1259;133.2808;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;17;-1008,320;Float;False;Property;_Cuttingperfection;Cutting perfection;1;0;Create;True;0;0;False;0;0;-611.32;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;14;-1072,48;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;22;-1053,224;Float;False;Property;_Position;Position;3;0;Create;True;0;0;False;0;0;69998;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-774,94;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;18;-384,96;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-82.33456,591.7258;Float;False;Property;_OpacityCutout;Opacity Cutout;5;0;Create;True;0;0;False;0;2;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;24;-113.5928,150.9295;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;34;-428.1971,-167.6129;Float;True;Property;_CastleDoorIronBars_02Default_MetallicSmth;Castle Door (Iron Bars)_02 - Default_MetallicSmth;8;0;Create;True;0;0;False;0;284c9eb93e70d5049bbba11f963114df;284c9eb93e70d5049bbba11f963114df;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;30;-433.397,-366.5128;Float;True;Property;_CastleDoorIronBars_02Default_AO;Castle Door (Iron Bars)_02 - Default_AO;6;0;Create;True;0;0;False;0;513d4885deaee3a40b5f3d0c8965f4a3;513d4885deaee3a40b5f3d0c8965f4a3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;26;-372.5708,332.7099;Float;True;Property;_Barsv2;Bars v2;4;0;Create;True;0;0;False;0;None;a215cf017f5c66842997b2feba728ee9;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;32;-429.4972,-553.7128;Float;True;Property;_CastleDoorIronBars_02Default_Normal;Castle Door (Iron Bars)_02 - Default_Normal;7;0;Create;True;0;0;False;0;5635f4b5efc68ad44bf72eb935a30607;5635f4b5efc68ad44bf72eb935a30607;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;33;-113.5971,-553.7127;Float;False;NORMAL;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;31;-118.7972,-366.5127;Float;False;OCC;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;116.6654,446.7258;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;35;-122.697,-166.313;Float;False;METALIC;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;50.69995,74.23566;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;20;-617,232;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;39;98.87411,222.7192;Float;False;Property;_Smoothness;Smoothness;2;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;37;206.8741,314.7192;Float;False;31;OCC;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;38;193.8741,114.7192;Float;False;35;METALIC;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;21;-770,302;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;36;189.9028,-9.312744;Float;False;33;NORMAL;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;29;256,448;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;432,73;Float;False;True;6;Float;ASEMaterialInspector;0;0;Standard;MyShaders/Bars01;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Transparent;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;2;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;5;False;-1;10;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;16;0;14;2
WireConnection;16;1;17;0
WireConnection;18;0;16;0
WireConnection;18;1;22;0
WireConnection;24;0;18;0
WireConnection;33;0;32;0
WireConnection;31;0;30;0
WireConnection;27;0;26;1
WireConnection;27;1;28;0
WireConnection;27;2;24;0
WireConnection;35;0;34;0
WireConnection;25;0;24;0
WireConnection;25;1;26;0
WireConnection;20;0;22;0
WireConnection;20;3;21;0
WireConnection;20;4;17;0
WireConnection;21;0;17;0
WireConnection;29;0;27;0
WireConnection;0;0;25;0
WireConnection;0;1;36;0
WireConnection;0;3;38;0
WireConnection;0;4;39;0
WireConnection;0;5;37;0
WireConnection;0;10;29;0
ASEEND*/
//CHKSM=9DA1B5DFB05322AA62A677F5672B9CE6AC27E050