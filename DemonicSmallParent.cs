using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class DemonicSmallParent : RoomPropParent
{

    public PlayableDirector gargEnterPlayable;
    public bool enterTrigger;
    public GameObject afterPlaySpawnPoint;
    public List<DunModel> activeModels;


    public override void EnvFill()
    {
        base.EnvFill(); // sets actives
        if (roomParent.roomType == CubeRoom.RoomType.portal)
        {
            HallStarterCube bossStarter = null;
            SceneBuilder builder = FindObjectOfType<SceneBuilder>();

            foreach (HallStarterCube starter in builder.createdStarters)
            {
                if (starter.hallType == HallStarterCube.HallType.boss)
                {
                    bossStarter = starter;
                    break;
                }
            }

            BossHallCube targetCube = bossStarter.generatedHallway[1].GetComponent<BossHallCube>();
            portbGameObject = targetCube.bossPortal;
            SetPortal();
        }
    }



    public void AfterEnter()
    {
        PartyController party = FindObjectOfType<PartyController>();
        SceneController controller = FindObjectOfType<SceneController>();
        PlayerController player = FindObjectOfType<PlayerController>();
        DunUIController uiController = FindObjectOfType<DunUIController>();

        controller.endAction = null;
        controller.activePlayable = null;

        if (controller.activePlayable == gargEnterPlayable)
        {
            controller.activePlayable = null;
        }
        foreach (DunModel garg in activeModels)
        {
            garg.gameObject.SetActive(false);
        }

        foreach (DunModel model in party.activeParty)
        {
            model.gameObject.SetActive(false);
        }
        player.controller.enabled = true;
        uiController.compassObj.SetActive(true);
    }


    IEnumerator FirstEnterENV()
    {
        Debug.Log("Room ENV Enter Trigger");

            SceneController controller = FindObjectOfType<SceneController>();
            PartyController party = FindObjectOfType<PartyController>();
            PlayerController player = FindObjectOfType<PlayerController>();
            MonsterController monsters = FindObjectOfType<MonsterController>();
            DunUIController uiController = FindObjectOfType<DunUIController>();

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
            }
            party.activeParty[0].torch.SetActive(false);

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
