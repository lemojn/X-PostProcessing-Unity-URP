Shader "Hidden/XPostProcessing/Story/SpaceWarp"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    half _SpaceWarpLength;

    half4 Frag_SpaceWarp(Varyings i) : SV_Target
    {
        float2 uvDis = (i.texcoord - 0.5f) * 2;
        float2 direct = normalize(float3(uvDis, 0)).xy;
        float r = length(uvDis);
        float maxLength = sqrt(2);
        float radius = 1;
        float maxAngle = acos(_SpaceWarpLength / radius);
        float length = sqrt(1 - _SpaceWarpLength * _SpaceWarpLength);
        r /= maxLength;
        float angle = asin(r * length / radius);
        float rl = angle / maxAngle;
        float2 uv = rl * direct * 0.5f * maxLength + 0.5f;
        return GetScreenColor(uv);
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
            #pragma fragment Frag_SpaceWarp

            ENDHLSL
        }
    }
}