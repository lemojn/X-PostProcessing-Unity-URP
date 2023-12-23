Shader "Hidden/XPostProcessing/ImageProcessing/SharpenV3"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    uniform half _CentralFactor;
    uniform half _SideFactor;

    struct Varyings_SharpenV3
    {
        float4 positionCS : SV_POSITION;
        float2 texcoord : TEXCOORD0;
        float4 uv1 : TEXCOORD1;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    Varyings_SharpenV3 Vert_SharpenV3(Attributes v)
    {
        Varyings_SharpenV3 o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        #if SHADER_API_GLES
            float4 pos = v.positionOS;
            float2 uv = v.uv;
        #else
            float4 pos = GetFullScreenTriangleVertexPosition(v.vertexID);
            float2 uv = GetFullScreenTriangleTexCoord(v.vertexID);
        #endif

        o.positionCS = pos;
        o.texcoord = uv * _BlitScaleBias.xy + _BlitScaleBias.zw;
        o.uv1 = float4(o.texcoord - _BlitTexture_TexelSize.xy, o.texcoord + _BlitTexture_TexelSize.xy);
        return o;
    }

    half4 Frag_SharpenV3(Varyings_SharpenV3 i) : SV_Target
    {
        half4 color = GetScreenColor(i.texcoord) * _CentralFactor;
        color -= GetScreenColor(i.uv1.xy) * _SideFactor;
        color -= GetScreenColor(i.uv1.xw) * _SideFactor;
        color -= GetScreenColor(i.uv1.zy) * _SideFactor;
        color -= GetScreenColor(i.uv1.zw) * _SideFactor;
        return color;
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

            #pragma vertex Vert_SharpenV3
            #pragma fragment Frag_SharpenV3

            ENDHLSL
        }
    }
}