Shader "Hidden/XPostProcessing/Blur/DualKawaseBlur"
{
    HLSLINCLUDE
    
    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    half _BlurOffset;
    
    // Down sample.
    struct Varyings_DownSample
    {
        float4 positionCS : SV_POSITION;
        float2 texcoord : TEXCOORD0;
        float4 uv01 : TEXCOORD1;
        float4 uv23 : TEXCOORD2;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    Varyings_DownSample Vert_DownSample(Attributes v)
    {
        Varyings_DownSample o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        #if SHADER_API_GLES
            float4 pos = v.positionOS;
            float2 uv = v.uv;
        #else
            float4 pos = GetFullScreenTriangleVertexPosition(v.vertexID);
            float2 uv = GetFullScreenTriangleTexCoord(v.vertexID);
        #endif
        float2 texelSize = _BlitTexture_TexelSize.xy * 0.5;
        float offset = 1 + _BlurOffset;

        o.positionCS = pos;
        o.texcoord = uv * _BlitScaleBias.xy + _BlitScaleBias.zw;
        o.uv01.xy = o.texcoord - texelSize * offset; // top right
        o.uv01.zw = o.texcoord + texelSize * offset; // bottom left
        o.uv23.xy = o.texcoord - float2(texelSize.x, -texelSize.y) * offset; // top left
        o.uv23.zw = o.texcoord + float2(texelSize.x, -texelSize.y) * offset; // bottom right
        return o;
    }
    
    half4 Frag_DownSample(Varyings_DownSample i) : SV_Target
    {
        half4 sum = GetScreenColor(i.texcoord) * 4;
        sum += GetScreenColor(i.uv01.xy);
        sum += GetScreenColor(i.uv01.zw);
        sum += GetScreenColor(i.uv23.xy);
        sum += GetScreenColor(i.uv23.zw);
        return sum * 0.125;
    }

    // Up sample.
    struct Varyings_UpSample
    {
        float4 positionCS : SV_POSITION;
        float4 uv01 : TEXCOORD0;
        float4 uv23 : TEXCOORD1;
        float4 uv45 : TEXCOORD2;
        float4 uv67 : TEXCOORD3;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    Varyings_UpSample Vert_UpSample(Attributes v)
    {
        Varyings_UpSample o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        #if SHADER_API_GLES
            float4 pos = v.positionOS;
            float2 uv = v.uv;
        #else
            float4 pos = GetFullScreenTriangleVertexPosition(v.vertexID);
            float2 uv = GetFullScreenTriangleTexCoord(v.vertexID);
        #endif
        float2 texelSize = _BlitTexture_TexelSize.xy * 0.5;
        float offset = 1 + _BlurOffset;

        o.positionCS = pos;
        float2 texcoord = uv * _BlitScaleBias.xy + _BlitScaleBias.zw;
        o.uv01.xy = texcoord + float2(-texelSize.x * 2, 0) * offset;
        o.uv01.zw = texcoord + float2(-texelSize.x, texelSize.y) * offset;
        o.uv23.xy = texcoord + float2(0, texelSize.y * 2) * offset;
        o.uv23.zw = texcoord + texelSize * offset;
        o.uv45.xy = texcoord + float2(texelSize.x * 2, 0) * offset;
        o.uv45.zw = texcoord + float2(texelSize.x, -texelSize.y) * offset;
        o.uv67.xy = texcoord + float2(0, -texelSize.y * 2) * offset;
        o.uv67.zw = texcoord - texelSize * offset;
        return o;
    }
    
    half4 Frag_UpSample(Varyings_UpSample i) : SV_Target
    {
        half4 sum = 0;
        sum += GetScreenColor(i.uv01.xy);
        sum += GetScreenColor(i.uv01.zw) * 2;
        sum += GetScreenColor(i.uv23.xy);
        sum += GetScreenColor(i.uv23.zw) * 2;
        sum += GetScreenColor(i.uv45.xy);
        sum += GetScreenColor(i.uv45.zw) * 2;
        sum += GetScreenColor(i.uv67.xy);
        sum += GetScreenColor(i.uv67.zw) * 2;
        return sum * 0.0833;
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

            #pragma vertex Vert_DownSample
            #pragma fragment Frag_DownSample
            
            ENDHLSL
        }
        
        Pass
        {
            HLSLPROGRAM

            #pragma vertex Vert_UpSample
            #pragma fragment Frag_UpSample
            
            ENDHLSL
        }
    }
}