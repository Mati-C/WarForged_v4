// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MyShaders/Landmark"
{
	Properties
	{
		_Landmarkv3_AlbedoTR("Landmark v3_AlbedoTR", 2D) = "white" {}
		_Landmarkv3_AO("Landmark v3_AO", 2D) = "white" {}
		_Landmarkv3_MetallicSM("Landmark v3_MetallicSM", 2D) = "white" {}
		_Landmarkv3_Normal("Landmark v3_Normal", 2D) = "white" {}
		_FillAmount("Fill Amount", Range( 0 , 1)) = 0
		_EmisionMask(" Emision Mask", 2D) = "white" {}
		_EmissionColor1("Emission Color 1", Color) = (0,0,0,0)
		_EmissionColor2("Emission Color 2", Color) = (0,0,0,0)
		_MaskFix("Mask Fix", Float) = 0
		_ColorLerp("Color Lerp", Range( 0 , 1)) = 0
		_EmissionIntensity("Emission Intensity", Range( 0 , 3)) = 0
		_MinY("Min Y", Float) = 0
		_MaxY("Max Y", Float) = 0
		_Gradient("Gradient", Float) = 42.8
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform sampler2D _Landmarkv3_Normal;
		uniform float4 _Landmarkv3_Normal_ST;
		uniform sampler2D _Landmarkv3_AlbedoTR;
		uniform float4 _Landmarkv3_AlbedoTR_ST;
		uniform float _FillAmount;
		uniform float _MinY;
		uniform float _MaxY;
		uniform float _Gradient;
		uniform sampler2D _EmisionMask;
		uniform float4 _EmisionMask_ST;
		uniform float _MaskFix;
		uniform float4 _EmissionColor1;
		uniform float4 _EmissionColor2;
		uniform float _ColorLerp;
		uniform float _EmissionIntensity;
		uniform sampler2D _Landmarkv3_MetallicSM;
		uniform float4 _Landmarkv3_MetallicSM_ST;
		uniform sampler2D _Landmarkv3_AO;
		uniform float4 _Landmarkv3_AO_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Landmarkv3_Normal = i.uv_texcoord * _Landmarkv3_Normal_ST.xy + _Landmarkv3_Normal_ST.zw;
			o.Normal = tex2D( _Landmarkv3_Normal, uv_Landmarkv3_Normal ).rgb;
			float2 uv_Landmarkv3_AlbedoTR = i.uv_texcoord * _Landmarkv3_AlbedoTR_ST.xy + _Landmarkv3_AlbedoTR_ST.zw;
			o.Albedo = tex2D( _Landmarkv3_AlbedoTR, uv_Landmarkv3_AlbedoTR ).rgb;
			float3 ase_worldPos = i.worldPos;
			float2 uv_EmisionMask = i.uv_texcoord * _EmisionMask_ST.xy + _EmisionMask_ST.zw;
			float4 lerpResult24 = lerp( _EmissionColor1 , _EmissionColor2 , _ColorLerp);
			o.Emission = saturate( ( saturate( ( 1.0 - ( ( ase_worldPos.y + (-_MinY + (_FillAmount - 0.0) * (-_MaxY - -_MinY) / (1.0 - 0.0)) ) * _Gradient ) ) ) * ( 1.0 - saturate( ( tex2D( _EmisionMask, uv_EmisionMask ).r - _MaskFix ) ) ) * saturate( ( lerpResult24 * _EmissionIntensity ) ) ) ).rgb;
			float2 uv_Landmarkv3_MetallicSM = i.uv_texcoord * _Landmarkv3_MetallicSM_ST.xy + _Landmarkv3_MetallicSM_ST.zw;
			o.Metallic = tex2D( _Landmarkv3_MetallicSM, uv_Landmarkv3_MetallicSM ).r;
			float2 uv_Landmarkv3_AO = i.uv_texcoord * _Landmarkv3_AO_ST.xy + _Landmarkv3_AO_ST.zw;
			o.Occlusion = tex2D( _Landmarkv3_AO, uv_Landmarkv3_AO ).r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16900
1036;73;603;651;1094.268;950.9788;1.832581;False;False
Node;AmplifyShaderEditor.RangedFloatNode;27;-928,-592;Float;False;Property;_MaxY;Max Y;12;0;Create;True;0;0;False;0;0;105;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;26;-944,-672;Float;False;Property;_MinY;Min Y;11;0;Create;True;0;0;False;0;0;100;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-1024,-512;Float;False;Property;_FillAmount;Fill Amount;4;0;Create;True;0;0;False;0;0;0.38;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;28;-768,-592;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;29;-768,-672;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;36;-640.9885,-539.6299;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;10;-592,-688;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-100;False;4;FLOAT;-105;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;5;-960.5635,-864.4033;Float;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;17;-578.3827,-251.5523;Float;False;Property;_MaskFix;Mask Fix;8;0;Create;True;0;0;False;0;0;-0.68;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;23;-960,-160;Float;False;Property;_EmissionColor2;Emission Color 2;7;0;Create;True;0;0;False;0;0,0,0,0;0,1,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;14;-960,-320;Float;False;Property;_EmissionColor1;Emission Color 1;6;0;Create;True;0;0;False;0;0,0,0,0;1,0.7778548,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;12;-698.3812,-459.328;Float;True;Property;_EmisionMask; Emision Mask;5;0;Create;True;0;0;False;0;f043182ab489c1c4cab15336c5d9ce0b;f043182ab489c1c4cab15336c5d9ce0b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;38;-815.0076,-953.9374;Float;False;Property;_Gradient;Gradient;13;0;Create;True;0;0;False;0;42.8;1.68;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-1024,0;Float;False;Property;_ColorLerp;Color Lerp;9;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;8;-416,-816;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;18;-370.3827,-427.5525;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-674.3827,4.447494;Float;False;Property;_EmissionIntensity;Emission Intensity;9;0;Create;True;0;0;False;0;0;2.02;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-288,-816;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;24;-608,-176;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;19;-226.3829,-427.5525;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;-370.3827,-171.5523;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;11;-144,-736;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;22;-210.3829,-155.5523;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;35;32,-736;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;16;-66.38282,-427.5525;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;173.6171,-459.5526;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;3;-368,352;Float;True;Property;_Landmarkv3_MetallicSM;Landmark v3_MetallicSM;2;0;Create;True;0;0;False;0;0e58a76e30f0cf34b8ada8936bebffe7;0e58a76e30f0cf34b8ada8936bebffe7;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-368,-32;Float;True;Property;_Landmarkv3_AlbedoTR;Landmark v3_AlbedoTR;0;0;Create;True;0;0;False;0;af74fd7c4aad0dc4e8affa7a949fd90e;af74fd7c4aad0dc4e8affa7a949fd90e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;15;333.6173,-459.5526;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;4;-368,160;Float;True;Property;_Landmarkv3_Normal;Landmark v3_Normal;3;0;Create;True;0;0;False;0;c0e45f83643fc404e96422b3f31c2c99;c0e45f83643fc404e96422b3f31c2c99;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-368,544;Float;True;Property;_Landmarkv3_AO;Landmark v3_AO;1;0;Create;True;0;0;False;0;857ea839299c02448be64dfccb9c4546;857ea839299c02448be64dfccb9c4546;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;704,48;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;MyShaders/Landmark;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;28;0;27;0
WireConnection;29;0;26;0
WireConnection;36;0;9;0
WireConnection;10;0;36;0
WireConnection;10;3;29;0
WireConnection;10;4;28;0
WireConnection;8;0;5;2
WireConnection;8;1;10;0
WireConnection;18;0;12;1
WireConnection;18;1;17;0
WireConnection;37;0;8;0
WireConnection;37;1;38;0
WireConnection;24;0;14;0
WireConnection;24;1;23;0
WireConnection;24;2;25;0
WireConnection;19;0;18;0
WireConnection;20;0;24;0
WireConnection;20;1;21;0
WireConnection;11;0;37;0
WireConnection;22;0;20;0
WireConnection;35;0;11;0
WireConnection;16;0;19;0
WireConnection;13;0;35;0
WireConnection;13;1;16;0
WireConnection;13;2;22;0
WireConnection;15;0;13;0
WireConnection;0;0;1;0
WireConnection;0;1;4;0
WireConnection;0;2;15;0
WireConnection;0;3;3;1
WireConnection;0;5;2;1
ASEEND*/
//CHKSM=5E70F06A64E38D29CF97066A369B004E31E1BC4B