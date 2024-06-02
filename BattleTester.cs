using DTT.PlayerPrefsEnhanced;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTester : BattleController
{
    public override void SetBattle(int enemyNum)
    {
        if (bossBattle)
        {
            bossBattle = false;
        }      
        if (!battleUI.phaseUI.gameObject.activeSelf)
        {
            battleUI.phaseUI.gameObject.SetActive(true);
        }
        phase = BattlePhase.start;
        battleUI.phaseUI.ringAnims[0].SetBool("highLight", true);
        musicC.CrossfadeToNextClip(musicC.battleMusicClips[Random.Range(0, musicC.battleMusicClips.Count)]);
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
        bCamController.activeCam = activeRoom.mainCam;

        foreach (BattleModel enemyMod in enemyParty)
        {
            enemyMod.anim.SetTrigger("taunt");
            enemyMod.spawnPoint = enemyMod.transform.position;
        }

        foreach (BattleModel heroMod in heroParty)
        {
            int x = heroParty.IndexOf(heroMod);
            heroMod.health = heroMod.maxH; // set automatically to max for testing
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
        comboC.BattleReset();
        heroIndex = 0;
        enemyIndex = 0;
        StartCoroutine(StartIntroPhase());
    }
}
