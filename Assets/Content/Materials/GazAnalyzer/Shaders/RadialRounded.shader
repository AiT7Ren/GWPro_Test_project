Shader "Custom/RadialRounded"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color1 ("First Color", Color) = (1,0,0,1)
        _Color2 ("Second Color", Color) = (0,0,1,1)
        _FillAmount ("Fill Amount", Range(0,1)) = 0.5
        _Antialias ("Antialias", Range(0,0.1)) = 0.02
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float2 localPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            fixed4 _Color1;
            fixed4 _Color2;
            float _FillAmount;
            float _Antialias;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color;
                OUT.localPos = IN.texcoord - 0.5;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 pos = IN.localPos * 2.0;
                float angle = atan2(pos.y, pos.x);

                float normalizedAngle = (angle + 3.14159265359) / (2 * 3.14159265359);

                float distance = length(pos);


                float fillTest = step(normalizedAngle, _FillAmount);


                float fillEdge = smoothstep(_FillAmount - _Antialias, _FillAmount + _Antialias, normalizedAngle);


                float circleAlpha = 1.0 - smoothstep(0.5 - _Antialias, 0.5 + _Antialias, distance);
                float finalAlpha = circleAlpha * (1.0 - fillEdge);


                fixed4 finalColor = lerp(_Color2, _Color1, fillTest);
                finalColor.a *= finalAlpha;

                finalColor *= IN.color;

                return finalColor;
            }
            ENDCG


        }
    }
}