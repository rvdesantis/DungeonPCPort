using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonicStatueRoomSwitch : DunSwitch
{
    public ArmorStandNPC armorStand;
    public DemonicStatueParent roomParent;

    public void DoorSwitch()
    {
        flipping = true;
        switchOn = true;
        StartCoroutine(DoorTimer());
    }

    public IEnumerator DoorTimer()
    {
        DistanceController distanceController = FindObjectOfType<DistanceController>();
        DunUIController uIController = FindObjectOfType<DunUIController>();
        uIController.ToggleKeyUI(gameObject, false);
        if (distanceController == null)
        {
            distanceController = FindObjectOfType<DistanceController>();
        }
        yield return new WaitForSeconds(.05f);
        armorStand.gameObject.SetActive(true);
        distanceController.npcS.Add(armorStand);
        roomParent.doorOpenPlayable.Play();
        yield return new WaitForSeconds((float)roomParent.doorOpenPlayable.duration);
        armorStand.NPCTrigger();
        distanceController.switches.Remove(this);
    
    }

    public override void Update()
    {
        if (inRange && !flipping && !switchOn)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0))
            {
                DoorSwitch();
            }
        }
    }
}
