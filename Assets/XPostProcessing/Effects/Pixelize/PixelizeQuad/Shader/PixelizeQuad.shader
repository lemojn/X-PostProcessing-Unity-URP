Shader "Hidden/XPostProcessing/Pixelate/PixelizeQuad"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    half4 _Params;
    #define _PixelSize _Params.x
    #define _PixelRatio _Params.y
    #define _PixelScaleX _Params.z
    #define _PixelScaleY _Params.w

    half4 Frag_PixelizeQuad(Varyings i) : SV_Target
    {
        float2 uv = i.texcoord;
        float pixelScale = 1.0 / _PixelSize;
        // Divide by the scaling factor, round up, and multiply by the scaling factor to get the segmented UV
        float2 coord = half2(pixelScale * _PixelScaleX * floor(uv.x / (pixelScale * _PixelScaleX)), (pixelScale * _PixelRatio * _PixelScaleY) * floor(uv.y / (pixelScale * _PixelRatio * _PixelScaleY)));
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
            #pragma fragment Frag_PixelizeQuad

            ENDHLSL
        }
    }
}