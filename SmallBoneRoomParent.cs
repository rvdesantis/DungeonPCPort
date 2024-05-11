using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SmallBoneRoomParent : RoomPropParent
{
    public PlayableDirector evilRDirector;
    public bool enterTrigger;
    public GameObject afterPlaySpawnPoint;
    public DunModel activeModel;

    public void AfterEnter()
    {
        PartyController party = FindObjectOfType<PartyController>();
        SceneController controller = FindObjectOfType<SceneController>();
        PlayerController player = FindObjectOfType<PlayerController>();
        DunUIController uiController = FindObjectOfType<DunUIController>();

        controller.activePlayable = null;
        controller.endAction = null;

        if (controller.activePlayable == evilRDirector)
        {
            controller.activePlayable = null;
        }
        activeModel.gameObject.SetActive(false);
        foreach (DunModel model in party.activeParty)
        {
            model.gameObject.SetActive(false);
        }
        player.controller.enabled = true;
        uiController.compassObj.SetActive(true);
        MusicController music = FindObjectOfType<MusicController>();
        music.CrossfadeToNextClip(music.dungeonMusicClips[Random.Range(0, music.dungeonMusicClips.Count)]);
    }


    IEnumerator FirstEnterENV()
    {
        Debug.Log("Room ENV Enter Trigger");
        SceneController controller = FindObjectOfType<SceneController>();
        PartyController party = FindObjectOfType<PartyController>();
        PlayerController player = FindObjectOfType<PlayerController>();
        MonsterController monsters = FindObjectOfType<MonsterController>();
        DunUIController uiController = FindObjectOfType<DunUIController>();

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
