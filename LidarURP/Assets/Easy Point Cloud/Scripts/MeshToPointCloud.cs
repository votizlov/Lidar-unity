
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace EasyPointCloud
{
    public class MeshToPointCloud : MonoBehaviour
    {
        [Header("Static Setups")]

        [SerializeField]  Mesh sourceMesh = default;

        [Tooltip("Warning: use a low-poly mesh here")]
        [SerializeField] Mesh pointsMesh = default;
        [Space]
        [SerializeField] Material sourceMaterial = default;

        [SerializeField] LayerMask pointsLayer = 1;

        [SerializeField] Texture2D sourceMeshTexture = default;

        [Header("Dynamic Setups")]
        [SerializeField, Range(0, 0.2f)] float step = 0.01f; //size
        [SerializeField] Color pointsColor = Color.white;
        [SerializeField, Range(0.0f, 1.0f)] float colorIntensity = 0.5f;
        [SerializeField, Range(0.0f, 1.0f)] float colorFromTextureLerp = 0;

        [SerializeField]  bool circular = false;

        [Space]
        [SerializeField]  GameObject meshPreview = default;

        [Space]
        [SerializeField] ForwardRendererData rendererData = default;

        ComputeBuffer positionsBuffer;
        ComputeBuffer uvsBuffer;
        ComputeBuffer normalsBuffer;

        Material material; 
                
        Bounds bounds;
        Vector3[] vertices;
        Vector2[] uvs;
        Vector3[] normals;
        private List<Vector3> points;

        int layerIndex;

        public void AddPoint(List<Vector3> points)
        {
            foreach (var vert in points)
            {
                this.points.Add(vert);
            }
        }

        private void OnEnable()
        {
            points = new List<Vector3>();
            InitializeMeshData(); 
            SetBound();
            DeactivateMeshPreview();
        }

        protected void InitializeMeshData()
        {
            MakePositionsBuffer();
            SetStaticMaterialData();
        }

        private void MakePositionsBuffer()
        {
            vertices = sourceMesh.vertices;
            // Print Vertices Count
            // Debug.Log("Vertices Count :" + vertices.Length);
            positionsBuffer = new ComputeBuffer(vertices.Length, 3 * 4);
            positionsBuffer.SetData(vertices);
        }

        private void SetStaticMaterialData()
        {
            material = sourceMaterial;
            material.SetBuffer("_Positions", positionsBuffer);
            material.SetFloat("_UseNormals", 0);
            colorFromTextureLerp = 0;
            if (sourceMeshTexture != default)
                SetUVAndTextureData();
            SetNormalsData();
            layerIndex = (int)Mathf.Log(pointsLayer.value, 2);
        }

        private void SetUVAndTextureData()
        {
            uvs = sourceMesh.uv;
            if(uvs.Length >0)
            {
                uvsBuffer = new ComputeBuffer(uvs.Length, 2 * 4);
                uvsBuffer.SetData(uvs);
                material.SetBuffer("_uvs", uvsBuffer);
            }

            material.SetTexture("_MainTex", sourceMeshTexture);
            colorFromTextureLerp = 1;
        }

        private void SetNormalsData()
        {
            normals = sourceMesh.normals;
            normalsBuffer = new ComputeBuffer(normals.Length, 3 * 4);
            normalsBuffer.SetData(normals);
            material.SetBuffer("_Normals", normalsBuffer);
            material.SetFloat("_UseNormals", 1);
        }

        private void SetBound()
        {
            bounds = new Bounds(Vector3.zero, Vector3.one * 200);
        }

        protected void Update()
        {
            SetMaterialDynamicData();
            DrawInstanceMeshes();
        }

        protected void DrawInstanceMeshes()
        {
            Graphics.DrawMeshInstancedProcedural(pointsMesh, 0, material, bounds, positionsBuffer.count,layer: layerIndex);
        }

        protected virtual void SetMaterialDynamicData()
        {
            material.SetFloat("_Step", step);
            material.SetFloat("_scale", this.transform.localScale.x);
            material.SetFloat("_intensity", colorIntensity);
            material.SetVector("_worldPos", this.transform.position);
            material.SetVector("_color", new Vector4(pointsColor.r, pointsColor.g, pointsColor.b, 1));
            material.SetFloat("_ColorFromTextureLerp", colorFromTextureLerp);
            material.SetFloat("_circular", circular ? 1 : 0);
            material.SetMatrix("_quaternion", Matrix4x4.TRS(new Vector3(0, 0, 0), transform.rotation, new Vector3(1, 1, 1)));
        }

        protected virtual void OnDisable()
        {
            positionsBuffer.Release();
            positionsBuffer = null;
            normalsBuffer.Release();
            normalsBuffer = null;
            if (sourceMeshTexture != default)
            {
                uvsBuffer?.Release();
                uvsBuffer = null;
            }
        }

        private void DeactivateMeshPreview()
        {
            try
            {
                meshPreview.SetActive(false);
            }
            catch
            {
                Debug.LogWarning("The mesh preview object has not been assigned!");
            }
        }
        // It is called from editor script(button)
        public void UpdateMeshPreview()
        {
            try
            {
                meshPreview.GetComponent<MeshFilter>().mesh = sourceMesh;
                meshPreview.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("MainTex", sourceMeshTexture);
            }
            catch 
            {
                Debug.LogWarning("The mesh preview object has not been assigned!");
            }
        }

        public void SetupRenderer()
        {
            if (rendererData)
                RendererSetup.AddForwardRendererFeature(rendererData, sourceMaterial, pointsLayer);
            else
                Debug.LogWarning("There is no Forward Renderer attached!");
        }

    }
}
