Shader "Hidden/XPostProcessing/Blur/GrainyBlur"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    half2 _Params;

    #define _BlurRadius _Params.x
    #define _Iteration _Params.y

    float Rand(float2 n)
    {
        return sin(dot(n, half2(1233.224, 1743.335)));
    }
    
    half4 Frag_GrainyBlur(Varyings i) : SV_Target
    {
        half2 randomOffset = float2(0.0, 0.0);
        half4 finalColor = half4(0.0, 0.0, 0.0, 0.0);
        float random = Rand(i.texcoord);
        
        for (int k = 0; k < int(_Iteration); k++)
        {
            random = frac(43758.5453 * random + 0.61432);;
            randomOffset.x = (random - 0.5) * 2.0;
            random = frac(43758.5453 * random + 0.61432);
            randomOffset.y = (random - 0.5) * 2.0;
            
            float2 uv = i.texcoord + randomOffset * _BlurRadius;
            finalColor += GetScreenColor(uv);
        }
        return finalColor / _Iteration;
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
            #pragma fragment Frag_GrainyBlur

            ENDHLSL
        }
    }
}