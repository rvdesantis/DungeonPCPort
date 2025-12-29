using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ScorpionSwitch : DunSwitch
{
    public CrystalLampRoomParent crystalRoom;
    public override void FlipSwitch()
    {
        if (!switchOn && !locked && !flipping)
        {
            StartCoroutine(SFlipTimer());
        }
    }

    IEnumerator SFlipTimer()
    {
        DunUIController uiController = FindAnyObjectByType<DunUIController>();
        uiController.rangeImage.gameObject.SetActive(false);
        uiController.customImage.gameObject.SetActive(false);
        flipping = true;
        switchOn = true;
        if (uiController.interactUI.activeObj == gameObject)
        {
            uiController.ToggleKeyUI(gameObject, false);
        }
   
        
        switchPlayable.Play();
        yield return new WaitForSeconds((float)switchPlayable.duration);
        switchOn = false;
        flipping = false;
        crystalRoom.LampRoll();
    }

}
