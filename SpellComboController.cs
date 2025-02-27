using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering.HighDefinition;
using UnityEngine;
using UnityEngine.Playables;

public class SpellComboController : MonoBehaviour
{
    public BattleController battleC;
    public PartyController party;
    public ComboController comboC;
    public int comboNumber;
    public List<PlayableDirector> spellComboPlayables;
    public BattleModel comboModel0;
    public BattleModel comboModel1;
    public BattleModel leftover0;
    public List<int> usedCombos;

    public bool SpellComboChecker()
    {
        Debug.Log("Spell Combo Checker Start (SpellComboController");
        bool comboBool = false;

        BattleModel fireMage = null;
        BattleModel thief = null;
        BattleModel madien = null;
        BattleModel warrior = null;
        BattleModel voidMage = null;

        foreach (BattleModel hero in battleC.heroParty)
        { 
           if (hero.modelName == party.combatMaster[0].modelName)
            {
                fireMage = hero; 
            }
            if (hero.modelName == party.combatMaster[4].modelName)
            {
                thief = hero;
            }
            if (hero.modelName == party.combatMaster[1].modelName)
            {
                madien = hero;
            }
            if (hero.modelName == party.combatMaster[2].modelName)
            {
                warrior = hero;
            }
            if (hero.modelName == party.combatMaster[5].modelName)
            {
                voidMage = hero;
            }
        }
        if (fireMage != null && !comboBool)
        {
            if (fireMage.actionType == BattleModel.ActionType.spell)
            {
                Debug.Log("Checking FireMage Spell Combos");
                if (thief != null)
                {
                    Debug.Log("Checking FMage & Thief Combos");
                    bool combo0Used = false;
                    foreach (int usedInt in usedCombos)
                    {
                        if (usedInt == 0)
                        {
                            combo0Used = true;
                        }
                    }
                    if (!combo0Used)
                    {
                        if (fireMage.selectedSpell == fireMage.masterSpells[0] && thief.selectedSpell == thief.masterSpells[0])
                        {
                            if (fireMage.actionTarget == thief.actionTarget)
                            {
                                comboC.comboState = ComboController.ComboState.spell;
                                Debug.Log("Fire Knives Combo Triggered");                  
                                comboNumber = 0;
                                comboBool = true;
                                comboModel0 = fireMage;
                                comboModel1 = thief;
                                foreach (BattleModel hero in battleC.heroParty)
                                {
                                    if (hero != thief && hero != fireMage)
                                    {
                                        leftover0 = hero;
                                        hero.afterAction = battleC.StartPostHeroTimer;
                                    }
                                }
                            }
                        }
                    }                   
                }      
                if (voidMage !=null)
                {
                    Debug.Log("Checking FMage & VMage Combos");
                    bool combo2Used = false;
                    foreach (int usedInt in usedCombos)
                    {
                        if (usedInt == 2)
                        {
                            combo2Used = true;
                        }
                    }
                    if (!combo2Used)
                    {
                        if (fireMage.selectedSpell == fireMage.masterSpells[0] && voidMage.selectedSpell == voidMage.masterSpells[0])
                        {
                            if (fireMage.actionTarget == voidMage.actionTarget)
                            {
                                comboC.comboState = ComboController.ComboState.spell;
                                Debug.Log("Dark Fire Combo Triggered");                 
                                comboNumber = 2;
                                comboBool = true;
                                comboModel0 = fireMage;
                                comboModel1 = voidMage;
                                foreach (BattleModel hero in battleC.heroParty)
                                {
                                    if (hero != voidMage && hero != fireMage)
                                    {
                                        leftover0 = hero;
                                        hero.afterAction = battleC.StartPostHeroTimer;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        if (warrior !=null && !comboBool)
        {
            if (warrior.actionType == BattleModel.ActionType.spell)
            {
                Debug.Log("Checking Warrior Spell Combos");
                {
                    if (madien != null && madien.actionType == BattleModel.ActionType.spell)
                    {
                        if (warrior.selectedSpell == warrior.masterSpells[0] && madien.selectedSpell == madien.masterSpells[0])
                        {
                            bool combo1Used = false;
                            foreach (int usedInt in usedCombos)
                            {
                                if (usedInt == 1)
                                {
                                    combo1Used = true;
                                }
                            }
                            if (madien.actionTarget == warrior.actionTarget && !combo1Used)
                            {
                                comboC.comboState = ComboController.ComboState.spell;
                                Debug.Log("Boost Combo Triggered");
                                comboNumber = 1;
                                comboBool = true;
                                comboModel0 = warrior;
                                comboModel1 = madien;
                                foreach (BattleModel hero in battleC.heroParty)
                                {
                                    if (hero != warrior && hero != madien)
                                    {
                                        leftover0 = hero;
                                        hero.afterAction = battleC.StartPostHeroTimer;
                                    }
                                }
                            }                            
                        }
                    }
                }
            }
        }
        return comboBool;
    }


    public void StartSpellCombo()
    {
        StartCoroutine(StartSpellTimer(comboNumber));
    }

    IEnumerator StartSpellTimer(int comboNum)
    {
        Debug.Log("SpellCombo Start");
        if (comboNum == 0) // fireKnives Combo
        {
            Debug.Log("Starting Spello Combo Fire Knives");
            BattleModel damSource = null;
            spellComboPlayables[0].transform.position = battleC.activeRoom.enemySpawnPoints[0].transform.position;
            damSource = comboModel0;


            float damageAdjusted = 0;
            float damX = 0;
            int roundedDamage = 0;
            
            float damageAdjusted2 = 0;
            float damX2 = 0;
            int roundedDamage2 = 0;
           
            int totalDam = 0;
   
            damageAdjusted = (comboModel0.spellBonusPercent / 100f) + 1f;
            damX = damageAdjusted * comboModel0.activeSpells[0].baseDamage;
            roundedDamage = Mathf.RoundToInt(damX);

            Debug.Log("Spell 0 Damage = " + comboModel0.activeSpells[0].baseDamage + " * " + damageAdjusted);

            damageAdjusted2 = (comboModel1.spellBonusPercent / 100f) + 1f;
            damX2 = damageAdjusted * comboModel1.activeSpells[0].baseDamage;
            roundedDamage2 = Mathf.RoundToInt(damX2);

            Debug.Log("Spell ` Damage = " + comboModel1.activeSpells[0].baseDamage + " * " + damageAdjusted);

            comboModel0.anim.SetTrigger("spell0");
            comboModel1.anim.SetTrigger("spell0");
            comboModel0.GetComponent<FireMageBattleModel>().handFX.gameObject.SetActive(true);
            comboModel0.GetComponent<FireMageBattleModel>().handFX.Play();


            yield return new WaitForSeconds(.5f);
            spellComboPlayables[0].Play();
            yield return new WaitForSeconds((float)spellComboPlayables[0].duration - .5f);

            totalDam = roundedDamage + roundedDamage2;

            Debug.Log("Total Spell Damage " + totalDam + " / (" + roundedDamage + " + " + roundedDamage2 + ")");
            foreach (BattleModel enemy in battleC.enemyParty)
            {
                if (!enemy.dead)
                {
                    Vector3 impactAdj = enemy.hitTarget.position;
                    ParticleSystem newImpact = Instantiate(comboModel0.selectedSpell.spellImpactFX[0], impactAdj, transform.rotation);
                    newImpact.Play();
                    enemy.TakeDamage(totalDam, damSource);
                    enemy.impactFX.elementalImpactFXs[0].Play();
                    
                }
            }
            usedCombos.Add(0);
            comboC.totalCombos++;
            yield return new WaitForSeconds(2);
            comboModel0.GetComponent<FireMageBattleModel>().handFX.gameObject.SetActive(false);


            battleC.heroIndex = 2;
            leftover0.StartAction();
        }  
        if (comboNum == 1) // boost Combo
        {
            Debug.Log("Starting Spello Combo Fire Knives");
            BattleModel damSource = null;
            spellComboPlayables[1].transform.position = comboModel0.actionTarget.transform.position;
            comboModel0.actionTarget.AssignSpecificDirector(spellComboPlayables[1], 0);
            damSource = comboModel0;

            float damageAdjusted = 0;
            float damX = 0;
            int roundedDamage = 0;

            float damageAdjusted2 = 0;
            float damX2 = 0;
            int roundedDamage2 = 0;

            int totalDam = 0;

            damageAdjusted = (comboModel0.spellBonusPercent / 100f) + 1f;
            damX = damageAdjusted * comboModel0.activeSpells[0].baseDamage;
            roundedDamage = Mathf.RoundToInt(damX);

            Debug.Log("Spell 0 Damage = " + comboModel0.activeSpells[0].baseDamage + " * " + damageAdjusted);

            damageAdjusted2 = (comboModel1.spellBonusPercent / 100f) + 1f;
            damX2 = damageAdjusted * comboModel1.activeSpells[0].baseDamage;
            roundedDamage2 = Mathf.RoundToInt(damX2);

            Debug.Log("Spell 1 Damage = " + comboModel1.activeSpells[0].baseDamage + " * " + damageAdjusted);

            totalDam = roundedDamage + roundedDamage2;

            comboModel0.anim.SetTrigger("spell0");
            comboModel1.anim.SetTrigger("spell0");

            yield return new WaitForSeconds(1);

            spellComboPlayables[1].Play();
            comboModel0.actionTarget.statusC.ActivateBoost(true);
            comboModel0.actionTarget.statusC.ActivateDEF(true);

            comboModel0.actionTarget.statusC.boostAmount = totalDam;
            comboModel0.actionTarget.statusC.DEFboost = totalDam;

            yield return new WaitForSeconds((float)spellComboPlayables[1].duration);
            usedCombos.Add(1);
            comboC.totalCombos++;
            yield return new WaitForSeconds(2);
            battleC.heroIndex = 2;
            leftover0.StartAction();
        }
        if (comboNum == 2) // DarkFire Combo
        {

            Debug.Log("Starting Spell2 Combo Dark Fire");
            BattleModel damSource = null;
            
            damSource = comboModel0;
            spellComboPlayables[2].transform.position = comboModel0.actionTarget.transform.position;

            float damageAdjusted = 0;
            float damX = 0;
            int roundedDamage = 0;

            float damageAdjusted2 = 0;
            float damX2 = 0;
            int roundedDamage2 = 0;

            int totalDam = 0;

            damageAdjusted = (comboModel0.spellBonusPercent / 100f) + 1f;
            damX = damageAdjusted * comboModel0.activeSpells[0].baseDamage;
            roundedDamage = Mathf.RoundToInt(damX);

            Debug.Log("Spell 0 Damage = " + comboModel0.activeSpells[0].baseDamage + " * " + damageAdjusted);

            damageAdjusted2 = (comboModel1.spellBonusPercent / 100f) + 1f;
            damX2 = damageAdjusted * comboModel1.activeSpells[0].baseDamage;
            roundedDamage2 = Mathf.RoundToInt(damX2);

            Debug.Log("Spell ` Damage = " + comboModel1.activeSpells[0].baseDamage + " * " + damageAdjusted);

            comboModel0.anim.SetTrigger("spell0");
            comboModel1.anim.SetTrigger("spell0");

            comboModel0.GetComponent<FireMageBattleModel>().handFX.gameObject.SetActive(true);
            comboModel0.GetComponent<FireMageBattleModel>().handFX.Play();

            comboModel1.GetComponent<VoidBattleModel>().handFX.gameObject.SetActive(true);
            comboModel1.GetComponent<VoidBattleModel>().handFX.Play();

            yield return new WaitForSeconds(.5f);
            spellComboPlayables[2].Play();
            yield return new WaitForSeconds((float)spellComboPlayables[2].duration - .5f);

            totalDam = Mathf.RoundToInt((roundedDamage + roundedDamage2) * 1.25f);

            Debug.Log("Total Spell Damage " + totalDam + " / (" + roundedDamage + " + " + roundedDamage2 + ")");
            comboModel0.actionTarget.TakeDamage(totalDam, damSource);
            comboModel0.actionTarget.GetHit(damSource);

            usedCombos.Add(2);
            comboC.totalCombos++;
            yield return new WaitForSeconds(2);
            comboModel0.GetComponent<FireMageBattleModel>().handFX.gameObject.SetActive(false);
            comboModel1.GetComponent<VoidBattleModel>().handFX.gameObject.SetActive(false);
            battleC.heroIndex = 2;
            leftover0.StartAction();
        }
    }
}
