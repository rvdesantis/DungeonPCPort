using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DunSwitchSwapRoom : DunSwitch
{
    public bool swapOne;
    public bool swapTwo;
    public StatueRoomParent statueRoomParent;

    public override void FlipSwitch()
    {
        base.FlipSwitch();
        if (swapOne)
        {
            statueRoomParent.TriggerSwapSwitchOne();
        }
        if (swapTwo)
        {
            statueRoomParent.TriggerSwapSwitchTwo();
        }

    }
}
