using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BattleCamController : MonoBehaviour
{
    public CinemachineVirtualCamera activeCam;
    public CinemachineBrain camBrain;
    public List<CinemachineVirtualCamera> roomCams;
    public CinemachineBasicMultiChannelPerlin camNoise;
    public float shakeDuration = 0f;
    public float shakeAmplitude = 2f; // Higher values for stronger shake
    public float shakeFrequency = 2f; // Higher values for faster shake
    public bool camShake;
    public void AssignCamNoise()
    {        
        if (activeCam != null)
        {
            camNoise = activeCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
        if (camNoise == null)
        {
            Debug.Log("Problem capturing camNoise from active vCam", activeCam.gameObject);
        }
    }

    public void TriggerShake(float duration, float amplitude, float frequency)
    {
        AssignCamNoise();
        shakeDuration = duration;
        shakeAmplitude = amplitude;
        shakeFrequency = frequency;
        camShake = true;
    }

    void Update()
    {
        if (camShake)
        {
            if (activeCam != null && camNoise != null)
            {
                if (shakeDuration > 0)
                {
                    camNoise.m_AmplitudeGain = shakeAmplitude;
                    camNoise.m_FrequencyGain = shakeFrequency;

                    shakeDuration -= Time.deltaTime;
                }
                else
                {
                    camShake = false;
                    camNoise.m_AmplitudeGain = 0f;
                    shakeDuration = 0f;
                }
            }
        }

        if (activeCam != null && roomCams.Count > 0)
        {
            foreach (CinemachineVirtualCamera vCam in roomCams)
            {
                if (vCam.m_Priority >= 10)
                {
                    activeCam = vCam;
                }
            }
        }
    }
}
