Shader "Hidden/XPostProcessing/Environment/LightShift"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    half4 _BloomTintAndThreshold;
    half4 _LightShaftParameters;

    #define BloomIntensity _LightShaftParameters.y
    #define InvOcclusionDepthRange _LightShaftParameters.x
    #define TextureSpaceBlurOrigin _LightShaftParameters.zw //光源对应的屏幕坐标: SunScreenPosition (0-1).

    half4 Frag_Highlight(Varyings i) : SV_TARGET
    {
        half3 SceneColor = GetScreenColor(i.texcoord).rgb;
        half SceneDepth01 = GetScreenLinear01Depth(i.texcoord);

        float EdgeMask = 1.0f - i.texcoord.x * (1.0f - i.texcoord.x) * i.texcoord.y * (1.0f - i.texcoord.y) * 8.0f;
        EdgeMask = EdgeMask * EdgeMask * EdgeMask * EdgeMask;
        // Only bloom colors over BloomThreshold
        float Luminance = max(dot(SceneColor, half3(.3f, .59f, .11f)), 6.10352e-5);
        float AdjustedLuminance = max(Luminance - _BloomTintAndThreshold.a, 0.0f);
        half3 BloomColor = BloomIntensity * SceneColor / Luminance * AdjustedLuminance * 2.0f;
        // Only allow bloom from pixels whose depth are in the far half of OcclusionDepthRange
        float BloomDistanceMask = saturate((SceneDepth01 - 0.5f / InvOcclusionDepthRange) * InvOcclusionDepthRange);
        // 需要考虑AspectRatio 先定为1
        float AspectRatioAndInvAspectRatio = 1;
        float BlurOriginDistanceMask = 1.0f - saturate(length(TextureSpaceBlurOrigin.xy - i.texcoord * AspectRatioAndInvAspectRatio) * 2.0f);
        // Calculate bloom color with masks applied
        half3 rgb = BloomColor * _BloomTintAndThreshold.rgb * BloomDistanceMask * (1.0f - EdgeMask) * BlurOriginDistanceMask * BlurOriginDistanceMask;
        return half4(rgb, 1);
    }

    //----------------------------------------------------------------------

    int _SampleDistance;
    #define SC 8.0

    half4 Frag_Blur(Varyings i) : SV_TARGET
    {
        half3 SceneColor = GetScreenColor(i.texcoord).rgb;
        float2 d = i.texcoord - TextureSpaceBlurOrigin;
        float p = 0.01;
        half3 rgb = SceneColor;
        float2 uvd = d * p * _SampleDistance / SC;
        for (int idx = 1; idx <= SC; idx++)
        {
            rgb += GetScreenColor(i.texcoord - uvd * idx).rgb;
        }
        rgb /= (SC + 1);
        return half4(rgb, 1);
    }

    //----------------------------------------------------------------------

    TEXTURE2D(_BluredTexture); SAMPLER(sampler_BluredTexture);
    half _Attenuation;

    half4 Frag_Final(Varyings i) : SV_TARGET
    {
        half3 originColor = GetScreenColor(i.texcoord).rgb;
        half4 blured = SAMPLE_TEXTURE2D(_BluredTexture, sampler_BluredTexture, i.texcoord);
        return half4(originColor + blured.rgb * _Attenuation, 1);
    }

    //----------------------------------------------------------------------

    // half4 Frag_Mask(Varyings i) : SV_TARGET
    // {
    //     half3 originColor = GetScreenColor(i.texcoord);
    //     half l = originColor.r * .299 + originColor.g * .587 + originColor.b * .114;
    //     return half4(l, l, l, 1);
    // }

    ENDHLSL

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }
        
        Cull Off
        ZWrite Off
        ZTest Always

        //0.提取高亮区域
        Pass
        {
            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag_Highlight

            ENDHLSL
        }

        //1.径向模糊
        Pass
        {
            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag_Blur

            ENDHLSL
        }

        //2.颜色相加
        Pass
        {
            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag_Final

            ENDHLSL
        }

        // //3.提取Mask
        // Pass
        // {
        //     HLSLPROGRAM

        //     #pragma vertex Vert
        //     #pragma fragment Frag_Mask

        //     ENDHLSL
        // }
    }
}