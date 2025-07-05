using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrozenHeroScript : BattleModel
{
    public override void FrozenPlayerSwap()
    {
        BattleModel unfrozen = frozenSelf;
        unfrozen.gameObject.SetActive(true);
        unfrozen.health = health;
        unfrozen.maxH = maxH;
        unfrozen.def = def;
        unfrozen.power = power;      
        unfrozen.powerBonusPercent = powerBonusPercent;
        unfrozen.defBonusPercent = defBonusPercent;
        unfrozen.spellBonusPercent = spellBonusPercent;       
        unfrozen.skip = false;
        int indexX = 0;
        foreach (BattleModel hero in battleC.heroParty)
        {
            if (hero == this)
            {
                indexX = battleC.heroParty.IndexOf(hero);
            }
        }
        battleC.heroParty[indexX] = unfrozen;
        gameObject.SetActive(false);
    }
}
