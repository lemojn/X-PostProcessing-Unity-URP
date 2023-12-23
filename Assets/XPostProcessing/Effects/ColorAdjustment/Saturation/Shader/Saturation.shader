Shader "Hidden/XPostProcessing/ColorAdjustment/Saturation"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    uniform half _Saturation;

    half3 Saturation(half3 In, half Saturation)
    {
        half luma = dot(In, half3(0.2126729, 0.7151522, 0.0721750));
        half3 Out = luma.xxx + Saturation.xxx * (In - luma.xxx);
        return Out;
    }

    half4 Frag_Saturation(Varyings i) : SV_Target
    {
        half4 sceneColor = GetScreenColor(i.texcoord);
        return half4(Saturation(sceneColor.rgb, _Saturation), 1.0);
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
            #pragma fragment Frag_Saturation

            ENDHLSL
        }
    }
}