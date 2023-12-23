Shader "Hidden/XPostProcessing/EdgeDetection/Roberts"
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
        float bottomLeft = intensity(GetScreenColor(center + float2(-stepx, -stepy)));
        float topRight = intensity(GetScreenColor(center + float2(stepx, stepy)));
        float bottomRight = intensity(GetScreenColor(center + float2(stepx, -stepy)));
        
        // Roberts Operator
        // X = -1   0      Y = 0  -1
        //      0   1          1   0
        
        // Gx = sum(kernelX[i][j]*image[i][j])
        float Gx = -1.0 * topLeft + 1.0 * bottomRight;
        
        // Gy = sum(kernelY[i][j]*image[i][j]);
        float Gy = -1.0 * topRight + 1.0 * bottomLeft;
        
        float sobelGradient = sqrt((Gx * Gx) + (Gy * Gy));
        return sobelGradient;
    }
    
    half4 Frag_Roberts(Varyings i) : SV_Target
    {
        half4 sceneColor = GetScreenColor(i.texcoord);
        float sobelGradient = sobel(_EdgeWidth / _ScreenParams.x, _EdgeWidth / _ScreenParams.y, i.texcoord);
        sobelGradient = saturate(sobelGradient); // fix.
        half4 backgroundColor = lerp(sceneColor, _BackgroundColor, _BackgroundFade);
        float3 edgeColor = lerp(backgroundColor.rgb, _EdgeColor.rgb, sobelGradient);
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
            Tags { "LightMode" = "UniversalForward" }
            
            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag_Roberts
            
            ENDHLSL
        }
    }
}
