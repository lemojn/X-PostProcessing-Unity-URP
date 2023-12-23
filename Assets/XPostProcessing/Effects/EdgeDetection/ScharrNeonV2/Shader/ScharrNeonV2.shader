Shader "Hidden/XPostProcessing/EdgeDetection/ScharrNeonV2"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    half4 _Params;
    half4 _BackgroundColor;

    #define _EdgeWidth _Params.x
    #define _EdgeNeonFade _Params.y
    #define _Brigtness _Params.z
    #define _BackgroundFade _Params.w
    
    float3 scharr(float stepx, float stepy, float2 center)
    {
        // get samples around pixel
        float3 topLeft = GetScreenColor(center + float2(-stepx, stepy)).rgb;
        float3 midLeft = GetScreenColor(center + float2(-stepx, 0)).rgb;
        float3 bottomLeft = GetScreenColor(center + float2(-stepx, -stepy)).rgb;
        float3 midTop = GetScreenColor(center + float2(0, stepy)).rgb;
        float3 midBottom = GetScreenColor(center + float2(0, -stepy)).rgb;
        float3 topRight = GetScreenColor(center + float2(stepx, stepy)).rgb;
        float3 midRight = GetScreenColor(center + float2(stepx, 0)).rgb;
        float3 bottomRight = GetScreenColor(center + float2(stepx, -stepy)).rgb;
        
        // scharr masks ( http://en.wikipedia.org/wiki/Sobel_operator#Alternative_operators)
        //     3 0 -3        3 10   3
        // X = 10 0 -10  Y = 0  0   0
        //     3 0 -3        -3 -10 -3
        
        // Gx = sum(kernelX[i][j]*image[i][j]);
        float3 Gx = 3.0 * topLeft + 10.0 * midLeft + 3.0 * bottomLeft - 3.0 * topRight - 10.0 * midRight - 3.0 * bottomRight;
        // Gy = sum(kernelY[i][j]*image[i][j]);
        float3 Gy = 3.0 * topLeft + 10.0 * midTop + 3.0 * topRight - 3.0 * bottomLeft - 10.0 * midBottom - 3.0 * bottomRight;
        
        float3 scharrGradient = sqrt((Gx * Gx) + (Gy * Gy)).rgb;
        return scharrGradient;
    }
    
    half4 Frag_ScharrNeonV2(Varyings i) : SV_Target
    {
        half4 sceneColor = GetScreenColor(i.texcoord);
        float3 scharrGradient = scharr(_EdgeWidth / _ScreenParams.x, _EdgeWidth / _ScreenParams.y, i.texcoord);
        scharrGradient = saturate(scharrGradient); // fix.
        half3 backgroundColor = lerp(_BackgroundColor.rgb, sceneColor.rgb, _BackgroundFade);
        half3 edgeColor = lerp(backgroundColor.rgb, scharrGradient.rgb, _EdgeNeonFade);
        return half4(edgeColor * _Brigtness, 1);
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
            #pragma fragment Frag_ScharrNeonV2

            ENDHLSL
        }
    }
}