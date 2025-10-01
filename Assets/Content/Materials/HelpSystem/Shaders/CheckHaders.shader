Shader "Custom/ForTutorial/FresnelEffect"
{
    Properties
    {
        [Header(Main Settings)]
        _MainColor ("Main Color", Color) = (1, 1, 1, 1)
        _MainTex ("Main Texture", 2D) = "white" {}
        
        [Header(Fresnel Effect)]
        _FresnelColor ("Fresnel Color", Color) = (1, 0.5, 0, 1)
        _FresnelPower ("Fresnel Power", Range(0.1, 10)) = 3
        _FresnelIntensity ("Fresnel Intensity", Range(0, 5)) = 1
        _FresnelBias ("Fresnel Bias", Range(0, 1)) = 0
        
        [Header(Animation)]
        _PulseSpeed ("Pulse Speed", Float) = 1
        _PulseAmplitude ("Pulse Amplitude", Range(0, 2)) = 0.5
        _ScrollSpeed ("Scroll Speed", Vector) = (0.1, 0.1, 0, 0)
        
        [Header(Rim Glow)]
        _RimColor ("Rim Color", Color) = (0, 1, 1, 1)
        _RimPower ("Rim Power", Range(0.1, 10)) = 5
        _RimIntensity ("Rim Intensity", Range(0, 5)) = 1
        
        [Header(Advanced)]
        _Smoothness ("Smoothness", Range(0, 1)) = 0.5
        _Metallic ("Metallic", Range(0, 1)) = 0
        [Toggle]_UseEmission ("Use Emission", Float) = 1
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque" 
            "Queue"="Geometry"
        }
        
        LOD 200
        
        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile _ _USE_EMISSION_ON
            
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float3 viewDir : TEXCOORD3;
                float3 tangentWorld : TEXCOORD4;
                float3 bitangentWorld : TEXCOORD5;
                SHADOW_COORDS(6)
                UNITY_FOG_COORDS(7)
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _MainColor;
            fixed4 _FresnelColor;
            float _FresnelPower;
            float _FresnelIntensity;
            float _FresnelBias;
            float _PulseSpeed;
            float _PulseAmplitude;
            float2 _ScrollSpeed;
            fixed4 _RimColor;
            float _RimPower;
            float _RimIntensity;
            float _Smoothness;
            float _Metallic;
            float _UseEmission;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(UnityWorldSpaceViewDir(o.worldPos));
                
                o.tangentWorld = normalize(mul(unity_ObjectToWorld, float4(v.tangent.xyz, 0.0)).xyz);
                o.bitangentWorld = normalize(cross(o.worldNormal, o.tangentWorld) * v.tangent.w);
                
                TRANSFER_SHADOW(o);
                UNITY_TRANSFER_FOG(o, o.pos);
                
                return o;
            }
            
            float AnimatedFresnel(float3 normal, float3 viewDir, float basePower, float pulseSpeed, float pulseAmplitude)
            {
                float baseFresnel = pow(1.0 - saturate(dot(normal, viewDir)), basePower);
                
                float pulse = (sin(_Time.y * pulseSpeed) + 1.0) * 0.5 * pulseAmplitude + (1.0 - pulseAmplitude);
                return baseFresnel * pulse;
            }
            
            float RimLight(float3 normal, float3 viewDir, float rimPower)
            {
                float rim = 1.0 - saturate(dot(normal, viewDir));
                return pow(rim, rimPower);
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float3 worldNormal = normalize(i.worldNormal);
                float3 viewDir = normalize(i.viewDir);
                
                float2 scrolledUV = i.uv + _ScrollSpeed * _Time.y;
                
                fixed4 mainTex = tex2D(_MainTex, scrolledUV);
                fixed4 albedo = mainTex * _MainColor;
                
                float fresnel = AnimatedFresnel(worldNormal, viewDir, _FresnelPower, _PulseSpeed, _PulseAmplitude);
                fresnel = saturate(fresnel + _FresnelBias) * _FresnelIntensity;
                
                float rim = RimLight(worldNormal, viewDir, _RimPower) * _RimIntensity;
                
                fixed4 finalColor = albedo;
                
                fixed4 fresnelEffect = _FresnelColor * fresnel;
                finalColor.rgb = lerp(finalColor.rgb, fresnelEffect.rgb, fresnel);
                
                finalColor.rgb += _RimColor.rgb * rim;
                
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float ndotl = saturate(dot(worldNormal, lightDir));
                fixed3 lighting = ndotl * _LightColor0.rgb + UNITY_LIGHTMODEL_AMBIENT.rgb;
                
                finalColor.rgb *= lighting;
                
                fixed shadow = SHADOW_ATTENUATION(i);
                finalColor.rgb *= shadow;
                
                #if defined(_USE_EMISSION_ON)
                    finalColor.rgb += (fresnelEffect.rgb + _RimColor.rgb * rim) * _UseEmission;
                #endif
                
                UNITY_APPLY_FOG(i.fogCoord, finalColor);
                
                return finalColor;
            }
            ENDCG
        }
        
        Pass
        {
            Name "FORWARD_ADD"
            Tags { "LightMode" = "ForwardAdd" }
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                LIGHTING_COORDS(2, 3)
            };
            
            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                float3 worldNormal = normalize(i.worldNormal);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos);
                float ndotl = saturate(dot(worldNormal, lightDir));
                fixed attenuation = LIGHT_ATTENUATION(i);
                fixed3 lightColor = _LightColor0.rgb * ndotl * attenuation;
                return fixed4(lightColor, 1);
            }
            ENDCG
        }
        
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            
            ZWrite On
            ZTest LEqual
            ColorMask 0
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"
            
            struct v2f {
                V2F_SHADOW_CASTER;
            };
            
            v2f vert(appdata_base v)
            {
                v2f o;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }
            
            float4 frag(v2f i) : SV_Target
            {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    
    FallBack "VertexLit"
    CustomEditor "FresnelEffectShaderEditor"
}