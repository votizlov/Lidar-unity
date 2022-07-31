using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyPointCloud
{
    public class PreviewRotate : MonoBehaviour
    {
        [SerializeField] float _speed = 30;
        void Update()
        {
            transform.Rotate(Vector3.up * Time.deltaTime * _speed, Space.World);
        }
    }
}
