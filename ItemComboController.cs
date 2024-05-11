using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemComboController : MonoBehaviour
{
    public BattleController battleC;
    public InventoryController inventory;
    public DunItem catalystPrefab;
    public int comboNumber;

    public bool ItemComboChecker()
    {
        bool comboBool = false;
        bool catalyst = false;
        BattleModel catalystModel = null;

        foreach (BattleModel hero in battleC.heroParty)
        {
            if (hero.actionType == BattleModel.ActionType.item)
            {
                if (hero.selectedItem == catalystPrefab)
                {
                    catalyst = true;
                    catalystModel = hero;
                }
            }

        }
        if (catalyst)
        {
            foreach (BattleModel hero in battleC.heroParty)
            {
                if (hero != catalystModel)
                {
                    if (hero.actionType == BattleModel.ActionType.item)
                    {
                        if (hero.selectedItem == inventory.battleItems[0])
                        {
                            if (hero.actionTarget == catalystModel.actionTarget)
                            {
                                comboBool = true;
                                comboNumber = 0;
                                battleC.comboC.totalCombos++;
                                Debug.Log("Potion Combo Triggered");
                            }
                        }
                    }                        
                }
            }
        }

        // set comboNumber
        return comboBool;
    }


    public void StartItemCombo()
    {
        StartCoroutine(StartItemTimer(comboNumber));
    }

    IEnumerator StartItemTimer(int comboNum)
    {


        yield return new WaitForSeconds(0);
        battleC.heroParty[0].StartAction(); // to loop test
    }

}
