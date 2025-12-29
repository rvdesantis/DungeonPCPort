using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.WebCam;

public class DemonPortal : MonoBehaviour
{
    public bool isOpen;
    public bool inRange;
    public bool uiToggle;
    public PlayerController player;
    public InventoryController inventory;
    public DunUIController uiController;
    public bool orbCheck;

    private void Start()
    {
        if (inventory == null)
        {
            inventory = FindAnyObjectByType<InventoryController>();
        }
        if (uiController == null)
        {
            uiController = FindAnyObjectByType<DunUIController>();
        }
        if (player == null)
        {
            player = FindAnyObjectByType<PlayerController>();
        }
    }
    public bool OrbChecker()
    {
        if (inventory.keyItems[0].itemCount > 0)
        {
            orbCheck = true;
        }
        return orbCheck;
    }

    public void PlayerMessage()
    {
        string orbMSS = "There appears to be a slot to insert an orb";
        if (inventory == null)
        {
            inventory = FindAnyObjectByType<InventoryController>();
        }
        if (uiController == null)
        {
            uiController = FindAnyObjectByType<DunUIController>();
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
                uiController.messagePanelUI.gameObject.SetActive(false);
            }
            StartCoroutine(MSSTIMER());
        }
        if (orbCheck)
        {
            uiController.confirmUI.gameObject.SetActive(true);
            uiController.confirmUI.targetAction = DemonOpenSwitch;
            uiController.confirmUI.noAction = NoAction;
            orbMSS = orbMSS + "\n\nSpend 1 Chaos Orb?";
            uiController.confirmUI.ConfirmMessageUI(orbMSS);

        }
    }

    public void DemonOpenSwitch()
    {

    }

    public void NoAction()
    {
        IEnumerator FlipTimer()
        {
            yield return new WaitForSeconds(.5f);
        }
        StartCoroutine(FlipTimer());
    }

    private void Update()
    {
        if (isOpen && !inRange)
        {
            if (Vector3.Distance(player.transform.position, transform.position) < 5f)
            {
                inRange = true;
            }
            else
            {
                inRange = false;
                uiToggle = false;
            }
        }
        if (inRange && isOpen)
        {
            if (!uiToggle)
            {
                uiToggle = true;
                Debug.Log("Devil's Portal UI Toggle activation");
                // add Icon or message

            }
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.JoystickButton0))
            {
                Debug.Log("Devil's Portal Activation");
                // activate Devils Portal 
                // current Idea: 3 devils, have a neutral Devil come out and offer a deal to the player, depending on which of 3 choices, they fight one of the 3 devils, and each devil drops a unique Artifact
            }
        }
    }


}
