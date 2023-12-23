Shader "Hidden/XPostProcessing/Blur/GaussianBlur"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    half4 _BlurOffset;
    
    struct Varyings_GaussianBlur
    {
        float4 positionCS : SV_POSITION;
        float2 texcoord : TEXCOORD0;
        float4 uv01 : TEXCOORD1;
        float4 uv23 : TEXCOORD2;
        float4 uv45 : TEXCOORD3;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    Varyings_GaussianBlur Vert_GaussianBlur(Attributes v)
    {
        Varyings_GaussianBlur o;
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
        o.uv01 = o.texcoord.xyxy + _BlurOffset.xyxy * float4(1, 1, -1, -1);
        o.uv23 = o.texcoord.xyxy + _BlurOffset.xyxy * float4(1, 1, -1, -1) * 2.0;
        o.uv45 = o.texcoord.xyxy + _BlurOffset.xyxy * float4(1, 1, -1, -1) * 6.0;
        return o;
    }
    
    float4 Frag_GaussianBlur(Varyings_GaussianBlur i) : SV_Target
    {
        half4 color = float4(0, 0, 0, 0);
        color += 0.40 * GetScreenColor(i.texcoord);
        color += 0.15 * GetScreenColor(i.uv01.xy);
        color += 0.15 * GetScreenColor(i.uv01.zw);
        color += 0.10 * GetScreenColor(i.uv23.xy);
        color += 0.10 * GetScreenColor(i.uv23.zw);
        color += 0.05 * GetScreenColor(i.uv45.xy);
        color += 0.05 * GetScreenColor(i.uv45.zw);
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

            #pragma vertex Vert_GaussianBlur
            #pragma fragment Frag_GaussianBlur
            
            ENDHLSL
        }
    }
}