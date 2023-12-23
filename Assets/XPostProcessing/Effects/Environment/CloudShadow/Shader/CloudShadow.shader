Shader "Hidden/XPostProcessing/Environment/CloudShadow"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    TEXTURE2D(_CloudTex); SAMPLER(sampler_CloudTex);

    bool _CloudShadowMode;
    half4 _CloudShadowColor;
    half4 _CloudTiling; // xy: Scale, z: Strength, w: Distance.
    half4 _WindFactor; // xy: WindDirection, zw: CutoffRange.
    uint _CloudHeight;
    
    float4 Frag_CloudShadow(VaryingsRay i) : SV_Target
    {
        half4 col = GetScreenColor(i.texcoord);
        float depth = GetScreenLinearEyeDepth(i.texcoord);
        float3 worldPos = GetWorldPositionByRay(i.interpolatedRay, depth);

        float dis = distance(_WorldSpaceCameraPos.xyz, worldPos);
        float disStrength = 1 - saturate(dis / _CloudTiling.w);
        float3 L = _MainLightPosition.xyz;

        float2 cloud_uv;
        float2 offuv = float2(_Time.x * _WindFactor.x, _Time.x * _WindFactor.y) * 0.05f;
        if (_CloudShadowMode)
        {
            //阴影受到光方向影响
            float hd = _CloudHeight - worldPos.y;          //获取与云层的高度差
            float t = hd / dot(L, float3(0, 1, 0));
            float3 cloudCrossPos = L * t + worldPos;    //光源到像素的射线与云层交点
            cloud_uv = cloudCrossPos.xz / _CloudTiling.xy;
        }
        else
        {
            //垂直投影
            cloud_uv.xy = worldPos.xz / _CloudTiling.xy;
        }
        cloud_uv += offuv;
        
        float cloud = (1 - SAMPLE_TEXTURE2D(_CloudTex, sampler_CloudTex, cloud_uv).r);
        cloud = smoothstep(_WindFactor.z, _WindFactor.w, cloud) * _CloudTiling.z;
        cloud = lerp(0, cloud, disStrength);
        float3 targetColor = cloud * (1 - _CloudShadowColor.rgb);
        col.rgb -= targetColor * col.rgb;
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

            #pragma vertex VertRay
            #pragma fragment Frag_CloudShadow
            
            ENDHLSL
        }
    }
}
