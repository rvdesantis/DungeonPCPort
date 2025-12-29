using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TortureSmallParent : RoomPropParent
{
    public PlayableDirector trollEnterPlayable;
    public bool enterTrigger;
    public GameObject afterPlaySpawnPoint;
    public List<DunModel> activeModels;


    // set up for Question room, if second tortuneRoom exists, add cage with NPC, and Jailer in other room drops key to cage.  Reward with Chaos Orb?

    public override void AfterBattle()
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();
        DunUIController uiController = FindFirstObjectByType<DunUIController>();

        player.controller.enabled = true;
        uiController.compassObj.SetActive(true);
    }
    public void AfterEnter()
    {
        Debug.Log("Starting Gargoyle Battle");
        PartyController party = FindFirstObjectByType<PartyController>();
        SceneController controller = FindFirstObjectByType<SceneController>();
        BattleController battleC = FindFirstObjectByType<BattleController>();

        controller.endAction = null;
        controller.activePlayable = null;

        foreach (DunModel mod in activeModels)
        {
            if (mod != null)
            {
                mod.gameObject.SetActive(false);
            }
        }
        foreach (DunModel model in party.activeParty)
        {
            model.gameObject.SetActive(false);
        }
        battleC.afterBattleAction = AfterBattle;
        battleC.SetBattle(25);

    }


    IEnumerator FirstEnterENV()
    {
        Debug.Log("Troll Room Enter Trigger");
        SceneController controller = FindFirstObjectByType<SceneController>();
        PartyController party = FindFirstObjectByType<PartyController>();
        PlayerController player = FindFirstObjectByType<PlayerController>();
        MonsterController monsters = FindFirstObjectByType<MonsterController>();
        NPCController NPCs = FindFirstObjectByType<NPCController>();
        DunUIController uiController = FindFirstObjectByType<DunUIController>();

        controller.activePlayable = trollEnterPlayable;
        controller.endAction += AfterEnter;

        party.AssignCamBrain(trollEnterPlayable, 3);
        foreach (DunModel model in party.activeParty)
        {
            model.AssignToDirector(trollEnterPlayable);
            model.transform.position = trollEnterPlayable.transform.position;
            model.transform.parent = trollEnterPlayable.transform;
            model.gameObject.SetActive(true);
            if (model.activeWeapon != null)
            {
                model.activeWeapon.SetActive(false);
            }
            if (model.torch != null)
            {
                model.torch.SetActive(false);
            }
        }

        DunModel npc = null;
        npc = Instantiate(NPCs.npcMasterList[4], trollEnterPlayable.transform, false);
        npc.gameObject.SetActive(true);
        npc.transform.position = trollEnterPlayable.transform.position;
        npc.AssignToDirector(trollEnterPlayable, 4);
        activeModels.Add(npc);


        DunModel troll = null;        
        troll = Instantiate(monsters.enemyMasterList[25], trollEnterPlayable.transform, false);
        troll.gameObject.SetActive(true);
        troll.transform.position = trollEnterPlayable.transform.position;
        troll.AssignToDirector(trollEnterPlayable, 5);
        activeModels.Add(troll);

        float clipTime = (float)trollEnterPlayable.duration;
        player.controller.enabled = false;
        uiController.compassObj.SetActive(false);
        trollEnterPlayable.gameObject.SetActive(true);
        trollEnterPlayable.Play();
        yield return new WaitForSeconds(clipTime);
        if (controller.activePlayable == trollEnterPlayable)
        {
            AfterEnter();
        }
    }


    private void Update()
    {
        if (!enterTrigger)
        {
            if (roomParent.inRoom)
            {
                enterTrigger = true;
                StartCoroutine(FirstEnterENV());
            }
        }
    }
}
