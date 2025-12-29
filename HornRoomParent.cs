using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Playables;

public class HornRoomParent : RoomPropParent
{   

    public PlayableDirector demonessPlayable;
    public bool enterTrigger;
    public bool skippedTrigger;
    public GameObject afterPlaySpawnPoint;
    public DunModel demonessMod;
    public bool portalOpen = true;
    public DemonPortal demonPortal;
   
    IEnumerator FirstEnterENV()
    {
        Debug.Log("Room ENV Enter Trigger");
        PartyController party = FindAnyObjectByType<PartyController>();
        PlayerController player = FindAnyObjectByType<PlayerController>();
        MonsterController monsters = FindAnyObjectByType<MonsterController>();
        SceneController controller = FindAnyObjectByType<SceneController>();
        DunUIController uiController = FindAnyObjectByType<DunUIController>();

        if (roomParent.roomType == CubeRoom.RoomType.battle)
        {
            controller.activePlayable = demonessPlayable;
            controller.endAction = EndEnter;

            party.AssignCamBrain(demonessPlayable, 3);
            foreach (DunModel model in party.activeParty)
            {
                model.AssignToDirector(demonessPlayable);
                model.gameObject.SetActive(true);
                model.transform.position = demonessPlayable.transform.position;
                model.transform.parent = demonessPlayable.transform;
            }
            party.activeParty[0].torch.SetActive(true);

            DunModel demoness = Instantiate(monsters.enemyMasterList[0], demonessPlayable.transform, false);
            demoness.transform.position = demonessPlayable.transform.position;
            demoness.AssignToDirector(demonessPlayable, 4);
            demonessMod = demoness;
            float clipTime = (float)demonessPlayable.duration;

            player.controller.enabled = false;
            uiController.compassObj.SetActive(false);

            demonessPlayable.Play();
            yield return new WaitForSeconds(clipTime);
            if (controller.activePlayable == demonessPlayable)
            {
                EndEnter();
            }
            demoness.gameObject.SetActive(false);
            Destroy(demoness.gameObject);
        }        

        if (roomParent.roomType == CubeRoom.RoomType.quest)
        {
            controller.activePlayable = demonessPlayable;
            controller.endAction = EndEnter;

            controller.activePlayable = demonessPlayable;
            party.AssignCamBrain(demonessPlayable, 3);
            foreach (DunModel model in party.activeParty)
            {
                model.AssignToDirector(demonessPlayable);
                model.gameObject.SetActive(true);
                model.transform.position = demonessPlayable.transform.position;
                model.transform.parent = demonessPlayable.transform;
            }
            party.activeParty[0].torch.SetActive(true);
            DunModel demoness = Instantiate(monsters.enemyMasterList[0], demonessPlayable.transform, false);
            demoness.transform.position = demonessPlayable.transform.position;
            demoness.AssignToDirector(demonessPlayable, 4);
            demonessMod = demoness;

            float clipTime = (float)demonessPlayable.duration;

            player.controller.enabled = false;

            uiController.compassObj.SetActive(false);
            demonessPlayable.Play();
            yield return new WaitForSeconds(clipTime + .1f);
            if (controller.activePlayable == demonessPlayable)
            {
                EndEnter();
            }
        }
    }

    public void EndEnter()
    {
        PartyController party = FindAnyObjectByType<PartyController>();  
        SceneController controller = FindAnyObjectByType<SceneController>();
        controller.activePlayable = null;
        controller.endAction = null;
        party.activeParty[0].torch.SetActive(false);
        foreach (DunModel model in party.activeParty)
        {
            model.gameObject.SetActive(false);
        }
        StartDemonessBattle();
        demonessMod.gameObject.SetActive(false);
    }


    public void StartDemonessBattle()
    {
        BattleController battleC = FindAnyObjectByType<BattleController>();
        battleC.afterBattleAction = DemonessBattleReturn;
        battleC.SetBattle(0);
    }

    public void DemonessBattleReturn()
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();
        DunUIController uiController = FindAnyObjectByType<DunUIController>();
        if (demonessMod != null)
        {
            demonessMod.gameObject.SetActive(false);
        }
        player.transform.position = afterPlaySpawnPoint.transform.position;
        player.transform.rotation = afterPlaySpawnPoint.transform.rotation;
        player.controller.enabled = true;
        uiController.compassObj.SetActive(true);
        portalOpen = true;
        demonPortal.player = FindAnyObjectByType<PlayerController>();
        demonPortal.isOpen = true;
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
