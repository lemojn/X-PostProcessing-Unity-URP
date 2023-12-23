Shader "Hidden/XPostProcessing/Blur/KawaseBlur"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    half _BlurOffset;
    
    half4 Frag_KawaseBlur(Varyings i) : SV_Target
    {
        half pixelOffset = _BlurOffset;
        float2 texelSize = _BlitTexture_TexelSize.xy;
        float2 uv = i.texcoord;

        half4 o = 0;
        o += GetScreenColor(uv + float2(pixelOffset +0.5, pixelOffset +0.5) * texelSize);
        o += GetScreenColor(uv + float2(-pixelOffset -0.5, pixelOffset +0.5) * texelSize);
        o += GetScreenColor(uv + float2(-pixelOffset -0.5, -pixelOffset -0.5) * texelSize);
        o += GetScreenColor(uv + float2(pixelOffset +0.5, -pixelOffset -0.5) * texelSize);
        return o * 0.25;
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
            #pragma fragment Frag_KawaseBlur

            ENDHLSL
        }
    }
}