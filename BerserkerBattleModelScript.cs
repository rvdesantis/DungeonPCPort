using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerserkerBattleModelScript : BattleModel
{
    public GameObject activeSecondWeapon;
    public List<GameObject> secondWeaponList;
    public ParticleSystem slashFX;

    public override void ActivateWeapon()
    {
        if (activeWeapon != null)
        {
            activeWeapon.gameObject.SetActive(true);
        }
        if (activeSecondWeapon != null)
        {
            activeSecondWeapon.gameObject.SetActive(true);
        }
    }

    public override void Cast(BattleModel target, Spell spell)
    {
        actionTarget = target;
        Debug.Log("Starting " + modelName + " Cast, Targeting " + target.modelName);
        if (spell == masterSpells[0])
        {
            StartCoroutine(BerserkerCastTimer(target, selectedSpell));

        }
       
    }

    IEnumerator BerserkerCastTimer(BattleModel target, Spell spell)
    {
        float xFL = strikeDistance + actionTarget.hitbuffer + 1; // 1 added for Spell FX
        Vector3 attackPosition = actionTarget.transform.position + (actionTarget.transform.forward * xFL);
        if (actionTarget.verticalHitBuffer != 0)
        {
            attackPosition = attackPosition + new Vector3(0, actionTarget.verticalHitBuffer, 0);
        }
        Vector3 returnPos = transform.position;

        transform.position = attackPosition;
        if (actionTarget.verticalHitBuffer == 0)
        {
            transform.LookAt(actionTarget.transform);
        }

        if (attCam != null)
        {
            attCam.m_Priority = 20;
        }

        yield return new WaitForSeconds(.75f);

        anim.SetTrigger("spell0");
 
        yield return new WaitForSeconds(.75f);
        target.GetHit(this);
        slashFX.Play();
        yield return new WaitForSeconds(1);
        target.TakeDamage(power, this);  
        target.GetHit(this);
        yield return new WaitForSeconds(.5f);
        slashFX.Play();
        yield return new WaitForSeconds(.75f);
        if (attCam != null)
        {
            attCam.m_Priority = -1;
        }
        target.TakeDamage(power, this);
        transform.position = returnPos;
        yield return new WaitForSeconds(1);

        if (spell.charges != 100)
        {
            spell.remainingCharges--;
        }
        battleC.heroIndex++;
        afterAction.Invoke();
        afterAction = null;
    }
}
