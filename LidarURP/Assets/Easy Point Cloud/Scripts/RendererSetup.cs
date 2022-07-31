using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;
using static UnityEngine.Experimental.Rendering.Universal.RenderObjects;
namespace EasyPointCloud
{
    public class RendererSetup : MonoBehaviour
    {

        public static void AddForwardRendererFeature(ForwardRendererData rendererData, Material material, LayerMask layer)
        {
#if UNITY_EDITOR
            string featureName = "PointCloudRenderer"+ String.Concat(Array.FindAll(material.name.ToCharArray(), Char.IsLetterOrDigit));

            var feature = rendererData.rendererFeatures.Where((f) => f.name == featureName).FirstOrDefault();

            if(feature != null)
                rendererData.rendererFeatures.Remove(feature);
        
            var newFeature = ScriptableObject.CreateInstance<RenderObjects>();

            AssetDatabase.CreateAsset(newFeature, $"Assets/Settings/" + featureName+ ".asset");
            AssetDatabase.SaveAssets();
            
            newFeature.settings.Event = RenderPassEvent.AfterRenderingTransparents;
            newFeature.settings.filterSettings.RenderQueueType = RenderQueueType.Transparent;
            newFeature.settings.filterSettings.LayerMask = layer;
            newFeature.settings.overrideMaterial = material;
            newFeature.settings.overrideDepthState = true;

            rendererData.rendererFeatures.Add(newFeature);
            rendererData.SetDirty();

            Debug.Log("The renderer has been set up successfully.");
#endif
        }
    }
}