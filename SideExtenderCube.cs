using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using static Unity.Burst.Intrinsics.X86;

public class SideExtenderCube : Cube
{
    public enum SideType { enemy, chest, NPC, portal}
    public SideType sideType;
    public GameObject triggerSpot;
    public bool triggered;
    public FakeWall lFakeWall;
    public FakeWall rFakeWall;
    public GameObject lSmall;
    public GameObject rSmall;

    public DunPortal lPortal;
    public DunPortal rPortal;
    public DunChest lChest;
    public DunChest rChest;
    // monsters with side hall type, indexed as: even number - left enemy, odd number - right enemy.  
    public DunModel lMonster;
    public DunModel rMonster;
    public List<PlayableDirector> monsterPlayables;
    public DunModel activeEnemy;


    public void SetWalls(bool left = true)
    {
        DistanceController distanceC = FindObjectOfType<DistanceController>();
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
        List<GameObject> spawnableList = new List<GameObject>();
        GameObject enemyObject = null;
        DistanceController distance = FindObjectOfType<DistanceController>(); 
        MonsterController monsters = FindObjectOfType<MonsterController>();
        int monsterCount = 0;


        if (left)
        {
            // add Enemy chance
            foreach (DunModel enemy in monsters.enemyMasterList)
            {
                if (enemy.spawnArea == DunModel.SpawnArea.sideHall)
                {
                    monsterCount++;
                }
            }
            if (monsterCount > 0)
            {
                spawnableList.Add(enemyObject);
            }

            if (lChest != null)
            {
                spawnableList.Add(lChest.gameObject);
            }
            // add NPC chance
            if (lPortal != null)
            {
                spawnableList.Add(lPortal.gameObject);
            }
            int objectPick = Random.Range(0, spawnableList.Count);  
            if (spawnableList[objectPick] == lChest.gameObject)
            {
                sideType = SideType.chest;
                distance.chests.Add(lChest);
                lChest.gameObject.SetActive(true);
            }
            if (spawnableList[objectPick] == lPortal.gameObject)
            {
                sideType = SideType.portal;          
            }
            // add NPC Spawn

            if (spawnableList[objectPick] == enemyObject)
            {
            
                List<DunModel> availableEnemies = new List<DunModel>();

                foreach (DunModel enemy in monsters.enemyMasterList)
                {
                    if (enemy.spawnArea == DunModel.SpawnArea.sideHall)
                    {
                        if (enemy.spawnPlayableInt % 2 == 0) // adds even numbered only
                        {
                            availableEnemies.Add(enemy);
                        }
                    }
                }
                if (availableEnemies.Count > 0)
                {
                    lMonster = availableEnemies[Random.Range(0, availableEnemies.Count)];
                    activeEnemy = lMonster;
                    sideType = SideType.enemy;
                }
            }
        }
        else
        {
            foreach (DunModel enemy in monsters.enemyMasterList)
            {
                if (enemy.spawnArea == DunModel.SpawnArea.sideHall)
                {
                    monsterCount++;
                }
            }
            if (monsterCount > 0)
            {
                spawnableList.Add(enemyObject);
            }

            if (rChest != null)
            {
                spawnableList.Add(rChest.gameObject);
            }
            // add NPC chance
            if (rPortal != null)
            {
                spawnableList.Add(rPortal.gameObject);
            }
            int objectPick = Random.Range(0, spawnableList.Count);
            if (spawnableList[objectPick] == rChest.gameObject)
            {
                sideType = SideType.chest;
                distance.chests.Add(rChest);
                rChest.gameObject.SetActive(true);
            }
            if (spawnableList[objectPick] == rPortal.gameObject)
            {
                sideType = SideType.portal;
              
            }
            // add NPC Spawn
            if (spawnableList[objectPick] == enemyObject)
            {                
                List<DunModel> availableEnemies = new List<DunModel>();
                foreach (DunModel enemy in monsters.enemyMasterList)
                {
                    if (enemy.spawnArea == DunModel.SpawnArea.sideHall)
                    {
                        if (enemy.spawnPlayableInt % 2 != 0) // adds odd numbered only
                        {
                            availableEnemies.Add(enemy);
                        }
                    }
                }
                if (availableEnemies.Count > 0)
                {
                    rMonster = availableEnemies[Random.Range(0, availableEnemies.Count)];
                    activeEnemy = rMonster;
                    sideType = SideType.enemy;
                }
            }
        }
    }

    public void SetPortal(DunPortal portal)
    {
        portal.closeOnJump = true;
        portal.gameObject.SetActive(true);

        List<DunPortal> availablePort = new List<DunPortal>();
        SceneController controller = FindObjectOfType<SceneController>();
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
        portal.ConnectPortals(availablePort[Random.Range(0, availablePort.Count)]);

        if (portal.connectedPortal == sact.eventCubes.enterPortalSMRoom) // sets Event Room if applicable
        {
            sact.eventCubes.SetTreasureRoom(portal, true);            
        }
        portal.connectedPortal.gameObject.SetActive(true);
    }

    public void TriggerEnemy()
    {
        bool left = lSmall.gameObject.activeSelf;
        triggered = true;

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
                rFakeWall.wallBroken = true;
                StartCoroutine(MonsterEventTimer(rMonster, monsterDir, true));
            }
        }
    }

    IEnumerator MonsterEventTimer(DunModel monster, PlayableDirector monsterDir, bool torch = false)
    {
        PartyController party = FindObjectOfType<PartyController>();
        PlayerController player = FindObjectOfType<PlayerController>();
        DunUIController uiController = FindObjectOfType<DunUIController>();
        SceneController controller = FindObjectOfType<SceneController>();
  
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
        monsterDir.Play();

        yield return new WaitForSeconds((float)monsterDir.duration);
        if (controller.activePlayable == monsterDir)
        {
            EndMonster();
        }
    }

    public void EndMonster()
    {
        PartyController party = FindObjectOfType<PartyController>();
        PlayerController player = FindObjectOfType<PlayerController>();
        DunUIController uiController = FindObjectOfType<DunUIController>();
        SceneController controller = FindObjectOfType<SceneController>();

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

        player.controller.enabled = true;
        player.playerLight.enabled = true;
        player.cinPersonCam.m_Priority = 5;
        uiController.compassObj.SetActive(true);
    }

}
