using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TemplarDunModels : DunNPC
{
    public bool vMagic;
    public DunUIController uiController;
    public BattleController bController;
    public PartyController party;
    public DunModel vMage;
    public LibraryRoomParent attachedLibrary;
    public bool attackRoom;
    public bool healthRoom;
    public List<DunModel> templars;
    public DunModel selfModel;

    private void Start()
    {
        if (party == null)
        {
            party = FindObjectOfType<PartyController>();
        }
        foreach (DunModel hero in party.activeParty)
        {
            if (hero.modelName == vMage.modelName)
            {
                vMagic = true;
            }
        }
        if (attachedLibrary.roomParent.roomType == CubeRoom.RoomType.health)
        {
            healthRoom = true;
        }
        if (attachedLibrary.roomParent.roomType == CubeRoom.RoomType.battle)
        {
            attackRoom = true;
        }
    }

    public override void NPCTrigger()
    {
        if (!opened)
        {
            if (inRange && !opened)
            {
                opened = true;
            }
            if (party == null)
            {
                party = FindObjectOfType<PartyController>();
            }
            MonsterController monster = FindObjectOfType<MonsterController>();
            if (!vMagic)
            {
                Debug.Log("vMage not in party, attack not triggered.  Opening UI");
                OpenUI();
            }
            if (vMagic)
            {
                Debug.Log("vMage  in party, Assigning Playable & Combat");
                attachedLibrary.templarGangIdle.Stop();    
      
                int skipCount = 0;
                foreach (DunModel hero in party.activeParty)
                {
                    hero.gameObject.SetActive(true);
                    hero.transform.parent = attachedLibrary.vMageBattleStart.transform;
                    hero.transform.position = attachedLibrary.vMageBattleStart.transform.position;
                    if (hero.modelName == party.masterParty[5].modelName)                    {
        
                        hero.AssignSpecificDirector(attachedLibrary.vMageBattleStart, 0); // assigns vMage to position 0
                    }
                    if (hero.modelName != party.masterParty[5].modelName)
                    {
                        skipCount++;
                        if (skipCount == 1)
                        {
                            hero.AssignSpecificDirector(attachedLibrary.vMageBattleStart, 1);
                        }
                        if (skipCount == 2)
                        {
                            hero.AssignSpecificDirector(attachedLibrary.vMageBattleStart, 2);
                        }                        
                    }    
                }
                if (party == null)
                {
                    party = FindObjectOfType<PartyController>();
                }
                party.AssignCamBrain(attachedLibrary.vMageBattleStart, 3);          
                PlayerController pController = FindObjectOfType<PlayerController>();
                pController.controller.enabled = false;                
                StartCoroutine(vMageLaunchTimer());
            }
        }

    }

    public override void OpenUI()
    {        
        if (uiController == null)
        {
            uiController = FindObjectOfType<DunUIController>();
        }
        if (uiController != null)
        {
            uiController.confirmUI.gameObject.SetActive(true);
            
            if (!vMagic)
            {
                string message = "Your Party is free of Void Magic Users.  You are free to leave";
                message = message + "\n\n Attack Templars? ";

                uiController.confirmUI.message.text = message;
                uiController.confirmUI.targetAction = SelectAttack;
                uiController.confirmUI.noBT.Select();
                uiController.controller.playerController.controller.enabled = false;
            }
        }
    }

    public void SelectAttack()
    {
        StartCoroutine(SelectAttackTimer());
    }

    IEnumerator SelectAttackTimer()
    {
        yield return new WaitForSeconds(.5f);
        if (bController == null)
        {
            bController = FindObjectOfType<BattleController>();
        }
        bController.afterBattleAction = attachedLibrary.AfterBattle;
        bController.SetBattle(11);        
        templars[0].gameObject.SetActive(false);
        templars[1].gameObject.SetActive(false);
        templars[2].gameObject.SetActive(false);
    }


    IEnumerator vMageLaunchTimer()
    {
        if (bController == null)
        {
            bController = FindObjectOfType<BattleController>();
        }


        float timer = (float)attachedLibrary.vMageBattleStart.duration;
        Debug.Log("Playable Duration = " + timer);
        attachedLibrary.vMageBattleStart.Play();
        yield return new WaitForSeconds(timer);
        bController.afterBattleAction = attachedLibrary.AfterBattle;
        bController.SetBattle(11);
        templars[0].gameObject.SetActive(false);
        templars[1].gameObject.SetActive(false);
   
        yield return new WaitForSeconds(.05f);
        attachedLibrary.vMageBattleStart.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (vMagic && !opened)
        {
            if (inRange)
            {
                NPCTrigger();
            }
        }
    }
}
