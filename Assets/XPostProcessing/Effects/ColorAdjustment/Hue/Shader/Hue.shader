Shader "Hidden/XPostProcessing/ColorAdjustment/Hue"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    uniform half _HueDegree;

    half3 Hue_Degree(float3 color, float offset)
    {
        float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
        float4 P = lerp(float4(color.bg, K.wz), float4(color.gb, K.xy), step(color.b, color.g));
        float4 Q = lerp(float4(P.xyw, color.r), float4(color.r, P.yzx), step(P.x, color.r));
        float D = Q.x - min(Q.w, Q.y);
        float E = 1e-10;
        float3 hsv = float3(abs(Q.z + (Q.w - Q.y) / (6.0 * D + E)), D / (Q.x + E), Q.x);

        float hue = hsv.x + offset / 360;
        hsv.x = (hue < 0)
        ? hue + 1
        : (hue > 1)
        ? hue - 1
        : hue;

        float4 K2 = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
        float3 P2 = abs(frac(hsv.xxx + K2.xyz) * 6.0 - K2.www);
        half3 Out = hsv.z * lerp(K2.xxx, saturate(P2 - K2.xxx), hsv.y);

        return Out;
    }

    half4 Frag_Hue(Varyings i) : SV_Target
    {
        half4 sceneColor = GetScreenColor(i.texcoord);
        return half4(Hue_Degree(sceneColor.rgb, _HueDegree), 1.0);
    }

    ENDHLSL

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }
        
        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag_Hue

            ENDHLSL
        }
    }
}