Shader "Hidden/XPostProcessing/Glitch/ImageBlockV4"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    uniform half4 _Params;
    #define _Speed _Params.x
    #define _BlockSize _Params.y
    #define _MaxRGBSplitX _Params.z
    #define _MaxRGBSplitY _Params.w

    inline float randomNoise(float2 seed)
    {
        return frac(sin(dot(seed * floor(_Time.y * _Speed), float2(17.13, 3.71))) * 43758.5453123);
    }

    inline float randomNoise(float seed)
    {
        return randomNoise(float2(seed, 1.0));
    }

    half4 Frag_ImageBlockV4(Varyings i) : SV_Target
    {
        half2 block = randomNoise(floor(i.texcoord * _BlockSize));
        float displaceNoise = pow(block.x, 8.0) * pow(block.x, 3.0);
        float splitRGBNoise = pow(randomNoise(7.2341), 17.0);
        float offsetX = displaceNoise - splitRGBNoise * _MaxRGBSplitX;
        float offsetY = displaceNoise - splitRGBNoise * _MaxRGBSplitY;
        float noiseX = 0.05 * randomNoise(13.0);
        float noiseY = 0.05 * randomNoise(7.0);
        float2 offset = float2(offsetX * noiseX, offsetY * noiseY);

        half4 colorR = GetScreenColor(i.texcoord);
        half4 colorG = GetScreenColor(i.texcoord + offset);
        half4 colorB = GetScreenColor(i.texcoord - offset);
        return half4(colorR.r, colorG.g, colorB.z, (colorR.a + colorG.a + colorB.a));
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
            #pragma fragment Frag_ImageBlockV4
            
            ENDHLSL
        }
    }
}