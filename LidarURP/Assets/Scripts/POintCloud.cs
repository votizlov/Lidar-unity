using UnityEngine;
 
public class POintCloud : MonoBehaviour
{
    public Material material;
    protected int number = 512 * 512;
    protected ComputeBuffer depthbuffer;
    protected ComputeBuffer colorbuffer;
 
    protected float[] depth;
    protected Vector4[] color;
 
    static readonly float[,] m = new float[,] { {0.8f,0.01f}, {0.01f,0.8f}};  
   
    float hash (Vector2 p )   //generates pseudorandom number from (0..1) range
    {
        return Mathf.Abs( (Mathf.Sin( p.x*12.9898f+p.y*78.233f )  * 43758.5453f) % 1);
    }
   
    float lerp (float a,float b, float t)
    {
        return Mathf.Lerp(a,b,t);
    }
 
    float noise(Vector2 p)   //makes random tiles with bilinear interpolation to create smooth surface
    {
        Vector2 i = new Vector2(Mathf.Floor(p.x),Mathf.Floor(p.y));
        Vector2 u = new Vector2 (Mathf.Abs (p.x % 1),Mathf.Abs (p.y % 1));
        u = new Vector2 (u.x*u.x*(3.0f-2.0f*u.x),u.y*u.y*(3.0f-2.0f*u.y));
        Vector2 a = new Vector2 (0.0f,0.0f);
        Vector2 b = new Vector2 (1.0f,0.0f);
        Vector2 c = new Vector2 (0.0f,1.0f);
        Vector2 d = new Vector2 (1.0f,1.0f);
        float r = lerp(lerp(hash(i+a),hash(i+b),u.x),lerp(hash(i+c),hash(i+d),u.x),u.y);
        return r*r;
    }
           
    float fbm( Vector2 p )   //deforms tiles to get more organic looking surface
    {
        float f = 0.0f;
        f += 0.5000f*noise( p ); p = p*2.02f;  p = new Vector2(p.x*m[0,0]+p.y*m[0,1],p.x*m[1,0]+p.y*m[1,1]);
        f += 0.2500f*noise( p ); p = p*2.03f;  p = new Vector2(p.x*m[0,0]+p.y*m[0,1],p.x*m[1,0]+p.y*m[1,1]);
        f += 0.1250f*noise( p ); p = p*2.01f;  p = new Vector2(p.x*m[0,0]+p.y*m[0,1],p.x*m[1,0]+p.y*m[1,1]);
        f += 0.0625f*noise( p );
        return f/0.9375f;
    }
 
    void Start ()
    {
        float r = Random.Range(0.0f,100.0f);
        depthbuffer = new ComputeBuffer(number, sizeof(float), ComputeBufferType.Default);
        colorbuffer = new ComputeBuffer(number, sizeof(float) * 4, ComputeBufferType.Default);
        depth = new float[number];
        color = new Vector4[number];
        int i = 0;
        for (int y = 0; y < 512; y++)
        {
            for (int x = 0; x < 512; x++)
            {
                Vector2 resolution = new Vector2 (512,512);
                Vector2 coordinates = new Vector2 ((float)x,(float)y);
                Vector2 uv = new Vector2( (2.0f*coordinates.x-resolution.x)/resolution.y+1.0f,(2.0f*coordinates.y-resolution.y)/resolution.y +1.0f );
                ushort h = System.Convert.ToUInt16((fbm(new Vector2(uv.x*5.0f+r,uv.y*5.0f+r))+0.1f) * ushort.MaxValue);
                depth[i] = (float) ((float)h / (float)ushort.MaxValue);
                if (depth[i]<0.1)
                    color[i] = new Vector4(0.77f,0.90f,0.98f,1.0f);
                else
                if (depth[i]<0.2)
                    color[i] = new Vector4(0.82f,0.92f,0.99f,1.0f);
                else
                if (depth[i]<0.3)
                    color[i] = new Vector4(0.91f,0.97f,0.99f,1.0f);
                else
                if (depth[i]<0.45)
                    color[i] = new Vector4(0.62f,0.75f,0.59f,1.0f);
                else
                if (depth[i]<0.55)
                    color[i] = new Vector4(0.86f,0.90f,0.68f,1.0f);
                else
                if (depth[i]<0.65)
                    color[i] = new Vector4(0.99f,0.99f,0.63f,1.0f);
                else
                if (depth[i]<0.75)
                    color[i] = new Vector4(0.99f,0.83f,0.59f,1.0f);
                else
                if (depth[i]<0.90)
                    color[i] = new Vector4(0.98f,0.71f,0.49f,1.0f);    
                else
                if (depth[i]<0.95)
                    color[i] = new Vector4(0.98f,0.57f,0.47f,1.0f);      
                else    
                    color[i] = new Vector4(0.79f,0.48f,0.43f,1.0f);
                i++;
            }
        }
        depthbuffer.SetData(depth);
        colorbuffer.SetData(color);
    }
   
    void OnPostRender()
    {
        Debug.Log("asdasdas");
        material.SetPass(0);
        material.SetBuffer("depthbuffer", depthbuffer);
        material.SetBuffer("colorbuffer", colorbuffer);
        Graphics.DrawProceduralNow(MeshTopology.Points, number, 1);
    }
    void OnDestroy()
    {
        depthbuffer.Release();
        colorbuffer.Release();
    }
}