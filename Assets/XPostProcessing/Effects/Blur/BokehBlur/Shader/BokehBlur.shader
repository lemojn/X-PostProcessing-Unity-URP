Shader "Hidden/XPostProcessing/Blur/BokehBlur"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    half4 _GoldenRot;
    half4 _Params;
    
    #define _Iteration _Params.x
    #define _Radius _Params.y
    #define _PixelSize _Params.zw

    half4 Frag_BokehBlur(Varyings i) : SV_Target
    {
        half2x2 rot = half2x2(_GoldenRot);
        half4 accumulator = 0.0;
        half4 divisor = 0.0;

        half r = 1.0;
        half2 angle = half2(0.0, _Radius);

        for (int j = 0; j < _Iteration; j++)
        {
            r += 1.0 / r;
            angle = mul(rot, angle);
            float2 uv = i.texcoord + _PixelSize * (r - 1.0) * angle;
            half4 bokeh = GetScreenColor(uv);
            accumulator += bokeh * bokeh;
            divisor += bokeh;
        }
        return accumulator / divisor;
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
            #pragma fragment Frag_BokehBlur

            ENDHLSL
        }
    }
}