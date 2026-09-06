Shader "KissAndRun/StylizedToon"
{
    Properties
    {
        _Color ("Main Color", Color) = (1, 1, 1, 1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _RampThreshold ("Cel Threshold", Range(0, 1)) = 0.5
        _RampSmooth ("Cel Smoothness", Range(0.001, 0.5)) = 0.05
        _RimColor ("Rim Light Color", Color) = (1, 1, 1, 1)
        _RimPower ("Rim Power", Range(0.5, 8.0)) = 3.0
        _EmissionColor ("Emission Color", Color) = (0, 0, 0, 1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 200

        CGPROGRAM
        #pragma surface surf StylizedToon fullforwardshadows addshadow
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
            float3 viewDir;
            float3 worldNormal;
        };

        fixed4 _Color;
        sampler2D _MainTex;
        half _RampThreshold;
        half _RampSmooth;
        fixed4 _RimColor;
        half _RimPower;
        fixed4 _EmissionColor;

        half4 LightingStylizedToon(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
        {
            half NdotL = dot(s.Normal, lightDir);
            half lightIntensity = smoothstep(_RampThreshold - _RampSmooth, _RampThreshold + _RampSmooth, NdotL);

            // Specular / Rim highlight
            half3 halfVector = normalize(lightDir + viewDir);
            half NdotH = max(0.0, dot(s.Normal, halfVector));
            half spec = smoothstep(0.7, 0.75, pow(NdotH, 32.0));

            half4 c;
            c.rgb = s.Albedo * _LightColor0.rgb * (lightIntensity * atten * 0.75 + 0.25) + (_LightColor0.rgb * spec * 0.3 * atten);
            c.a = s.Alpha;
            return c;
        }

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;

            // Comic Rim Light
            half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
            half rimIntensity = pow(rim, _RimPower);
            o.Emission = _EmissionColor.rgb + (_RimColor.rgb * rimIntensity * 0.5);

            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
