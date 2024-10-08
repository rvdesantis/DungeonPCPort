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
    public List<DunSwitch> lampSwitches;
    public GameObject spawnPointObj;
    public DunModel activeModel;
    public PlayableDirector scorpSummonPlayable;
    public PlayableDirector gobChestPlayable;
    public DunChest gobChest;
    public PlayableDirector knightPlayable;
    public PlayableDirector chaosPlayable;
    public DunItem cOrb;

    public bool gKnightBattle;
    public bool scorpBattle;

    public override void AfterBattle()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        DunUIController uiController = FindObjectOfType<DunUIController>();

        if (gKnightBattle)
        {
            Debug.Log("After Playable for Ghost Knight");
            activeModel.gameObject.SetActive(false);
            activeModel = null;
        }
        if (scorpBattle)
        {
            Debug.Log("After Playable for Scorp");
            activeModel.gameObject.SetActive(false);
            activeModel = null;
        }
        player.controller.enabled = true;
        uiController.compassObj.SetActive(true);
    }

    private void Start()
    {
        DistanceController distance = FindObjectOfType<DistanceController>();
        distance.switches.Add(scorpionSwitch);
        foreach (DunSwitch lampSw in lampSwitches)
        {
            distance.switches.Add(lampSw);
        }
    }

    public void SummonEEnd()
    {
        PartyController party = FindObjectOfType<PartyController>();
        PlayerController player = FindObjectOfType<PlayerController>();
        DunUIController uiController = FindObjectOfType<DunUIController>();
        SceneController controller = FindObjectOfType<SceneController>();
        MusicController music = FindObjectOfType<MusicController>();
        BattleController battleC = controller.battleController;

        scorpSummonPlayable.time = scorpSummonPlayable.duration;
        activeModel.gameObject.SetActive(false);
        controller.activePlayable = null;
        controller.endAction = null;
        foreach (DunModel model in party.activeParty)
        {
            model.gameObject.SetActive(false);
        }
        battleC.afterBattleAction = AfterBattle;
        scorpBattle = true;
        gKnightBattle = false;
        battleC.SetBattle(6);
    }

    public void SummonGEnd()
    {
        PartyController party = FindObjectOfType<PartyController>();
        PlayerController player = FindObjectOfType<PlayerController>();
        DunUIController uiController = FindObjectOfType<DunUIController>();
        SceneController controller = FindObjectOfType<SceneController>();
        MusicController music = FindObjectOfType<MusicController>();

        activeModel.gameObject.SetActive(false);
        controller.activePlayable = null;
        controller.endAction = null;
        foreach (DunModel model in party.activeParty)
        {
            model.gameObject.SetActive(false);
        }
        player.controller.enabled = true;
        uiController.compassObj.SetActive(true);

        UnlockController unlockables = controller.unlockables;
        if (!unlockables.mapVendorUnlock)
        {
            unlockables.UnlockNPC(0);
        }

        music.CrossfadeToNextClip(music.dungeonMusicClips[Random.Range(0, music.dungeonMusicClips.Count)]);
    }

    IEnumerator SummonETimer()
    {
        Debug.Log("Trigger Scorpion Summon");
        PartyController party = FindObjectOfType<PartyController>();
        PlayerController player = FindObjectOfType<PlayerController>();
        MonsterController monsters = FindAnyObjectByType<MonsterController>();
        DunUIController uiController = FindObjectOfType<DunUIController>();
        SceneController controller = FindObjectOfType<SceneController>();

        controller.activePlayable = scorpSummonPlayable;
        controller.endAction += SummonEEnd;
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

        DunModel scorp = null;
        foreach (DunModel enemy in monsters.enemyMasterList)
        {
            if (enemy.spawnArea == DunModel.SpawnArea.smallRoom)
            {
                if (enemy.spawnPlayableInt == 1)
                {
                    scorp = Instantiate(enemy, scorpSummonPlayable.transform.position, scorpSummonPlayable.transform.rotation);
                    scorp.AssignToDirector(scorpSummonPlayable, 4);
                    scorp.gameObject.SetActive(true);
                    scorp.transform.parent = scorpSummonPlayable.transform;
                    scorp.transform.position = scorpSummonPlayable.transform.position;
                    scorp.transform.rotation = scorpSummonPlayable.transform.rotation;
                    activeModel = scorp;
                    break;
                }
            }
        }
       
        float clipTime = (float)scorpSummonPlayable.duration;

        player.controller.enabled = false;
        uiController.compassObj.SetActive(false);

        scorpSummonPlayable.Play();
        yield return new WaitForSeconds(clipTime);
       
        scorp.gameObject.SetActive(false);
        if (distanceController == null)
        {
            distanceController = FindObjectOfType<DistanceController>();
        }
        distanceController.switches.Remove(scorpionSwitch);
        distanceController.switches.Remove(lampSwitches[0]);
        distanceController.switches.Remove(lampSwitches[1]);
        distanceController.switches.Remove(lampSwitches[2]);

        if (controller.activePlayable == scorpSummonPlayable)
        {
            SummonEEnd();
        }
    }

    IEnumerator SummonGobTimer()
    {
        Debug.Log("Trigger Goblin Summon");
        PartyController party = FindObjectOfType<PartyController>();
        PlayerController player = FindObjectOfType<PlayerController>();
        NPCController npcS = FindObjectOfType<NPCController>();
        DunUIController uiController = FindObjectOfType<DunUIController>();
        SceneController controller = FindObjectOfType<SceneController>();

        controller.activePlayable = gobChestPlayable;
        controller.endAction += SummonGEnd;

        gobChestPlayable.gameObject.SetActive(true);

        party.AssignCamBrain(gobChestPlayable, 3);
        foreach (DunModel model in party.activeParty)
        {
            model.AssignToDirector(gobChestPlayable);
            model.gameObject.SetActive(true);
            model.transform.position = gobChestPlayable.transform.position;
            model.transform.rotation = gobChestPlayable.transform.rotation;
            model.transform.parent = gobChestPlayable.transform;
        }

        DunModel goblin = null;
        foreach (DunModel npc in npcS.npcMasterList)
        {
            if (npc.spawnArea == DunModel.SpawnArea.smallRoom)
            {
                if (npc.spawnPlayableInt == 0)
                {
                    goblin = Instantiate(npc, gobChestPlayable.transform.position, gobChestPlayable.transform.rotation);
                    goblin.AssignToDirector(gobChestPlayable, 4);
                    goblin.gameObject.SetActive(true);
                    goblin.transform.parent = gobChestPlayable.transform;
                    goblin.transform.position = gobChestPlayable.transform.position;
                    goblin.transform.rotation = gobChestPlayable.transform.rotation;
                    activeModel = goblin;
                    break;
                }
            }
        }

        float clipTime = (float)gobChestPlayable.duration;

        player.controller.enabled = false;
        uiController.compassObj.SetActive(false);

        gobChestPlayable.Play();
        yield return new WaitForSeconds(clipTime);
        if (distanceController == null)
        {
            distanceController = FindObjectOfType<DistanceController>();
        }
        distanceController.chests.Add(gobChest);
        distanceController.switches.Remove(scorpionSwitch);
        distanceController.switches.Remove(lampSwitches[0]);
        distanceController.switches.Remove(lampSwitches[1]);
        distanceController.switches.Remove(lampSwitches[2]);
        goblin.gameObject.SetActive(false);
        if (controller.activePlayable == gobChestPlayable)
        {
            SummonGEnd();
        }      
    }

    IEnumerator SummonKnightTimer()
    {
        Debug.Log("Trigger Knight Summon");
        PartyController party = FindObjectOfType<PartyController>();
        PlayerController player = FindObjectOfType<PlayerController>();
        MonsterController monster = FindObjectOfType<MonsterController>();
        DunUIController uiController = FindObjectOfType<DunUIController>();
        SceneController controller = FindObjectOfType<SceneController>();
        BattleController battleC = controller.battleController;

        knightPlayable.gameObject.SetActive(true);
        party.AssignCamBrain(knightPlayable, 3);
        DunModel knight = null;

        knight = Instantiate(monster.enemyMasterList[8], knightPlayable.transform.position, knightPlayable.transform.rotation);
        knight.AssignToDirector(knightPlayable, 4);
        knight.gameObject.SetActive(true);
        knight.transform.parent = knightPlayable.transform;
        knight.transform.position = knightPlayable.transform.position;
        knight.transform.rotation = knightPlayable.transform.rotation;
        activeModel = knight;                 
                   
        float clipTime = (float)knightPlayable.duration;
        player.controller.enabled = false;
        uiController.compassObj.SetActive(false);
        GhostKnightModel ghostK = knight.GetComponent<GhostKnightModel>();
        ghostK.headSmoke.gameObject.SetActive(true);
        ghostK.headSmoke.Play();
        knightPlayable.Play();
        yield return new WaitForSeconds(clipTime);

        if (distanceController == null)
        {
            distanceController = FindObjectOfType<DistanceController>();
        }
        GhostNightBlackSmith GNblackSmith = knight.GetComponent<GhostNightBlackSmith>();
        distanceController.npcS.Add(GNblackSmith);
        GNblackSmith.uiObject = uiController.blackSmithUI.gameObject;

        distanceController.switches.Remove(scorpionSwitch);
        distanceController.switches.Remove(lampSwitches[0]);
        distanceController.switches.Remove(lampSwitches[1]);
        distanceController.switches.Remove(lampSwitches[2]);
        foreach (DunModel model in party.activeParty)
        {
            model.gameObject.SetActive(false);
        }
        battleC.afterBattleAction = AfterBattle;
        gKnightBattle = true;
        player.controller.enabled = true;
    }

    IEnumerator ChaosOrbTimer()
    {
        Debug.Log("Trigger Orb Summon");
        PlayerController player = FindObjectOfType<PlayerController>();
        DunUIController uiController = FindObjectOfType<DunUIController>();
        PartyController party = FindObjectOfType<PartyController>();
        DistanceController distanceController = FindObjectOfType<DistanceController>();

        
        distanceController.dunItems.Add(cOrb);
        distanceController.switches.Remove(scorpionSwitch);
        distanceController.switches.Remove(lampSwitches[0]);
        distanceController.switches.Remove(lampSwitches[1]);
        distanceController.switches.Remove(lampSwitches[2]);

        chaosPlayable.gameObject.SetActive(true);
        party.AssignCamBrain(chaosPlayable, 3);


        float clipTime = (float)chaosPlayable.duration;

        player.controller.enabled = false;
        uiController.compassObj.SetActive(false);

        chaosPlayable.Play();
        yield return new WaitForSeconds(clipTime);
        player.controller.enabled = true;
        uiController.compassObj.SetActive(true);
        MusicController music = FindObjectOfType<MusicController>();
        music.CrossfadeToNextClip(music.dungeonMusicClips[Random.Range(0, music.dungeonMusicClips.Count)]);
    }

    public void LampRoll()
    {
        int currentX = 100;
        int currentY = 100;
        int currentZ = 100;

        if (!lampSwitches[0].switchOn)
        {
            foreach (GameObject lamp in lamp0List)
            {
                if (lamp.activeSelf)
                {
                    currentX = lamp0List.IndexOf(lamp);
                    lamp.SetActive(false);
                }
            }
        }

        if (!lampSwitches[1].switchOn)
        {
            foreach (GameObject lamp in lamp1List)
            {
                if (lamp.activeSelf)
                {
                    currentY = lamp1List.IndexOf(lamp);
                    lamp.SetActive(false);
                }
            }
        }

        if (!lampSwitches[2].switchOn)
        {
            foreach (GameObject lamp in lamp2List)
            {
                if (lamp.activeSelf)
                {
                    currentZ = lamp2List.IndexOf(lamp);
                    lamp.SetActive(false);
                }
            }
        }

        int x = Random.Range(0, lamp0List.Count);
        if (x == currentX)
        {
            x++;
            if (x == lamp0List.Count)
            {
                x = 0;
            }
        }
        if (lampSwitches[0].switchOn)
        {
            foreach (GameObject lamp in lamp0List)
            {
                if (lamp.activeSelf)
                {
                    x = lamp0List.IndexOf(lamp);
                    break;
                }
            }
        }

        int y = Random.Range(0, lamp1List.Count);
        if (y == currentY)
        {
            y++;
            if (y == lamp1List.Count)
            {
                y = 0;
            }
        }
        if (lampSwitches[1].switchOn)
        {
            foreach (GameObject lamp in lamp1List)
            {
                if (lamp.activeSelf)
                {
                    y = lamp1List.IndexOf(lamp);
                    break;
                }
            }
        }

        int z = Random.Range(0, lamp2List.Count);
        if (z == currentZ)
        {
            z++;
            if (z == lamp2List.Count)
            {
                z = 0;
            }
        }
        if (lampSwitches[2].switchOn)
        {
            foreach (GameObject lamp in lamp2List)
            {
                if (lamp.activeSelf)
                {
                    z = lamp2List.IndexOf(lamp);
                    break;
                }
            }
        }

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
        StartCoroutine(SummonKnightTimer());
    }

    public void SummonGreen()
    {
        StartCoroutine(SummonGobTimer());
    }

    public void SummonWhite()
    {
        StartCoroutine(ChaosOrbTimer());
    }

}
