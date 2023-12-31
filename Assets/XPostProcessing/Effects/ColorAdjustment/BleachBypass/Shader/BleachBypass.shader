﻿Shader "Hidden/XPostProcessing/ColorAdjustment/BleachBypass"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    half _Indensity;
    
    half luminance(half3 color)
    {
        return dot(color, half3(0.222, 0.707, 0.071));
    }
    
    //reference : https://developer.download.nvidia.com/shaderlibrary/webpages/shader_library.html
    half4 Frag_BleachBypass(Varyings i) : SV_Target
    {
        half4 color = GetScreenColor(i.texcoord);
        half lum = luminance(color.rgb);
        half3 blend = half3(lum, lum, lum);
        half L = min(1.0, max(0.0, 10.0 * (lum - 0.45)));
        half3 result1 = 2.0 * color.rgb * blend;
        half3 result2 = 1.0 - 2.0 * (1.0 - blend) * (1.0 - color.rgb);
        half3 newColor = lerp(result1, result2, L);

        return lerp(color, half4(newColor, color.a), _Indensity);
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
            #pragma fragment Frag_BleachBypass

            ENDHLSL
        }
    }
}