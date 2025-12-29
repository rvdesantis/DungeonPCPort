using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Playables;

public class DemonicSmallParent : RoomPropParent
{

    public PlayableDirector gargEnterPlayable;
    public bool enterTrigger;
    public GameObject afterPlaySpawnPoint;
    public List<DunModel> activeModels;

    public override void AfterBattle()
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();
        DunUIController uiController = FindAnyObjectByType<DunUIController>();

        player.controller.enabled = true;
        uiController.compassObj.SetActive(true);
    }
    public void AfterEnter()
    {
        Debug.Log("Starting Gargoyle Battle");
        PartyController party = FindAnyObjectByType<PartyController>();
        SceneController controller = FindAnyObjectByType<SceneController>();
        BattleController battleC = FindAnyObjectByType<BattleController>();

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
        battleC.SetBattle(3);

    }


    IEnumerator FirstEnterENV()
    {
        Debug.Log("Garg Room Enter Trigger");
        SceneController controller = FindAnyObjectByType<SceneController>();
        PartyController party = FindAnyObjectByType<PartyController>();
        PlayerController player = FindAnyObjectByType<PlayerController>();
        MonsterController monsters = FindAnyObjectByType<MonsterController>();
        DunUIController uiController = FindAnyObjectByType<DunUIController>();

        controller.activePlayable = gargEnterPlayable;
        controller.endAction += AfterEnter;

        party.AssignCamBrain(gargEnterPlayable, 3);
        foreach (DunModel model in party.activeParty)
        {
            model.AssignToDirector(gargEnterPlayable);
            model.transform.position = gargEnterPlayable.transform.position;
            model.transform.parent = gargEnterPlayable.transform;
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
           

        DunModel garg0 = null;
        DunModel garg1 = null;
        DunModel garg2 = null;
        foreach (DunModel enemy in monsters.enemyMasterList)
        {
            if (enemy.spawnArea == DunModel.SpawnArea.smallRoom)
            {
                if (enemy.spawnPlayableInt == 0)
                {
                    garg0 = Instantiate(enemy, gargEnterPlayable.transform, false);
                    garg0.gameObject.SetActive(true);
                    garg0.transform.position = gargEnterPlayable.transform.position;
                    garg0.AssignToDirector(gargEnterPlayable, 4);
                    activeModels.Add(garg0);

                    garg1 = Instantiate(enemy, gargEnterPlayable.transform, false);
                    garg1.gameObject.SetActive(true);
                    garg1.transform.position = gargEnterPlayable.transform.position;
                    garg1.AssignToDirector(gargEnterPlayable, 5);
                    activeModels.Add(garg1);

                    garg2 = Instantiate(enemy, gargEnterPlayable.transform, false);
                    garg2.gameObject.SetActive(true);
                    garg2.transform.position = gargEnterPlayable.transform.position;
                    garg2.AssignToDirector(gargEnterPlayable, 6);
                    activeModels.Add(garg2);
                    break;
                }
            }
        }

        float clipTime = (float)gargEnterPlayable.duration;
        player.controller.enabled = false;
        uiController.compassObj.SetActive(false);
        gargEnterPlayable.gameObject.SetActive(true);
        gargEnterPlayable.Play();
        yield return new WaitForSeconds(clipTime);
        if (controller.activePlayable == gargEnterPlayable)
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
