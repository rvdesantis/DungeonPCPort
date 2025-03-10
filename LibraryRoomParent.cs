using Runemark.DarkFantasyKit.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using DTT.PlayerPrefsEnhanced;
using UnityEngine.InputSystem.XR;

public class LibraryRoomParent : RoomPropParent 
{
    public bool vMageUnlocked;
    public SkullNPC skull;
    public bool skulltest;
    public PlayableDirector templarIdlePlayable;
    public PlayableDirector vMageUnlockPlayable;
    public PlayableDirector templarGangIdle;
    public PlayableDirector vMageBattleStart;
    public bool hostile;
    public List<DunModel> templarModels;


    public override void EnvFill()
    {
        StartCoroutine(SetActives());
        AvailableWall();
        roomParent.activeENV = this;
        vMageUnlocked = EnhancedPrefs.GetPlayerPref("voidMageUnlock", false);
        if (!vMageUnlocked)
        {
            skull.gameObject.SetActive(true);
            if (distanceController == null)
            {
                distanceController = FindAnyObjectByType<DistanceController>();
            }
            distanceController.npcS.Add(skull);
            skull.player = FindObjectOfType<PlayerController>();
            skull.activeTarget = skull.player.transform;
            skull.eyeTracking = true;
            skull.roomParent = roomParent;
        }
 
        if (roomParent.roomType == CubeRoom.RoomType.NPC)
        {
            if (!vMageUnlocked)
            {
                NPCController npcController = FindObjectOfType<NPCController>();
                DistanceController distance = FindObjectOfType<DistanceController>();
                bool inDun = false;
                foreach (DunNPC npc in distance.npcS)
                {
                    if (npc.GetComponent<TemplarNPC>() != null)
                    {
                        inDun = true;
                        Debug.Log("Templar already in dungeon", gameObject);
                        break;
                    }
                }
                if (!inDun)
                {
                    DunModel target = npcController.npcMasterList[1];
                    DunModel templar = Instantiate(target, templarIdlePlayable.transform);
                    templar.AssignToDirector(templarIdlePlayable, 3);

                    TemplarNPC templarNPC = templar.GetComponent<TemplarNPC>();
                    templarNPC.idlePlayableLoop = templarIdlePlayable;
                    templarNPC.roomSkull = skull;
                    skull.activeTemplar = templarNPC;
                    skull.activeTarget = templar.transform;

                    distance.npcS.Add(templarNPC);
                    templarNPC.idlePlayableLoop.Play();
                    Debug.Log("Templar added to Library", gameObject);
                }
            }
            if (vMageUnlocked)
            {
                SummonTemplarGang();
                PartyController party = FindObjectOfType<PartyController>();
                bool inParty = false;
                foreach (DunModel partyMem in party.activeParty)
                {
                    if (partyMem == party.masterParty[5])
                    {
                        inParty = true;
                        break;
                    }
                }
                if (inParty)
                {
                    hostile = true;
                }
            }
        }
        if (roomParent.roomType == CubeRoom.RoomType.battle)
        {
            if (vMageUnlocked)
            {
                SummonTemplarGang();
                PartyController party = FindObjectOfType<PartyController>();
                bool inParty = false;

                foreach (DunModel partyMem in party.activeParty)
                {
                    if (partyMem == party.masterParty[5])
                    {
                        inParty = true;
                        break;
                    }
                }

                if (inParty)
                {
                    hostile = true;
                }
                
            }
        }
    }


    public void UnlockVoidMage()
    {
        StartCoroutine(VoidMageUnlock());
    }

    public void SummonTemplarGang()
    {
        MonsterController monsters = FindObjectOfType<MonsterController>();
        DunModel tempA = monsters.enemyMasterList[11]; //x2
        DunModel tempB = monsters.enemyMasterList[12];
      

        DunModel templar0 = Instantiate(tempA, templarGangIdle.transform);
        DunModel templar1 = Instantiate(tempA, templarGangIdle.transform);
        DunModel templar2 = Instantiate(tempB, templarGangIdle.transform);

        templar0.AssignToDirector(templarGangIdle, 3);
        templar1.AssignToDirector(templarGangIdle, 4);
        templar2.AssignToDirector(templarGangIdle, 5);

        templar0.AssignToDirector(vMageBattleStart, 4);
        templar1.AssignToDirector(vMageBattleStart, 5);
        templar2.AssignToDirector(vMageBattleStart, 6);

        templarModels.Add(templar0);
        templarModels.Add(templar1);
        templarModels.Add(templar2);

        TemplarDunModels tempNPC = templar2.GetComponent<TemplarDunModels>();
        if (distanceController == null)
        {
            distanceController = FindObjectOfType<DistanceController>();
        }
        distanceController.npcS.Add(tempNPC);
        tempNPC.attachedLibrary = this;
        tempNPC.templars.Add(templar0);
        tempNPC.templars.Add(templar1);        

        Debug.Log("Templar Gang Spawned in room(GameObject)", gameObject);
        templarGangIdle.Play();
        
    }

    public override void AfterBattle()
    {
        base.AfterBattle();
        PartyController party = FindObjectOfType<PartyController>();
        foreach (DunModel mod in party.activeParty)
        {
            mod.gameObject.SetActive(false);
        }
    }

    public void UnlockTemplarEnemies()
    {
        UnlockController unlock = FindObjectOfType<UnlockController>();
        PlayerController player = FindObjectOfType<PlayerController>();
        DunUIController uiController = FindObjectOfType<DunUIController>();
        unlock.unlockUI.nextUnlockAction = null; 
        unlock.UnlockEnemy(0);
        player.controller.enabled = true;
        uiController.compassObj.SetActive(true);
    }
    IEnumerator VoidMageUnlock()
    {
        Debug.Log("Trigger vMage Unlock Summon");
        PartyController party = FindObjectOfType<PartyController>();
        PlayerController player = FindObjectOfType<PlayerController>();
        DunUIController uiController = FindObjectOfType<DunUIController>();
        SceneController controller = FindObjectOfType<SceneController>();

        yield return new WaitForSeconds(.5f);
        controller.activePlayable = vMageUnlockPlayable;
        controller.endAction += EndVoidMageSummon;

        vMageUnlockPlayable.gameObject.SetActive(true);

        party.AssignCamBrain(vMageUnlockPlayable, 3);
        foreach (DunModel model in party.activeParty)
        {
            model.AssignToDirector(vMageUnlockPlayable);
            model.gameObject.SetActive(true);
            model.transform.position = vMageUnlockPlayable.transform.position;
            model.transform.rotation = vMageUnlockPlayable.transform.rotation;
            model.transform.parent = vMageUnlockPlayable.transform;
        }

        DunModel vMage = party.masterParty[5];
        vMage.gameObject.SetActive(true);
        vMage.transform.position = vMageUnlockPlayable.transform.position;
        vMage.transform.rotation = vMageUnlockPlayable.transform.rotation;
        vMage.transform.parent = vMageUnlockPlayable.transform;

        vMage.AssignToDirector(vMageUnlockPlayable, 4);
        vMage.AssignToDirector(vMageUnlockPlayable, 5);

        player.controller.enabled = false;
        uiController.compassObj.SetActive(false); 
        vMageUnlockPlayable.Play();
        yield return new WaitForSeconds((float)vMageUnlockPlayable.duration);
        if (controller.activePlayable == vMageUnlockPlayable)
        {
            EndVoidMageSummon();
        }

    }

    public void EndVoidMageSummon()
    {
        PartyController party = FindObjectOfType<PartyController>();
        PlayerController player = FindObjectOfType<PlayerController>();
        SceneController controller = FindObjectOfType<SceneController>();
        DunUIController uiController = FindObjectOfType<DunUIController>();
        UnlockController unlock = FindObjectOfType<UnlockController>();

        foreach (DunModel model in party.activeParty)
        {
            model.gameObject.SetActive(false);
        }
        party.masterParty[5].gameObject.SetActive(false);

        if (!unlock.vMageUnlock)
        {
            unlock.unlockUI.nextUnlockAction = UnlockTemplarEnemies; // sets next Unlock to Templars after NXT button is selected on UnlockUI
            unlock.UnlockCharacter(5);
        }
        controller.activePlayable = null;
        controller.endAction = null;

        foreach (CubeRoom room in controller.builder.createdRooms)
        {
            if (room.activeENV == this)
            {
                if (room != roomParent)
                {
                    LibraryRoomParent otherLib = room.activeENV.GetComponent<LibraryRoomParent>();
                    otherLib.skull.gameObject.SetActive(false);
                    distanceController.npcS.Remove(otherLib.skull);
                }
            }
        }

        MusicController music = FindObjectOfType<MusicController>();
        music.CrossfadeToNextClip(music.dungeonMusicClips[Random.Range(0, music.dungeonMusicClips.Count)]);

    }
       

    public void TemplarBattleReturn()
    {
        foreach (DunModel temp in templarModels)
        {
            temp.gameObject.SetActive(false);
        }
    }


    private void Update()
    {
        
    }
}
