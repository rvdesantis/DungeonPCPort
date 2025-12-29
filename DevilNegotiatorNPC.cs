using UnityEngine;

public class DevilNegotiatorNPC : DunNPC
{
    public override void NPCTrigger()
    {
        if (inRange && !opened)
        {
            opened = true;           
        }
    } // triggered from DistanceController

    public override void NPCIdleLoopTrigger()
    {

    }

    public override void OpenUI()
    {
        if (uiObject != null)
        {

        }
    }

    public override void Confirm()
    {

    }

}
