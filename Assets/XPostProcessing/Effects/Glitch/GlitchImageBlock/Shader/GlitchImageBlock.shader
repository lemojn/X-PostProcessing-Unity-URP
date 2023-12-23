Shader "Hidden/XPostProcessing/Glitch/ImageBlock"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    uniform half3 _Params;
    uniform half4 _Params2;
    uniform half3 _Params3;

    #define _TimeX _Params.x
    #define _Offset _Params.y
    #define _Fade _Params.z

    #define _BlockLayer1_U _Params2.w
    #define _BlockLayer1_V _Params2.x
    #define _BlockLayer2_U _Params2.y
    #define _BlockLayer2_V _Params2.z

    #define _RGBSplit_Indensity _Params3.x
    #define _BlockLayer1_Indensity _Params3.y
    #define _BlockLayer2_Indensity _Params3.z

    float randomNoise(float2 seed)
    {
        return frac(sin(dot(seed * floor(_TimeX * 30.0), float2(127.1, 311.7))) * 43758.5453123);
    }
    
    float randomNoise(float seed)
    {
        return randomNoise(float2(seed, 1.0));
    }
    
    half4 Frag_ImageBlock(Varyings i) : SV_Target
    {
        float2 uv = i.texcoord;
        //求解第一层blockLayer
        float2 blockLayer1 = floor(uv * float2(_BlockLayer1_U, _BlockLayer1_V));
        float2 blockLayer2 = floor(uv * float2(_BlockLayer2_U, _BlockLayer2_V));

        float lineNoise1 = pow(randomNoise(blockLayer1), _BlockLayer1_Indensity);
        float lineNoise2 = pow(randomNoise(blockLayer2), _BlockLayer2_Indensity);
        float RGBSplitNoise = pow(randomNoise(5.1379), 7.1) * _RGBSplit_Indensity;
        float lineNoise = lineNoise1 * lineNoise2 * _Offset -RGBSplitNoise;
        
        half4 colorR = GetScreenColor(uv);
        half4 colorG = GetScreenColor(uv + float2(lineNoise * 0.05 * randomNoise(7.0), 0));
        half4 colorB = GetScreenColor(uv - float2(lineNoise * 0.05 * randomNoise(23.0), 0));
        
        half4 result = half4(colorR.x, colorG.y, colorB.z, (colorR.a + colorG.a + colorB.a));
        result = lerp(colorR, result, _Fade);
        return result;
    }
    
    half4 Frag_Debug(Varyings i) : SV_Target
    {
        float2 uv = i.texcoord;
        
        float2 blockLayer1 = floor(uv * float2(_BlockLayer1_U, _BlockLayer1_V));
        float2 blockLayer2 = floor(uv * float2(_BlockLayer2_U, _BlockLayer2_V));
        
        float lineNoise1 = pow(randomNoise(blockLayer1), _BlockLayer1_Indensity);
        float lineNoise2 = pow(randomNoise(blockLayer2), _BlockLayer2_Indensity);
        float RGBSplitNoise = pow(randomNoise(5.1379), 7.1) * _RGBSplit_Indensity;
        float lineNoise = lineNoise1 * lineNoise2 * _Offset -RGBSplitNoise;
        
        return half4(lineNoise, lineNoise, lineNoise, 1);
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
            #pragma fragment Frag_ImageBlock
            
            ENDHLSL
        }
        
        Pass
        {
            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag_Debug
            
            ENDHLSL
        }
    }
}
