using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThiefBattleModel : BattleModel
{
    public override void Cast(BattleModel target, Spell spell)
    {
        Debug.Log("Starting " + modelName + " Cast, Targeting " + target.modelName);
        battleC.heroIndex++;
        StartCoroutine(CastTimer(target, selectedSpell));
    }

    public IEnumerator CastTimer(BattleModel target, Spell spell)
    {
        transform.LookAt(target.transform);
        anim.SetTrigger("spell0");
        yield return new WaitForSeconds(.25f);
        Vector3 impactAdj = target.hitTarget.position;
        Spell newSpell = Instantiate(spell, transform);
        yield return new WaitForSeconds(newSpell.castTime);

        float damageAdjusted = (spellBonusPercent / 100f) + 1f;
        float damX = damageAdjusted * spell.baseDamage;
        int roundedDamage = Mathf.RoundToInt(damX);

        Debug.Log(modelName + " casting " + spell.spellName + " at all targets for " + roundedDamage + " damage");
        yield return new WaitForSeconds(1); // set a spell "aftertime?"
        foreach (EnemyBattleModel enemy in battleC.enemyParty)
        {
            if (!enemy.dead)
            {
                enemy.TimelineHit();
                enemy.TakeDamage(roundedDamage, this);
            }        
        }       
        if (afterAction != null)
        {
            afterAction.Invoke();
        }
        if (spell.charges != 100)
        {
            spell.remainingCharges--;
        }
        afterAction = null;
        newSpell.gameObject.SetActive(false);
    }

}
