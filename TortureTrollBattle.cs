using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TortureTrollBattle : EnemyBattleModel
{
    public override void TakeDamage(int damage, BattleModel damSource, bool crit = false)
    {
        if (battleC == null)
        {
            battleC = FindObjectOfType<BattleController>();
        }
        DamageMSS damCan = Instantiate(battleC.damageCanvas, hitTarget.transform.position, Quaternion.identity);
        damCan.activeCam = battleC.bCamController.activeCam;
        audioSource.PlayOneShot(actionSounds[2]);
        if (damSource.actionType == ActionType.melee)
        {
            if (statusC.DEFboost > 0)
            {
                damage = damage - statusC.DEFboost;
                if (damage < 0)
                {
                    damage = 0;
                }
                statusC.ActivateDEF(false);
            }
        }

        if (crit)
        {
            damCan.ShowDamage(damage, true);
        }
        if (!crit)
        {
            damCan.ShowDamage(damage);
        }   

        health = health - damage;
        if (health <= 0)
        {
            health = 0;
            Die(damSource);
        }
        if (health > 0 && damSource.actionType == ActionType.melee)
        {
            statusC.ActivateBoost(true);
            statusC.boostAmount = statusC.boostAmount + 10;
            anim.SetTrigger("hitTaunt");
        }
    }

    public void TauntAudio()
    {
        audioSource.PlayOneShot(actionSounds[1]);
    }

    public override void Die(BattleModel damSource)
    {
        base.Die(damSource);
        audioSource.PlayOneShot(actionSounds[3]);
    }
}
