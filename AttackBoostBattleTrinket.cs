using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBoostBattleTrinket : BattleTrinket
{

    public override void ActiveBattleTrinket()
    {
        foreach (BattleModel hero in battleC.heroParty)
        {
            hero.statusC.ActivateBoost(true);
            hero.statusC.boostAmount = hero.statusC.boostAmount + effectAmount;
        }
    }
}
