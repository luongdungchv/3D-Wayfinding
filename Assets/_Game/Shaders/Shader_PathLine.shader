Shader "Custom/Line"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurTex ("Blur Texture", 2D) = "white" {}
        _BlurColor ("Blur Color", Color) = (1,1,1,1)
        _Scale("Scale", Range(0, 255)) = 1
        _Duration("Duration", Range(0, 255)) = 1
        [HDR] _Color ("Shine Color", Color) = (1,1,1,1)
        _ShineRange("Shine Range", Range(0, 255)) = 1
        _ShineDistance("Shine Distance", Range(0, 255)) = 1
        _Frequency ("Frequency", Range(0, 255)) = 1
        _Power ("Power", Float) = 1
        _Gap ("Gap", Float) = 1
        
        [IntRange] _StencilRef ("Stencil Reference Value", Range(0,255)) = 0
        [IntRange] _StencilComp ("Stencil Comp", Range(0,8)) = 0
        [IntRange] _StencilReadMask ("Stencil Read Mask", Range(0,255)) = 0
        [IntRange] _StencilWriteMask ("Stencil Write Mask", Range(0,255)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent"}
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Zwrite Off
        ZTest Always
        
        Stencil{
            Ref [_StencilRef]
            Comp [_StencilComp]
            WriteMask [_StencilWriteMask]
            ReadMask [_StencilReadMask]
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 uv : TEXCOORD0;
            };

            struct v2f
            {
                float3 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex, _BlurTex;
            float4 _MainTex_ST;
            float _Scale;
            float _Duration;
            float4 _Color, _BlurColor;
            float _ShineRange;
            float _ShineDistance;
            float _Frequency;
            float _Power;
            float _Gap;
            
            float _LengthArray[256];

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = 1 - (i.uv % 1);
                float isCorner = !(uv.x == 1 || uv.x == 0);
                
                float length = _LengthArray[(int)i.uv.x] * 1;
                
                uv.x *= length;
                float mult = 1 - step(0.1, (int)uv.x % 2);
                mult *= isCorner;
                
                float isCut = ((int)(uv.x / 1) == (int)(length - 0.01)) && (uv.x % 1 < 0.98);
                
                uv.x = uv.x % 1; 
                uv.x = 1 - uv.x;
                //uv.x *= mult;
                uv.x *= isCorner;
                
                fixed4 col = tex2D(_MainTex, uv);
                col *= _Color;
                
                half blurAlpha = tex2D(_BlurTex, uv).a;
                float mask = step(0.96, col.r);
                blurAlpha -= mask;
                half4 blurCol = half4(_BlurColor.rgb, blurAlpha);
                
                col += saturate(blurCol);
                return col * !isCut;
            }
            ENDCG
        }
    }
}
