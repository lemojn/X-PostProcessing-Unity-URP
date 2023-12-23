Shader "Hidden/XPostProcessing/Vignette/RapidVignetteV2"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    half _VignetteIndensity;
    half _VignetteSharpness;
    half2 _VignetteCenter;
    half4 _VignetteColor;
    
    half4 Frag_VignetteV2(Varyings i) : SV_Target
    {
        half4 sceneColor = GetScreenColor(i.texcoord);
        half indensity = distance(i.texcoord, _VignetteCenter.xy);
        indensity = smoothstep(0.8, _VignetteSharpness * 0.799, indensity * (_VignetteIndensity + _VignetteSharpness));
        return sceneColor * indensity;
    }
    
    half4 Frag_ColorAdjustVignetteV2(Varyings i) : SV_Target
    {
        half4 sceneColor = GetScreenColor(i.texcoord);
        half indensity = distance(i.texcoord, _VignetteCenter.xy);
        indensity = smoothstep(0.8, _VignetteSharpness * 0.799, indensity * (_VignetteIndensity + _VignetteSharpness));
        half3 finalColor = lerp(_VignetteColor.rgb, sceneColor.rgb, indensity);
        return half4(finalColor.rgb, _VignetteColor.a);
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
            #pragma fragment Frag_VignetteV2
            
            ENDHLSL
        }
        
        Pass
        {
            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag_ColorAdjustVignetteV2
            
            ENDHLSL
        }
    }
}