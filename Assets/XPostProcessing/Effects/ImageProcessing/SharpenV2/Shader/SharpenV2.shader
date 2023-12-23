Shader "Hidden/XPostProcessing/ImageProcessing/SharpenV2"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    uniform half _Sharpness;

    half4 Frag_SharpenV2(Varyings i) : SV_Target
    {
        half2 pixelSize = float2(1 / _ScreenParams.x, 1 / _ScreenParams.y);
        pixelSize *= 1.5f;

        half4 blur = GetScreenColor(i.texcoord + half2(pixelSize.x, -pixelSize.y));
        blur += GetScreenColor(i.texcoord + half2(-pixelSize.x, -pixelSize.y));
        blur += GetScreenColor(i.texcoord + half2(pixelSize.x, pixelSize.y));
        blur += GetScreenColor(i.texcoord + half2(-pixelSize.x, pixelSize.y));
        blur *= 0.25;

        half4 sceneColor = GetScreenColor(i.texcoord);
        return sceneColor + (sceneColor - blur) * _Sharpness;
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
            #pragma fragment Frag_SharpenV2

            ENDHLSL
        }
    }
}