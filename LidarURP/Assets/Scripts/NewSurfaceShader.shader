Shader "Point Cloud"
{
    SubShader
    {
        Pass
        {
            CGPROGRAM        
            #pragma vertex VSMain
            #pragma fragment PSMain
            #pragma target 5.0
 
            StructuredBuffer<float> depthbuffer;
            StructuredBuffer<float4> colorbuffer;
           
            struct shaderdata
            {
                float4 vertex : SV_POSITION;
                float4 color : TEXCOORD1;
            };
 
            shaderdata VSMain(uint id : SV_VertexID)
            {
                shaderdata vs;
                float depth = depthbuffer[id];
                float factor = 512;
                float u = fmod (id, factor) / factor - 0.5;
                float v = floor (id / factor) / factor - 0.5;
                vs.vertex = UnityObjectToClipPos(float4(4.0*u, depth, 4.0*v, 1.0));
                vs.color = colorbuffer[id];
                return vs;
            }
 
            float4 PSMain(shaderdata ps) : SV_TARGET
            {
                return ps.color;
            }
           
            ENDCG
        }
    }
}