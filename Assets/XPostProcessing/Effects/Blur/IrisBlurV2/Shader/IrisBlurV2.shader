Shader "Hidden/XPostProcessing/Blur/IrisBlurV2"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    half3 _Gradient;
    half4 _GoldenRot;
    half4 _Params;
    
    #define _Offset _Gradient.xy
    #define _AreaSize _Gradient.z
    #define _Iteration _Params.x
    #define _Radius _Params.y
    #define _PixelSize _Params.zw
    
    float IrisMask(float2 uv)
    {
        float2 center = uv * 2.0 - 1.0 + _Offset; // [0,1] -> [-1,1]
        return dot(center, center) * _AreaSize;
    }
    
    half4 Frag_Preview(Varyings i) : SV_Target
    {
        return IrisMask(i.texcoord);
    }
    
    half4 Frag_IrisBlur(Varyings i) : SV_Target
    {
        half2x2 rot = half2x2(_GoldenRot);
        half4 accumulator = 0.0;
        half4 divisor = 0.0;
        
        half r = 1.0;
        half2 angle = half2(0.0, _Radius * saturate(IrisMask(i.texcoord)));
        
        for (int j = 0; j < _Iteration; j++)
        {
            r += 1.0 / r;
            angle = mul(rot, angle);
            float2 uv = i.texcoord + _PixelSize * (r - 1.0) * angle;
            half4 bokeh = GetScreenColor(uv);
            bokeh = saturate(bokeh); // track
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

        // Pass 0 - IrisBlur
        Pass
        {
            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag_IrisBlur
            
            ENDHLSL
        }
        
        // Pass 1 - Preview
        Pass
        {
            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag_Preview
            
            ENDHLSL
        }
    }
}