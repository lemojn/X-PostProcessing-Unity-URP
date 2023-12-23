Shader "Hidden/XPostProcessing/Vignette/RapidOldTVVignetteV2"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    uniform half _VignetteSize;
    uniform half _SizeOffset;
    uniform half4 _VignetteColor;
    
    half4 Frag_VignetteV2(Varyings i) : SV_Target
    {
        float2 uv = -i.texcoord * i.texcoord + i.texcoord; // MAD
        half VignetteIndensity = saturate(uv.x * uv.y * _VignetteSize + _SizeOffset);
        return VignetteIndensity * GetScreenColor(i.texcoord);
    }
    
    half4 Frag_ColorAdjustVignetteV2(Varyings i) : SV_Target
    {
        float2 uv = -i.texcoord * i.texcoord + i.texcoord; // MAD
        half VignetteIndensity = saturate(uv.x * uv.y * _VignetteSize + _SizeOffset);
        return lerp(_VignetteColor, GetScreenColor(i.texcoord), VignetteIndensity);
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