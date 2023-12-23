Shader "Hidden/XPostProcessing/Vignette/RapidVignette"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    half _VignetteIndensity;
    half2 _VignetteCenter;
    half4 _VignetteColor;

    struct Varyings_Vignette
    {
        float4 positionCS : SV_POSITION;
        float4 texcoord : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    Varyings_Vignette Vert_Vignette(Attributes v)
    {
        Varyings_Vignette o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        #if SHADER_API_GLES
            float4 pos = v.positionOS;
            float2 uv = v.uv;
        #else
            float4 pos = GetFullScreenTriangleVertexPosition(v.vertexID);
            float2 uv = GetFullScreenTriangleTexCoord(v.vertexID);
        #endif
        uv = uv * _BlitScaleBias.xy + _BlitScaleBias.zw;

        o.positionCS = pos;
        o.texcoord = float4(uv, uv - _VignetteCenter); // uv.zw: [0, 1]->[-0.5, 0.5]
        return o;
    }

    half4 Frag_Vignette(Varyings_Vignette i) : SV_Target
    {
        half4 finalColor = GetScreenColor(i.texcoord.xy);
        //求解vignette强度
        half vignetteIndensity = saturate(1.0 - dot(i.texcoord.zw, i.texcoord.zw) * _VignetteIndensity);
        return vignetteIndensity * finalColor;
    }

    half4 Frag_ColorAdjustVignette(Varyings_Vignette i) : SV_Target
    {
        half4 finalColor = GetScreenColor(i.texcoord.xy);
        //求解vignette强度
        half vignetteIndensity = saturate(1.0 - dot(i.texcoord.zw, i.texcoord.zw) * _VignetteIndensity);
        //基于vignette强度，插值VignetteColor颜色和场景颜色
        finalColor.rgb = lerp(_VignetteColor.rgb, finalColor.rgb, vignetteIndensity);
        return half4(finalColor.rgb, _VignetteColor.a);
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

            #pragma vertex Vert_Vignette
            #pragma fragment Frag_Vignette
            
            ENDHLSL
        }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex Vert_Vignette
            #pragma fragment Frag_ColorAdjustVignette
            
            ENDHLSL
        }
    }
}