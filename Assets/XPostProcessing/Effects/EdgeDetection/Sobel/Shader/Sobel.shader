Shader "Hidden/XPostProcessing/EdgeDetection/Sobel"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    half2 _Params;
    half4 _EdgeColor;
    half4 _BackgroundColor;

    #define _EdgeWidth _Params.x
    #define _BackgroundFade _Params.y

    float intensity(in float4 color)
    {
        return sqrt((color.x * color.x) + (color.y * color.y) + (color.z * color.z));
    }
    
    float sobel(float stepx, float stepy, float2 center)
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
        
        // Sobel masks (see http://en.wikipedia.org/wiki/Sobel_operator)
        //        1 0 -1     -1 -2 -1
        //    X = 2 0 -2  Y = 0  0  0
        //        1 0 -1      1  2  1

        // Gx = sum(kernelX[i][j]*image[i][j])
        float Gx = topLeft + 2.0 * midLeft + bottomLeft - topRight - 2.0 * midRight - bottomRight;
        // Gy = sum(kernelY[i][j]*image[i][j]);
        float Gy = -topLeft - 2.0 * midTop - topRight + bottomLeft + 2.0 * midBottom + bottomRight;
        float sobelGradient = sqrt((Gx * Gx) + (Gy * Gy));
        return sobelGradient;
    }

    half4 Frag_Sobel(Varyings i) : SV_Target
    {
        half4 sceneColor = GetScreenColor(i.texcoord);
        float sobelGradient = sobel(_EdgeWidth / _ScreenParams.x, _EdgeWidth / _ScreenParams.y, i.texcoord);
        sobelGradient = saturate(sobelGradient); // fix.
        half4 backgroundColor = lerp(sceneColor, _BackgroundColor, _BackgroundFade);
        half3 edgeColor = lerp(backgroundColor.rgb, _EdgeColor.rgb, sobelGradient);
        return half4(edgeColor, 1);
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
            #pragma fragment Frag_Sobel

            ENDHLSL
        }
    }
}