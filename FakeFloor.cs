using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


public class FakeFloor : MonoBehaviour
{
    public bool inRange;
    public bool floorBreak;
    public bool floorBreakTest;
    public PlayableDirector standardBreak;
    public GameObject repairFloor;

    public TrapHallCube trapCube;
    public FallRoom fallRoom;

    public GameObject front;
    public GameObject back;

    public DunModel activeEnemy;
    public List<GameObject> activationList;

    public int launchNum;


    public Transform spawnPoint;

    public void StartBreak(bool back)
    {
        StatsTracker stats = FindAnyObjectByType<StatsTracker>();
        stats.trapsTotal++;
        if (back)
        {
            fallRoom.returnPortal.transportPosition = front;
            trapCube.transform.Rotate(0, 180, 0);
        }
        Fall();
    }

    public int RandomMonsterNumber()
    {
        int randomNum = Random.Range(0, trapCube.monsterDirectors.Count);

        int monsterNum = randomNum;
        return monsterNum;
    }

    public void Fall()
    {
        // measure distance and flip if walking up from behind.  
        if (trapCube.trapType == TrapHallCube.TrapType.empty)
        {
            StandardFall();
        }
        if (trapCube.trapType == TrapHallCube.TrapType.enemy)
        {            
            int monsterNum = RandomMonsterNumber();
            MonsterFall(monsterNum);
        }
        if (trapCube.trapType == TrapHallCube.TrapType.other)
        {
            int mystNum = Random.Range(0, trapCube.otherObjects.Count);
            if (mystNum == 0)
            {
                PortalTrap portalTrap = trapCube.otherObjects[0].GetComponent<PortalTrap>();

                portalTrap.AssignPortals();
            }
            OtherFall(mystNum);
        }
    }
    public void EndFall()
    {
        Debug.Log("ending fall");
        SceneController controller = FindAnyObjectByType<SceneController>();
        DunUIController uiController = FindAnyObjectByType<DunUIController>();
        PartyController party = FindAnyObjectByType<PartyController>();
        PlayerController player = controller.playerController;

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

        if (activeEnemy != null)
        {
            activeEnemy.gameObject.SetActive(false);
        }
        if (activationList.Count > 0)
        {
            foreach (GameObject obj in activationList)
            {
                obj.SetActive(true);
            }
        }

        foreach (PlayableDirector fallPlayable in trapCube.landingDirectors)
        {
            if (fallPlayable.state == PlayState.Playing)
            {
                fallPlayable.time = fallPlayable.duration;
                break;
            }
        }


        player.transform.position = trapCube.fallRoomSpawnPoint.transform.position;
        player.transform.rotation = trapCube.fallRoomSpawnPoint.transform.rotation;
        player.controller.enabled = true;
        player.enabled = true;
        player.gravity = 15;
        player.playerLight.enabled = true;
        player.cinPersonCam.m_Priority = 5;

        if (!fallRoom.exitPortal.gameObject.activeSelf)
        {
            Debug.Log("Error - Exit Portal Manually Opened (GameObject)", fallRoom.exitPortal.gameObject);
            fallRoom.exitPortal.gameObject.SetActive(true);
        }

        uiController.compassObj.SetActive(true);
    }

    public void MonsterEndFall()
    {
        PartyController party = FindAnyObjectByType<PartyController>();
        SceneController controller = FindAnyObjectByType<SceneController>();
        PlayerController player = FindAnyObjectByType<PlayerController>();
        BattleController battleC = FindAnyObjectByType<BattleController>();


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
        controller.activePlayable = null;
        controller.endAction = null;

        player.transform.position = trapCube.fallRoomSpawnPoint.transform.position;
        player.transform.rotation = trapCube.fallRoomSpawnPoint.transform.rotation;
   
        battleC.afterBattleAction = PitBattleReturn;
        battleC.SetBattle(launchNum);       
    }
    public void StandardFall()
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();
        DistanceController distance = FindAnyObjectByType<DistanceController>();
        SceneController sceneController = FindAnyObjectByType<SceneController>();
        if (fallRoom != null)
        {            
            fallRoom.exitPortal.sceneController = sceneController;
            fallRoom.returnPortal.sceneController = sceneController;
            distance.portals.Add(fallRoom.exitPortal);
            fallRoom.exitPortal.gameObject.SetActive(true);

        }
        StartCoroutine(StandardFallTimer(player));
    }
    public void MonsterFall(int monsterNum)
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();
        DistanceController distance = FindAnyObjectByType<DistanceController>();
        if (fallRoom != null)
        {
            SceneController sceneController = FindAnyObjectByType<SceneController>();
            fallRoom.exitPortal.sceneController = sceneController;
            fallRoom.returnPortal.sceneController = sceneController;
            distance.portals.Add(fallRoom.exitPortal);
            fallRoom.exitPortal.gameObject.SetActive(true);
        }
        
        StartCoroutine(MonsterFallTimer(player, monsterNum));
    }
    public void OtherFall(int mystNum)
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();
        DistanceController distance = FindAnyObjectByType<DistanceController>();
        SceneController sceneController = FindAnyObjectByType<SceneController>();
        if (fallRoom != null)
        {

            fallRoom.exitPortal.sceneController = sceneController;
            fallRoom.returnPortal.sceneController = sceneController;
            distance.portals.Add(fallRoom.exitPortal);
            fallRoom.exitPortal.gameObject.SetActive(true);
        }
        StartCoroutine(MysteryFallTimer(player, mystNum));
    }
    private IEnumerator MysteryFallTimer(PlayerController player, int mysteryNumber)
    {
        PartyController party = FindAnyObjectByType<PartyController>();
        PlayableDirector recoverDir = trapCube.landingDirectors[0];
        DunUIController uiController = FindAnyObjectByType<DunUIController>();
        float gravX = player.gravity;
        player.gravity = 0;
        player.controller.enabled = false;
        player.transform.position = repairFloor.transform.position;
        player.cinPersonCam.m_Priority = -1;

        
        uiController.compassObj.SetActive(false);
        party.AssignCamBrain(standardBreak);
        foreach (DunModel activeModel in party.activeParty)
        {
            activeModel.AssignToDirector(recoverDir);
        }
        party.AssignCamBrain(recoverDir, 3);

        standardBreak.Play();
        yield return new WaitForSeconds((float)standardBreak.duration / 2);
        player.playerLight.enabled = false;
        yield return new WaitForSeconds((float)standardBreak.duration / 2);
        repairFloor.SetActive(true);

        fallRoom.returnPortal.gameObject.SetActive(true);
        fallRoom.exitPortal.gameObject.SetActive(true);

        // standard fall End.  

        StartCoroutine(MysterActivation(player, mysteryNumber, gravX));

    }
    private IEnumerator MysterActivation(PlayerController player, int mysteryNumber, float gravity)
    {
        PartyController party = FindAnyObjectByType<PartyController>();
        DunUIController uiController = FindAnyObjectByType<DunUIController>();
        SceneController controller = FindAnyObjectByType<SceneController>();

        PlayableDirector recoverDir = null;
        if (mysteryNumber == 0)
        {
            recoverDir = trapCube.landingDirectors[0];
            foreach (DunModel activeModel in party.activeParty)
            {
                activeModel.transform.position = trapCube.transform.position;
                activeModel.transform.rotation = trapCube.transform.rotation;
                activeModel.transform.parent = trapCube.transform;
                if (activeModel.activeWeapon != null)
                {
                    activeModel.activeWeapon.SetActive(false);
                }
                if (party.activeParty.IndexOf(activeModel) == 0)
                {
                    activeModel.torch.SetActive(true);
                }
                activeModel.gameObject.SetActive(true);
            }
        
            }
        if (recoverDir != null)
        {
            controller.activePlayable = recoverDir;
            controller.endAction += EndFall;

            activationList.Clear();
            activationList.Add(trapCube.otherObjects[mysteryNumber]);
            recoverDir.Play();

            yield return new WaitForSeconds((float)recoverDir.duration);
            if (controller.activePlayable == recoverDir)
            {
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
                player.transform.position = trapCube.fallRoomSpawnPoint.transform.position;
                player.transform.rotation = trapCube.fallRoomSpawnPoint.transform.rotation;
                player.controller.enabled = true;
                player.gravity = gravity;
                player.playerLight.enabled = true;
                player.cinPersonCam.m_Priority = 5;
                trapCube.otherObjects[0].SetActive(true);
                uiController.compassObj.SetActive(true);
            }


            controller.activePlayable = null;
            controller.endAction = null;
            MusicController music = FindAnyObjectByType<MusicController>();
            music.CrossfadeToNextClip(music.dungeonMusicClips[Random.Range(0, music.dungeonMusicClips.Count)]);
        }        
    }
    private IEnumerator StandardFallTimer(PlayerController player)
    {
        PartyController party = FindAnyObjectByType<PartyController>();
        PlayableDirector recoverDir = trapCube.landingDirectors[0];
        SceneController controller = FindAnyObjectByType<SceneController>();
        DunUIController uiController = FindAnyObjectByType<DunUIController>();

        float gravX = player.gravity;
        player.gravity = 0;
        player.controller.enabled = false;

        Vector3 newPosition = repairFloor.transform.position;
     

        player.transform.SetPositionAndRotation(newPosition, front.transform.rotation);

        player.cinPersonCam.m_Priority = -1;
        uiController.compassObj.SetActive(false);

        party.AssignCamBrain(standardBreak);
        foreach (DunModel activeModel in party.activeParty)
        {
            activeModel.AssignToDirector(recoverDir); 
        }
        party.AssignCamBrain(recoverDir, 3);

        standardBreak.Play();
        yield return new WaitForSeconds((float)standardBreak.duration / 2);
        player.playerLight.enabled = false;
        yield return new WaitForSeconds((float)standardBreak.duration / 2);
        repairFloor.SetActive(true);

        fallRoom.returnPortal.gameObject.SetActive(true);
        fallRoom.exitPortal.gameObject.SetActive(true);
        foreach (DunModel activeModel in party.activeParty)
        {            
            activeModel.transform.position = trapCube.transform.position;
            activeModel.transform.rotation = trapCube.transform.rotation;
            activeModel.transform.parent = trapCube.transform;
            if (activeModel.activeWeapon != null)
            {
                activeModel.activeWeapon.SetActive(false);
            }
            if (party.activeParty.IndexOf(activeModel) == 0)
            {
                activeModel.torch.SetActive(true);
            }
            activeModel.gameObject.SetActive(true);
        }

        recoverDir.Play();
        controller.activePlayable = recoverDir;
        controller.endAction += EndFall;

        yield return new WaitForSeconds((float)recoverDir.duration);
        if (controller.activePlayable == recoverDir)
        {
            EndFall();
        }
        MusicController music = FindAnyObjectByType<MusicController>();
        music.CrossfadeToNextClip(music.dungeonMusicClips[Random.Range(0, music.dungeonMusicClips.Count)]);
    }
    private IEnumerator MonsterFallTimer(PlayerController player, int monsterNum)
    {
        PartyController party = FindAnyObjectByType<PartyController>();
        PlayableDirector monsterDir = trapCube.monsterDirectors[monsterNum];
        MonsterController monsters = FindAnyObjectByType<MonsterController>();
        SceneController controller = FindAnyObjectByType<SceneController>();
        DunUIController uiController = FindAnyObjectByType<DunUIController>();
        BattleController battleC = FindAnyObjectByType<BattleController>();

        float gravX = player.gravity;
        player.gravity = 0;
        player.controller.enabled = false;
        player.transform.position = repairFloor.transform.position;
        player.cinPersonCam.m_Priority = -1;
        uiController.compassObj.SetActive(false);
        party.AssignCamBrain(standardBreak);
        foreach (DunModel activeModel in party.activeParty)
        {
            activeModel.AssignToDirector(monsterDir);
        }
        party.AssignCamBrain(monsterDir, 3);

       
        standardBreak.Play();
        yield return new WaitForSeconds((float)standardBreak.duration / 2);
        player.playerLight.enabled = false;

        DunModel activeMonster = null;
        List<DunModel> spawnedMonsters = new List<DunModel>();

        if (monsterNum == 0)
        {
            DunModel boneDragon = Instantiate(monsters.enemyMasterList[1], monsterDir.transform, false);
            boneDragon.transform.position = monsterDir.transform.position;
            boneDragon.AssignToDirector(monsterDir, 4);

            activeMonster = boneDragon;
            spawnedMonsters.Add(boneDragon);
            launchNum = 1;
        }
        if (monsterNum == 1)
        {
            DunModel pitImp = Instantiate(monsters.enemyMasterList[26], monsterDir.transform, false);
            pitImp.transform.position = monsterDir.transform.position;
            pitImp.AssignToDirector(monsterDir, 4);

            DunModel pitImp2 = Instantiate(monsters.enemyMasterList[26], monsterDir.transform, false);
            pitImp2.transform.position = monsterDir.transform.position;
            pitImp2.AssignToDirector(monsterDir, 5);

            DunModel pitImp3 = Instantiate(monsters.enemyMasterList[26], monsterDir.transform, false);
            pitImp3.transform.position = monsterDir.transform.position;
            pitImp3.AssignToDirector(monsterDir, 6);

            activeMonster = pitImp;
            spawnedMonsters.Add(pitImp);
            spawnedMonsters.Add(pitImp2);
            spawnedMonsters.Add(pitImp3);

            launchNum = 26;
        }

        yield return new WaitForSeconds((float)standardBreak.duration / 2);
        repairFloor.SetActive(true);

        fallRoom.returnPortal.gameObject.SetActive(true);
        fallRoom.exitPortal.gameObject.SetActive(true);
        foreach (DunModel activeModel in party.activeParty)
        {
            activeModel.transform.position = trapCube.transform.position;
            activeModel.transform.rotation = trapCube.transform.rotation;
            activeModel.transform.parent = trapCube.transform;
            if (activeModel.activeWeapon != null)
            {
                activeModel.activeWeapon.SetActive(false);
            }
            activeModel.gameObject.SetActive(true);
        }

        activeMonster.gameObject.SetActive(true);
        activeEnemy = activeMonster;

        controller.activePlayable = monsterDir;
        controller.endAction += MonsterEndFall;

        monsterDir.Play();

        yield return new WaitForSeconds((float)monsterDir.duration);

        if (controller.activePlayable == monsterDir)
        {
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
            activeMonster.gameObject.SetActive(false);
            foreach (DunModel mon in spawnedMonsters)
            {
                mon.gameObject.SetActive(false);
                Destroy(mon.gameObject);
            }

            controller.activePlayable = null;
            controller.endAction = null;

            player.transform.position = trapCube.fallRoomSpawnPoint.transform.position;
            player.transform.rotation = trapCube.fallRoomSpawnPoint.transform.rotation;
            battleC.afterBattleAction = PitBattleReturn;
            battleC.SetBattle(launchNum);
        }
    }

    public void PitBattleReturn()
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();
        DunUIController uiController = FindAnyObjectByType<DunUIController>();

        player.controller.enabled = true;
        player.gravity = 9.5f;
        player.playerLight.enabled = true;
        player.cinPersonCam.m_Priority = 5;
        uiController.compassObj.SetActive(true);
    }
}
