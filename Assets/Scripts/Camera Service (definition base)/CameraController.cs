using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] private float scale = 5;
    [SerializeField] private int maxScale = 10;
    [SerializeField] private int minScale = 3;

    private void Update() 
    {
        if(Input.mouseScrollDelta.y != 0)
        {
            scale = Mathf.Clamp(scale - Input.mouseScrollDelta.y, minScale, maxScale);
            
            cinemachineVirtualCamera.m_Lens.OrthographicSize = scale;
        }
        
    }
}
