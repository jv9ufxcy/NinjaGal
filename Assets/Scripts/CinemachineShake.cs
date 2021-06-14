using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineShake : MonoBehaviour
{
    public static CinemachineShake instance;
    private CinemachineVirtualCamera vCam;
    CinemachineBasicMultiChannelPerlin perlin;
    private float shakeTimer;
    private void Awake()
    {
        instance = this;
        vCam = GetComponent<CinemachineVirtualCamera>();
        perlin = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
    public void ShakeCamera(float intensity, float time)
    {
        perlin.m_AmplitudeGain = intensity;
        shakeTimer = time;
    }
    private void Update()
    {
        if (shakeTimer>0)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer<=0f)
            {
                perlin.m_AmplitudeGain = 0f;
            }
        }
    }
}
