Shader "Hidden/XPostProcessing/ImageProcessing/Sharpen"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    uniform half _Strength;
    uniform half _Threshold;
    
    half4 Frag_Sharpen(Varyings i) : SV_Target
    {
        half2 pixelSize = float2(1 / _ScreenParams.x, 1 / _ScreenParams.y);
        half2 halfPixelSize = pixelSize * 0.5;

        half4 blur = GetScreenColor(i.texcoord + half2(halfPixelSize.x, -pixelSize.y));
        blur += GetScreenColor(i.texcoord + half2(-pixelSize.x, -halfPixelSize.y));
        blur += GetScreenColor(i.texcoord + half2(pixelSize.x, halfPixelSize.y));
        blur += GetScreenColor(i.texcoord + half2(-halfPixelSize.x, pixelSize.y));
        blur *= 0.25;

        half4 sceneColor = GetScreenColor(i.texcoord);
        half4 lumaStrength = half4(0.222, 0.707, 0.071, 0.0) * _Strength;
        half4 sharp = sceneColor - blur;

        sceneColor += clamp(dot(sharp, lumaStrength), -_Threshold, _Threshold);
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
            #pragma fragment Frag_Sharpen

            ENDHLSL
        }
    }
}