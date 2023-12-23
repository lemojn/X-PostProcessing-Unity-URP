Shader "Hidden/XPostProcessing/BulletTimeBlur"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    half4 _Params;

    #define _BulletTimeBlurControl _Params.x
    #define _BulletTimeUnblurRadius _Params.y
    #define _BulletTimeBlurCenterPoint _Params.zw

    half4 Frag_BulletTimeBlur(Varyings i) : SV_Target
    {
        float2 uvDis = i.texcoord - _BulletTimeBlurCenterPoint;
        float blurRadius = saturate(length(uvDis) - _BulletTimeUnblurRadius);
        float2 uv1 = i.texcoord - uvDis * blurRadius * _BulletTimeBlurControl;
        float2 uv2 = i.texcoord - uvDis * blurRadius * _BulletTimeBlurControl * 2;

        half4 col = GetScreenColor(i.texcoord);
        col += GetScreenColor(uv1);
        col += GetScreenColor(uv2);
        col.rgb /= 3;
        col.a = 1;
        return col;
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
            #pragma fragment Frag_BulletTimeBlur

            ENDHLSL
        }
    }
}