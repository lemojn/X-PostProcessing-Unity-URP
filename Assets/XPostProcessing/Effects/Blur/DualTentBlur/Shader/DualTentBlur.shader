Shader "Hidden/XPostProcessing/Blur/DualTentBlur"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    half4 _BlurOffset;
    
    // 9-tap tent filter
    half4 TentFilter_9Tap(float2 uv, float2 texelSize)
    {
        float4 d = texelSize.xyxy * float4(1.0, 1.0, -1.0, 0.0);
        
        half4 s;
        s = GetScreenColor(uv - d.xy);
        s += GetScreenColor(uv - d.wy) * 2.0; // 1 MAD
        s += GetScreenColor(uv - d.zy); // 1 MAD
        
        s += GetScreenColor(uv + d.zw) * 2.0; // 1 MAD
        s += GetScreenColor(uv) * 4.0; // 1 MAD
        s += GetScreenColor(uv + d.xw) * 2.0; // 1 MAD
        
        s += GetScreenColor(uv + d.zy);
        s += GetScreenColor(uv + d.wy) * 2.0; // 1 MAD
        s += GetScreenColor(uv + d.xy);
        
        return s / 16.0;
    }
    
    half4 Frag_TentBlur(Varyings i) : SV_Target
    {
        return TentFilter_9Tap(i.texcoord, _BlurOffset.xy);
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
            #pragma fragment Frag_TentBlur
            
            ENDHLSL
        }
    }
}