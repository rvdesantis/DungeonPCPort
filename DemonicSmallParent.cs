using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class DemonicSmallParent : RoomPropParent
{

    public PlayableDirector gargEnterPlayable;
    public bool enterTrigger;
    public GameObject afterPlaySpawnPoint;


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



    IEnumerator FirstEnterENV()
    {
        Debug.Log("Room ENV Enter Trigger");
        PartyController party = FindObjectOfType<PartyController>();
        PlayerController player = FindObjectOfType<PlayerController>();
        MonsterController monsters = FindAnyObjectByType<MonsterController>();
        gargEnterPlayable.gameObject.SetActive(true);
        if (roomParent.roomType == CubeRoom.RoomType.battle)
        {
            Debug.Log("Gargoyle Battle Trigger");
            party.AssignCamBrain(gargEnterPlayable, 3);
            foreach (DunModel model in party.activeParty)
            {
                model.AssignToDirector(gargEnterPlayable);
                model.gameObject.SetActive(true);
                model.transform.position = gargEnterPlayable.transform.position;
                model.transform.parent = gargEnterPlayable.transform;
            }
            party.activeParty[0].torch.SetActive(true);

            DunModel garg0 = Instantiate(monsters.enemyMasterList[3], gargEnterPlayable.transform, false);
            garg0.transform.position = gargEnterPlayable.transform.position;
            garg0.AssignToDirector(gargEnterPlayable, 4);

            DunModel garg1 = Instantiate(monsters.enemyMasterList[3], gargEnterPlayable.transform, false);
            garg1.transform.position = gargEnterPlayable.transform.position;
            garg1.AssignToDirector(gargEnterPlayable, 5);

            DunModel garg2 = Instantiate(monsters.enemyMasterList[3], gargEnterPlayable.transform, false);
            garg2.transform.position = gargEnterPlayable.transform.position;
            garg2.AssignToDirector(gargEnterPlayable, 6);


            float clipTime = (float)gargEnterPlayable.duration;

            player.controller.enabled = false;
            gargEnterPlayable.Play();
            yield return new WaitForSeconds(clipTime);
            party.activeParty[0].torch.SetActive(false);
            foreach (DunModel model in party.activeParty)
            {
                model.gameObject.SetActive(false);
            }
            player.controller.enabled = true;
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
