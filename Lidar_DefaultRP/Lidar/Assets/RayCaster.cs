using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCaster : MonoBehaviour
{
    [SerializeField] private Transform transform;
    [SerializeField] private TrianglesTest LidarRendererVfx;
    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private float radius = 0.1f;
    [SerializeField] private int resolutionX;
    [SerializeField] private int resolutionY;
    [SerializeField] private float angleX;
    [SerializeField] private float angleY;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            List<Vector3> points = Scan(angleX, angleY, resolutionX, resolutionY);
            LidarRendererVfx.AddPoint(points);
        } else if (Input.GetMouseButton(1))
        {
            List<Vector3> points = Scan(360,360, 16, 16,true);
            LidarRendererVfx.AddPoint(points);
        }
        



//        Debug.Log(points.Count);
    }

    private List<Vector3> Scan(float angleX, float angleY,int resolutionX, int resolutionY,bool isRandomise = false)
    {
        List<Vector3> points = new List<Vector3>();

        RaycastHit hit;
        //Physics.SphereCast(raycastOrigin.position, radius, raycastOrigin.forward, out hit);
        float xDeltaAngle = angleX / resolutionX;
        float yDeltaAngle = angleY / resolutionY;
        Vector3 tempDirection = Quaternion.AngleAxis(-angleX/2, raycastOrigin.up) * raycastOrigin.forward;
        tempDirection = Quaternion.AngleAxis(-angleY/2, raycastOrigin.right) * tempDirection;
        
        for (int i = 0; i < resolutionX; i++)
        {
            for (int j = 0; j < resolutionY;j++)
            {
                if (Physics.Raycast(raycastOrigin.position, tempDirection, out hit, 100.0f))
                {
                    points.Add(hit.point);
                }
                tempDirection = Quaternion.AngleAxis( isRandomise?yDeltaAngle * Random.Range(0,1):yDeltaAngle, raycastOrigin.right) * tempDirection;
            }
            tempDirection = Quaternion.AngleAxis(-yDeltaAngle*resolutionY, raycastOrigin.right) * tempDirection;
            tempDirection = Quaternion.AngleAxis(isRandomise?xDeltaAngle*Random.Range(0,1):xDeltaAngle, raycastOrigin.up) * tempDirection;
        }
        
        return points;
    }
}
