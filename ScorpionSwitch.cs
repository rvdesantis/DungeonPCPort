using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorpionSwitch : DunSwitch
{
    public CrystalLampRoomParent crystalRoom;
    public override void FlipSwitch()
    {
        if (!switchOn && !locked)
        {
            StartCoroutine(FlipTimer());
        }
    }

    IEnumerator FlipTimer()
    {
        switchOn = true;
        switchPlayable.Play();
        yield return new WaitForSeconds((float)switchPlayable.duration);
        switchOn = false;
        crystalRoom.LampRoll();
    }

}
