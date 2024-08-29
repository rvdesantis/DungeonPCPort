using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DTT.PlayerPrefsEnhanced;
using System;

public class BattleController : MonoBehaviour
{
    public SceneController sceneController;
    public BattleUIController battleUI;
    public PartyController party;
    public MonsterController monsters;
    public InventoryController inventory;
    public MusicController musicC;
    public ComboController comboC;
    public StatsTracker statsTimer;

    public enum BattlePhase { start, select, preHero, hero, afterHero, preEnemy, Enemy, afterEnemy, endPhase}
    public BattlePhase phase;

    public List<GameObject> playerSpawnPoints;
    public List<GameObject> enemySpawnPoints;
    public BattleCamController bCamController;
    
    public List<BattleModel> heroParty;
    public List<BattleModel> enemyParty;
    public BattleModel placeHolder;
    public BattleRoom activeRoom;
    public List<BattleRoom> battleRooms; // 0 - hall, 1 - fall room, 2 - small room, 3 - large room
    public int turnCount;
    public int heroIndex;
    public int enemyIndex;

    public bool bossBattle;

    public List<BattleTrinket> activeTrinkets;
    public DamageMSS damageCanvas;
    public Action afterBattleAction;

    public int GetEnemyCount()
    {
        int x = 0;
        foreach (BattleModel enemy in enemyParty)
        {
            if (!enemy.dead)
            {
                x++;
            }
        }
        return x;
    }

    public virtual void SetBossBattle(int bossNum, BattleRoom bossRoom)
    {
        statsTimer.battles++;
        statsTimer.bosses++;
        bossBattle = true;
       
        sceneController.gameState = SceneController.GameState.Battle;
        sceneController.uiController.lowerUIobj.SetActive(false);
        sceneController.uiController.compassObj.SetActive(false);

        sceneController.playerController.cinPersonCam.m_Priority = -10;
        if (!battleUI.phaseUI.gameObject.activeSelf)
        {
            battleUI.phaseUI.gameObject.SetActive(true);
        }
        phase = BattlePhase.start;
        battleUI.phaseUI.ringAnims[0].SetBool("highLight", true);
        musicC.CrossfadeToNextClip(musicC.battleMusicClips[UnityEngine.Random.Range(0, musicC.battleMusicClips.Count)]);
        Debug.Log("Setting Up Battle For Boss Number " + bossNum);

        if (bossNum == 0)
        {
            activeRoom = bossRoom;
            BattleModel hero0 = null;
            BattleModel hero1 = null;
            BattleModel hero2 = null;

            BattleModel enemy0 = null;
            BattleModel enemy1 = null;
            BattleModel enemy2 = null;

            hero0 = Instantiate(party.combatParty[0], activeRoom.playerSpawnPoints[0].transform);
            heroParty.Add(hero0);
            hero1 = Instantiate(party.combatParty[1], activeRoom.playerSpawnPoints[1].transform);
            heroParty.Add(hero1);
            hero2 = Instantiate(party.combatParty[2], activeRoom.playerSpawnPoints[2].transform);
            heroParty.Add(hero2);

            enemy0 = Instantiate(monsters.bossMasterList[0], activeRoom.enemySpawnPoints[0].transform);
            enemyParty.Add(enemy0);
            enemy1 = Instantiate(placeHolder, activeRoom.enemySpawnPoints[1].transform);
            enemyParty.Add(enemy1);
            enemy2 = Instantiate(placeHolder, activeRoom.enemySpawnPoints[2].transform);
            enemyParty.Add(enemy2);

            comboC.BattleReset();
            heroIndex = 0;
            enemyIndex = 0;
            StartCoroutine(StartIntroPhase());
            sceneController.playerController.transform.position = bossRoom.afterBattleSpawnPoint.position;
            sceneController.playerController.transform.rotation = bossRoom.afterBattleSpawnPoint.rotation;
        }

        if (bossNum == 1)
        {
            activeRoom = battleRooms[3];
            foreach (BattleRoom room in battleRooms)
            {
                if (room != battleRooms[3])
                {
                    room.gameObject.SetActive(false);
                }
            }
            activeRoom.gameObject.SetActive(true);
            activeRoom.SetProps(2); // garg environment in small room position 1
            activeRoom.introPlayable = activeRoom.intros[0]; // assign playable from list for small & large rooms

            BattleModel hero0 = null;
            BattleModel hero1 = null;
            BattleModel hero2 = null;

            BattleModel enemy0 = null;
            BattleModel enemy1 = null;
            BattleModel enemy2 = null;

            hero0 = Instantiate(party.combatParty[0], activeRoom.playerSpawnPoints[0].transform);
            heroParty.Add(hero0);
            hero1 = Instantiate(party.combatParty[1], activeRoom.playerSpawnPoints[1].transform);
            heroParty.Add(hero1);
            hero2 = Instantiate(party.combatParty[2], activeRoom.playerSpawnPoints[2].transform);
            heroParty.Add(hero2);

            enemy0 = Instantiate(monsters.bossMasterList[1], activeRoom.enemySpawnPoints[0].transform);
            enemyParty.Add(enemy0);
            enemy0.spawnPoint = enemy0.transform.position;

            enemy1 = Instantiate(placeHolder, activeRoom.enemySpawnPoints[1].transform);
            enemyParty.Add(enemy1);

            enemy2 = Instantiate(placeHolder, activeRoom.enemySpawnPoints[2].transform);
            enemyParty.Add(enemy2);

            comboC.BattleReset();
            heroIndex = 0;
            enemyIndex = 0;
            StartCoroutine(StartIntroPhase());
        }

        bCamController.activeCam = activeRoom.mainCam;
        foreach (BattleModel enemyMod in enemyParty)
        {
            enemyMod.anim.SetTrigger("taunt");
        }
        foreach (BattleModel heroMod in heroParty)
        {
            int x = heroParty.IndexOf(heroMod);
            heroMod.health = party.combatHealthTracker[x];
            heroMod.defBonusPercent = EnhancedPrefs.GetPlayerPref(heroMod.modelName + "DefPercent", 0f);
            heroMod.powerBonusPercent = EnhancedPrefs.GetPlayerPref(heroMod.modelName + "PowPercent", 0f);
            heroMod.spellBonusPercent = EnhancedPrefs.GetPlayerPref(heroMod.modelName + "SpellPercent", 0f);
            heroMod.spawnPoint = heroMod.transform.position;

            if (heroMod.health == 0)
            {
                heroMod.dead = true;
                heroMod.anim.SetTrigger("dead");
            }
        }       
    }

    public virtual void SetBattle(int enemyNum)
    {
        statsTimer.battles++;
        if (bossBattle)
        {
            bossBattle = false;
        }
        sceneController.gameState = SceneController.GameState.Battle;
        sceneController.uiController.lowerUIobj.SetActive(false);
        sceneController.uiController.compassObj.SetActive(false);

        sceneController.playerController.cinPersonCam.m_Priority = -10;
        if (!battleUI.phaseUI.gameObject.activeSelf)
        {
            battleUI.phaseUI.gameObject.SetActive(true);
        }
        phase = BattlePhase.start;
        battleUI.phaseUI.ringAnims[0].SetBool("highLight", true);
        musicC.CrossfadeToNextClip(musicC.battleMusicClips[UnityEngine.Random.Range(0, musicC.battleMusicClips.Count)]);
        Debug.Log("Setting Up Battle For Enemy Number " + enemyNum);

        if (enemyNum == 0)
        {
            activeRoom = battleRooms[3];
            foreach (BattleRoom room in battleRooms)
            {
                if (room != battleRooms[3])
                {
                    room.gameObject.SetActive(false);
                }
            }
            activeRoom.gameObject.SetActive(true);
            activeRoom.SetProps(1); // garg environment in small room position 1
            activeRoom.introPlayable = activeRoom.intros[0]; // assign playable from list for small & large rooms

            BattleModel hero0 = null;
            BattleModel hero1 = null;
            BattleModel hero2 = null;

            BattleModel enemy0 = null;
            BattleModel enemy1 = null;
            BattleModel enemy2 = null;

            hero0 = Instantiate(party.combatParty[0], activeRoom.playerSpawnPoints[0].transform);
            heroParty.Add(hero0);
            hero1 = Instantiate(party.combatParty[1], activeRoom.playerSpawnPoints[1].transform);
            heroParty.Add(hero1);
            hero2 = Instantiate(party.combatParty[2], activeRoom.playerSpawnPoints[2].transform);
            heroParty.Add(hero2);



            enemy0 = Instantiate(monsters.battleMasterList[0], activeRoom.enemySpawnPoints[0].transform);
            enemyParty.Add(enemy0);
            enemy1 = Instantiate(placeHolder, activeRoom.enemySpawnPoints[1].transform);
            enemyParty.Add(enemy1);
            enemy2 = Instantiate(placeHolder, activeRoom.enemySpawnPoints[2].transform);
            enemyParty.Add(enemy2);

            enemy0.AssignBattleDirector(activeRoom.introPlayable, 4);
        }
        if (enemyNum == 1) // bone dragon / pit
        {
            activeRoom = battleRooms[1];
            foreach (BattleRoom room in battleRooms)
            {
                if (room != battleRooms[1])
                {
                    room.gameObject.SetActive(false);
                }
            }
            activeRoom.gameObject.SetActive(true);
            activeRoom.SetProps(0);
           

            BattleModel hero0 = null;
            BattleModel hero1 = null;
            BattleModel hero2 = null;

            BattleModel enemy0 = null;
            BattleModel enemy1 = null;
            BattleModel enemy2 = null;

            hero0 = Instantiate(party.combatParty[0], activeRoom.playerSpawnPoints[0].transform);
            heroParty.Add(hero0);
            hero1 = Instantiate(party.combatParty[1], activeRoom.playerSpawnPoints[1].transform);
            heroParty.Add(hero1);
            hero2 = Instantiate(party.combatParty[2], activeRoom.playerSpawnPoints[2].transform);
            heroParty.Add(hero2);



            enemy0 = Instantiate(monsters.battleMasterList[1], activeRoom.enemySpawnPoints[0].transform);
            enemyParty.Add(enemy0);
            enemy1 = Instantiate(placeHolder, activeRoom.enemySpawnPoints[1].transform);
            enemyParty.Add(enemy1);
            enemy2 = Instantiate(placeHolder, activeRoom.enemySpawnPoints[2].transform);
            enemyParty.Add(enemy2);
        }
        if (enemyNum == 2)
        {
            HiddenMeadow meadow = FindObjectOfType<HiddenMeadow>();
            activeRoom = meadow.battleRoom;

            BattleModel hero0 = null;
            BattleModel hero1 = null;
            BattleModel hero2 = null;

            BattleModel enemy0 = null;
            BattleModel enemy1 = null;
            BattleModel enemy2 = null;

            hero0 = Instantiate(party.combatParty[0], activeRoom.playerSpawnPoints[0].transform);
            heroParty.Add(hero0);
            hero1 = Instantiate(party.combatParty[1], activeRoom.playerSpawnPoints[1].transform);
            heroParty.Add(hero1);
            hero2 = Instantiate(party.combatParty[2], activeRoom.playerSpawnPoints[2].transform);
            heroParty.Add(hero2);



            enemy0 = Instantiate(monsters.battleMasterList[2], activeRoom.enemySpawnPoints[0].transform);
            enemyParty.Add(enemy0);
            enemy1 = Instantiate(placeHolder, activeRoom.enemySpawnPoints[1].transform);
            enemyParty.Add(enemy1);
            enemy2 = Instantiate(placeHolder, activeRoom.enemySpawnPoints[2].transform);
            enemyParty.Add(enemy2);
        }
        if (enemyNum == 3) // gargolyes - Small Room
        {
            activeRoom = battleRooms[2];
            foreach (BattleRoom room in battleRooms)
            {
                if (room != battleRooms[2])
                {
                    room.gameObject.SetActive(false);
                }
            }
            activeRoom.gameObject.SetActive(true);
            activeRoom.SetProps(1); // garg environment in small room position 1
            activeRoom.introPlayable = activeRoom.intros[0]; // assign playable from list for small & large rooms

            BattleModel hero0 = null;
            BattleModel hero1 = null;
            BattleModel hero2 = null;

            BattleModel enemy0 = null;
            BattleModel enemy1 = null;
            BattleModel enemy2 = null;

            hero0 = Instantiate(party.combatParty[0], activeRoom.playerSpawnPoints[0].transform);
            heroParty.Add(hero0);
            hero1 = Instantiate(party.combatParty[1], activeRoom.playerSpawnPoints[1].transform);
            heroParty.Add(hero1);
            hero2 = Instantiate(party.combatParty[2], activeRoom.playerSpawnPoints[2].transform);
            heroParty.Add(hero2);



            enemy0 = Instantiate(monsters.battleMasterList[3], activeRoom.enemySpawnPoints[0].transform);
            enemyParty.Add(enemy0);
            enemy1 = Instantiate(monsters.battleMasterList[3], activeRoom.enemySpawnPoints[1].transform);
            enemyParty.Add(enemy1);
            enemy2 = Instantiate(monsters.battleMasterList[3], activeRoom.enemySpawnPoints[2].transform);
            enemyParty.Add(enemy2);
        }
        if (enemyNum == 4) // Minotaur Runt - Hallway Room
        {
            activeRoom = battleRooms[0];
            activeRoom.gameObject.SetActive(true);
            foreach (BattleRoom room in battleRooms)
            {
                if (room != battleRooms[0])
                {
                    room.gameObject.SetActive(false);
                }
            }

            BattleModel hero0 = null;
            BattleModel hero1 = null;
            BattleModel hero2 = null;

            BattleModel enemy0 = null;
            BattleModel enemy1 = null;
            BattleModel enemy2 = null;

            hero0 = Instantiate(party.combatParty[0], activeRoom.playerSpawnPoints[0].transform);
            heroParty.Add(hero0);
            hero1 = Instantiate(party.combatParty[1], activeRoom.playerSpawnPoints[1].transform);
            heroParty.Add(hero1);
            hero2 = Instantiate(party.combatParty[2], activeRoom.playerSpawnPoints[2].transform);
            heroParty.Add(hero2);

            enemy0 = Instantiate(monsters.battleMasterList[4], activeRoom.enemySpawnPoints[0].transform);
            enemyParty.Add(enemy0);
            enemy1 = Instantiate(placeHolder, activeRoom.enemySpawnPoints[1].transform);
            enemyParty.Add(enemy1);
            enemy2 = Instantiate(placeHolder, activeRoom.enemySpawnPoints[2].transform);
            enemyParty.Add(enemy2);
        }
        if (enemyNum == 5) // Jelly - Hallway Room
        {
            activeRoom = battleRooms[0];
            activeRoom.gameObject.SetActive(true);  
            foreach (BattleRoom room in battleRooms)
            {
                if (room != battleRooms[0])
                {
                    room.gameObject.SetActive(false);
                }
            }

            BattleModel hero0 = null;
            BattleModel hero1 = null;
            BattleModel hero2 = null;

            BattleModel enemy0 = null;
            BattleModel enemy1 = null;
            BattleModel enemy2 = null;

            hero0 = Instantiate(party.combatParty[0], activeRoom.playerSpawnPoints[0].transform);
            heroParty.Add(hero0);
            hero1 = Instantiate(party.combatParty[1], activeRoom.playerSpawnPoints[1].transform);
            heroParty.Add(hero1);
            hero2 = Instantiate(party.combatParty[2], activeRoom.playerSpawnPoints[2].transform);
            heroParty.Add(hero2);

            enemy0 = Instantiate(monsters.battleMasterList[5], activeRoom.enemySpawnPoints[0].transform);
            enemyParty.Add(enemy0);
            enemy1 = Instantiate(placeHolder, activeRoom.enemySpawnPoints[1].transform);
            enemyParty.Add(enemy1);
            enemy2 = Instantiate(placeHolder, activeRoom.enemySpawnPoints[2].transform);
            enemyParty.Add(enemy2);

            // set Skip to player 1 
            hero0.skip = true;
        }
        if (enemyNum == 6) // scorp
        {
            activeRoom = battleRooms[2];
            foreach (BattleRoom room in battleRooms)
            {
                if (room != battleRooms[2])
                {
                    room.gameObject.SetActive(false);
                }
            }
            activeRoom.gameObject.SetActive(true);
            activeRoom.SetProps(6);
            activeRoom.introPlayable = null;

            BattleModel hero0 = null;
            BattleModel hero1 = null;
            BattleModel hero2 = null;

            BattleModel enemy0 = null;
            BattleModel enemy1 = null;
            BattleModel enemy2 = null;

            hero0 = Instantiate(party.combatParty[0], activeRoom.playerSpawnPoints[0].transform);
            heroParty.Add(hero0);
            hero1 = Instantiate(party.combatParty[1], activeRoom.playerSpawnPoints[1].transform);
            heroParty.Add(hero1);
            hero2 = Instantiate(party.combatParty[2], activeRoom.playerSpawnPoints[2].transform);
            heroParty.Add(hero2);



            enemy0 = Instantiate(monsters.battleMasterList[6], activeRoom.enemySpawnPoints[0].transform);
            enemyParty.Add(enemy0);
            enemy1 = Instantiate(placeHolder, activeRoom.enemySpawnPoints[1].transform);
            enemyParty.Add(enemy1);
            enemy2 = Instantiate(placeHolder, activeRoom.enemySpawnPoints[2].transform);
            enemyParty.Add(enemy2);
        }
        if (enemyNum == 8)
        {
            activeRoom = battleRooms[2];
            foreach (BattleRoom room in battleRooms)
            {
                if (room != battleRooms[2])
                {
                    room.gameObject.SetActive(false);
                }
            }
            activeRoom.gameObject.SetActive(true);
            activeRoom.SetProps(2);
            activeRoom.introPlayable = null;

            BattleModel hero0 = null;
            BattleModel hero1 = null;
            BattleModel hero2 = null;

            BattleModel enemy0 = null;
            BattleModel enemy1 = null;
            BattleModel enemy2 = null;

            hero0 = Instantiate(party.combatParty[0], activeRoom.playerSpawnPoints[0].transform);
            heroParty.Add(hero0);
            hero1 = Instantiate(party.combatParty[1], activeRoom.playerSpawnPoints[1].transform);
            heroParty.Add(hero1);
            hero2 = Instantiate(party.combatParty[2], activeRoom.playerSpawnPoints[2].transform);
            heroParty.Add(hero2);



            enemy0 = Instantiate(monsters.battleMasterList[8], activeRoom.enemySpawnPoints[0].transform);
            enemyParty.Add(enemy0);
            enemy1 = Instantiate(placeHolder, activeRoom.enemySpawnPoints[1].transform);
            enemyParty.Add(enemy1);
            enemy2 = Instantiate(placeHolder, activeRoom.enemySpawnPoints[2].transform);
            enemyParty.Add(enemy2);
        }
        if (enemyNum == 10)
        {
            activeRoom = battleRooms[2];
            foreach (BattleRoom room in battleRooms)
            {
                if (room != battleRooms[2])
                {
                    room.gameObject.SetActive(false);
                }
            }
            activeRoom.gameObject.SetActive(true);
            activeRoom.SetProps(5); // garg environment in small room position 1
            activeRoom.introPlayable = activeRoom.intros[0]; // assign playable from list for small & large rooms

            BattleModel hero0 = null;
            BattleModel hero1 = null;
            BattleModel hero2 = null;

            BattleModel enemy0 = null;
            BattleModel enemy1 = null;
            BattleModel enemy2 = null;

            hero0 = Instantiate(party.combatParty[0], activeRoom.playerSpawnPoints[0].transform);
            heroParty.Add(hero0);
            hero1 = Instantiate(party.combatParty[1], activeRoom.playerSpawnPoints[1].transform);
            heroParty.Add(hero1);
            hero2 = Instantiate(party.combatParty[2], activeRoom.playerSpawnPoints[2].transform);
            heroParty.Add(hero2);



            enemy0 = Instantiate(placeHolder, activeRoom.enemySpawnPoints[0].transform);
            enemyParty.Add(enemy0);
            enemy1 = Instantiate(monsters.battleMasterList[10], activeRoom.enemySpawnPoints[1].transform);
            enemyParty.Add(enemy1);
            enemy2 = Instantiate(monsters.battleMasterList[10], activeRoom.enemySpawnPoints[2].transform);
            enemyParty.Add(enemy2);
        }
        if (enemyNum == 11)
        {
            activeRoom = battleRooms[3];
            foreach (BattleRoom room in battleRooms)
            {
                if (room != battleRooms[3])
                {
                    room.gameObject.SetActive(false);
                }
            }
            activeRoom.gameObject.SetActive(true);
            activeRoom.SetProps(2); 
            activeRoom.introPlayable = activeRoom.intros[0]; 

            BattleModel hero0 = null;
            BattleModel hero1 = null;
            BattleModel hero2 = null;

            BattleModel enemy0 = null;
            BattleModel enemy1 = null;
            BattleModel enemy2 = null;

            hero0 = Instantiate(party.combatParty[0], activeRoom.playerSpawnPoints[0].transform);
            heroParty.Add(hero0);
            hero1 = Instantiate(party.combatParty[1], activeRoom.playerSpawnPoints[1].transform);
            heroParty.Add(hero1);
            hero2 = Instantiate(party.combatParty[2], activeRoom.playerSpawnPoints[2].transform);
            heroParty.Add(hero2);           

            enemy0 = Instantiate(monsters.battleMasterList[12], activeRoom.enemySpawnPoints[0].transform);
            enemyParty.Add(enemy0);
            enemy1 = Instantiate(monsters.battleMasterList[11], activeRoom.enemySpawnPoints[1].transform);
            enemyParty.Add(enemy1);
            enemy2 = Instantiate(monsters.battleMasterList[11], activeRoom.enemySpawnPoints[2].transform);
            enemyParty.Add(enemy2);
        }
        if (enemyNum == 13) // mimic battle in hallway
        {
            activeRoom = battleRooms[0];
            activeRoom.gameObject.SetActive(true);

            foreach (BattleRoom room in battleRooms)
            {
                if (room != battleRooms[0])
                {
                    room.gameObject.SetActive(false);
                }
            }

            BattleModel hero0 = null;
            BattleModel hero1 = null;
            BattleModel hero2 = null;

            BattleModel enemy0 = null;
            BattleModel enemy1 = null;
            BattleModel enemy2 = null;

            hero0 = Instantiate(party.combatParty[0], activeRoom.playerSpawnPoints[0].transform);
            heroParty.Add(hero0);
            hero1 = Instantiate(party.combatParty[1], activeRoom.playerSpawnPoints[1].transform);
            heroParty.Add(hero1);
            hero2 = Instantiate(party.combatParty[2], activeRoom.playerSpawnPoints[2].transform);
            heroParty.Add(hero2);

            enemy0 = Instantiate(monsters.battleMasterList[13], activeRoom.enemySpawnPoints[0].transform);
            enemyParty.Add(enemy0);
            enemy1 = Instantiate(placeHolder, activeRoom.enemySpawnPoints[1].transform);
            enemyParty.Add(enemy1);
            enemy2 = Instantiate(placeHolder, activeRoom.enemySpawnPoints[2].transform);
            enemyParty.Add(enemy2);
        }
        if (enemyNum == 14) // weeper battle in hallway
        {
            activeRoom = battleRooms[0];
            activeRoom.gameObject.SetActive(true);

            foreach (BattleRoom room in battleRooms)
            {
                if (room != battleRooms[0])
                {
                    room.gameObject.SetActive(false);
                }
            }

            BattleModel hero0 = null;
            BattleModel hero1 = null;
            BattleModel hero2 = null;

            BattleModel enemy0 = null;
            BattleModel enemy1 = null;
            BattleModel enemy2 = null;

            hero0 = Instantiate(party.combatParty[0], activeRoom.playerSpawnPoints[0].transform);
            heroParty.Add(hero0);
            hero1 = Instantiate(party.combatParty[1], activeRoom.playerSpawnPoints[1].transform);
            heroParty.Add(hero1);
            hero2 = Instantiate(party.combatParty[2], activeRoom.playerSpawnPoints[2].transform);
            heroParty.Add(hero2);

            enemy0 = Instantiate(monsters.battleMasterList[14], activeRoom.enemySpawnPoints[0].transform);
            enemyParty.Add(enemy0);
            enemy1 = Instantiate(placeHolder, activeRoom.enemySpawnPoints[1].transform);
            enemyParty.Add(enemy1);
            enemy2 = Instantiate(placeHolder, activeRoom.enemySpawnPoints[2].transform);
            enemyParty.Add(enemy2);
        }
        if (enemyNum == 15)
        {
            activeRoom = battleRooms[3];
            foreach (BattleRoom room in battleRooms)
            {
                if (room != battleRooms[2])
                {
                    room.gameObject.SetActive(false);
                }
            }
            activeRoom.gameObject.SetActive(true);
            activeRoom.SetProps(0); 
            activeRoom.introPlayable = activeRoom.intros[0]; // assign playable from list for small & large rooms

            BattleModel hero0 = null;
            BattleModel hero1 = null;
            BattleModel hero2 = null;

            BattleModel enemy0 = null;
            BattleModel enemy1 = null;
            BattleModel enemy2 = null;

            hero0 = Instantiate(party.combatParty[0], activeRoom.playerSpawnPoints[0].transform);
            heroParty.Add(hero0);
            hero1 = Instantiate(party.combatParty[1], activeRoom.playerSpawnPoints[1].transform);
            heroParty.Add(hero1);
            hero2 = Instantiate(party.combatParty[2], activeRoom.playerSpawnPoints[2].transform);
            heroParty.Add(hero2);



            enemy0 = Instantiate(monsters.battleMasterList[15], activeRoom.enemySpawnPoints[0].transform);
            enemyParty.Add(enemy0);
            enemy1 = Instantiate(monsters.battleMasterList[16], activeRoom.enemySpawnPoints[1].transform);
            enemyParty.Add(enemy1);
            enemy2 = Instantiate(monsters.battleMasterList[16], activeRoom.enemySpawnPoints[2].transform);
            enemyParty.Add(enemy2);
        }
        if (enemyNum == 17)
        {
            activeRoom = battleRooms[2];
            foreach (BattleRoom room in battleRooms)
            {
                if (room != battleRooms[2])
                {
                    room.gameObject.SetActive(false);
                }
            }
            activeRoom.gameObject.SetActive(true);
            activeRoom.SetProps(4);
            activeRoom.introPlayable = activeRoom.intros[0]; // assign playable from list for small & large rooms

            BattleModel hero0 = null;
            BattleModel hero1 = null;
            BattleModel hero2 = null;

            BattleModel enemy0 = null;
            BattleModel enemy1 = null;
            BattleModel enemy2 = null;

            hero0 = Instantiate(party.combatParty[0], activeRoom.playerSpawnPoints[0].transform);
            heroParty.Add(hero0);
            hero1 = Instantiate(party.combatParty[1], activeRoom.playerSpawnPoints[1].transform);
            heroParty.Add(hero1);
            hero2 = Instantiate(party.combatParty[2], activeRoom.playerSpawnPoints[2].transform);
            heroParty.Add(hero2);

            int pupChance = UnityEngine.Random.Range(0, 3);
            if (pupChance == 0)
            {
                enemy0 = Instantiate(monsters.battleMasterList[18], activeRoom.enemySpawnPoints[0].transform);
            }
            if (pupChance != 0)
            {
                enemy0 = Instantiate(monsters.battleMasterList[17], activeRoom.enemySpawnPoints[0].transform);
            }

            enemyParty.Add(enemy0);
            enemy1 = Instantiate(monsters.battleMasterList[17], activeRoom.enemySpawnPoints[1].transform);
            enemyParty.Add(enemy1);
            enemy2 = Instantiate(monsters.battleMasterList[17], activeRoom.enemySpawnPoints[2].transform);
            enemyParty.Add(enemy2);

            if (pupChance == 0)
            {
                WolfBattleModel puppy = enemy0.GetComponent<WolfBattleModel>();
                if (puppy != null)
                {
                    Debug.Log("Wolf component captured");
                    puppy.pup = true;
                    puppy.wolves.Add(enemy1);
                    puppy.wolves.Add(enemy2);
                }
                if (puppy == null)
                {
                    Debug.Log("ERROR: Failed to capture Wolf Battle Model Component", enemy0.gameObject);
                }
            }
        }
        bCamController.activeCam = activeRoom.mainCam;
        
        foreach (BattleModel enemyMod in enemyParty)
        {
            enemyMod.anim.SetTrigger("taunt");
            enemyMod.spawnPoint = enemyMod.transform.position;
        }

        foreach (BattleModel heroMod in heroParty)
        {
            int x = heroParty.IndexOf(heroMod);
            heroMod.health = party.combatHealthTracker[x];
            heroMod.defBonusPercent = EnhancedPrefs.GetPlayerPref(heroMod.modelName + "DefPercent", 0f);
            heroMod.powerBonusPercent = EnhancedPrefs.GetPlayerPref(heroMod.modelName + "PowPercent", 0f);
            heroMod.spellBonusPercent = EnhancedPrefs.GetPlayerPref(heroMod.modelName + "SpellPercent", 0f);
            heroMod.spawnPoint = heroMod.transform.position;

            heroMod.anim.SetTrigger("unsheath");

            if (heroMod.health == 0)
            {
                heroMod.dead = true;
                heroMod.anim.SetTrigger("dead");
            }
        }       
        comboC.BattleReset();
        heroIndex = 0;
        enemyIndex = 0;

        foreach (BattleTrinket battleT in activeTrinkets)
        {
            if (battleT.combatPhase == BattleTrinket.BattlePhase.start)
            {
                battleT.ActiveBattleTrinket();
            }
        }

        StartCoroutine(StartIntroPhase());
    }

    public IEnumerator StartIntroPhase()
    {
        phase = BattlePhase.start;
        if (activeRoom.introPlayable != null)
        {
            party.AssignCamBrain(activeRoom.introPlayable, 3);
            foreach (BattleModel heroMod in heroParty)
            {
                heroMod.AssignBattleDirector(activeRoom.introPlayable);
            }
            activeRoom.IntroTimer();
            yield return new WaitForSeconds((float)activeRoom.introPlayable.duration);
        }

        heroParty[0].afterAction = heroParty[1].IntroPlayable;
        heroParty[0].IntroAction();
        heroParty[1].afterAction = heroParty[2].IntroPlayable;
        heroParty[1].IntroAction();
        heroParty[2].afterAction = enemyParty[0].IntroPlayable;
        heroParty[2].IntroAction();
        enemyParty[0].afterAction = enemyParty[1].IntroPlayable;
        enemyParty[0].IntroAction();
        enemyParty[1].afterAction = enemyParty[2].IntroPlayable;
        enemyParty[1].IntroAction();
        enemyParty[2].afterAction = PreSelectTrinketPhase;
        enemyParty[0].IntroAction();

        heroParty[0].IntroPlayable();
    }

    public void PreSelectTrinketPhase()
    {
        float totalTime = 0;
        foreach(BattleTrinket dunT in activeTrinkets)
        {
            if (dunT.combatPhase == BattleTrinket.BattlePhase.select)
            {
                if (totalTime < dunT.battleDelay)
                {
                    totalTime = dunT.battleDelay;
                }   
            }
        }
        StartCoroutine(PreSelectTrinketTimer(totalTime));
    }

    IEnumerator PreSelectTrinketTimer(float timePause)
    {
        foreach (BattleTrinket dunT in activeTrinkets)
        {
            if (dunT.combatPhase == BattleTrinket.BattlePhase.select)
            {
                dunT.ActiveBattleTrinket();
            }
        }
        yield return new WaitForSeconds(timePause);
        HeroZeroSelect();
    }

    public void HeroZeroSelect()
    {
        heroParty[0].transform.position = heroParty[0].spawnPoint;
        phase = BattlePhase.select;
        if (heroParty[0].dead || heroParty[0].skip || heroParty[0].health <= 0)
        {
            heroIndex = 1;
            HeroOneSelect();
        }
        else
        {     
            StartCoroutine(ZeroTimer());
        }
    }

    IEnumerator ZeroTimer()
    {
        activeRoom.mainCam.m_Priority = -1;
        activeRoom.targetingCams[0].m_Priority = 20;
        battleUI.SetActionButtons(false);
        yield return new WaitForSeconds(.25f);  
    }

    public void HeroOneSelect()
    {
        heroParty[1].transform.position = heroParty[1].spawnPoint;
        if (heroParty[1].dead || heroParty[1].skip || heroParty[1].health <= 0)
        {
            activeRoom.mainCam.m_Priority = -1;
            heroIndex = 2;
            HeroTwoSelect();
        }
        else
        {
            StartCoroutine(OneTimer());
        }

    }

    IEnumerator OneTimer()
    {
        activeRoom.targetingCams[1].m_Priority = 20;
        activeRoom.targetingCams[0].m_Priority = -1;
        battleUI.SetActionButtons(true);
        yield return new WaitForSeconds(.25f);
    }
    public void HeroTwoSelect()
    {
        heroParty[2].transform.position = heroParty[2].spawnPoint;
        if (heroParty[2].dead || heroParty[2].skip || heroParty[2].health <= 0)
        {
            activeRoom.mainCam.m_Priority = -1;
            activeRoom.targetingCams[1].m_Priority = -1;
            activeRoom.targetingCams[0].m_Priority = -1;
            StartPreHeroTimer();
        }
        else
        {
            StartCoroutine(TwoTimer());
        }
    }

    IEnumerator TwoTimer()
    {
        activeRoom.targetingCams[2].m_Priority = 20;
        activeRoom.targetingCams[1].m_Priority = -1;
        battleUI.SetActionButtons(false);
        yield return new WaitForSeconds(.25f);
    }

    public void StartPreHeroTimer()
    {
        StartCoroutine(PreHeroTimer());
    }

    IEnumerator PreHeroTimer()
    {
        battleUI.CloseAllButtons();
        Debug.Log("Starting Pre Hero Phase");
        phase = BattlePhase.preHero;
        heroIndex = 0;

        activeRoom.mainCam.m_Priority = 20;
        foreach (CinemachineVirtualCamera cam in activeRoom.targetingCams)
        {
            cam.m_Priority = -1;
        }



        bool comboCheck = false;
        comboC.comboState = ComboController.ComboState.none;
        comboC.CheckForCombo();
        if (comboC.comboState != ComboController.ComboState.none)
        {
            comboCheck = true;
        }
        yield return new WaitForSeconds(1);
        battleUI.phaseUI.gameObject.SetActive(false);
        if (!comboCheck)
        {
            heroParty[0].StartAction(); // sets afterAction depending on index position
        }       
    }

    public void StartPostHeroTimer()
    {
        StartCoroutine(PostHeroTimer());
    }

    IEnumerator PostHeroTimer()
    {
        Debug.Log("Starting After Hero Phase");
        battleUI.phaseUI.gameObject.SetActive(true);
        phase = BattlePhase.afterHero;
        PostHeroStatusCheck();
        yield return new WaitForSeconds(1);
        StartCoroutine(PreEnemyTimer());
    }

    public void PostHeroStatusCheck()
    {
        foreach (BattleModel hero in heroParty)
        {
            if (!hero.dead)
            {
                hero.statusC.PostActionCheck();
            }
        }
    }

    public void PostEnemyStatusCheck()
    {
        foreach (BattleModel enemy in enemyParty)
        {
            if (!enemy.dead)
            {
                enemy.statusC.PostActionCheck();
            }
        }
    }

    public void StartPreEnemyTimer()
    {
        StartCoroutine(PreEnemyTimer());
    }

    IEnumerator PreEnemyTimer()
    {
        Debug.Log("Starting Pre Enemy Phase");
        phase = BattlePhase.preEnemy;
        enemyIndex = 0;
        yield return new WaitForSeconds(1);
        StartCoroutine(PreEnemyTrinketTimer());
    }

    IEnumerator PreEnemyTrinketTimer()
    {
        float totalTime = 0;
        foreach (BattleTrinket dunT in activeTrinkets)
        {
            if (dunT.combatPhase == BattleTrinket.BattlePhase.preEnemy)
            {
                if (totalTime < dunT.battleDelay)
                {
                    totalTime = dunT.battleDelay;
                }
                dunT.ActiveBattleTrinket();
            }
        }
        yield return new WaitForSeconds(totalTime);
        battleUI.phaseUI.gameObject.SetActive(false);
        enemyParty[0].StartAction();
    }

    public void StartEnemyTimer()
    {
        StartCoroutine(EnemyTimer());
    }

    IEnumerator EnemyTimer()
    {
        Debug.Log("Starting Enemy Phase");
        phase = BattlePhase.Enemy;
        yield return new WaitForSeconds(1);
        StartCoroutine(PostEnemyTimer());
    }

    public void StartPostEnemyTimer()
    {
        StartCoroutine(PostEnemyTimer());
    }

    IEnumerator PostEnemyTimer()
    {
        Debug.Log("Starting After Enemy Phase");
        battleUI.phaseUI.gameObject.SetActive(true);
        phase = BattlePhase.afterEnemy;
        PostEnemyStatusCheck();
        yield return new WaitForSeconds(2);
        StartCoroutine(EndPhaseTimer());
    }

    public void Defeat()
    {
        DefeatUI defeatUI = battleUI.defeatUI;
        battleUI.phaseUI.gameObject.SetActive(false);
        defeatUI.gameObject.SetActive(true);

        int goldLoss = inventory.gold / 2;
        if (GoldenChestTrinket.goldenChestActive)
        {
            goldLoss = 0;
        }
        Debug.Log("Gold Loss From Deated - " + goldLoss);
        inventory.ReduceGold(goldLoss);
        defeatUI.OpenDefeat(goldLoss);

    }

    IEnumerator EndPhaseTimer()
    {
        Debug.Log("Starting End Phase");
        phase = BattlePhase.endPhase;
        yield return new WaitForSeconds(1);
        if (!EndChecker()) // loop back
        {
            heroIndex = 0;
            comboC.comboState = ComboController.ComboState.none;
            HeroZeroSelect();
        }
        if (EndChecker()) // end battle
        {
            int deadHero = 0;
            int deadEnemy = 0;

            foreach (BattleModel hero in heroParty)
            {
                if (hero.dead)
                {
                    deadHero++;
                }
            }

            foreach (BattleModel enemy in enemyParty)
            {
                if (enemy.dead)
                {
                    deadEnemy++;
                }
            }

            if (deadEnemy == 3)
            {
                ReturnToDungeon();
   
            }

            if (deadEnemy < 3 && deadHero == 3)
            {
                Defeat();
          
            }

        }
    }

    public bool EndChecker()
    {
        bool end = false;
        int deadHero = 0;
        int deadEnemy = 0;

        foreach (BattleModel hero in heroParty)
        {
            if (hero.dead)
            {
                deadHero++;
            }
        }

        foreach (BattleModel enemy in enemyParty)
        {
            if (enemy.dead)
            {
                deadEnemy++;
            }
        }

        if (deadEnemy == 3 || deadHero == 3)
        {
            end = true;
        }

        return end;
    }

    public void ClearBattle()
    {
        activeRoom.gameObject.SetActive(false);
        for (int i = 0; i < heroParty.Count; i++)
        {
            heroParty[i].gameObject.SetActive(false);
            Destroy(heroParty[i].gameObject);
        }
        heroParty.Clear();
        for (int i = 0; i < enemyParty.Count; i++)
        {
            enemyParty[i].gameObject.SetActive(false);
            Destroy(enemyParty[i].gameObject);
        }
        enemyParty.Clear();

        heroIndex = 0;
        enemyIndex = 0;
        musicC.CrossfadeToNextClip(musicC.dungeonMusicClips[UnityEngine.Random.Range(0, musicC.dungeonMusicClips.Count)]);
    }

    public void BattleRewards()
    {
        int battleXP = enemyParty[0].XP + enemyParty[1].XP + enemyParty[2].XP;
        int battleGold = enemyParty[0].gold + enemyParty[1].gold + enemyParty[2].gold;

        Debug.Log("Adding Rewards: Gold + " + battleGold + "; XP + " + battleXP);

        inventory.AddGold(battleGold);
        foreach (BattleModel hero in heroParty)
        {
            int currntXP = EnhancedPrefs.GetPlayerPref(hero.modelName + "XP", 0);
            hero.XP = currntXP + battleXP;
            EnhancedPrefs.SetPlayerPref(hero.modelName + "XP", hero.XP);
        }
        EnhancedPrefs.SavePlayerPrefs();
        // set Item PickUp
        battleUI.victoryUI.OpenVictory(battleGold, battleXP, comboC.totalCombos);
    }

    public void ReturnToDungeon()
    {
        BattleRewards();
        battleUI.CloseAllButtons();
        // save updated party stats
        battleUI.phaseUI.gameObject.SetActive(false);

        party.combatHealthTracker[0] = heroParty[0].health;
        party.combatHealthTracker[1] = heroParty[1].health;
        party.combatHealthTracker[2] = heroParty[2].health;

        comboC.comboState = ComboController.ComboState.none; // reset for next battle

        activeRoom.mainCam.m_Priority = -1;
        foreach (GameObject obj in activeRoom.activeObjects)
        {
            obj.SetActive(false);
        }
        sceneController.playerController.cinPersonCam.m_Priority = 10;
        ClearBattle();
        if (afterBattleAction != null)
        {
            afterBattleAction.Invoke();
        }
        afterBattleAction = null;
        sceneController.gameState = SceneController.GameState.Dungeon;
    }
}
