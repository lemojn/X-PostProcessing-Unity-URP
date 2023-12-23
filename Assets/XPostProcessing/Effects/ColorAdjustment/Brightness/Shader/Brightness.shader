Shader "Hidden/XPostProcessing/ColorAdjustment/Brightness"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    half _Brightness;

    half4 Frag_Brightness(Varyings i) : SV_Target
    {
        half3 screenColor = GetScreenColor(i.texcoord).rgb;
        return half4(screenColor * _Brightness, 1.0);
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
            #pragma fragment Frag_Brightness

            ENDHLSL
        }
    }
}