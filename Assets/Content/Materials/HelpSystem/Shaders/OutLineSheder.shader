Shader "Custom/ForTutorial/OutLine"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _EmissionColor ("Emission Color", Color) = (1,1,1,1)
        _EmissionIntensity ("Emission Intensity", Range(0, 10)) = 1
        _EmissionMap ("Emission Map", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,0,0,1)
        _OutlineWidth ("Outline Width", Range(0, 0.1)) = 0.01
        _OutlineIntensity ("Outline Intensity", Range(0, 5)) = 1
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent+100"
            "RenderType" = "Transparent"
        }
        LOD 200
        
        Pass
        {
            Name "OUTLINE"
            Tags { "LightMode" = "Always" }

            Cull Front
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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            float _OutlineWidth;
            fixed4 _OutlineColor;
            float _OutlineIntensity;

            v2f vert(appdata v)
            {
                v2f o;
                float3 normal = normalize(v.normal);
                float3 outlineOffset = normal * _OutlineWidth;
                float3 pos = v.vertex.xyz + outlineOffset;

                o.vertex = UnityObjectToClipPos(float4(pos, 1.0));
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = _OutlineColor;
                col.rgb *= _OutlineIntensity;
                return col;
            }
            ENDCG
        }
        
        Pass
        {
            Name "MAIN"
            Tags { "LightMode" = "ForwardBase" }

            Cull Back
            ZTest LEqual
            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uv_emission : TEXCOORD1;
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            sampler2D _EmissionMap;
            float4 _EmissionMap_ST;
            fixed4 _EmissionColor;
            float _EmissionIntensity;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv_emission = TRANSFORM_TEX(v.uv, _EmissionMap);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET
            {
               
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                

                fixed4 emission = tex2D(_EmissionMap, i.uv_emission) * _EmissionColor;
                emission.rgb *= _EmissionIntensity;

                col.rgb += emission.rgb;
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float ndotl = saturate(dot(i.worldNormal, lightDir));
                fixed3 lighting = ndotl * _LightColor0.rgb + UNITY_LIGHTMODEL_AMBIENT.rgb;
                
                col.rgb *= lighting;
                
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}