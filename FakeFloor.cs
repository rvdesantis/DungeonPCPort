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

    public void Fall()
    {  
        if (trapCube.trapType == TrapHallCube.TrapType.empty)
        {
            StandardFall();
        }
        if (trapCube.trapType == TrapHallCube.TrapType.enemy)
        {
            List<DunModel> pitList = new List<DunModel>();
            MonsterController monsters = FindObjectOfType<MonsterController>();

            foreach (DunModel monster in monsters.enemyMasterList)
            {
                if (monster.spawnArea == DunModel.SpawnArea.fallRoom)
                {
                    pitList.Add(monster);
                }
            }

            int listPick = Random.Range(0, pitList.Count);
            DunModel selectedMonster = pitList[listPick];

            int monsterNum = selectedMonster.spawnPlayableInt;
           
            MonsterFall(monsterNum);
        }
        if (trapCube.trapType == TrapHallCube.TrapType.other)
        {
            int mystNum = Random.Range(0, trapCube.otherObjects.Count);
            OtherFall(mystNum);
        }
    }
    public void StandardFall()
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();
        DistanceController distance = FindAnyObjectByType<DistanceController>();

        if (fallRoom != null)
        {
            SceneController sceneController = FindObjectOfType<SceneController>();
            fallRoom.exitPortal.sceneController = sceneController;
            fallRoom.returnPortal.sceneController = sceneController;
            distance.portals.Add(fallRoom.exitPortal);

        }
        StartCoroutine(StandardFallTimer(player));
    }

    public void MonsterFall(int monsterNum)
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();
        DistanceController distance = FindAnyObjectByType<DistanceController>();
        if (fallRoom != null)
        {
            SceneController sceneController = FindObjectOfType<SceneController>();
            fallRoom.exitPortal.sceneController = sceneController;
            fallRoom.returnPortal.sceneController = sceneController;
            distance.portals.Add(fallRoom.exitPortal);
        }
        
        StartCoroutine(MonsterFallTimer(player, monsterNum));
    }

    public void OtherFall(int mystNum)
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();
        DistanceController distance = FindAnyObjectByType<DistanceController>();
        if (fallRoom != null)
        {
            SceneController sceneController = FindObjectOfType<SceneController>();
            fallRoom.exitPortal.sceneController = sceneController;
            fallRoom.returnPortal.sceneController = sceneController;
            distance.portals.Add(fallRoom.exitPortal);
        }
        StartCoroutine(MysteryFallTimer(player, mystNum));
    }

    private IEnumerator MysteryFallTimer(PlayerController player, int mysteryNumber)
    {
        PartyController party = FindObjectOfType<PartyController>();
        PlayableDirector recoverDir = trapCube.landingDirectors[0];
        DunUIController uiController = FindObjectOfType<DunUIController>();

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
        PartyController party = FindObjectOfType<PartyController>();
        DunUIController uiController = FindObjectOfType<DunUIController>();

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
            recoverDir.Play();

            yield return new WaitForSeconds((float)recoverDir.duration);
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

            // activate mystery scriptable object, Start() activates.
            trapCube.otherObjects[0].SetActive(true);


            yield return new WaitForSeconds(.1f);

            player.transform.position = trapCube.fallRoomSpawnPoint.transform.position;
            player.transform.rotation = trapCube.fallRoomSpawnPoint.transform.rotation;
            player.controller.enabled = true;
            player.gravity = gravity;
            player.playerLight.enabled = true;
            player.cinPersonCam.m_Priority = 5;
            uiController.compassObj.SetActive(true);
        }        
    }

    private IEnumerator StandardFallTimer(PlayerController player)
    {
        PartyController party = FindObjectOfType<PartyController>();
        PlayableDirector recoverDir = trapCube.landingDirectors[0];
        DunUIController uiController = FindObjectOfType<DunUIController>();
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

        yield return new WaitForSeconds((float)recoverDir.duration);
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
        player.gravity = gravX;
        player.playerLight.enabled = true;
        player.cinPersonCam.m_Priority = 5;
        uiController.compassObj.SetActive(true);
    }

    private IEnumerator MonsterFallTimer(PlayerController player, int monsterNum)
    {
        PartyController party = FindObjectOfType<PartyController>();
        PlayableDirector monsterDir = trapCube.monsterDirectors[monsterNum];
        MonsterController monsters = FindAnyObjectByType<MonsterController>();
        DunUIController uiController = FindObjectOfType<DunUIController>();

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

        DunModel activeMonster = null;

        if (monsterNum == 0)
        {
            DunModel boneDragon = Instantiate(monsters.enemyMasterList[1], monsterDir.transform, false);
            boneDragon.gameObject.SetActive(false);
            boneDragon.transform.position = monsterDir.transform.position;
            boneDragon.AssignToDirector(monsterDir, 4);

            activeMonster = boneDragon;
        }
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
            activeModel.gameObject.SetActive(true);
        }

        activeMonster.gameObject.SetActive(true);
        monsterDir.Play();

        yield return new WaitForSeconds((float)monsterDir.duration);
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
        player.transform.position = trapCube.fallRoomSpawnPoint.transform.position;
        player.transform.rotation = trapCube.fallRoomSpawnPoint.transform.rotation;
        player.controller.enabled = true;
        player.gravity = gravX;
        player.playerLight.enabled = true;
        player.cinPersonCam.m_Priority = 5;
        uiController.compassObj.SetActive(true);
    }


}
