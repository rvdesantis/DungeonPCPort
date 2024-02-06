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

    public List<GameObject> playerSpawnPoints;
    public List<GameObject> enemySpawnPoints;
    public List<CinemachineVirtualCamera> roomVCams;
    public List<BattleModel> heroParty;
    public List<BattleModel> enemyParty;
    public BattleModel placeHolder;
    public BattleRoom activeRoom;
    public List<BattleRoom> battleRooms; // 0 - hall, 1 - fall room, 2 - small room, 3 - large room
    public int turnCount;
    public int heroIndex;
    public int enemyIndex;

    public List<DunItem> activeTrinkets;

    public Action afterBattleAction;

    public void SetBattle(int enemyNum)
    {
        sceneController.gameState = SceneController.GameState.Battle;

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

        if (activeRoom.introPlayable != null)
        {
            foreach (BattleModel heroMod in heroParty)
            {
                heroMod.AssignBattleDirector(activeRoom.introPlayable);
            }
            activeRoom.IntroTimer();
        }      
        
        foreach (BattleModel enemyMod in enemyParty)
        {
            enemyMod.anim.SetTrigger("taunt");
        }
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
    }

    public void BattleRewards()
    {
        int battleXP = enemyParty[0].XP + enemyParty[1].XP + enemyParty[2].XP;
        int battleGold = enemyParty[0].gold + enemyParty[1].gold + enemyParty[2].gold;

        Debug.Log("Adding Rewards: Gold + " + battleGold + "; XP + " + battleXP);

        inventory.gold = inventory.gold + battleGold;
        foreach (BattleModel hero in heroParty)
        {            
            hero.XP = hero.XP + battleXP;
            EnhancedPrefs.SetPlayerPref(hero.modelName + "XP", hero.XP);
        }

        EnhancedPrefs.SavePlayerPrefs();
    }
}
