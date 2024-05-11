using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.HighDefinition;
using UnityEngine;

public class SpellComboController : MonoBehaviour
{
    public BattleController battleC;
    public PartyController party;
    public ComboController comboC;
    public int comboNumber;

    public bool SpellComboChecker()
    {
        Debug.Log("Spell Combo Checker Start (SpellComboController");
        bool comboBool = false;

        BattleModel fireMage = null;
        foreach (BattleModel hero in battleC.heroParty)
        {
           // check for FMage Combos
           if (hero.modelName == party.combatMaster[0].modelName)
            {
                fireMage = hero;
                break;
            }
        }
        if (fireMage != null)
        {
            if (fireMage.actionType == BattleModel.ActionType.spell)
            {
                Debug.Log("Checking FireMage Spell Combos");
                if (fireMage.selectedSpell == fireMage.masterSpells[0])
                {
                    Debug.Log("Checking FireMage Combo 0");
                    foreach (BattleModel hero in battleC.heroParty)
                    {
                        if (hero != fireMage)
                        {
                            if (hero.actionType == BattleModel.ActionType.item)
                            {
                                if (hero.selectedItem == comboC.itemComboC.catalystPrefab)
                                {
                                    if (hero.actionTarget == fireMage || hero.actionTarget == fireMage.actionTarget)
                                    {
                                        //trigger Fire Blast / Catalyst Combo
                                        comboC.comboState = ComboController.ComboState.spell;
                                        Debug.Log("Fire Mage Fire Blast / Calayst Combo Triggered");
                                        battleC.comboC.totalCombos++;
                                        comboNumber = 0;
                                        comboBool = true;
                                        break;
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


        yield return new WaitForSeconds(0);
        battleC.heroParty[0].StartAction(); // to loop test
    }
}
