using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
public class TrianglesTest : MonoBehaviour
{
 
    // Vertices Buffer
    ComputeBuffer _verticesBuffer;
 
    static readonly int verticesBuffer = Shader.PropertyToID("verticesBuffer");
 
    public Vector4[] vertices;
    public Material triangleMaterial;
    private List<Vector4> points;

    void Awake()
    {
        points = new List<Vector4>();
        RenderPipelineManager.endFrameRendering += (arg1, arg2) => Render();
    }
 
    void OnRenderObject()
    {
        Render();
    }
 
 
    void OnDrawGizmos()
    {
        //Render();
    }
 
 
    void CopyFromCpuToGpu()
    {
        if (_verticesBuffer == null || _verticesBuffer.count != points.Count)
        {
            if (_verticesBuffer != null)
            {
                _verticesBuffer.Release();
                _verticesBuffer = null;
            }
 
            _verticesBuffer = new ComputeBuffer(points.Count, 4*4);
            triangleMaterial.SetBuffer(verticesBuffer, _verticesBuffer);
        }
 
        _verticesBuffer.SetData(points, 0, 0, points.Count);
    }
 
    internal void Render()
    {
        if (points.Count == 0) return;
        CopyFromCpuToGpu();
        triangleMaterial.SetPass(0);
        Graphics.DrawProceduralNow(MeshTopology.Points, points.Count);
    }

    public void AddPoint(List<Vector3> vector3s)
    {
        foreach (var v3 in vector3s)
        {
            points.Add(v3);
        }
    }
}