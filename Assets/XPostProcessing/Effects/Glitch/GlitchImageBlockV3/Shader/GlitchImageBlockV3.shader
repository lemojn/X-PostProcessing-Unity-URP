Shader "Hidden/XPostProcessing/Glitch/ImageBlockV3"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    half3 _Params;
    #define _Speed _Params.x
    #define _BlockSize _Params.y

    inline float randomNoise(float2 seed)
    {
        return frac(sin(dot(seed * floor(_Time.y * _Speed), float2(17.13, 3.71))) * 43758.5453123);
    }

    inline float randomNoise(float seed)
    {
        return rand(float2(seed, 1.0));
    }

    half4 Frag_ImageBlockV3(Varyings i) : SV_Target
    {
        float2 block = randomNoise(floor(i.texcoord * _BlockSize));
        float displaceNoise = pow(block.x, 8.0) * pow(block.x, 3.0);
        half ColorR = GetScreenColor(i.texcoord).r;
        half ColorG = GetScreenColor(i.texcoord + float2(displaceNoise * 0.05 * randomNoise(7.0), 0.0)).g;
        half ColorB = GetScreenColor(i.texcoord - float2(displaceNoise * 0.05 * randomNoise(13.0), 0.0)).b;
        return half4(ColorR, ColorG, ColorB, 1.0);
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
            #pragma fragment Frag_ImageBlockV3
            
            ENDHLSL
        }
    }
}
