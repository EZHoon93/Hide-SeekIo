// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Animmal/Fog"
{
	Properties
	{
		_FogIntensity("FogIntensity", Range( 0 , 1)) = 0
		_FogMaxIntensity("FogMaxIntensity", Range( 0 , 1)) = 0
		_FogColor("FogColor", Color) = (0.1470588,1,0.634,0)
		[HideInInspector] __dirty( "", Int ) = 1
		[Header(Forward Rendering Options)]
		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
		[ToggleOff] _GlossyReflections("Reflections", Float) = 1.0
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "ForceNoShadowCasting" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma shader_feature _SPECULARHIGHLIGHTS_OFF
		#pragma shader_feature _GLOSSYREFLECTIONS_OFF
		#pragma surface surf Standard alpha:fade keepalpha noshadow dithercrossfade 
		struct Input
		{
			float4 screenPos;
		};

		uniform float4 _FogColor;
		uniform sampler2D _CameraDepthTexture;
		uniform float _FogIntensity;
		uniform float _FogMaxIntensity;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Emission = _FogColor.rgb;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float eyeDepth10 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float clampResult19 = clamp( ( abs( ( eyeDepth10 - ase_screenPos.w ) ) * (0.1 + (_FogIntensity - 0) * (0.4 - 0.1) / (1 - 0)) ) , 0 , _FogMaxIntensity );
			o.Alpha = clampResult19;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14501
2116;75;1587;882;1095.764;545.0623;1.729117;True;True
Node;AmplifyShaderEditor.ScreenPosInputsNode;13;-939.0226,298.4225;Float;False;1;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScreenDepthNode;10;-671.123,197.2475;Float;False;0;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;11;-400.373,351.1476;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-653.323,574.6724;Float;False;Property;_FogIntensity;FogIntensity;0;0;Create;True;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;15;-213.6984,346.8727;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;17;-316.7581,521.9324;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.1;False;4;FLOAT;0.4;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;17.09721,348.3156;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.3;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-384.5396,795.3659;Float;False;Property;_FogMaxIntensity;FogMaxIntensity;1;0;Create;True;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;20;533.1391,6.177148;Float;False;Property;_FogColor;FogColor;3;0;Create;True;0;0.1470588,1,0.634,0;0.3211505,0.5868482,0.8088235,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;19;221.6826,509.4538;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;27;-68.3651,23.04821;Float;True;Property;_TextureSample1;Texture Sample 1;4;0;Create;True;0;None;adb118ead3d37734d976d32af0d04860;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;29;349.2773,175.7619;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;225.98,121.9498;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;22;-663.2499,-244.6065;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;5,5;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;23;-290.9201,-242.1864;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.01,0.0005;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;21;-17.25087,-316.5262;Float;True;Property;_TextureSample0;Texture Sample 0;5;0;Create;True;0;None;adb118ead3d37734d976d32af0d04860;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;31;497.0587,337.8504;Float;False;Property;_Color0;Color 0;2;0;Create;True;0;0.1470588,1,0.634,0;0.07136677,0.4411765,0.4105715,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;405.7218,-99.37071;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;850.7698,-44.09;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Animmal/Fog;False;False;False;False;False;False;False;False;False;False;False;False;True;False;True;True;True;True;True;Back;0;0;False;0;0;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;False;2;SrcAlpha;OneMinusSrcAlpha;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;10;0;13;0
WireConnection;11;0;10;0
WireConnection;11;1;13;4
WireConnection;15;0;11;0
WireConnection;17;0;16;0
WireConnection;14;0;15;0
WireConnection;14;1;17;0
WireConnection;19;0;14;0
WireConnection;19;2;18;0
WireConnection;27;1;23;0
WireConnection;29;0;28;0
WireConnection;28;0;27;0
WireConnection;23;0;22;0
WireConnection;21;1;23;0
WireConnection;24;0;21;0
WireConnection;0;2;20;0
WireConnection;0;9;19;0
ASEEND*/
//CHKSM=5C3935528D9EC082756F3FAB70D7D5BF59BC44C5