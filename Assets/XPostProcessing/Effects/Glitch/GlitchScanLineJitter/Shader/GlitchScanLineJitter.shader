Shader "Hidden/XPostProcessing/Glitch/ScanLineJitter"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"
    #pragma shader_feature USING_FREQUENCY_INFINITE
    
    uniform half3 _Params;
    #define _Amount _Params.x
    #define _Threshold _Params.y
    #define _Frequency _Params.z
    
    float randomNoise(float x, float y)
    {
        return frac(sin(dot(float2(x, y), float2(12.9898, 78.233))) * 43758.5453);
    }
    
    half4 Frag_Horizontal(Varyings i) : SV_Target
    {
        half strength = 0;
        #if USING_FREQUENCY_INFINITE
            strength = 1;
        #else
            strength = 0.5 + 0.5 * cos(_Time.y * _Frequency);
        #endif
        
        float jitter = randomNoise(i.texcoord.y, _Time.x) * 2 - 1;
        jitter *= step(_Threshold, abs(jitter)) * _Amount * strength;
        half4 sceneColor = GetScreenColor(frac(i.texcoord + float2(jitter, 0)));
        return sceneColor;
    }
    
    half4 Frag_Vertical(Varyings i) : SV_Target
    {
        half strength = 0;
        #if USING_FREQUENCY_INFINITE
            strength = 1;
        #else
            strength = 0.5 + 0.5 * cos(_Time.y * _Frequency);
        #endif
        
        float jitter = randomNoise(i.texcoord.x, _Time.x) * 2 - 1;
        jitter *= step(_Threshold, abs(jitter)) * _Amount * strength;
        half4 sceneColor = GetScreenColor(frac(i.texcoord + float2(0, jitter)));
        return sceneColor;
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
    }
}
