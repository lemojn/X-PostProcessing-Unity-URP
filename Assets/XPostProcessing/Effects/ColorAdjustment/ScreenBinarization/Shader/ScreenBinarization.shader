Shader "Hidden/XPostProcessing/ColorAdjustment/ScreenBinarization"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    half _BinarizationAmount;

    half4 Frag_ScreenBinarization(Varyings i) : SV_Target
    {
        half4 col = GetScreenColor(i.texcoord);
        half binarization = 0.299 * col.r + 0.587 * col.g + 0.114 * col.b;
        col = lerp(col, binarization, _BinarizationAmount);
        return col;
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
            #pragma fragment Frag_ScreenBinarization

            ENDHLSL
        }
    }
}