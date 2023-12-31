﻿Shader "Hidden/XPostProcessing/Pixelate/PixelizeTriangle"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    half4 _Params;
    #define _PixelSize _Params.x
    #define _PixelRatio _Params.y
    #define _PixelScaleX _Params.z
    #define _PixelScaleY _Params.w

    half4 Frag_PixelizeTriangle(Varyings i) : SV_Target
    {
        float2 uv = i.texcoord;
        float2 pixelScale = _PixelSize * float2(_PixelScaleX, _PixelScaleY / _PixelRatio);
        //乘以缩放，向下取整，再除以缩放，得到分段UV
        float2 coord = floor(uv * pixelScale) / pixelScale;
        uv -= coord;
        uv *= pixelScale;
        //进行三角形像素偏移处理
        coord += float2(step(1.0 - uv.y, uv.x) / (2.0 * pixelScale.x), //X.
        step(uv.x, uv.y) / (2.0 * pixelScale.y)); //Y.
        return GetScreenColor(coord);
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
            #pragma fragment Frag_PixelizeTriangle

            ENDHLSL
        }
    }
}