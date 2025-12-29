using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrysLampSwitch : DunSwitch
{
    public override void FlipSwitch()
    {
        if (!locked && !flipping)
        {
            DunUIController uiController = FindAnyObjectByType<DunUIController>();
            uiController.rangeImage.gameObject.SetActive(false);
            uiController.customImage.gameObject.SetActive(false);
            uiController.ToggleKeyUI(gameObject, false);
            if (!switchOn)
            {
                switchOn = true;
                flipping = true;

                switchAnim.SetTrigger("switchOn");
                StartCoroutine(FlipTimer());
                return;
            }
            if (switchOn)
            {
                switchOn = false;
                flipping = true;

                switchAnim.SetTrigger("switchOff");
                StartCoroutine(FlipTimer());
            }
        }
    }

    public override void Update()
    {
        if (inRange && !flipping)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0))
            {
                FlipSwitch();
            }
        }
    }
}
