Shader "Hidden/XPostProcessing/ColorAdjustment/ColorReplaceV2"
{
    HLSLINCLUDE
    
    #include "../../../../ShaderLibrary/XPostProcessing.hlsl"
    
    uniform half4 _FromColor;
    uniform half4 _ToColor;
    uniform half _Range;
    uniform half _Fuzziness;

    half3 ColorReplaceV2(half3 color, half3 from, half3 to, half range, half fuzziness)
    {
        half dist = distance(from, color);
        half3 col = lerp(to, color, saturate((dist - range) / max(fuzziness, 0.1)));
        return col;
    }

    half4 Frag_ColorReplaceV2(Varyings i) : SV_Target
    {
        half4 sceneColor = GetScreenColor(i.texcoord);
        half3 finalColor = ColorReplaceV2(sceneColor.rgb, _FromColor.rgb, _ToColor.rgb, _Range, _Fuzziness);
        return half4(finalColor, 1.0);
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
            #pragma fragment Frag_ColorReplaceV2

            ENDHLSL
        }
    }
}