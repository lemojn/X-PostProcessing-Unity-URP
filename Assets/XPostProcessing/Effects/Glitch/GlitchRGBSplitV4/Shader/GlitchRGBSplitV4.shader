Shader "Hidden/XPostProcessing/Glitch/RGBSplitV4"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    uniform half2 _Params;
    #define _Indensity _Params.x
    #define _TimeX _Params.y

    float randomNoise(float x, float y)
    {
        return frac(sin(dot(float2(x, y), float2(12.9898, 78.233))) * 43758.5453);
    }

    half4 Frag_Horizontal(Varyings i) : SV_Target
    {
        float splitAmount = _Indensity * randomNoise(_TimeX, 2);
        half4 ColorR = GetScreenColor(float2(i.texcoord.x + splitAmount, i.texcoord.y));
        half4 ColorG = GetScreenColor(i.texcoord);
        half4 ColorB = GetScreenColor(float2(i.texcoord.x - splitAmount, i.texcoord.y));
        return half4(ColorR.r, ColorG.g, ColorB.b, 1);
    }

    half4 Frag_Vertical(Varyings i) : SV_Target
    {
        float splitAmount = _Indensity * randomNoise(_TimeX, 2);
        half4 ColorR = GetScreenColor(i.texcoord);
        half4 ColorG = GetScreenColor(float2(i.texcoord.x, i.texcoord.y + splitAmount));
        half4 ColorB = GetScreenColor(float2(i.texcoord.x, i.texcoord.y - splitAmount));
        return half4(ColorR.r, ColorG.g, ColorB.b, 1);
    }

    half4 Frag_Horizontal_Vertical(Varyings i) : SV_Target
    {
        float splitAmount = _Indensity * randomNoise(_TimeX, 2);
        half4 ColorR = GetScreenColor(i.texcoord);
        half4 ColorG = GetScreenColor(float2(i.texcoord.x + splitAmount, i.texcoord.y + splitAmount));
        half4 ColorB = GetScreenColor(float2(i.texcoord.x - splitAmount, i.texcoord.y - splitAmount));
        return half4(ColorR.r, ColorG.g, ColorB.b, 1);
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
            #pragma fragment Frag_Horizontal
            
            ENDHLSL
        }
        
        Pass
        {
            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag_Vertical
            
            ENDHLSL
        }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag_Horizontal_Vertical

            ENDHLSL
        }
    }
}
