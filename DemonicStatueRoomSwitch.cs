using Cinemachine;
using System.Collections;
using UnityEngine;

public class DemonicStatueRoomSwitch : DunSwitch
{
    public ArmorStandNPC armorStand;
    public DemonicStatueParent roomParent;
    public InventoryController inventory;
    public DunUIController uiController;
    public bool orbCheck;
    public CinemachineVirtualCamera keyCam;

    public void DoorSwitch()
    {
        flipping = true;
        switchOn = true;
        StartCoroutine(DoorTimer());
    }

    public void PlayerMessage()
    {
        keyCam.m_Priority = 20;
        string orbMSS = "There appears to be a slot to insert an orb";
        if (inventory == null)
        {
            inventory = FindObjectOfType<InventoryController>();
        }
        if (uiController == null)
        {
            uiController = FindObjectOfType<DunUIController>();
        }
        OrbChecker();
        if (!orbCheck)
        {
            uiController.messagePanelUI.gameObject.SetActive(true);
            orbMSS = orbMSS + "\n(No Chaos Orbs in Inventory)";
            uiController.messagePanelUI.text.text = orbMSS;
            IEnumerator MSSTIMER()
            {
                yield return new WaitForSeconds(4);
                keyCam.m_Priority = -5;
                uiController.messagePanelUI.gameObject.SetActive(false);
            } StartCoroutine(MSSTIMER());
        }
        if (orbCheck)
        {
            uiController.confirmUI.gameObject.SetActive(true);
            uiController.confirmUI.targetAction = DoorSwitch;
            uiController.confirmUI.noAction = NoAction;
            orbMSS = orbMSS + "\n\nSpend 1 Chaos Orb?";
            uiController.confirmUI.ConfirmMessageUI(orbMSS);

        }
    }

    public void NoAction()
    {
        keyCam.m_Priority = -5;
        flipping = true;
        IEnumerator FlipTimer()
        {
            yield return new WaitForSeconds(.5f);
            flipping = false;
        } StartCoroutine(FlipTimer());
    }

    public IEnumerator DoorTimer()
    {
        inventory.keyItems[0].itemCount--;

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
        yield return new WaitForSeconds((float)roomParent.doorOpenPlayable.duration / 2);
        keyCam.m_Priority = -5;
        yield return new WaitForSeconds((float)roomParent.doorOpenPlayable.duration / 2);
        armorStand.NPCTrigger();
        distanceController.switches.Remove(this);    
    }

    public bool OrbChecker()
    {     
        if (inventory.keyItems[0].itemCount > 0)
        {
            orbCheck = true;
        }
        return orbCheck;
    }

    public override void Update()
    {
        if (inRange && !flipping && !switchOn)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0))
            {
                PlayerMessage();
            }
        }
    }
}
