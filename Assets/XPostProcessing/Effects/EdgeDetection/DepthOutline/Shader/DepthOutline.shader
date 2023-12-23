Shader "Hidden/XPostProcessing/EdgeDetection/DepthOutline"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    half2 _Params;

    #define _EdgeWidth _Params.x
    #define _Threshold _Params.y

    half GetEdge(float2 uv)
    {
        float2 offsetUV[4] = {
            _BlitTexture_TexelSize.xy * half2(1, 1) * _EdgeWidth,
            _BlitTexture_TexelSize.xy * half2(-1, -1) * _EdgeWidth,
            _BlitTexture_TexelSize.xy * half2(-1, 1) * _EdgeWidth,
            _BlitTexture_TexelSize.xy * half2(1, -1) * _EdgeWidth,
        };

        float sceneRawDepth0 = GetScreenDepth(uv + offsetUV[0]);
        float sceneRawDepth1 = GetScreenDepth(uv + offsetUV[1]);
        float sceneRawDepth2 = GetScreenDepth(uv + offsetUV[2]);
        float sceneRawDepth3 = GetScreenDepth(uv + offsetUV[3]);

        float zCS0 = LinearEyeDepth(sceneRawDepth0, _ZBufferParams);
        float zCS1 = LinearEyeDepth(sceneRawDepth1, _ZBufferParams);
        float zCS2 = LinearEyeDepth(sceneRawDepth2, _ZBufferParams);
        float zCS3 = LinearEyeDepth(sceneRawDepth3, _ZBufferParams);

        int dp0 = abs(zCS0 - zCS1) < _Threshold;
        int dp1 = abs(zCS2 - zCS3) < _Threshold;
        return dp0 * dp1;
    }

    half4 Frag_EdageOutline(Varyings i) : SV_Target
    {
        half4 col = GetScreenColor(i.texcoord);
        half edge = GetEdge(i.texcoord);
        col.rgb = lerp(col.rgb, 0, 1 - edge);
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
            #pragma fragment Frag_EdageOutline

            ENDHLSL
        }
    }
}