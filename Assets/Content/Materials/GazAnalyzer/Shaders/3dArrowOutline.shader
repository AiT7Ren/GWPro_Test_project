Shader "Custom/3dArrowOutline"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,0,0,1)
        _OutlineWidth ("Outline Width", Range(0, 0.1)) = 0.02
    }
    SubShader
    {
        Tags { "Queue" = "Transparent+100" "RenderType" = "Transparent" }
        LOD 200
        
        // Проход 1: Обводка (всегда видимая)
        Pass
        {
            ZTest Always
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Front
            
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
            
            fixed4 _OutlineColor;
            float _OutlineWidth;
            
            v2f vert (appdata v)
            {
                v2f o;
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                float3 worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
                worldPos.xyz += worldNormal * _OutlineWidth;
                o.vertex = mul(UNITY_MATRIX_VP, worldPos);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }
        
        // Проход 2: Основной объект (всегда видимый)
        Pass
        {
            ZTest Always
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                return col;
            }
            ENDCG
        }
        
        // Проход 3: Маскировка обводки при наложении на другие объекты
        Pass
        {
            ZWrite Off
            ColorMask 0
            Blend Zero One
            
            Stencil {
                Ref 1
                Comp Always
                Pass Replace
            }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
            };
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                return 0;
            }
            ENDCG
        }
    }
}

