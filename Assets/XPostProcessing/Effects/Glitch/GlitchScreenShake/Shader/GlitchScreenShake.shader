Shader "Hidden/XPostProcessing/Glitch/ScreenShake"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"
    
    uniform half _ScreenShake;
    
    float randomNoise(float x, float y)
    {
        return frac(sin(dot(float2(x, y), float2(127.1, 311.7))) * 43758.5453);
    }
    
    half4 Frag_Horizontal(Varyings i) : SV_Target
    {
        float shake = (randomNoise(_Time.x, 2) - 0.5) * _ScreenShake;
        half4 sceneColor = GetScreenColor(frac(float2(i.texcoord.x + shake, i.texcoord.y)));
        return sceneColor;
    }
    
    half4 Frag_Vertical(Varyings i) : SV_Target
    {
        float shake = (randomNoise(_Time.x, 2) - 0.5) * _ScreenShake;
        half4 sceneColor = GetScreenColor(frac(float2(i.texcoord.x, i.texcoord.y + shake)));
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
