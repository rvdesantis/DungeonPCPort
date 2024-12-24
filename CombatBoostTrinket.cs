using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatBoostTrinket : DunTrinket
{
    public TrinketController trinketC;
    public BattleController battleC;

    public override void SetTrinket()
    {

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        if (!trinketC.masterBattleTs[0].gameObject.activeSelf)
        {
            trinketC.masterBattleTs[0].gameObject.SetActive(true);
        }

        if (battleC == null)
        {
            battleC = FindObjectOfType<BattleController>();
        }
        bool active = false;
        foreach (BattleTrinket battleT in battleC.activeTrinkets)
        {
            if (battleT == this)
            {
                active = true;
                break;
            }
        }
        if (!active)
        {
            battleC.activeTrinkets.Add(trinketC.masterBattleTs[0]);
            trinketC.activeBattleTrinkets.Add(trinketC.masterBattleTrinkets[0]);
        }
    }
}
