Shader "Hidden/XPostProcessing/Blur/RadialBlur"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    half4 _Params;
    
    #define _BlurRadius _Params.x
    #define _Iteration _Params.y
    #define _RadialCenter _Params.zw
    
    half4 Frag_RadialBlur(Varyings i) : SV_Target
    {
        half4 acumulateColor = half4(0, 0, 0, 0);
        float2 blurVector = (_RadialCenter - i.texcoord) * _BlurRadius;
        float2 uv = i.texcoord;
        
        [unroll(30)]
        for (int j = 0; j < _Iteration; j++)
        {
            acumulateColor += GetScreenColor(uv);
            uv += blurVector;
        }
        
        return acumulateColor / _Iteration;
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
            #pragma fragment Frag_RadialBlur

            ENDHLSL
        }
    }
}