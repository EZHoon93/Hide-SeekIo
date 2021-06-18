Shader "Outline/OutlineShader"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_OutlineWidth("Width", Range(0, 20)) = 2
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType" = "Opaque" }
		LOD 100

		Pass {
		  Cull Off
		  ZTest Greater
		  ZWrite Off
		  ColorMask 0
		Fog { Mode Off }

		  Stencil {
			Ref 1
			Pass Replace
		  }
		}

		Pass
		{
			Fog { Mode Off }
			ZTest Always
			ZWrite Off

		Stencil {
			Ref 1
			Comp NotEqual
		}

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"

		struct appdata
		{
			float4 vertex : POSITION;
			float3 normal : NORMAL;
		};

		struct v2f
		{
			float4 vertex : SV_POSITION;
		};

		fixed4 _Color;
		uniform float _OutlineWidth;

		v2f vert(appdata v)
		{
			v2f o;
			float3 normal = normalize(v.vertex.xyz) * _OutlineWidth / 100.0;
			v.vertex.xyz += normal;
			o.vertex = UnityObjectToClipPos(v.vertex);
			return o;
		}

		fixed4 frag(v2f i) : SV_Target
		{
			return _Color;
		}
		ENDCG
	}
	}
}