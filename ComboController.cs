using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

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
        BattleModel fireMage = battleC.party.combatMaster[0];
        BattleModel madien = battleC.party.combatMaster[1];
        BattleModel warrior = battleC.party.combatMaster[2];
        BattleModel berserk = battleC.party.combatMaster[3];
        BattleModel thief = battleC.party.combatMaster[4];
        BattleModel voidMage = battleC.party.combatMaster[5];

        string name0 = battleC.heroParty[0].modelName;
        string name1 = battleC.heroParty[1].modelName;
        string name2 = battleC.heroParty[2].modelName;

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
                    if (name0 == fireMage.modelName || name0 == madien.modelName || name0 == warrior.modelName)
                    {
                        if (name1 == fireMage.modelName || name1 == madien.modelName || name1 == warrior.modelName)
                        {
                            if (name2 == fireMage.modelName || name2 == madien.modelName || name2 == warrior.modelName)
                            {
                                Debug.Log("Melee Combo 0 Triggered, 0 - 1 - 2");
                                comboState = ComboState.melee;
                                meleeComboC.triggered = true;
                                totalCombos++;
                                foreach (BattleModel hero in battleC.heroParty)
                                {
                                    if (hero.modelName == fireMage.modelName)
                                    {
                                        battleC.comboC.AssignComboPlayable(hero, meleeComboC.comboPlayables[0], 0);
                                        battleC.comboC.AssignComboPlayable(hero, meleeComboC.comboPlayables[0], 4);
                                    }
                                    if (hero.modelName == madien.modelName)
                                    {
                                        battleC.comboC.AssignComboPlayable(hero, meleeComboC.comboPlayables[0], 1);
                                        battleC.comboC.AssignComboPlayable(hero, meleeComboC.comboPlayables[0], 5);
                                    }
                                    if (hero.modelName == warrior.modelName)
                                    {
                                        battleC.comboC.AssignComboPlayable(hero, meleeComboC.comboPlayables[0], 2);
                                        battleC.comboC.AssignComboPlayable(hero, meleeComboC.comboPlayables[0], 6);
                                    }
                                }
                                meleeComboC.StartMeleeCombo(0);
                                return;
                            }
                        }
                    }

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

    public void AssignComboPlayable(BattleModel bModel, PlayableDirector dir, int pos)
    {
        BattleController battleC = FindObjectOfType<BattleController>();
        Debug.Log("Assigning " + bModel.modelName + " to position " + pos);
        PlayableBinding playableBinding = dir.playableAsset.outputs.ElementAt(pos);
        dir.SetGenericBinding(playableBinding.sourceObject, bModel.anim);
    }


}
