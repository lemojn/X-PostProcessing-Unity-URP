Shader "Hidden/XPostProcessing/ColorAdjustment/ContrastV3"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    uniform half4 _Contrast;

    half3 ColorAdjustment_Contrast_V3(float3 In, half3 ContrastFactor, float Contrast)
    {
        half3 Out = (In - ContrastFactor) * Contrast + ContrastFactor;
        return Out;
    }

    half4 Frag_Contrast_V3(Varyings i) : SV_Target
    {
        half4 finalColor = GetScreenColor(i.texcoord);
        finalColor.rgb = ColorAdjustment_Contrast_V3(finalColor.rgb, _Contrast.xyz, 1 - _Contrast.w);
        return finalColor;
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
            #pragma fragment Frag_Contrast_V3

            ENDHLSL
        }
    }
}