Shader "Hidden/XPostProcessing/Glitch/RGBSplitV3"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    half3 _Params;
    #define _Frequency _Params.x
    #define _Amount _Params.y
    #define _Speed _Params.z

    half4 RGBSplit_Horizontal(float2 uv, float Amount, float time)
    {
        Amount *= 0.001;
        float3 splitAmountX = float3(uv.x, uv.x, uv.x);
        splitAmountX.r += sin(time * 0.2) * Amount;
        splitAmountX.g += sin(time * 0.1) * Amount;
        half4 splitColor = half4(0.0, 0.0, 0.0, 0.0);
        splitColor.r = GetScreenColor(float2(splitAmountX.r, uv.y)).r;
        splitColor.g = GetScreenColor(float2(splitAmountX.g, uv.y)).g;
        splitColor.b = GetScreenColor(float2(splitAmountX.b, uv.y)).b;
        splitColor.a = 1;
        return splitColor;
    }
    
    half4 RGBSplit_Vertical(float2 uv, float Amount, float time)
    {
        Amount *= 0.001;
        float3 splitAmountY = float3(uv.y, uv.y, uv.y);
        splitAmountY.r += sin(time * 0.2) * Amount;
        splitAmountY.g += sin(time * 0.1) * Amount;
        half4 splitColor = half4(0.0, 0.0, 0.0, 0.0);
        splitColor.r = GetScreenColor(float2(uv.x, splitAmountY.r)).r;
        splitColor.g = GetScreenColor(float2(uv.x, splitAmountY.g)).g;
        splitColor.b = GetScreenColor(float2(uv.x, splitAmountY.b)).b;
        splitColor.a = 1;
        return splitColor;
    }

    half4 RGBSplit_Horizontal_Vertical(float2 uv, float Amount, float time)
    {
        Amount *= 0.001;
        float splitAmountR = sin(time * 0.2) * Amount;
        float splitAmountG = sin(time * 0.1) * Amount;
        half4 splitColor = half4(0.0, 0.0, 0.0, 0.0);
        splitColor.r = GetScreenColor(float2(uv.x + splitAmountR, uv.y + splitAmountR)).r;
        splitColor.g = GetScreenColor(float2(uv.x, uv.y)).g;
        splitColor.b = GetScreenColor(float2(uv.x + splitAmountG, uv.y + splitAmountG)).b;
        splitColor.a = 1;
        return splitColor;
    }
    
    half4 Frag_Horizontal(Varyings i) : SV_Target
    {
        half strength = 0;
        #if USING_Frequency_INFINITE
            strength = 1;
        #else
            strength = 0.5 + 0.5 * cos(_Time.y * _Frequency);
        #endif
        half3 color = RGBSplit_Horizontal(i.texcoord, _Amount * strength, _Time.y * _Speed).rgb;
        return half4(color, 1);
    }
    
    half4 Frag_Vertical(Varyings i) : SV_Target
    {
        half strength = 0;
        #if USING_Frequency_INFINITE
            strength = 1;
        #else
            strength = 0.5 + 0.5 * cos(_Time.y * _Frequency);
        #endif
        half3 color = RGBSplit_Vertical(i.texcoord, _Amount * strength, _Time.y * _Speed).rgb;
        return half4(color, 1);
    }

    half4 Frag_Horizontal_Vertical(Varyings i) : SV_Target
    {
        half strength = 0;
        #if USING_Frequency_INFINITE
            strength = 1;
        #else
            strength = 0.5 + 0.5 * cos(_Time.y * _Frequency);
        #endif
        half3 color = RGBSplit_Horizontal_Vertical(i.texcoord, _Amount * strength, _Time.y * _Speed).rgb;
        return half4(color, 1);
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