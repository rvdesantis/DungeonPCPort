using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using DTT.PlayerPrefsEnhanced;
using UnityEngine.InputSystem.XR;

public class JailRoomParent : RoomPropParent
{
    public DunSwitch jailSwitch;
    public DunSwitch hiddenSwitch;
    public PlayableDirector leftGate;
    public PlayableDirector rightGate;
    public PlayableDirector jailerReturn;
    public PlayableDirector idlePrisoner;
    public FakeWall hiddenWall;
    public bool lGate;
    public bool rGate;
    public DunModel prisoner;
    public bool whisper;
    public DunModel activeJailer;
    public DunChest cellChest;

    public override void EnvFill()
    {
        if (distanceController == null)
        {
            distanceController = FindObjectOfType<DistanceController>();
        }
        AvailableWall();
        foreach (GameObject obj in inactiveList)
        {
            if (obj.activeSelf)
            {
                obj.SetActive(false);
            }
        }
        foreach (GameObject obj in activeList)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
            }
        }
        roomParent.activeENV = this;

        distanceController.fakeWalls.Add(hiddenWall);
        distanceController.switches.Add(jailSwitch);

        if (roomParent.roomType == CubeRoom.RoomType.quest)
        {
            SceneBuilder builder = FindObjectOfType<SceneBuilder>();
            bool inDun = false;
            foreach (CubeRoom room in builder.createdRooms)
            {
                if (room.activeENV == this)
                {
                    if (room.roomType == CubeRoom.RoomType.quest)
                    {
                        Debug.Log("Checking for Duplicate Rogue", room.gameObject);
                        JailRoomParent rogueCheck = room.activeENV.GetComponent<JailRoomParent>();
                        if (rogueCheck.prisoner != null)
                        {
                            Debug.Log("Rogue Already Added to Dungeon.  Skipping Room", room.gameObject);
                            inDun = true;
                            break;
                        }
                    }
                }
            }
            if (!inDun)
            {
                AddRogue();
            }
            
        }

        if (roomParent.roomType == CubeRoom.RoomType.shop)
        {
            AddShop();
        }
        if (roomParent.roomType == CubeRoom.RoomType.NPC)
        {
            int x = Random.Range(0, 2);
            if (x == 0)
            {
                AddShop();
            }
            if (x == 1)
            {
                AddNecro();
            }
        }
    }
    public void AddRogue() // add a way of checking if has already been added to duplicate room
    {
        bool rogueCheck = EnhancedPrefs.GetPlayerPref("rogueUnlock", false);
        {
            if (!rogueCheck)
            {
                PartyController party = FindObjectOfType<PartyController>();
                prisoner = party.masterParty[4];
                prisoner.AssignToDirector(idlePrisoner, 4, false, false);
                prisoner.gameObject.SetActive(true);
                prisoner.transform.position = idlePrisoner.transform.position;
                prisoner.transform.parent = idlePrisoner.transform;
                idlePrisoner.Play();

                Debug.Log("Rogue added to Jail Room", gameObject);
            }
        }
    }
    public void JailerReturn()
    {        
        StartCoroutine(JailerTimer());
    }
    IEnumerator JailerTimer()
    {
        Debug.Log("Jailer Has Returned");
        PartyController party = FindObjectOfType<PartyController>();
        PlayerController player = FindObjectOfType<PlayerController>();
        MonsterController monsters = FindObjectOfType<MonsterController>();
        SceneController controller = FindObjectOfType<SceneController>();
        DunUIController uiController = FindObjectOfType<DunUIController>();

        controller.activePlayable = jailerReturn;
        controller.endAction = JailerReturnEnd;

        controller.activePlayable = jailerReturn;
        foreach (DunModel model in party.activeParty)
        {
            model.AssignToDirector(jailerReturn);
            model.gameObject.SetActive(true);
            model.transform.position = jailerReturn.transform.position;
            model.transform.parent = jailerReturn.transform;
            if (model.torch != null)
            {
                model.torch.SetActive(false);
            }
            if (model.activeWeapon != null)
            {
                model.activeWeapon.SetActive(false);
            }
        }

        party.AssignCamBrain(jailerReturn, 3);
        DunModel jailer = null;
        DunModel jailerOrc = null;
        foreach (DunModel enemy in monsters.enemyMasterList)
        {
            if (enemy.spawnArea == DunModel.SpawnArea.largeRoom)
            {
                if (enemy.spawnPlayableInt == 1)
                {
                    jailer = enemy;
                    break;
                }
            }
        }
        jailerOrc = Instantiate(jailer, jailerReturn.transform, false);
        jailerOrc.transform.position = jailerReturn.transform.position;
        jailerOrc.AssignToDirector(jailerReturn, 4);
        activeJailer = jailerOrc;
        float clipTime = (float)jailerReturn.duration;
        player.controller.enabled = false;
        uiController.compassObj.SetActive(false);
        jailerReturn.Play();
        yield return new WaitForSeconds(clipTime);
        if (controller.activePlayable == jailerReturn)
        {
            JailerReturnEnd();
        }

    }
    public void JailerReturnEnd()
    {
        PartyController party = FindObjectOfType<PartyController>();
        BattleController battleC = FindAnyObjectByType<BattleController>();
        SceneController controller = FindObjectOfType<SceneController>();

        controller.activePlayable = null;
        controller.endAction = null;

        foreach (DunModel model in party.activeParty)
        {
            model.gameObject.SetActive(false);
        }
        activeJailer.gameObject.SetActive(false);
        battleC.SetBattle(9);
        battleC.afterBattleAction = AfterJailerBattle;
    }

    public void AfterJailerBattle()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        SceneController controller = FindObjectOfType<SceneController>();
        DunUIController uiController = FindObjectOfType<DunUIController>();
        if (prisoner != null)
        {
            UnlockController unlockables = FindObjectOfType<UnlockController>();
            if (!unlockables.rogueUnlock)
            {
                unlockables.UnlockCharacter(4);
                prisoner.gameObject.SetActive(false);
            }
        }
        controller.activePlayable = null;
        controller.endAction = null;
        player.controller.enabled = true;
        uiController.compassObj.SetActive(true);
        MusicController music = FindObjectOfType<MusicController>();
        music.CrossfadeToNextClip(music.dungeonMusicClips[Random.Range(0, music.dungeonMusicClips.Count)]);
    }

    private void Update()
    {
        
    }

}
