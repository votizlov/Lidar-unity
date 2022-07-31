using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LidarRenderer : MonoBehaviour
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
        [SerializeField] private GameObject debugPrefab;

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
                Instantiate(debugPrefab, vert, Quaternion.identity);
            }
            sourceMesh.Clear();
            sourceMesh.vertices = this.points.ToArray();
            UpdatePositionsBuffer();
        }

        private void OnEnable()
        {
            points = new List<Vector3>();
            InitializeMeshData(); 
            SetBound();
        }

        protected void InitializeMeshData()
        {
            sourceMesh = new Mesh();
            UpdatePositionsBuffer();
            SetStaticMaterialData();
        }

        private void UpdatePositionsBuffer()
        {
            vertices = sourceMesh.vertices;
            // Print Vertices Count
            // Debug.Log("Vertices Count :" + vertices.Length);
            if (vertices.Length == 0) return;
            positionsBuffer = new ComputeBuffer(vertices.Length, 3 * 4);
            positionsBuffer.SetData(vertices);
        }

        private void SetStaticMaterialData()
        {
            material = sourceMaterial;
            material.SetBuffer("_Positions", positionsBuffer);
            material.SetFloat("_UseNormals", 0);
            colorFromTextureLerp = 0;
            /*if (sourceMeshTexture != default)
                SetUVAndTextureData();*/
            //SetNormalsData();
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
            if (sourceMesh.normals.Length == 0) return;
            normals = sourceMesh.normals;
            normalsBuffer = new ComputeBuffer(normals.Length, 3 * 4);
            normalsBuffer.SetData(normals);
            material.SetBuffer("_Normals", normalsBuffer);
            material.SetFloat("_UseNormals", 1);
        }

        private void SetBound()
        {
            bounds = new Bounds(Vector3.zero, Vector3.one * 1000);
        }

        protected void Update()
        {/*
            if (positionsBuffer == null) return;
            SetMaterialDynamicData();
            DrawInstanceMeshes();*/
        }

        protected void DrawInstanceMeshes()
        {
            Debug.Log(positionsBuffer.count);
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
}
