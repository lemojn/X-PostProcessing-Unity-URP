Shader "Hidden/XPostProcessing/Glitch/ScreenJump"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"
    
    uniform half2 _Params;
    #define _JumpIndensity _Params.x
    #define _JumpTime _Params.y
    
    half4 Frag_Horizontal(Varyings i) : SV_Target
    {
        float jump = lerp(i.texcoord.x, frac(i.texcoord.x + _JumpTime), _JumpIndensity);
        half4 sceneColor = GetScreenColor(frac(float2(jump, i.texcoord.y)));
        return sceneColor;
    }
    
    half4 Frag_Vertical(Varyings i) : SV_Target
    {
        float jump = lerp(i.texcoord.y, frac(i.texcoord.y + _JumpTime), _JumpIndensity);
        half4 sceneColor = GetScreenColor(frac(float2(i.texcoord.x, jump)));
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
