Shader "Custom/Line"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Width ("Width", Float) = 1
        _Color ("Color", Color) = (1,1,1,1)
        
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
            
            float _Width;           
            float _Length;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                i.uv.x *= _Length / _Width;
                float4 col = tex2D(_MainTex, i.uv);
                col *= _Color;
                return col;
            }
            ENDCG
        }
    }
}
