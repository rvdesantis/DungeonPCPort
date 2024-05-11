using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboController : MonoBehaviour
{
    public BattleController battleC;
    public MeleeComboController meleeComboC;
    public ItemComboController itemComboC;
    public SpellComboController spellComboC;

    public enum ComboState { none, melee, item, spell}
    public ComboState comboState;

    public Action comboAction;
    public Action afterAction;
    public int totalCombos;
    public void CheckForCombo()
    {

        if (!meleeComboC.triggered)
        {
            int meleeCount = 0;
            foreach (BattleModel hero in battleC.heroParty)
            {
                if (hero.actionType == BattleModel.ActionType.melee)
                {
                    meleeCount++;
                }
            }
            if (meleeCount == 3)
            {
                Debug.Log("3 Melee Attackers, Checking Targeting for Combo");
                if (battleC.heroParty[0].actionTarget == battleC.heroParty[1].actionTarget && battleC.heroParty[0].actionTarget == battleC.heroParty[2].actionTarget)
                {
                    comboState = ComboState.melee;
                    meleeComboC.triggered = true;
                    totalCombos++;
                    meleeComboC.StartMeleeCombo();
                    return;
                }
            }
        }
        // check for item combo

        if (itemComboC.ItemComboChecker())
        {
            comboState = ComboState.item;
            itemComboC.StartItemCombo();
            return;
        }

        if (spellComboC.SpellComboChecker())
        {
            comboState = ComboState.spell;
            spellComboC.StartSpellCombo();
            return;                
        }
        
      
        

    }

    public void BattleReset()
    {
        totalCombos = 0;
        comboState = ComboState.none;
        meleeComboC.triggered = false;
        // reset Items
        // reset Spells
    }


}
