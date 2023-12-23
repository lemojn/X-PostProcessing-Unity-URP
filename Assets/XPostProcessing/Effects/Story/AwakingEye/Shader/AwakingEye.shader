Shader "Hidden/XPostProcessing/Story/AwakingEye"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    half2 _Params;
    #define _OpenEyeValue _Params.x;
    #define _OpenEyeLength _Params.y;

    half4 Frag_AwakingEye(Varyings i) : SV_Target
    {
        float a = _OpenEyeLength;
        float b = _OpenEyeValue;

        half4 darkColor = half4(0, 0, 0, 1);
        half4 color = GetScreenColor(i.texcoord);
        half x = i.texcoord.x - 0.5f;
        half y = i.texcoord.y - 0.5f;
        half oval = x * x / (a * a) + y * y / (b * b);
        color = lerp(color, darkColor, oval);
        return color;
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
            #pragma fragment Frag_AwakingEye

            ENDHLSL
        }
    }
}