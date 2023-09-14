using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CrystalLampRoomParent : RoomPropParent
{
    // list colors set to 0 - blue, 1 - green, 2 - red, 3- white
    // Scorp enemy - small room 1 in Monster controller
    public List<GameObject> lamp0List;
    public List<GameObject> lamp1List;
    public List<GameObject> lamp2List;
    public ScorpionSwitch scorpionSwitch;
    public GameObject spawnPointObj;
    public PlayableDirector scorpSummonPlayable;

    private void Start()
    {
        DistanceController distance = FindObjectOfType<DistanceController>();
        distance.switches.Add(scorpionSwitch);
    }

    IEnumerator SummonETimer()
    {
        Debug.Log("Trigger Scorpion Summon");
        PartyController party = FindObjectOfType<PartyController>();
        PlayerController player = FindObjectOfType<PlayerController>();
        MonsterController monsters = FindAnyObjectByType<MonsterController>();
        DunUIController uiController = FindObjectOfType<DunUIController>();

        scorpSummonPlayable.gameObject.SetActive(true);

        party.AssignCamBrain(scorpSummonPlayable, 3);
        foreach (DunModel model in party.activeParty)
        {
            model.AssignToDirector(scorpSummonPlayable);
            model.gameObject.SetActive(true);
            model.transform.position = scorpSummonPlayable.transform.position;
            model.transform.rotation = scorpSummonPlayable.transform.rotation;
            model.transform.parent = scorpSummonPlayable.transform;
        }
        party.activeParty[0].torch.SetActive(true);


        DunModel scorp = null;
        foreach (DunModel enemy in monsters.enemyMasterList)
        {
            if (enemy.spawnArea == DunModel.SpawnArea.smallRoom)
            {
                if (enemy.spawnPlayableInt == 1)
                {
                    scorp = enemy;
                    break;
                }
            }
        }

        Instantiate(scorp, scorpSummonPlayable.transform, false);
        scorp.transform.position = scorpSummonPlayable.transform.position;
        scorp.AssignToDirector(scorpSummonPlayable, 4);

        float clipTime = (float)scorpSummonPlayable.duration;

        player.controller.enabled = false;
        uiController.compassObj.SetActive(false);

        scorpSummonPlayable.Play();
        yield return new WaitForSeconds(clipTime);
        party.activeParty[0].torch.SetActive(false);
        foreach (DunModel model in party.activeParty)
        {
            model.gameObject.SetActive(false);
        }
        player.controller.enabled = true;
        uiController.compassObj.SetActive(true);
    }

    public void LampRoll()
    {
        foreach(GameObject lamp in lamp0List)
        {
            if (lamp.activeSelf)
            {
                lamp.SetActive(false);
            }
        }
        foreach (GameObject lamp in lamp1List)
        {
            if (lamp.activeSelf)
            {
                lamp.SetActive(false);
            }
        }
        foreach (GameObject lamp in lamp2List)
        {
            if (lamp.activeSelf)
            {
                lamp.SetActive(false);
            }
        }

        int x = Random.Range(0, lamp0List.Count);
        int y = Random.Range(0, lamp1List.Count);
        int z = Random.Range(0, lamp2List.Count);

        lamp0List[x].SetActive(true);
        lamp1List[y].SetActive(true);
        lamp2List[z].SetActive(true);

        LampCheck(x, y, z);
    }

    public void LampCheck(int lamp0, int lamp1, int lamp2)
    {
        if (lamp0 == lamp1 && lamp1 == lamp2 && lamp0 == lamp2)
        {
            scorpionSwitch.locked = true;
            if (lamp0 == 0)
            {
                Debug.Log("Trigger Blue Lamp Room");
                SummonBlue();
            }
            if (lamp0 == 1)
            {
                Debug.Log("Trigger Green Lamp Room");
                SummonGreen();
            }
            if (lamp0 == 2)
            {
                SummonEnemy();
            }
            if (lamp0 == 3)
            {
                SummonWhite();
            }
        }
    }


    public void SummonEnemy()
    {
        StartCoroutine(SummonETimer());
    }

    public void SummonBlue()
    {

    }

    public void SummonGreen()
    {

    }

    public void SummonWhite()
    {

    }

}
