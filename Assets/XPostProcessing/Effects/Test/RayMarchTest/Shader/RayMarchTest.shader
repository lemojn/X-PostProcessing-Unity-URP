Shader "Hidden/XPostProcessing/Test/RayMarchTest"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    float4x4 _FrustumCornersRay;

    struct Attributes_FrustumCornersRay
    {
        float4 positionOS : POSITION;
        float2 uv : TEXCOORD0;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    VaryingsRay Vert_FrustumCornersRay(Attributes_FrustumCornersRay v)
    {
        VaryingsRay o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
        o.texcoord = v.uv;

        int index = 0;
        float uvX = v.uv.x;
        float uvY = v.uv.y;
        if (uvX < 0.5 && uvY < 0.5)
            index = 0;
        else if (uvX > 0.5 && uvY < 0.5)
            index = 1;
        else if (uvX > 0.5 && uvY > 0.5)
            index = 2;
        else
            index = 3;

        #if UNITY_UV_STARTS_AT_TOP
            if (_BlitTexture_TexelSize.y < 0)
                index = 3 - index;
        #endif

        o.interpolatedRay = _FrustumCornersRay[index].xyz;
        return o;
    }

    half4 FragRay(VaryingsRay i) : SV_Target
    {
        float depth = GetScreenLinearEyeDepth(i.texcoord);
        float3 worldPos = GetWorldPositionByRay(i.interpolatedRay, depth);
        return half4(worldPos, 1);
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

            #pragma vertex Vert_FrustumCornersRay
            #pragma fragment FragRay
            
            ENDHLSL
        }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex VertRay
            #pragma fragment FragRay
            
            ENDHLSL
        }
    }
}