Shader "PluginMaster/Indicator"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        [HideInInspector] _opacity("Opacity", Range(0.0, 1.0)) = 1.0

    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ZTest Always
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _opacity;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 texcoord : TEXCOORD0;
                float4 color : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 color = tex2D(_MainTex, i.texcoord) * i.color;
                color.a *= 0.5 * _opacity;
                return color;
            }
            ENDCG
        }
    }
}
