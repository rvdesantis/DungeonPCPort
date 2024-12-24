using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkullNPC : DunNPC
{
    public PlayerController player;
    
    public Transform eyeL;
    public Transform eyeR;
    public CubeRoom roomParent;
    public bool messageToggle;

    public TemplarNPC activeTemplar;
    public Transform activeTarget;

    public string emptyMessage;
    public string templarMessage;

    public bool warning;
    public bool eyeTracking;

    IEnumerator MessageTimer()
    {
        DunUIController uiController = FindObjectOfType<DunUIController>();

        messageToggle = true;
        string whisper = "You hear a voice in your head:\n...On The Table...";
        uiController.messagePanelUI.gameObject.SetActive(true);
        uiController.messagePanelUI.OpenMessage(whisper);
        yield return new WaitForSeconds(3);
        if (uiController.messagePanelUI.currentString == whisper)
        {
            uiController.messagePanelUI.gameObject.SetActive(false);
        }
    }

    IEnumerator WarningTimer()
    {
        warning = true;
        DunUIController uiController = FindObjectOfType<DunUIController>();
        DistanceController distance = FindObjectOfType<DistanceController>();
        player.enabled = false;
        if (activeTemplar != null)
        {
            faceCam.gameObject.SetActive(true);
            faceCam.m_Priority = 20;
            string newWhisper = "You hear a voice in your head:\n...He is here...\n...KILL KIM...Set Me Free...";
            activeTemplar.skullBounty = true;
            uiController.messagePanelUI.gameObject.SetActive(true);
            uiController.messagePanelUI.OpenMessage(newWhisper);
            yield return new WaitForSeconds(3);
            if (uiController.messagePanelUI.currentString == newWhisper)
            {
                uiController.messagePanelUI.gameObject.SetActive(false);
            }
            faceCam.gameObject.SetActive(false);
            faceCam.m_Priority = -5;
      
            distance.npcS.Remove(this);
            uiController.interactUI.gameObject.SetActive(false);
            uiController.interactUI.activeObj = null;
            uiController.rangeImage.gameObject.SetActive(false);
            uiController.customImage.gameObject.SetActive(false);
        }
        if (activeTemplar == null)
        {
            faceCam.gameObject.SetActive(true);
            faceCam.m_Priority = 20;
            string newWhisper = "You hear a voice in your head:\n...Find...The...Templar...";
            uiController.messagePanelUI.gameObject.SetActive(true);
            uiController.messagePanelUI.OpenMessage(newWhisper);
            yield return new WaitForSeconds(3);
            if (uiController.messagePanelUI.currentString == newWhisper)
            {
                uiController.messagePanelUI.gameObject.SetActive(false);
            }
            faceCam.gameObject.SetActive(false);
            faceCam.m_Priority = -5;
            
            distance.npcS.Remove(this);
            uiController.interactUI.gameObject.SetActive(false);
            uiController.interactUI.activeObj = null;
            uiController.rangeImage.gameObject.SetActive(false);
            uiController.customImage.gameObject.SetActive(false);
        }
        player.enabled = true;
    }

    public override void NPCTrigger()
    {
        if (!warning)
        {
            StartCoroutine(WarningTimer());
        }

    }

    public void CreepyLook(Vector3 playerPosition)
    {
        Vector3 LookPosition = new Vector3(playerPosition.x, transform.position.y, playerPosition.z);
        eyeL.LookAt(LookPosition);
        eyeR.LookAt(LookPosition);
    }

    private void Update()
    {
        if (eyeTracking)
        {
            if (activeTarget != null)
            {
                Vector3 playerPosition = activeTarget.position;
                if (roomParent.inRoom)
                {
                    CreepyLook(playerPosition);
                    if (!messageToggle)
                    {
                        messageToggle = true;
                        StartCoroutine(MessageTimer());
                    }
                }
            }
        }
    }
}
