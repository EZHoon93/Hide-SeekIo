Shader "Blur/BlurShader"
{
    Properties
    {
        _BlurAmount("Blur Amount", Range(0, 0.03)) = 0.0128
        _OutlineWidth("Width", Range(0, 20)) = 2
    }
        SubShader
    {
        Tags { "Queue" = "Transparent+1" }
        Cull Back
        ZTest Always
        Fog { Mode Off }

        GrabPass { "_GrabTexture" }

        Pass
        {
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
                float4 screenPos : TEXCOORD0;
            };

            half _BlurAmount;
            sampler2D _GrabTexture;
            uniform float _OutlineWidth;

            v2f vert(appdata v)
            {
                v2f o;
                float3 normal = normalize(v.vertex.xyz) * _OutlineWidth / 100.0;
                v.vertex.xyz += normal;
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.screenPos = o.vertex;
                return o;
            }

            half4 pixel;
            half2 uv;
            fixed i = 0;
            half iBlur;

            half4 frag(v2f input) : SV_Target
            {
                uv = input.screenPos.xy / input.screenPos.w;
                uv.x = (uv.x + 1) * .5;
                uv.y = 1.0 - (uv.y + 1) * .5;

                pixel = 0;

                pixel += tex2D(_GrabTexture, half2(uv.x + 1.5 * _BlurAmount, uv.y + 0.5 * _BlurAmount));
                pixel += tex2D(_GrabTexture, half2(uv.x + 1.5 * _BlurAmount, uv.y - 0.5 * _BlurAmount));
                pixel += tex2D(_GrabTexture, half2(uv.x + 1.5 * _BlurAmount, uv.y - 1.5 * _BlurAmount));
                pixel += tex2D(_GrabTexture, half2(uv.x + 1.5 * _BlurAmount, uv.y - 2.5 * _BlurAmount));
                pixel += tex2D(_GrabTexture, half2(uv.x + 0.5 * _BlurAmount, uv.y + 2.5 * _BlurAmount));
                pixel += tex2D(_GrabTexture, half2(uv.x + 0.5 * _BlurAmount, uv.y + 1.5 * _BlurAmount));

                pixel += tex2D(_GrabTexture, half2(uv.x + 0.5 * _BlurAmount, uv.y + 0.5 * _BlurAmount));
                pixel += tex2D(_GrabTexture, half2(uv.x + 0.5 * _BlurAmount, uv.y - 0.5 * _BlurAmount));

                pixel += tex2D(_GrabTexture, half2(uv.x + 0.5 * _BlurAmount, uv.y - 1.5 * _BlurAmount));
                pixel += tex2D(_GrabTexture, half2(uv.x + 0.5 * _BlurAmount, uv.y - 2.5 * _BlurAmount));
                pixel += tex2D(_GrabTexture, half2(uv.x - 0.5 * _BlurAmount, uv.y + 2.5 * _BlurAmount));
                pixel += tex2D(_GrabTexture, half2(uv.x - 0.5 * _BlurAmount, uv.y + 1.5 * _BlurAmount));

                pixel += tex2D(_GrabTexture, half2(uv.x - 0.5 * _BlurAmount, uv.y + 0.5 * _BlurAmount));
                pixel += tex2D(_GrabTexture, half2(uv.x - 0.5 * _BlurAmount, uv.y - 0.5 * _BlurAmount));

                pixel += tex2D(_GrabTexture, half2(uv.x - 0.5 * _BlurAmount, uv.y - 1.5 * _BlurAmount));
                pixel += tex2D(_GrabTexture, half2(uv.x - 0.5 * _BlurAmount, uv.y - 2.5 * _BlurAmount));
                pixel += tex2D(_GrabTexture, half2(uv.x - 1.5 * _BlurAmount, uv.y + 2.5 * _BlurAmount));
                pixel += tex2D(_GrabTexture, half2(uv.x - 1.5 * _BlurAmount, uv.y + 1.5 * _BlurAmount));
                pixel += tex2D(_GrabTexture, half2(uv.x - 1.5 * _BlurAmount, uv.y + 0.5 * _BlurAmount));
                pixel += tex2D(_GrabTexture, half2(uv.x - 1.5 * _BlurAmount, uv.y - 0.5 * _BlurAmount));

                pixel += tex2D(_GrabTexture, half2(uv.x, uv.y));

                return (pixel / 20.0);
            }
            ENDCG
        }
    }
}