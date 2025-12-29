using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class NecromancerNPC : DunNPC
{
    public int price;

    public PlayableDirector resurrectPlayable;
    private void Start()
    {
        DistanceController distance = FindAnyObjectByType<DistanceController>();
        distance.npcS.Add(this);
        Debug.Log("Necromancer Spawned", gameObject);
        idlePlayableLoop.Play();
    }

    public override void OpenUI()
    {
        DunUIController uiController = FindAnyObjectByType<DunUIController>();
        PlayerController player = FindAnyObjectByType<PlayerController>();
        PartyController party = FindAnyObjectByType<PartyController>();

        if (!uiController.isToggling && !uiController.uiActive && uiController.controller.gameState == SceneController.GameState.Dungeon)
        {
            opened = true;
            ConfirmUI confirmUI = uiController.confirmUI;

            if (confirmUI != null)
            {
                int deadParty = 0;
                if (party.combatHealthTracker[0] == 0)
                {
                    deadParty++;
                }
                if (party.combatHealthTracker[1] == 0)
                {
                    deadParty++;
                }
                if (party.combatHealthTracker[2] == 0)
                {
                    deadParty++;                }

                if (deadParty == 0)
                {
                    uiController.messagePanelUI.OpenMessage("'You are all alive...how disappointing'\nThe Necromancer seems uninterested in your party");
                    uiController.messagePanelUI.CloseMessageTimer(4);
                }
                if (deadParty > 0)
                {
                    player.controller.enabled = false;
                    if (faceCam != null)
                    {
                        faceCam.gameObject.SetActive(true);
                        faceCam.m_Priority = 20;                        
                    }
                    string mss = "Raise Fallen Party Members?\n" + (price * deadParty) + " Gold";
                    confirmUI.ConfirmMessageNPC(mss, null, null, null, null, this);
                }

               
            }
            if (confirmUI == null)
            {
                Debug.Log("ERROR: Did not capture ConfirmUI from uiController", gameObject);
            }
        }
    }

    public override void NPCTrigger()
    {
        base.NPCTrigger();
        OpenUI();
    }

    public void Resurrect()
    {

    }

    public void AfterNecro()
    {

    }
}   
