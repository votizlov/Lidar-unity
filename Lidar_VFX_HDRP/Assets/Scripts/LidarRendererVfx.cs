using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class LidarRendererVfx : MonoBehaviour
{
    [SerializeField] private Texture2D texColor;
    [SerializeField] private Texture2D texPosScale;
    [SerializeField] private VisualEffect vfx;
    [SerializeField] uint resolution = 4096;
    [SerializeField] private Transform tr;

    public float ParticleSize = 1f;
    bool toUpdate = false;
    [SerializeField] uint particleCount = 0;
    private List<Vector3> pointCoords;

    private void Start()
    {
        pointCoords = new List<Vector3>();
        InitTextures();
    }
 
    private void Update() {
        if (toUpdate) {
            toUpdate = false;
 
            vfx.Reinit();
            vfx.SetUInt(Shader.PropertyToID("count"), particleCount);
            vfx.SetTexture(Shader.PropertyToID("texColor"), texColor);
            vfx.SetTexture(Shader.PropertyToID("texPosScale"), texPosScale);
            vfx.SetUInt(Shader.PropertyToID("resolution"), resolution);
        
            
        
            
            //texColor = new Texture2D(positions.Length > (int)resolution ? (int)resolution : positions.Length, Mathf.Clamp(positions.Length / (int)resolution, 1, (int)resolution), TextureFormat.RGBAFloat, false);
            //texPosScale = new Texture2D(positions.Length > (int)resolution ? (int)resolution : positions.Length, Mathf.Clamp(positions.Length / (int)resolution, 1, (int)resolution), TextureFormat.RGBAFloat, false);
        }
    }
    
    void InitTextures(){
        texColor = new Texture2D((int)resolution, (int)resolution, TextureFormat.RGBAFloat, false);
        texPosScale = new Texture2D((int)resolution , (int)resolution, TextureFormat.RGBAFloat, false);}
    

    public void SetParticles(Vector3[] positions, Color color)
    {
        SetParticles(positions,new []{color});
    }
 
    public void SetParticles(Vector3[] positions, Color[] colors) {
        if(positions.Length<=16) return;
        //texColor = new Texture2D(positions.Length > (int)resolution ? (int)resolution : positions.Length, Mathf.Clamp(positions.Length / (int)resolution, 1, (int)resolution), TextureFormat.RGBAFloat, false);
        //texPosScale = new Texture2D(positions.Length > (int)resolution ? (int)resolution : positions.Length, Mathf.Clamp(positions.Length / (int)resolution, 1, (int)resolution), TextureFormat.RGBAFloat, false);
        int texWidth = texColor.width;
        int texHeight = texColor.height;
 
        for (int y = 0; y < texHeight; y++) {
            for (int x = 0; x < texWidth; x++) {
                int index = x + y * texWidth;
                if (index > positions.Length+1)
                {
                    break;
                }
                texColor.SetPixel(x, y, Color.red);
                try
                {
                    var data = new Color(positions[index].x, positions[index].y, positions[index].z, ParticleSize);
                    texPosScale.SetPixel(x, y, data);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    texPosScale.SetPixel(x, y,Color.black);
                }
                
            }
        }
 
        texColor.Apply();
        texPosScale.Apply();
        particleCount = (uint)positions.Length;
        toUpdate = true;
    }

    public void AddPoint(Vector3 point)
    {
        pointCoords.Add(point);
        SetParticles(pointCoords.ToArray(),Color.red);
        toUpdate = true;
    }

    public void AddPoint(List<Vector3> points)
    {
        foreach (var v3 in points)
        {
            pointCoords.Add(v3);
        }
        SetParticles(pointCoords.ToArray(),Color.red);
        toUpdate = true; 
    }

    public void AddPoint(RaycastHit[] point)
    {
        foreach (var v3 in point)
        {
            pointCoords.Add(v3.point);
        }
        SetParticles(pointCoords.ToArray(),Color.red);
        toUpdate = true;
    }
}
