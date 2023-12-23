Shader "Hidden/XPostProcessing/Blur/DirectionalBlur"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    half3 _Params;

    #define _Iteration _Params.x
    #define _Direction _Params.yz

    half4 Frag_DirectionalBlur(Varyings i) : SV_Target
    {
        half4 finalColor = half4(0.0, 0.0, 0.0, 0.0);
        for (int k = -_Iteration; k < _Iteration; k++)
        {
            float2 uv = i.texcoord - _Direction * k;
            finalColor += GetScreenColor(uv);
        }
        return finalColor / (_Iteration * 2.0);
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
            #pragma fragment Frag_DirectionalBlur

            ENDHLSL
        }
    }
}