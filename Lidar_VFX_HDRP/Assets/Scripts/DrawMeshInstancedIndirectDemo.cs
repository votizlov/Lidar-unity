using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DrawMeshInstancedIndirectDemo : MonoBehaviour {

    public Material material;
    public Mesh mesh;

    private ComputeBuffer meshPropertiesBuffer;
    private ComputeBuffer argsBuffer;
    private Bounds bounds;
    private List<Vector3> pointCoords = new List<Vector3>();
    
    [SerializeField] private Transform tr;
    [SerializeField] private GameObject prefab;


    // Mesh Properties struct to be read from the GPU.
    // Size() is a convenience funciton which returns the stride of the struct.
    private struct MeshProperties {
        public Matrix4x4 mat;
        public Vector4 color;

        public static int Size() {
            return
                sizeof(float) * 4 * 4 + // matrix;
                sizeof(float) * 4;      // color;
        }
    }

    private void Setup() {

        // Boundary surrounding the meshes we will be drawing.  Used for occlusion.
        bounds = new Bounds(transform.position, Vector3.one * (10));

        UpdateBuffers();
    }

    private void UpdateBuffers()
    {
        // Argument buffer used by DrawMeshInstancedIndirect.
        uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
        // Arguments for drawing mesh.
        // 0 == number of triangle indices, 1 == population, others are only relevant if drawing submeshes.
        args[0] = (uint)mesh.GetIndexCount(0);
        args[1] = (uint)pointCoords.Count;
        args[2] = (uint)mesh.GetIndexStart(0);
        args[3] = (uint)mesh.GetBaseVertex(0);
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        argsBuffer.SetData(args);

        // Initialize buffer with the given population.
        MeshProperties[] properties = new MeshProperties[pointCoords.Count];
        for (int i = 0; i < pointCoords.Count; i++) {
            MeshProperties props = new MeshProperties();
            Vector3 position = pointCoords[i];
            Quaternion rotation = Quaternion.identity;
            Vector3 scale = Vector3.one;

            props.mat = Matrix4x4.TRS(position, rotation, scale);
            props.color = Color.Lerp(Color.red, Color.blue, Random.value);

            properties[i] = props;
        }

        meshPropertiesBuffer = new ComputeBuffer(pointCoords.Count, MeshProperties.Size());
        meshPropertiesBuffer.SetData(properties);
        material.SetBuffer("_Properties", meshPropertiesBuffer);
    }

    private void Start() {
        Setup();
    }

    private void Update() {
        RaycastHit hit;
        if (Physics.Raycast(tr.position, tr.forward, out hit, 100.0f))
        {
            Debug.Log(hit.collider.name);
            pointCoords.Add(hit.point);
            Debug.DrawRay(tr.position, tr.forward,Color.cyan);
            Instantiate(prefab, hit.point, Quaternion.identity);
        }
        //UpdateBuffers();
        Graphics.DrawMeshInstancedIndirect(mesh, 0, material, bounds, argsBuffer);
    }

    private void OnDisable() {
        // Release gracefully.
        if (meshPropertiesBuffer != null) {
            meshPropertiesBuffer.Release();
        }
        meshPropertiesBuffer = null;

        if (argsBuffer != null) {
            argsBuffer.Release();
        }
        argsBuffer = null;
    }
}
