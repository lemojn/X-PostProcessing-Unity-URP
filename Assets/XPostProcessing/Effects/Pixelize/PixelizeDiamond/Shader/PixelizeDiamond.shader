﻿Shader "Hidden/XPostProcessing/Pixelate/PixelizeDiamond"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    float _PixelSize;
    
    half4 Frag_PixelizeDiamond(Varyings i) : SV_Target
    {
        half2 pixelSize = 10 / _PixelSize;
        half2 coord = i.texcoord * pixelSize;
        //计算当前Diamond的朝向
        int direction = int(dot(frac(coord), half2(1, 1)) >= 1.0) + 2 * int(dot(frac(coord), half2(1, -1)) >= 0.0);
        //进行向下取整
        coord = floor(coord);
        //处理Diamond的四个方向
        if (direction == 0) coord += half2(0, 0.5);
        if (direction == 1) coord += half2(0.5, 1);
        if (direction == 2) coord += half2(0.5, 0);
        if (direction == 3) coord += half2(1, 0.5);
        //最终缩放uv
        coord /= pixelSize;
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
            #pragma fragment Frag_PixelizeDiamond

            ENDHLSL
        }
    }
}