Shader "Hidden/XPostProcessing/ColorAdjustment/Tint"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    uniform half _Indensity;
    uniform half4 _ColorTint;
    
    half4 Frag_Tint(Varyings i) : SV_Target
    {
        half4 sceneColor = GetScreenColor(i.texcoord);
        half3 finalColor = lerp(sceneColor.rgb, sceneColor.rgb * _ColorTint.rgb, _Indensity);
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
            #pragma fragment Frag_Tint

            ENDHLSL
        }
    }
}