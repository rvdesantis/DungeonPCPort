using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MimicBattleModel : EnemyBattleModel
{

    public override void GetHit(BattleModel modelSource)
    {
        if (modelSource.actionType == ActionType.melee)
        {
            def = 30;
            StartCoroutine(ShellTimer());
        }
        if (modelSource.actionType == ActionType.spell || modelSource.actionType == ActionType.item)
        {
            def = 0;
            audioSource.PlayOneShot(actionSounds[1]);
            anim.SetTrigger("hit");            
        }   
    }

    IEnumerator ShellTimer()
    {
        anim.SetTrigger("block");
        audioSource.PlayOneShot(actionSounds[4]);
        yield return new WaitForSeconds(.5f);
        audioSource.PlayOneShot(actionSounds[2]);
        yield return new WaitForSeconds(.5f);
        anim.SetTrigger("blockEnd");
        audioSource.PlayOneShot(actionSounds[5]);
        transform.LookAt(battleC.playerSpawnPoints[0].transform);
    }

    public override void Attack(BattleModel target)
    {
        audioSource.PlayOneShot(actionSounds[0]);
        base.Attack(target);
    }

    public override void Die(BattleModel damSource)
    {
        audioSource.PlayOneShot(actionSounds[3]);
        base.Die(damSource);
    }
}
