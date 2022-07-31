Shader "BurstDrawTrianglesShader" {
    Properties
    {
    }
    SubShader
    {
        Tags {"RenderType" = "Opaque" "IgnoreProjector" = "True" "RenderPipeline" = "UniversalPipeline" "ShaderModel"="2.0"}
        Cull Off
 
        Pass
        {
            Name "Unlit"
 
            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5
 
            #pragma vertex vert
            #pragma fragment frag
 
 
            #include "UnityCG.cginc"
 
            uniform StructuredBuffer<float4> verticesBuffer;
 
            struct vtf
            {
                float4 vertex : SV_POSITION;
                   float4 color : TEXCOORD0;
            };
 
            // color packed into float4.w (1 byte per RGBA channel)
            float4 unpack(float i)
            {
                return float4(i / 262144.0, i / 4096.0, i / 64.0, i) % 64.0 / 63;
            }
 
            vtf vert(uint vid : SV_VertexID)
            {
                vtf output;
                float4 pos = verticesBuffer[vid];
                output.vertex = mul(UNITY_MATRIX_VP, float4(pos.xyz, 1));
                output.color = unpack(pos.w);
                return output;
            }
 
            half4 frag(vtf input) : SV_Target
            {
                return float4(1,0,0,1);
            }
            ENDHLSL
        }
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
 
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            #pragma target 4.5
 
            #include "UnityCG.cginc"
 
            StructuredBuffer<float4> verticesBuffer;
 
            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 color : TEXCOORD0;
            };
 
            float4 unpack(float i)
            {
                return float4(i / 262144.0, i / 4096.0, i / 64.0, i) % 64.0 / 63;
            }
 
            v2f vert(uint vid : SV_VertexID)
            {
                v2f o;
                float4 pos = verticesBuffer[vid];
                o.pos = mul(UNITY_MATRIX_VP, float4(pos.xyz, 1));
                o.color = unpack(pos.w);
                return o;
            }
 
            fixed4 frag(v2f i) : SV_Target
            {
                return i.color;
            }
 
            ENDCG
        }
    }
}