using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SmallBoneRoomParent : RoomPropParent
{
    public PlayableDirector evilRDirector;
    public PlayableDirector wolfDirector;
    public bool enterTrigger;
    public bool rabbit;
    public GameObject afterPlaySpawnPoint;
    public DunModel activeModel;
    public List<DunModel> activeWolves;

    public void AfterEnter()
    {
        PartyController party = FindObjectOfType<PartyController>();
        SceneController controller = FindObjectOfType<SceneController>();
        PlayerController player = FindObjectOfType<PlayerController>();
        DunUIController uiController = FindObjectOfType<DunUIController>();

        Debug.Log("Starting Battle Launch");
        controller.activePlayable = null;
        controller.endAction = null;
        if (rabbit)
        {
            activeModel.gameObject.SetActive(false);
            foreach (DunModel model in party.activeParty)
            {
                model.gameObject.SetActive(false);
            }             
            AfterBattle();
            return;
        }
        if (!rabbit)
        {
            Debug.Log("Starting Wolf Launch");
            foreach (DunModel woofy in activeWolves)
            {
                woofy.gameObject.SetActive(false);
            }
            foreach (DunModel model in party.activeParty)
            {
                model.gameObject.SetActive(false);
            }
            if (activeModel != null)
            {
                activeModel.gameObject.SetActive(false);
            }
            controller.battleController.afterBattleAction = AfterBattle;
            Debug.Log("Beginning Battle");
            controller.battleController.SetBattle(17);
        }
    }


    IEnumerator FirstEnterENV()
    {
        Debug.Log("Room ENV Enter Trigger");
        SceneController controller = FindObjectOfType<SceneController>();
        PartyController party = FindObjectOfType<PartyController>();
        PlayerController player = FindObjectOfType<PlayerController>();
        MonsterController monsters = FindObjectOfType<MonsterController>();
        DunUIController uiController = FindObjectOfType<DunUIController>();

        int hopper = Random.Range(0, 10);
        if (hopper == 0)
        {
            rabbit = true;
            controller.activePlayable = evilRDirector;
            controller.endAction += AfterEnter;

            party.AssignCamBrain(evilRDirector, 3);
            foreach (DunModel model in party.activeParty)
            {
                model.AssignToDirector(evilRDirector);
                model.transform.position = evilRDirector.transform.position;
                model.transform.parent = evilRDirector.transform;
                model.gameObject.SetActive(true);
                if (model.activeWeapon != null)
                {
                    model.activeWeapon.SetActive(false);
                }
            }
            party.activeParty[0].torch.SetActive(false);

            DunModel bunny = null;
            foreach (DunModel enemy in monsters.enemyMasterList)
            {
                if (enemy.spawnArea == DunModel.SpawnArea.smallRoom)
                {
                    if (enemy.spawnPlayableInt == 2)
                    {
                        bunny = Instantiate(enemy, evilRDirector.transform, false);
                        bunny.gameObject.SetActive(true);
                        bunny.transform.position = evilRDirector.transform.position;
                        bunny.AssignToDirector(evilRDirector, 4);
                        activeModel = bunny;
                        break;
                    }
                }
            }

            float clipTime = (float)evilRDirector.duration;
            player.controller.enabled = false;
            uiController.compassObj.SetActive(false);
            evilRDirector.Play();
            yield return new WaitForSeconds(clipTime);
            if (controller.activePlayable == evilRDirector)
            {
                AfterEnter();
            }
        }
        if (hopper != 0)
        {
            controller.activePlayable = wolfDirector;
            controller.endAction = null;
            controller.endAction += AfterEnter;

            party.AssignCamBrain(wolfDirector, 3);
            foreach (DunModel model in party.activeParty)
            {
                model.AssignToDirector(wolfDirector);
                model.transform.position = wolfDirector.transform.position;
                model.transform.parent = wolfDirector.transform;
                model.gameObject.SetActive(true);
                if (model.activeWeapon != null)
                {
                    model.activeWeapon.SetActive(false);
                }
            }
            party.activeParty[0].torch.SetActive(true);

            DunModel wolf = null;          
  
            wolf = Instantiate(monsters.enemyMasterList[17], wolfDirector.transform, false);
            wolf.gameObject.SetActive(true);
            wolf.transform.position = wolfDirector.transform.position;
            wolf.AssignToDirector(wolfDirector, 4);

            DunModel wolf2 = null;

            wolf2 = Instantiate(monsters.enemyMasterList[17], wolfDirector.transform, false);
            wolf2.gameObject.SetActive(true);
            wolf2.transform.position = wolfDirector.transform.position;
            wolf2.AssignToDirector(wolfDirector, 5);           

            DunModel wolf3 = null;

            wolf3 = Instantiate(monsters.enemyMasterList[17], wolfDirector.transform, false);
            wolf3.gameObject.SetActive(true);
            wolf3.transform.position = wolfDirector.transform.position;
            wolf3.AssignToDirector(wolfDirector, 6);

            activeWolves.Add(wolf);
            activeWolves.Add(wolf2);
            activeWolves.Add(wolf3);

            float clipTime = (float)wolfDirector.duration;
            player.controller.enabled = false;
            uiController.compassObj.SetActive(false);
            wolfDirector.Play();
            Debug.Log("Starting Wolf Playable");
            yield return new WaitForSeconds(clipTime);
            
            if (controller.activePlayable == wolfDirector)
            {  
                AfterEnter();
            }
        }
    }

    public override void AfterBattle()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        DunUIController uiController = FindObjectOfType<DunUIController>();
        player.controller.enabled = true;
        uiController.compassObj.SetActive(true);
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
