using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraManager : MonoBehaviour
{
    CinemachineVirtualCamera followCamera;
    void Start()
    {
        followCamera = GetComponent<CinemachineVirtualCamera>();
        followCamera.Follow = FindObjectOfType<PlayerController>()?.transform;   
    }
}
