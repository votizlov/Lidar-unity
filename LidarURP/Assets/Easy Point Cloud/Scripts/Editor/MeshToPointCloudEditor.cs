
using UnityEngine;
using UnityEditor;

namespace EasyPointCloud
{
    [CustomEditor(typeof(MeshToPointCloud))]
    public class MeshToPointCloudEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            MeshToPointCloud mesh2point = (MeshToPointCloud)target;

            GUILayout.Space(10);

            if (GUILayout.Button("Setup Renderer"))
            {
                mesh2point.SetupRenderer();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Update Mesh Preview"))
            {
                mesh2point.UpdateMeshPreview();
            }
        }
    }
}
