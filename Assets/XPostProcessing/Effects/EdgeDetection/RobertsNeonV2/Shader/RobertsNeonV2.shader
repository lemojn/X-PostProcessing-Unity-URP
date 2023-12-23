Shader "Hidden/XPostProcessing/EdgeDetection/RobertsNeonV2"
{
    HLSLINCLUDE

    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"

    half4 _Params;
    half4 _BackgroundColor;

    #define _EdgeWidth _Params.x
    #define _EdgeNeonFade _Params.y
    #define _Brigtness _Params.z
    #define _BackgroundFade _Params.w
    
    float3 sobel(float stepx, float stepy, float2 center)
    {
        // get samples around pixel
        float3 topLeft = GetScreenColor(center + float2(-stepx, stepy)).rgb;
        float3 bottomLeft = GetScreenColor(center + float2(-stepx, -stepy)).rgb;
        float3 topRight = GetScreenColor(center + float2(stepx, stepy)).rgb;
        float3 bottomRight = GetScreenColor(center + float2(stepx, -stepy)).rgb;
        
        // Roberts Operator
        // X = -1   0      Y = 0  -1
        //      0   1          1   0
        
        // Gx = sum(kernelX[i][j]*image[i][j])
        float3 Gx = -1.0 * topLeft + 1.0 * bottomRight;
        
        // Gy = sum(kernelY[i][j]*image[i][j]);
        float3 Gy = -1.0 * topRight + 1.0 * bottomLeft;
        
        float3 sobelGradient = sqrt((Gx * Gx) + (Gy * Gy));
        return sobelGradient;
    }
    
    half4 Frag_RobertsNeonV2(Varyings i) : SV_Target
    {
        half4 sceneColor = GetScreenColor(i.texcoord);
        float3 sobelGradient = sobel(_EdgeWidth / _ScreenParams.x, _EdgeWidth / _ScreenParams.y, i.texcoord);
        sobelGradient = saturate(sobelGradient); // fix.
        half3 backgroundColor = lerp(_BackgroundColor.rgb, sceneColor.rgb, _BackgroundFade);
        //Edge Opacity
        float3 edgeColor = lerp(backgroundColor.rgb, sobelGradient.rgb, _EdgeNeonFade);
        return float4(edgeColor * _Brigtness, 1);
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
            #pragma fragment Frag_RobertsNeonV2

            ENDHLSL
        }
    }
}