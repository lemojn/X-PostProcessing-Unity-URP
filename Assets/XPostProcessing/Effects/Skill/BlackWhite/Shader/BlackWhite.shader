Shader "Hidden/XPostProcessing/Skill/BlackWhite"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    TEXTURE2D(_NoiseTex); SAMPLER(sampler_NoiseTex);
    TEXTURE2D(_DissolveTex); SAMPLER(sampler_DissolveTex);

    float4 _Params1;
    float4 _Params2;
    half3 _Color;

    #define _GreyThreshold _Params1.x
    #define _Center _Params1.yz
    #define _NoiseTillingX _Params2.x
    #define _NoiseTillingY _Params2.y
    #define _NoiseSpeed _Params2.z
    #define _ChangeRate _Params2.w


    half luminance(half3 color)
    {
        return dot(color, half3(0.222, 0.707, 0.071));
    }

    half4 Frag_BlackWhite(Varyings i) : SV_Target
    {
        half grey = luminance(GetScreenColor(i.texcoord).rgb);
        //极坐标纹理
        float2 centerdUV = i.texcoord - _Center;
        float2 polarUV = float2(length(centerdUV) * _NoiseTillingX * 2, atan2(centerdUV.x, centerdUV.y) * (1.0 / TWO_PI) * _NoiseTillingY);
        polarUV += _Time.y * _NoiseSpeed.xx;
        half polarColor = luminance(SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, polarUV).rgb);
        half dissloveColor = SAMPLE_TEXTURE2D(_DissolveTex, sampler_DissolveTex, polarUV * 0.5).r;
        polarColor *= dissloveColor;
        grey = grey + grey * polarColor;
        grey = lerp(grey, 1 - grey, _ChangeRate);
        grey = saturate(grey);
        half3 finalColor = saturate(grey.rrr * _Color);
        return smoothstep(1 - _GreyThreshold, _GreyThreshold, half4(finalColor, 1));
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
            #pragma fragment Frag_BlackWhite

            ENDHLSL
        }
    }
}