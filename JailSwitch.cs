using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JailSwitch : DunSwitch
{
    public JailRoomParent jailParent;
    public bool hidden;
    public FakeWall stallWall;


    public override void FlipSwitch()
    {
        if (!locked && !flipping)
        {
            DunUIController uiController = FindObjectOfType<DunUIController>();
            uiController.rangeImage.gameObject.SetActive(false);
            uiController.customImage.gameObject.SetActive(false);
            uiController.ToggleKeyUI(gameObject, false);

            if (!switchOn)
            {
                DistanceController distanceController = FindObjectOfType<DistanceController>();
                if (animType == AnimType.animator)
                {                  
                    if (!hidden)
                    {
                        switchOn = true;
                        flipping = true;
                        switchAnim.SetTrigger("switchOn");
                        if (audioSource != null && switchSounds.Count > 0)
                        {
                            audioSource.PlayOneShot(switchSounds[0]);
                        }
                        StartCoroutine(FlipTimer());                       
                        Debug.Log("flipping left gate");
                        jailParent.lGate = true;                       
                        distanceController.switches.Remove(this);
                        distanceController.switches.Add(jailParent.hiddenSwitch);
                        jailParent.leftGate.Play();
                    }
                    if (hidden)
                    {
                        Debug.Log("flipping right gate");
                        jailParent.rGate = true;
                        flipping = true;                      
                        distanceController.switches.Remove(jailParent.hiddenSwitch);
                        distanceController.chests.Add(jailParent.cellChest);
                        jailParent.rightGate.Play();
                        jailParent.JailerReturn();
                        switchOn = true;
                        switchAnim.SetTrigger("switchOn");
                        if (audioSource != null && switchSounds.Count > 0)
                        {
                            audioSource.PlayOneShot(switchSounds[0]);
                        }
                        StartCoroutine(FlipTimer());                     
                        distanceController.switches.Remove(this);

                    }

                }
            }            
        }
    }    
}
