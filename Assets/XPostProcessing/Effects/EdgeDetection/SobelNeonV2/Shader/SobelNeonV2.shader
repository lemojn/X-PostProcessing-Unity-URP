Shader "Hidden/XPostProcessing/EdgeDetection/SobelNeonV2"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    half4 _Params;
    half4 _BackgroundColor;

    #define _EdgeWidth _Params.x
    #define _EdgeNeonFade _Params.y
    #define _Brigtness _Params.z
    #define _BackgroundFade _Params.w

    float intensity(in float4 color)
    {
        return sqrt((color.x * color.x) + (color.y * color.y) + (color.z * color.z));
    }

    float3 sobel(float stepx, float stepy, float2 center)
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

        // Sobel masks (see http://en.wikipedia.org/wiki/Sobel_operator)
        //        1 0 -1     -1 -2 -1
        //    X = 2 0 -2  Y = 0  0  0
        //        1 0 -1      1  2  1

        // Gx = sum(kernelX[i][j]*image[i][j])
        float3 Gx = topLeft + 2.0 * midLeft + bottomLeft - topRight - 2.0 * midRight - bottomRight;
        // Gy = sum(kernelY[i][j]*image[i][j]);
        float3 Gy = -topLeft - 2.0 * midTop - topRight + bottomLeft + 2.0 * midBottom + bottomRight;
        float3 sobelGradient = sqrt((Gx * Gx) + (Gy * Gy));
        return sobelGradient;
    }

    half4 Frag_SobelNeonV2(Varyings i) : SV_Target
    {
        half4 sceneColor = GetScreenColor(i.texcoord);
        float3 sobelGradient = sobel(_EdgeWidth / _ScreenParams.x, _EdgeWidth / _ScreenParams.y, i.texcoord);
        sobelGradient = saturate(sobelGradient); // fix.
        half3 backgroundColor = lerp(_BackgroundColor.rgb, sceneColor.rgb, _BackgroundFade);
        half3 edgeColor = lerp(backgroundColor.rgb, sobelGradient.rgb, _EdgeNeonFade);
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
            #pragma fragment Frag_SobelNeonV2

            ENDHLSL
        }
    }
}