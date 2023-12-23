Shader "Hidden/XPostProcessing/Glitch/RGBSplitV2"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    uniform half3 _Params;

    #define _TimeX _Params.x
    #define _Amount _Params.y
    #define _Amplitude _Params.z

    half4 Frag_Horizontal(Varyings i) : SV_Target
    {
        float splitAmout = (1.0 + sin(_TimeX * 6.0)) * 0.5;
        splitAmout *= 1.0 + sin(_TimeX * 16.0) * 0.5;
        splitAmout *= 1.0 + sin(_TimeX * 19.0) * 0.5;
        splitAmout *= 1.0 + sin(_TimeX * 27.0) * 0.5;
        splitAmout = pow(splitAmout, _Amplitude);
        splitAmout *= (0.05 * _Amount);
        
        half3 finalColor;
        finalColor.g = GetScreenColor(i.texcoord).g;
        finalColor.r = GetScreenColor(float2(i.texcoord.x + splitAmout, i.texcoord.y)).r;
        finalColor.b = GetScreenColor(float2(i.texcoord.x - splitAmout, i.texcoord.y)).b;
        finalColor *= (1.0 - splitAmout * 0.5);
        return half4(finalColor, 1.0);
    }

    half4 Frag_Vertical(Varyings i) : SV_Target
    {
        float splitAmout = (1.0 + sin(_TimeX * 6.0)) * 0.5;
        splitAmout *= 1.0 + sin(_TimeX * 16.0) * 0.5;
        splitAmout *= 1.0 + sin(_TimeX * 19.0) * 0.5;
        splitAmout *= 1.0 + sin(_TimeX * 27.0) * 0.5;
        splitAmout = pow(splitAmout, _Amplitude);
        splitAmout *= (0.05 * _Amount);
        
        half3 finalColor;
        finalColor.g = GetScreenColor(i.texcoord).g;
        finalColor.r = GetScreenColor(float2(i.texcoord.x, i.texcoord.y + splitAmout)).r;
        finalColor.b = GetScreenColor(float2(i.texcoord.x, i.texcoord.y - splitAmout)).b;
        finalColor *= (1.0 - splitAmout * 0.5);
        return half4(finalColor, 1.0);
    }

    half4 Frag_Vertical_Horizontal(Varyings i) : SV_Target
    {
        float splitAmout = (1.0 + sin(_TimeX * 6.0)) * 0.5;
        splitAmout *= 1.0 + sin(_TimeX * 16.0) * 0.5;
        splitAmout *= 1.0 + sin(_TimeX * 19.0) * 0.5;
        splitAmout *= 1.0 + sin(_TimeX * 27.0) * 0.5;
        splitAmout = pow(splitAmout, _Amplitude);
        splitAmout *= (0.05 * _Amount);

        half3 finalColor;
        finalColor.g = GetScreenColor(i.texcoord).g;
        finalColor.r = GetScreenColor(float2(i.texcoord.x + splitAmout, i.texcoord.y + splitAmout)).r;
        finalColor.b = GetScreenColor(float2(i.texcoord.x - splitAmout, i.texcoord.y + splitAmout)).b;
        finalColor *= (1.0 - splitAmout * 0.5);
        return half4(finalColor, 1.0);
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
            #pragma fragment Frag_Vertical_Horizontal

            ENDHLSL
        }
    }
}
