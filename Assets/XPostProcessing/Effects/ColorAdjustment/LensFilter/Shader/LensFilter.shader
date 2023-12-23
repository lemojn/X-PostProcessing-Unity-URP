Shader "Hidden/XPostProcessing/ColorAdjustment/LensFilter"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    uniform half _Indensity;
    uniform half4 _LensColor;
    
    half luminance(half3 color)
    {
        return dot(color, half3(0.222, 0.707, 0.071));
    }

    half4 Frag_LensFilter(Varyings i) : SV_Target
    {
        half4 sceneColor = GetScreenColor(i.texcoord);
        half lum = luminance(sceneColor.rgb);
        // Interpolate with half4(0.0, 0.0, 0.0, 0.0) based on luminance
        half4 filterColor = lerp(half4(0.0, 0.0, 0.0, 0.0), _LensColor, saturate(lum * 2.0));
        // Interpolate with half4(1.0, 1.0, 1.0, 1.0) based on luminance
        filterColor = lerp(filterColor, half4(1.0, 1.0, 1.0, 1.0), saturate(lum - 0.5) * 2.0);
        filterColor = lerp(sceneColor, filterColor, saturate(lum * _Indensity));
        return half4(filterColor.rgb, sceneColor.a);
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
            #pragma fragment Frag_LensFilter

            ENDHLSL
        }
    }
}