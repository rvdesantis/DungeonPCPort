using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;


public class SideExtenderCube : Cube
{
    public enum SideType { enemy, chest, NPC, portal, trap}
    public SideType sideType;
    public HallStarterCube starter;
    public GameObject triggerSpot;
    public bool triggered;
    public FakeWall lFakeWall;
    public FakeWall rFakeWall;
    public GameObject lSmall;
    public GameObject rSmall;
    public GameObject lSmallSpawn;
    public GameObject rSmallSpawn;
    public DunPortal lPortal;
    public DunPortal rPortal;
    public DunChest lChest;
    public DunChest rChest;
    // monsters with side hall type, indexed as: even number - left enemy, odd number - right enemy.  
    public DunModel lMonster;
    public DunModel rMonster;
    public List<PlayableDirector> monsterPlayables;
    public DunModel activeEnemy;
    public int monsterNum;
    public int trapNum;
    public List<DunTrap> traps;
    public DunTrap activeTrap;



    public void SetWalls(bool left = true)
    {
        DistanceController distanceC = FindAnyObjectByType<DistanceController>();
        if (left)
        {
            foreach(GameObject wall in lWalls)
            {
                wall.SetActive(false);
            }
            lFakeWall.gameObject.SetActive(true);
            lSmall.SetActive(true);
            distanceC.fakeWalls.Add(lFakeWall);
            FillCube(true);
        }
        else
        {
            foreach (GameObject wall in rWalls)
            {
                wall.SetActive(false);
            }
            rFakeWall.gameObject.SetActive(true);
            rSmall.SetActive(true);
            distanceC.fakeWalls.Add(rFakeWall);
            FillCube(false);
        }
    }
    public void FillCube(bool left)
    {
        DistanceController distance = FindAnyObjectByType<DistanceController>(); 
        MonsterController monsters = FindAnyObjectByType<MonsterController>();
        int fillTypeNum = Random.Range(0, 4); // 0-enemy, 1-chest, 2-NPC, 3-portal, 4-trap
        if (left)
        {
            if (fillTypeNum == 0)
            {
                lFakeWall.locked = true;
                lFakeWall.hideType = FakeWall.HideType.monster;
                List<DunModel> availableEnemies = new List<DunModel>();
                // add Minotaur chance
                availableEnemies.Add(monsters.enemyMasterList[4]);

                if (availableEnemies.Count > 0)
                {
                    lMonster = availableEnemies[Random.Range(0, availableEnemies.Count)];
                    activeEnemy = lMonster;
                    sideType = SideType.enemy;
                }
            }

            if (fillTypeNum == 1) // chest
            {
                sideType = SideType.chest;
                lFakeWall.hideType = FakeWall.HideType.treasure;
                distance.chests.Add(lChest);
                lChest.gameObject.SetActive(true);
                Debug.Log("Added Left Chest to Side Extender, Index " + distance.chests.IndexOf(lChest) + " in DistanceController");
            }
            if (fillTypeNum == 2) 
            {
                NPCController npcC = FindAnyObjectByType<NPCController>();
                sideType = SideType.NPC;
                List<DunNPC> dunNPCs = new List<DunNPC>();
                dunNPCs.Add(npcC.npcMasterList[0].GetComponent<DunNPC>()); // Map Vendor
                dunNPCs.Add(npcC.npcMasterList[2].GetComponent<DunNPC>()); // BattleShop
                dunNPCs.Add(npcC.npcMasterList[3].GetComponent<DunNPC>()); // Necro 

                DunNPC targetNPC = dunNPCs[Random.Range(0, dunNPCs.Count)];

                DunNPC spawnedNPC = Instantiate(targetNPC, lSmallSpawn.transform, false);
                distance.npcS.Add(spawnedNPC);
                Debug.Log("Added " + spawnedNPC.name + " to Side Extender, Index " + distance.npcS.IndexOf(spawnedNPC) + " in DistanceController");

            }
            if (fillTypeNum == 3)
            {
                sideType = SideType.portal;
                lFakeWall.hideType = FakeWall.HideType.treasure;
            }
            // Traps are handled separately
        }
        else
        {
            if (fillTypeNum == 0)
            {
                rFakeWall.locked = true;
                rFakeWall.hideType = FakeWall.HideType.monster;
                List<DunModel> availableEnemies = new List<DunModel>();
                // add Jelly chance
                availableEnemies.Add(monsters.enemyMasterList[5]);

                if (availableEnemies.Count > 0)
                {
                    rMonster = availableEnemies[Random.Range(0, availableEnemies.Count)];
                    activeEnemy = rMonster;
                    sideType = SideType.enemy;
                }
            }

            if (fillTypeNum == 1) // chest
            {
                sideType = SideType.chest;
                rFakeWall.hideType = FakeWall.HideType.treasure;
                distance.chests.Add(rChest);
                rChest.gameObject.SetActive(true);
                Debug.Log("Added Right Chest to Side Extender, Index " + distance.chests.IndexOf(rChest) + " in DistanceController");
            }
            if (fillTypeNum == 2) 
            {
                NPCController npcC = FindAnyObjectByType<NPCController>();
                sideType = SideType.NPC;
                List<DunNPC> dunNPCs = new List<DunNPC>();
                dunNPCs.Add(npcC.npcMasterList[0].GetComponent<DunNPC>()); // Map Vendor
                dunNPCs.Add(npcC.npcMasterList[2].GetComponent<DunNPC>()); // BattleShop
                dunNPCs.Add(npcC.npcMasterList[3].GetComponent<DunNPC>()); // Necro 

                DunNPC targetNPC = dunNPCs[Random.Range(0, dunNPCs.Count)];

                DunNPC spawnedNPC = Instantiate(targetNPC, rSmallSpawn.transform, false);
                distance.npcS.Add(spawnedNPC);
                Debug.Log("Added " + spawnedNPC.name + " to Side Extender, Index " + distance.npcS.IndexOf(spawnedNPC) + " in DistanceController");
            }
            if (fillTypeNum == 3)
            {
                sideType = SideType.portal;
                rFakeWall.hideType = FakeWall.HideType.treasure;
            }
            // Traps are handled separately
        }
    }

    public void SetPortal(DunPortal portal)
    {
        portal.closeOnJump = true;
        portal.gameObject.SetActive(true);

        List<DunPortal> availablePort = new List<DunPortal>();
        SceneController controller = FindAnyObjectByType<SceneController>();
        SanctuaryCube sact = controller.sanctuary.GetComponent<SanctuaryCube>();

        availablePort.Add(sact.returnPortal);
        if (!sact.eventCubes.treasureChest.opened)
        {
            availablePort.Add(sact.eventCubes.enterPortalSMRoom);
        }
        foreach (HallStarterCube starter in controller.builder.createdStarters)
        {
            if (starter.hallType == HallStarterCube.HallType.boss)
            {
                DunPortal bossPortal = starter.generatedHallway[1].GetComponent<BossHallCube>().bossPortal.GetComponent<DunPortal>();
                availablePort.Add(bossPortal);
                break;
            }
        }
        foreach (Cube endCube in controller.builder.createdSecretEnds)
        {
            HiddenEndCube hidden = endCube.GetComponent<HiddenEndCube>();
            if (hidden != null)
            {
                if (hidden.secretPortal != null)
                {
                    availablePort.Add(hidden.secretPortal);
                }
            }
        }
        int portNum = Random.Range(0, availablePort.Count);
        if (availablePort[portNum] == sact.eventCubes.enterPortalSMRoom) // sets Event Room if applicable
        {
            sact.eventCubes.SetTreasureRoom(portal, true);            
        }
        portal.ConnectPortals(availablePort[portNum]);
        portal.connectedPortal.gameObject.SetActive(true);
    }

    public void TriggerEnemy()
    {
        MonsterController monsters = FindAnyObjectByType<MonsterController>();
        StatsTracker stats = FindAnyObjectByType<StatsTracker>();
        bool left = lSmall.gameObject.activeSelf;
        triggered = true;
        stats.trapsTotal++;
        if (left)
        {
            if (lMonster == null)
            {
                Debug.Log("Error, Left Enemy not Assigned to Hidden Cube, not Triggered");
                return;
            }
            if (lMonster != null)
            {    
                PlayableDirector monsterDir = monsterPlayables[lMonster.spawnPlayableInt];
                monsterNum = monsters.enemyMasterList.IndexOf(lMonster);
                lFakeWall.wallBroken = true;          
                StartCoroutine(MonsterEventTimer(lMonster, monsterDir, true));
            }
        }
        else
        {
            if (rMonster == null)
            {
                Debug.Log("Error, Left Enemy not Assigned to Hidden Cube, not Triggered");
                return;
            }
            if (rMonster != null)
            {
                PlayableDirector monsterDir = monsterPlayables[rMonster.spawnPlayableInt];
                monsterNum = monsters.enemyMasterList.IndexOf(rMonster);
                rFakeWall.wallBroken = true;      
                StartCoroutine(MonsterEventTimer(rMonster, monsterDir, true));
            }
        }
    }

    public void SetTrap()
    {
        if (traps.Count > 0)
        {
            trapNum = Random.Range(0, traps.Count);
            traps[trapNum].gameObject.SetActive(true);
            traps[trapNum].SetSideTrap(this);
        }
    }

    public void TriggerTrap()
    {

    }

    IEnumerator MonsterEventTimer(DunModel monster, PlayableDirector monsterDir, bool torch = false)
    {
        PartyController party = FindAnyObjectByType<PartyController>();
        PlayerController player = FindAnyObjectByType<PlayerController>();
        DunUIController uiController = FindAnyObjectByType<DunUIController>();
        SceneController controller = FindAnyObjectByType<SceneController>();
        BattleUIController battleUI = FindAnyObjectByType<BattleUIController>();
  
        player.controller.enabled = false;  
        player.cinPersonCam.m_Priority = -1;

        controller.activePlayable = monsterDir;
        controller.endAction += EndMonster;
        foreach (DunModel activeModel in party.activeParty)
        {
            activeModel.transform.parent = monsterDir.gameObject.transform;
            activeModel.AssignToDirector(monsterDir);
            activeModel.transform.position = monsterDir.transform.position;
            activeModel.transform.rotation = monsterDir.transform.rotation;
            activeModel.gameObject.SetActive(true);
            if (activeModel.activeWeapon != null)
            {
                activeModel.activeWeapon.SetActive(false);
            }
            if (torch)
            {
                if (party.activeParty.IndexOf(activeModel) == 0)
                {
                    if (activeModel.torch != null)
                    {
                        activeModel.torch.SetActive(true);
                    }
                }
            }
        }
        party.AssignCamBrain(monsterDir, 3);

        DunModel activeMonster = Instantiate(monster, monsterDir.transform, false);
        activeMonster.transform.position = monsterDir.transform.position;
        activeMonster.AssignToDirector(monsterDir, 4);
        activeMonster.gameObject.SetActive(true);
        activeEnemy = activeMonster;

        if (monsterDir == monsterPlayables[0])
        {
            if (party.activeParty[0].torch != null)
            {
                party.activeParty[0].torch.SetActive(true);
            }
        }

        uiController.compassObj.SetActive(false);
        // battleUI.AssignFadeImage(monsterDir);
        monsterDir.Play();

        yield return new WaitForSeconds((float)monsterDir.duration);
        if (controller.activePlayable == monsterDir)
        {
            EndMonster();
        }
    }

    public void EndMonster()
    {
        PartyController party = FindAnyObjectByType<PartyController>();
        PlayerController player = FindAnyObjectByType<PlayerController>();
        DunUIController uiController = FindAnyObjectByType<DunUIController>();
        SceneController controller = FindAnyObjectByType<SceneController>();        
        BattleController battleController = FindAnyObjectByType<BattleController>();
        battleController.SetBattle(monsterNum);
        foreach (PlayableDirector monPlayable in monsterPlayables)
        {
            if (monPlayable.state == PlayState.Playing)
            {              
                monPlayable.time = monPlayable.duration;
                monPlayable.Play();
            }
        }
        // does not skip to end of playable so that walls finish animating
        controller.activePlayable = null;
        controller.endAction = null;
        foreach (DunModel activeModel in party.activeParty)
        {
            activeModel.transform.parent = null;
            activeModel.gameObject.SetActive(false);
            if (activeModel.torch != null)
            {
                activeModel.torch.SetActive(false);
            }
            if (activeModel.activeWeapon != null)
            {
                activeModel.activeWeapon.SetActive(false);
            }
        }
        activeEnemy.gameObject.SetActive(false);
        player.controller.enabled = false;
        player.playerLight.enabled = true;
        player.cinPersonCam.m_Priority = -1;
    }

}
