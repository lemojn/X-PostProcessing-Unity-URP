Shader "Hidden/XPostProcessing/ColorAdjustment/Contrast"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    uniform half _Contrast;

    half3 ColorAdjustment_Contrast(half3 In, half Contrast)
    {
        half midpoint = 0.21763h; // pow(0.5, 2.2);
        half3 Out = (In - midpoint) * Contrast + midpoint;
        return Out;
    }

    half4 Frag_Contrast(Varyings i) : SV_Target
    {
        half4 finalColor = GetScreenColor(i.texcoord);
        finalColor.rgb = ColorAdjustment_Contrast(finalColor.rgb, _Contrast);
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
            #pragma fragment Frag_Contrast

            ENDHLSL
        }
    }
}