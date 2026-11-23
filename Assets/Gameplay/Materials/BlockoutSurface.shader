Shader "GameMiners/Blockout Surface"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "UnityCG.cginc"

            struct Input
            {
                float4 position : POSITION;
                float3 normal : NORMAL;
            };

            struct Varyings
            {
                float4 position : SV_POSITION;
                float3 normal : TEXCOORD0;
            };

            float4 _Color;

            Varyings Vert(Input input)
            {
                Varyings output;
                output.position = UnityObjectToClipPos(input.position);
                output.normal = UnityObjectToWorldNormal(input.normal);
                return output;
            }

            float4 Frag(Varyings input) : SV_Target
            {
                float3 normal = normalize(input.normal);
                float lighting = 0.52 + 0.48 * saturate(dot(normal, normalize(float3(-0.42, 0.8, -0.4))));
                return float4(_Color.rgb * lighting, 1.0);
            }
            ENDHLSL
        }
    }
}
