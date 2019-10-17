// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MyShaders/Life Orb"
{
	Properties
	{
		_OrbColor("Orb Color", Color) = (0,0,0,0)
		_Saturation("Saturation", Float) = 0
		_FresnelBias1("Fresnel Bias1", Float) = 0
		_FresnelScale1("Fresnel Scale1", Float) = 0
		_FresnelPower1("Fresnel Power1", Float) = 0
		_FresnelBias2("Fresnel Bias2", Float) = 0
		_FresnelScale2("Fresnel Scale2", Float) = 0
		_FresnelPower2("Fresnel Power2", Float) = 0
		_FresnelBias3("Fresnel Bias3", Float) = 0
		_FresnelScale3("Fresnel Scale3", Float) = 0
		_FresnelPower3("Fresnel Power3", Float) = 0
		_RingIntensity("Ring Intensity", Float) = 1
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha , SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow 
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
		};

		uniform float4 _OrbColor;
		uniform float _FresnelBias1;
		uniform float _FresnelScale1;
		uniform float _FresnelPower1;
		uniform float _FresnelBias3;
		uniform float _FresnelScale3;
		uniform float _FresnelPower3;
		uniform float _FresnelBias2;
		uniform float _FresnelScale2;
		uniform float _FresnelPower2;
		uniform float _RingIntensity;
		uniform float _Saturation;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV1 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode1 = ( _FresnelBias1 + _FresnelScale1 * pow( 1.0 - fresnelNdotV1, _FresnelPower1 ) );
			float fresnelNdotV19 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode19 = ( _FresnelBias3 + _FresnelScale3 * pow( 1.0 - fresnelNdotV19, _FresnelPower3 ) );
			float fresnelNdotV15 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode15 = ( _FresnelBias2 + _FresnelScale2 * pow( 1.0 - fresnelNdotV15, _FresnelPower2 ) );
			float temp_output_23_0 = ( ( 1.0 - saturate( fresnelNode1 ) ) + ( ( saturate( fresnelNode19 ) - saturate( fresnelNode15 ) ) * _RingIntensity ) );
			o.Emission = ( _OrbColor * temp_output_23_0 ).rgb;
			o.Alpha = ( temp_output_23_0 * _Saturation );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16400
780;73;859;641;1026.339;350.2844;1.175818;False;False
Node;AmplifyShaderEditor.RangedFloatNode;16;-889.7367,780.0543;Float;False;Property;_FresnelPower3;Fresnel Power3;11;0;Create;True;0;0;False;0;0;1.02;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-889.7367,716.0538;Float;False;Property;_FresnelScale3;Fresnel Scale3;10;0;Create;True;0;0;False;0;0;3.39;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-886.3851,484.3512;Float;False;Property;_FresnelScale2;Fresnel Scale2;7;0;Create;True;0;0;False;0;0;3.39;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-889.7367,652.0538;Float;False;Property;_FresnelBias3;Fresnel Bias3;9;0;Create;True;0;0;False;0;0;-0.52;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;14;-886.3851,420.3512;Float;False;Property;_FresnelBias2;Fresnel Bias2;6;0;Create;True;0;0;False;0;0;-0.86;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-886.3851,548.3516;Float;False;Property;_FresnelPower2;Fresnel Power2;8;0;Create;True;0;0;False;0;0;1.36;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-896,192;Float;False;Property;_FresnelPower1;Fresnel Power1;5;0;Create;True;0;0;False;0;0;1.02;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-896,128;Float;False;Property;_FresnelScale1;Fresnel Scale1;4;0;Create;True;0;0;False;0;0;3.39;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-896,64;Float;False;Property;_FresnelBias1;Fresnel Bias1;3;0;Create;True;0;0;False;0;0;-0.08;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;19;-658.2365,643.0538;Float;True;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;15;-654.8848,411.3512;Float;True;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;20;-353.3466,416;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;21;-368,640;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;1;-664.5,55;Float;True;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;24;-341.3052,737.605;Float;False;Property;_RingIntensity;Ring Intensity;12;0;Create;True;0;0;False;0;1;0.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;3;-331,131;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;22;-146.4048,579.4327;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;9;-192,128;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;17.40717,632.6484;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;23;-58.32078,147.7225;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-307.2209,296.0204;Float;False;Property;_Saturation;Saturation;2;0;Create;True;0;0;False;0;0;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;2;-640,-112;Float;False;Property;_OrbColor;Orb Color;1;0;Create;True;0;0;False;0;0,0,0,0;1,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;86.18504,46.67144;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;84.29266,248.0777;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;263.628,-61.34287;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;MyShaders/Life Orb;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Transparent;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;2;5;False;-1;10;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;19;1;18;0
WireConnection;19;2;17;0
WireConnection;19;3;16;0
WireConnection;15;1;14;0
WireConnection;15;2;13;0
WireConnection;15;3;12;0
WireConnection;20;0;15;0
WireConnection;21;0;19;0
WireConnection;1;1;5;0
WireConnection;1;2;6;0
WireConnection;1;3;7;0
WireConnection;3;0;1;0
WireConnection;22;0;21;0
WireConnection;22;1;20;0
WireConnection;9;0;3;0
WireConnection;25;0;22;0
WireConnection;25;1;24;0
WireConnection;23;0;9;0
WireConnection;23;1;25;0
WireConnection;4;0;2;0
WireConnection;4;1;23;0
WireConnection;10;0;23;0
WireConnection;10;1;11;0
WireConnection;0;2;4;0
WireConnection;0;9;10;0
ASEEND*/
//CHKSM=15270B7BC264A47085090E03AF7CBB372E29DB07