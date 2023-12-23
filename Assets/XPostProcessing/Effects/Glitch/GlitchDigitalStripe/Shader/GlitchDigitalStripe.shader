Shader "Hidden/XPostProcessing/Glitch/DigitalStripe"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"
    #pragma shader_feature NEED_TRASH_FRAME

    TEXTURE2D(_NoiseTex); SAMPLER(sampler_NoiseTex);

    uniform half _Indensity;
    uniform half4 _StripColorAdjustColor;
    uniform half _StripColorAdjustIndensity;

    half4 Frag_DigitalStripe(Varyings i) : SV_Target
    {
        // 基础数据准备
        half4 stripNoise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, i.texcoord);
        half threshold = 1.001 - _Indensity * 1.001;
        // uv偏移
        half uvShift = step(threshold, pow(abs(stripNoise.x), 3));
        float2 uv = frac(i.texcoord + stripNoise.yz * uvShift);
        half4 source = GetScreenColor(uv);

        #ifndef NEED_TRASH_FRAME
            return source;
        #endif

        // 基于废弃帧插值
        half stripIndensity = step(threshold, pow(abs(stripNoise.w), 3)) * _StripColorAdjustIndensity;
        half3 color = lerp(source, _StripColorAdjustColor, stripIndensity).rgb;
        return half4(color, source.a);
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
            #pragma fragment Frag_DigitalStripe
            
            ENDHLSL
        }
    }
}
