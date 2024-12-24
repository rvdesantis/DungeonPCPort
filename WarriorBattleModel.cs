using DTT.PlayerPrefsEnhanced;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorBattleModel : BattleModel
{
    public override void Cast(BattleModel target, Spell spell)
    {
        actionTarget = target;
        Debug.Log("Starting " + modelName + " Cast, Targeting " + target.modelName);
        StartCoroutine(CastTimer(target, selectedSpell));
    }

    IEnumerator CastTimer(BattleModel target, Spell spell)
    {
        if (spell == masterSpells[0])
        {
            anim.SetTrigger("spell0");
            yield return new WaitForSeconds(spell.impactTime);
            actionTarget.statusC.ActivateDEF(true);
            float magic = EnhancedPrefs.GetPlayerPref(modelName + "SpellPercent", 0f);
            float multiplyer = (magic / 100) + 1;
            int totalBase = Mathf.RoundToInt(multiplyer * spell.baseDamage);
            actionTarget.statusC.DEFboost = actionTarget.statusC.boostAmount + totalBase;
            yield return new WaitForSeconds(spell.castTime - spell.impactTime);
            battleC.heroIndex++;
            afterAction.Invoke();
            afterAction = null;
        }

    }
}
