Shader "Hidden/XPostProcessing/EdgeDetection/ScharrNeon"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    half4 _Params;
    half4 _BackgroundColor;

    #define _EdgeWidth _Params.x
    #define _Brigtness _Params.y
    #define _BackgroundFade _Params.z

    float intensity(in float4 color)
    {
        return sqrt((color.x * color.x) + (color.y * color.y) + (color.z * color.z));
    }

    float scharr(float stepx, float stepy, float2 center)
    {
        // get samples around pixel
        float topLeft = intensity(GetScreenColor(center + float2(-stepx, stepy)));
        float midLeft = intensity(GetScreenColor(center + float2(-stepx, 0)));
        float bottomLeft = intensity(GetScreenColor(center + float2(-stepx, -stepy)));
        float midTop = intensity(GetScreenColor(center + float2(0, stepy)));
        float midBottom = intensity(GetScreenColor(center + float2(0, -stepy)));
        float topRight = intensity(GetScreenColor(center + float2(stepx, stepy)));
        float midRight = intensity(GetScreenColor(center + float2(stepx, 0)));
        float bottomRight = intensity(GetScreenColor(center + float2(stepx, -stepy)));

        // scharr masks ( http://en.wikipedia.org/wiki/Sobel_operator#Alternative_operators)
        //     3 0 -3        3 10   3
        // X = 10 0 -10  Y = 0  0   0
        //     3 0 -3        -3 -10 -3

        // Gx = sum(kernelX[i][j]*image[i][j]);
        float Gx = 3.0 * topLeft + 10.0 * midLeft + 3.0 * bottomLeft - 3.0 * topRight - 10.0 * midRight - 3.0 * bottomRight;
        // Gy = sum(kernelY[i][j]*image[i][j]);
        float Gy = 3.0 * topLeft + 10.0 * midTop + 3.0 * topRight - 3.0 * bottomLeft - 10.0 * midBottom - 3.0 * bottomRight;

        float scharrGradient = sqrt((Gx * Gx) + (Gy * Gy));
        return scharrGradient;
    }

    half4 Frag_ScharrNeon(Varyings i) : SV_Target
    {
        half4 sceneColor = GetScreenColor(i.texcoord);
        float scharrGradient = scharr(_EdgeWidth / _ScreenParams.x, _EdgeWidth / _ScreenParams.y, i.texcoord);
        scharrGradient = saturate(scharrGradient); // fix.
        //BackgroundFading
        half4 backgroundColor = lerp(sceneColor, _BackgroundColor, _BackgroundFade);
        //Edge Opacity
        half3 edgeColor = lerp(backgroundColor.rgb, sceneColor.rgb, scharrGradient);
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
            #pragma fragment Frag_ScharrNeon

            ENDHLSL
        }
    }
}