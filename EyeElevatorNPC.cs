using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class EyeElevatorNPC : DunNPC
{
    public EyeHiddenEndCube eyeHiddenEndCube;
    public PlayableDirector eyeIdle;
    public bool upPosition;


    public override void NPCTrigger()
    {
        if (!upPosition)
        {
            eyeIdle.Stop();
            eyeHiddenEndCube.ElevatorUp();
        }
        if (upPosition)
        {
            eyeHiddenEndCube.ElevaatorDown();
        }
    }

}
