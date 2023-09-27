using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrysLampSwitch : DunSwitch
{
    public override void FlipSwitch()
    {
        if (!locked)
        {
            switchPlayable.Play();
            locked = true;
        }
    }

    public override void Update()
    {
        if (inRange)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0))
            {
                FlipSwitch();
            }
        }
    }
}
