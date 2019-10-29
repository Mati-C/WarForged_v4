// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "N_Shaders/FireHandEffect"
{
	Properties
	{
		_BURN("BURN", Range( -1 , 1)) = 0.3458117
		_COLOR_FIRE01("COLOR_FIRE01", Color) = (0,0,0,0)
		_COLOR_FIRE02("COLOR_FIRE02", Color) = (1,0,0,0)
		_ALBEDO("ALBEDO", 2D) = "white" {}
		_METALIC("METALIC", 2D) = "white" {}
		_AO("AO", 2D) = "white" {}
		_EdgeLength ( "Edge length", Range( 2, 50 ) ) = 15
		_Mask("Mask", 2D) = "white" {}
		_DistorsionMap("Distorsion Map", 2D) = "bump" {}
		_DivideAmmount("DivideAmmount", Float) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_DistortionAmmount("Distortion Ammount", Range( 0 , 1)) = 0.2
		_ScrollSpeed("Scroll Speed", Range( 0 , 0.5)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "Tessellation.cginc"
		#pragma target 4.6
		#pragma surface surf Standard keepalpha vertex:vertexDataFunc tessellate:tessFunction 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform sampler2D _DistorsionMap;
		uniform float4 _DistorsionMap_ST;
		uniform sampler2D _ALBEDO;
		uniform float4 _ALBEDO_ST;
		uniform float4 _COLOR_FIRE01;
		uniform float4 _COLOR_FIRE02;
		uniform sampler2D _Mask;
		uniform float _DistortionAmmount;
		uniform float _ScrollSpeed;
		uniform float _BURN;
		uniform float4 _Mask_ST;
		uniform float _DivideAmmount;
		uniform sampler2D _METALIC;
		uniform float4 _METALIC_ST;
		uniform float _Smoothness;
		uniform sampler2D _AO;
		uniform float4 _AO_ST;
		uniform float _EdgeLength;

		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			return UnityEdgeLengthBasedTess (v0.vertex, v1.vertex, v2.vertex, _EdgeLength);
		}

		void vertexDataFunc( inout appdata_full v )
		{
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_DistorsionMap = i.uv_texcoord * _DistorsionMap_ST.xy + _DistorsionMap_ST.zw;
			float3 tex2DNode13 = UnpackNormal( tex2D( _DistorsionMap, uv_DistorsionMap ) );
			float3 _Normal45 = tex2DNode13;
			o.Normal = _Normal45;
			float2 uv_ALBEDO = i.uv_texcoord * _ALBEDO_ST.xy + _ALBEDO_ST.zw;
			float4 _ALBEDO134 = tex2D( _ALBEDO, uv_ALBEDO );
			o.Albedo = _ALBEDO134.rgb;
			float2 temp_cast_1 = (0.1592142).xx;
			float2 panner20 = ( ( _Time.y * _ScrollSpeed ) * temp_cast_1 + float2( 0,0 ));
			float2 uv_TexCoord18 = i.uv_texcoord + panner20;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float2 temp_cast_2 = (_BURN).xx;
			float2 uv_TexCoord118 = i.uv_texcoord + temp_cast_2;
			float fresnelNdotV113 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode113 = ( 0.0 + uv_TexCoord118.x * pow( 1.0 - fresnelNdotV113, 5.0 ) );
			float4 lerpResult27 = lerp( _COLOR_FIRE01 , _COLOR_FIRE02 , ( tex2D( _Mask, ( ( (tex2DNode13).xy * _DistortionAmmount ) + uv_TexCoord18 ) ) + saturate( fresnelNode113 ) ));
			float2 uv_Mask = i.uv_texcoord * _Mask_ST.xy + _Mask_ST.zw;
			float4 tex2DNode89 = tex2D( _Mask, uv_Mask );
			float temp_output_88_0 = step( tex2DNode89.r , _BURN );
			float4 _Emission29 = ( lerpResult27 * ( temp_output_88_0 + ( temp_output_88_0 - step( tex2DNode89.r , ( _BURN / _DivideAmmount ) ) ) ) );
			o.Emission = _Emission29.rgb;
			float2 uv_METALIC = i.uv_texcoord * _METALIC_ST.xy + _METALIC_ST.zw;
			float4 _METALIC138 = tex2D( _METALIC, uv_METALIC );
			o.Metallic = _METALIC138.r;
			o.Smoothness = _Smoothness;
			float2 uv_AO = i.uv_texcoord * _AO_ST.xy + _AO_ST.zw;
			float4 _AO141 = tex2D( _AO, uv_AO );
			o.Occlusion = _AO141.r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16900
798;73;503;656;1723.458;1418.283;1.903439;False;False
Node;AmplifyShaderEditor.TexturePropertyNode;12;-3932.488,-212.3093;Float;True;Property;_DistorsionMap;Distorsion Map;14;0;Create;True;0;0;False;0;None;ec85c297e3d5c8c4e9884ba560162bf7;True;bump;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SimpleTimeNode;23;-3690.711,288.7655;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-3699.711,399.7653;Float;False;Property;_ScrollSpeed;Scroll Speed;18;0;Create;True;0;0;False;0;0;0.08548943;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-3403.711,320.7656;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;13;-3653.575,-207.2339;Float;True;Property;_TextureSample0;Texture Sample 0;6;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;21;-3531.71,176.7657;Float;False;Constant;_Speed;Speed;7;0;Create;True;0;0;False;0;0.1592142;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;15;-3300.575,-198.2341;Float;False;True;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;20;-3179.71,160.7657;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-3333.575,-47.23399;Float;False;Property;_DistortionAmmount;Distortion Ammount;17;0;Create;True;0;0;False;0;0.2;0.1043083;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;90;-3489.688,1119.366;Float;False;Property;_BURN;BURN;0;0;Create;True;0;0;False;0;0.3458117;1;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-3045.575,-223.2339;Float;True;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;18;-2955.71,64.7657;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;10;-2836.439,248.8704;Float;True;Property;_Mask;Mask;13;0;Create;True;0;0;False;0;None;60545ef3ec4a955439c19a07126009d2;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;94;-2418.419,1102.31;Float;False;Property;_DivideAmmount;DivideAmmount;15;0;Create;True;0;0;False;0;0;1.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;118;-3165.597,878.4366;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FresnelNode;113;-2787.603,647.5262;Float;True;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;19;-2586.395,42.5888;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;93;-2171.551,1032.62;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;89;-2257.104,398.7573;Float;True;Property;_TextureSample1;Texture Sample 1;1;0;Create;True;0;0;False;0;5cc85b0f88186a54ea5c0c20c1fc7047;5cc85b0f88186a54ea5c0c20c1fc7047;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;131;-2126.164,708.8035;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-2354.668,10.81723;Float;True;Property;_Sratches;Sratches;1;0;Create;True;0;0;False;0;5cc85b0f88186a54ea5c0c20c1fc7047;5cc85b0f88186a54ea5c0c20c1fc7047;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;88;-1611.949,493.356;Float;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;95;-1811.838,1027.486;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;3;-1940.98,-204.828;Float;False;Property;_COLOR_FIRE01;COLOR_FIRE01;1;0;Create;True;0;0;False;0;0,0,0,0;0.8396226,0.1465376,0.1465376,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;26;-1943.524,-25.04744;Float;False;Property;_COLOR_FIRE02;COLOR_FIRE02;2;0;Create;True;0;0;False;0;1,0,0,0;1,0.9733968,0.3726412,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;133;-1794.378,173.7616;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;96;-1456.71,988.1951;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;97;-1121.123,506.4585;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;27;-1596.855,-139.8722;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;109;-1443.757,-636.832;Float;True;Property;_METALIC;METALIC;4;0;Create;True;0;0;False;0;None;92c6b6ca406397b4597b5c6cdf79b659;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;140;-1450.672,-1051.268;Float;True;Property;_AO;AO;6;0;Create;True;0;0;False;0;None;2909cb98272c0e54a9ef63b797a7b774;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;106;-1451.69,-1310.769;Float;True;Property;_ALBEDO;ALBEDO;3;0;Create;True;0;0;False;0;None;fdb170200887ef64591671fe06e0f63e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;91;-878.4753,367.8063;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;141;-1116.774,-1049.729;Float;False;_AO;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;134;-1116.71,-1311.323;Float;False;_ALBEDO;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;29;-634.6271,377.7943;Float;False;_Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;138;-1118.101,-637.3787;Float;False;_METALIC;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;45;-3319.718,-333.3192;Float;False;_Normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;135;-293.5437,-230.3513;Float;False;134;_ALBEDO;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;137;-290.462,-151.7006;Float;False;45;_Normal;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;30;-293.8196,-71.41064;Float;False;29;_Emission;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;107;-1447.153,-842.2902;Float;True;Property;_NORMAL;NORMAL;5;1;[Normal];Create;True;0;0;False;0;None;1f2a18e884dde81458dcfd5ea5b59253;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;110;-361.6681,121.1417;Float;False;Property;_Smoothness;Smoothness;16;0;Create;True;0;0;False;0;0;0.3603315;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;139;-287.7228,19.58783;Float;False;138;_METALIC;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;136;-1121.344,-841.7151;Float;False;_NORMAL;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;142;-288.2434,224.0512;Float;False;141;_AO;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;135,-60;Float;False;True;6;Float;ASEMaterialInspector;0;0;Standard;N_Shaders/FireHandEffect;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Transparent;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;12;-1;-1;7;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;24;0;23;0
WireConnection;24;1;25;0
WireConnection;13;0;12;0
WireConnection;15;0;13;0
WireConnection;20;2;21;0
WireConnection;20;1;24;0
WireConnection;16;0;15;0
WireConnection;16;1;17;0
WireConnection;18;1;20;0
WireConnection;118;1;90;0
WireConnection;113;2;118;0
WireConnection;19;0;16;0
WireConnection;19;1;18;0
WireConnection;93;0;90;0
WireConnection;93;1;94;0
WireConnection;89;0;10;0
WireConnection;131;0;113;0
WireConnection;1;0;10;0
WireConnection;1;1;19;0
WireConnection;88;0;89;1
WireConnection;88;1;90;0
WireConnection;95;0;89;1
WireConnection;95;1;93;0
WireConnection;133;0;1;0
WireConnection;133;1;131;0
WireConnection;96;0;88;0
WireConnection;96;1;95;0
WireConnection;97;0;88;0
WireConnection;97;1;96;0
WireConnection;27;0;3;0
WireConnection;27;1;26;0
WireConnection;27;2;133;0
WireConnection;91;0;27;0
WireConnection;91;1;97;0
WireConnection;141;0;140;0
WireConnection;134;0;106;0
WireConnection;29;0;91;0
WireConnection;138;0;109;0
WireConnection;45;0;13;0
WireConnection;136;0;107;0
WireConnection;0;0;135;0
WireConnection;0;1;137;0
WireConnection;0;2;30;0
WireConnection;0;3;139;0
WireConnection;0;4;110;0
WireConnection;0;5;142;0
ASEEND*/
//CHKSM=ED3901B6C5865DF489108D09821F43FCFC89E801