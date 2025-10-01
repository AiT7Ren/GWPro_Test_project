Shader "Custom/AlwaysVisible"
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
        
        Pass
        {
            Stencil {
                Ref 1
                Comp Always
                Pass Replace
            }
            
            ZTest LEqual
            ZWrite On
            ColorMask 0 
            
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
        Pass
        {
            Stencil {
                Ref 1
                Comp NotEqual
            }
            
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
        Pass
        {
            Stencil {
                Ref 1
                Comp Equal
            }
            
            ZTest Always
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Back
            
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
                worldPos.xyz -= worldNormal * (_OutlineWidth * 0.5);
                
                o.vertex = mul(UNITY_MATRIX_VP, worldPos);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = _OutlineColor;
                col.a *= 0.5;
                return col;
            }
            ENDCG
        }
    }
}