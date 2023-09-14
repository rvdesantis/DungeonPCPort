using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class HornRoomParent : RoomPropParent
{   

    public PlayableDirector demonessPlayable;
    public bool enterTrigger;
    public GameObject afterPlaySpawnPoint;
   
    IEnumerator FirstEnterENV()
    {
        Debug.Log("Room ENV Enter Trigger");
        PartyController party = FindObjectOfType<PartyController>();
        PlayerController player = FindObjectOfType<PlayerController>();
        MonsterController monsters = FindObjectOfType<MonsterController>();
        DunUIController uiController = FindObjectOfType<DunUIController>();
        if (roomParent.roomType == CubeRoom.RoomType.NPC)
        {
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


            float clipTime = (float)demonessPlayable.duration;

            player.controller.enabled = false;
            uiController.compassObj.SetActive(false);

            demonessPlayable.Play();
            yield return new WaitForSeconds(clipTime);
            party.activeParty[0].torch.SetActive(false);
            foreach (DunModel model in party.activeParty)
            {
                model.gameObject.SetActive(false);
            }
            player.controller.enabled = true;
            uiController.compassObj.SetActive(true);
        }
        if (roomParent.roomType == CubeRoom.RoomType.quest)
        {
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


            float clipTime = (float)demonessPlayable.duration;

            player.controller.enabled = false;

            uiController.compassObj.SetActive(false);
            demonessPlayable.Play();
            yield return new WaitForSeconds(clipTime);
            party.activeParty[0].torch.SetActive(false);
            foreach (DunModel model in party.activeParty)
            {
                model.gameObject.SetActive(false);
            }
            player.transform.position = afterPlaySpawnPoint.transform.position;
            player.transform.rotation = afterPlaySpawnPoint.transform.rotation;
            player.controller.enabled = true;
            uiController.compassObj.SetActive(true);
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
