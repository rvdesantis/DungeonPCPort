using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SpiderRoomParent : RoomPropParent
{
    public DunModel activeQ;
    public DunModel activeSpider;
    public PlayableDirector spiderQattackIntroPlayable;
    public bool enterTrigger;

    public override void EnvFill()
    {
        base.EnvFill();  
    }



    IEnumerator FirstEnterENV()
    {
        Debug.Log("Room ENV Enter Trigger");

        SceneController controller = FindAnyObjectByType<SceneController>();
        PartyController party = FindAnyObjectByType<PartyController>();
        PlayerController player = FindAnyObjectByType<PlayerController>();
        MonsterController monsters = FindAnyObjectByType<MonsterController>();
        DunUIController uiController = FindAnyObjectByType<DunUIController>();

        if (roomParent.roomType == CubeRoom.RoomType.battle || roomParent.roomType == CubeRoom.RoomType.chest || roomParent.roomType == CubeRoom.RoomType.quest)
        {
            DunModel spiderQ = Instantiate(monsters.enemyMasterList[15], spiderQattackIntroPlayable.transform);
            DunModel spiderOne = Instantiate(monsters.enemyMasterList[16], spiderQattackIntroPlayable.transform);

            activeQ = spiderQ;
            activeSpider = spiderOne;

            party.AssignCamBrain(spiderQattackIntroPlayable, 3);
            spiderQ.AssignToDirector(spiderQattackIntroPlayable, 4);
            spiderOne.AssignToDirector(spiderQattackIntroPlayable, 5);

            Debug.Log("Spiders Spawned in Large Room", roomParent.gameObject);


            controller.activePlayable = spiderQattackIntroPlayable;
            controller.endAction += AfterEnter;

            float clipTime = (float)spiderQattackIntroPlayable.duration;
            player.controller.enabled = false;
            uiController.compassObj.SetActive(false);
            spiderQattackIntroPlayable.gameObject.SetActive(true);
            spiderQattackIntroPlayable.Play();
            yield return new WaitForSeconds(clipTime);          

            if (controller.activePlayable == spiderQattackIntroPlayable)
            {
                AfterEnter();
            }
        }

       
    }


    public void AfterEnter()
    {
        BattleController battleC = FindAnyObjectByType<BattleController>();
        PartyController party = FindAnyObjectByType<PartyController>();
        SceneController controller = FindAnyObjectByType<SceneController>();
    

        controller.endAction = null;
        controller.activePlayable = null;
        activeQ.gameObject.SetActive(false);
        activeSpider.gameObject.SetActive(false);


        battleC.afterBattleAction = AfterSpider;
        battleC.SetBattle(15);
    }

    public void AfterSpider()
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();
        MusicController music = FindAnyObjectByType<MusicController>();
        music.CrossfadeToNextClip(music.dungeonMusicClips[Random.Range(0, music.dungeonMusicClips.Count)]);
        player.enabled = true;
        activeQ.gameObject.SetActive(false);
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

