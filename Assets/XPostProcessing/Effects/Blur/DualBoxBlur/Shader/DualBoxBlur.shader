Shader "Hidden/XPostProcessing/Blur/DualBoxBlur"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    half4 _BlurOffset;

    half4 BoxFilter_4Tap(float2 uv, float2 texelSize)
    {
        float4 d = texelSize.xyxy * float4(-1.0, -1.0, 1.0, 1.0);
        
        half4 s = 0;
        s = GetScreenColor(uv + d.xy) * 0.25h;  // 1 MUL
        s += GetScreenColor(uv + d.zy) * 0.25h; // 1 MAD
        s += GetScreenColor(uv + d.xw) * 0.25h; // 1 MAD
        s += GetScreenColor(uv + d.zw) * 0.25h; // 1 MAD
        return s;
    }
    
    half4 Frag_BoxBlur(Varyings i) : SV_Target
    {
        return BoxFilter_4Tap(i.texcoord, _BlurOffset.xy);
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
            #pragma fragment Frag_BoxBlur
            
            ENDHLSL
        }
    }
}