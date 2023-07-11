using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HornRoomParent : RoomPropParent
{
    public List<Light> lights;
    public float duration = 7f; // Total duration of the light animation (in seconds)
    public float startIntensity = 1f; // Initial intensity value
    public float endIntensity = 5f; // Target intensity value


    private float timer = 0f; // Timer variable
    public bool increasing = true; // Flag to determine if the intensity is increasing or decreasing
    public bool loop;
    // Start is called before the first frame update






    public void StartIntensityAnimation()
    {
        // Reset timer and flag
        timer = 0f;
        increasing = true;
       StartCoroutine(AnimateIntensity());
    }

    private IEnumerator AnimateIntensity()
    {
        while (timer <= duration)
        {
            float intensity;
            intensity = Mathf.Lerp(1, 5, timer / duration);
            foreach (Light light in lights)
            {
                light.intensity = intensity;
            }
            timer += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(AnimateLower());
    }


    private IEnumerator AnimateLower()
    {
        timer = 0f;
        while (timer <= duration)
        {
            float intensity;
            intensity = Mathf.Lerp(5, 1, timer / duration);
            foreach (Light light in lights)
            {
                light.intensity = intensity;
            }
            timer += Time.deltaTime;
            yield return null;
        }
        loop = false;
    }

    public void SetPortal()
    {
        int x = 0;
        foreach (GameObject wall in roomParent.wallCovers)
        {
            if (wall.activeSelf)
            {
                x = roomParent.wallCovers.IndexOf(wall);
            }
        }
        portA = portalAList[x].GetComponent<DunPortal>();
        portB = bosshallPortal.GetComponent<DunPortal>();
        if (distanceController == null)
        {
            distanceController = FindAnyObjectByType<DistanceController>();
        }
        distanceController.portals.Add(portA);

        portA.connectedPortal = portB;
    }


    private void Update()
    {
        // Check if the animation coroutine has finished
        if (loop == false)
        {
            loop = true;
            StartIntensityAnimation();
        }
    }
}
