﻿Shader "Hidden/XPostProcessing/Pixelate/PixelizeSector"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    float4 _Params;
    float2 _Params2;
    half4 _BackgroundColor;

    #define _PixelIntervalX _Params2.x
    #define _PixelIntervalY _Params2.y
    
    half4 Frag_PixelizeSector(Varyings i) : SV_Target
    {
        float2 uv = i.texcoord;
        float pixelScale = 1.0 / _Params.x;
        float ratio = _ScreenParams.y / _ScreenParams.x;
        uv.x = uv.x / ratio;
        //x和y坐标分别除以缩放系数，在用floor向下取整，再乘以缩放系数，得到分段UV
        float2 coord = half2(_PixelIntervalX * floor(uv.x / (pixelScale * _PixelIntervalX)), (_PixelIntervalY) * floor(uv.y / (pixelScale * _PixelIntervalY)));
        //设定扇形坐标
        float2 circleCenter = coord * pixelScale;
        //计算当前uv值隔圆心的距离，并乘以缩放系数
        float dist = length(uv - circleCenter) * _Params.x;
        //圆心坐标乘以缩放系数
        circleCenter.x *= ratio;
        //采样
        half4 screenColor = GetScreenColor(circleCenter);
        //对于距离大于半径的像素，替换为背景色
        if (dist > _Params.z)
            screenColor = _BackgroundColor;
        return screenColor;
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
            #pragma fragment Frag_PixelizeSector

            ENDHLSL
        }
    }
}