using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TreantBattleModel : EnemyBattleModel
{
    public PlayableDirector introPlayable;
    public List<PlayableDirector> meleePlayables;
    public List<PlayableDirector> throwPlayables;
    public GameObject handParent;
    public bool pickedUp;
    public CinemachineVirtualCamera throwCam;


    public override void StartAction()
    {
        if (battleC == null)
        {
            battleC = FindObjectOfType<BattleController>();
        }
        if (throwCam == null)
        {
            throwCam = battleC.activeRoom.targetingCams[3];
            throwCam.transform.position = battleC.activeRoom.mainCam.transform.position;
            throwCam.transform.rotation = battleC.activeRoom.mainCam.transform.rotation;
            throwCam.m_LookAt = handParent.transform;
        }
        Debug.Log("StartAction() started for enemy " + battleC.enemyIndex);
        if (battleC.enemyIndex == 0) // works for Enemy side
        {
            afterAction = battleC.enemyParty[1].StartAction;
        }
        if (battleC.enemyIndex == 1)
        {
            afterAction = battleC.enemyParty[2].StartAction;
        }
        if (battleC.enemyIndex == 2)
        {
            afterAction = battleC.StartPostEnemyTimer;
        }

        if (skip || dead || DeadEnemiesCheck())
        {
            skip = false;
            battleC.enemyIndex++;
            afterAction.Invoke();
            afterAction = null;
            return;
        }
        if (!skip && !dead)
        {
            SelectRandomTarget();
            int act = Random.Range(0, 2);

            if (act == 0)
            {
                Attack(actionTarget);
            }
            if (act == 1)
            {
                ThrowAttack(actionTarget);
            }

        }
    }

    public override void Attack(BattleModel target)
    {
        Debug.Log("Starting " + modelName + " Attack, Targeting " + target.modelName);
        actionTarget = target;
        battleC.enemyIndex++;
        StartCoroutine(EnemyAttackTimer());
    }

    public void ThrowAttack(BattleModel target)
    {
        Debug.Log("Starting " + modelName + " Attack, Targeting " + target.modelName);
        actionTarget = target;
        battleC.enemyIndex++;
        StartCoroutine(ThrowTimer());
    }


    IEnumerator ThrowTimer()
    {
        int heroIndex = battleC.heroParty.IndexOf(actionTarget);
        if (actionTarget.verticalHitBuffer == 0)
        {
            transform.LookAt(actionTarget.transform);
        }
        if (heroIndex == 0)
        {
            throwPlayables[0].Play();
        }
        if (heroIndex == 1)
        {
            throwPlayables[1].Play();
        }
        if (heroIndex == 2)
        {
            throwPlayables[2].Play();
        }
        yield return new WaitForSeconds((float)throwPlayables[0].duration);
        afterAction.Invoke();
        afterAction = null;
    }

    IEnumerator EnemyAttackTimer()
    {
        if (!dead)
        {
            int heroIndex = battleC.heroParty.IndexOf(actionTarget);

            float xFL = strikeDistance + actionTarget.hitbuffer;
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
            if (heroIndex == 0)
            {
                meleePlayables[0].Play();
            }
            if (heroIndex == 1)
            {
                meleePlayables[1].Play();
            }
            if (heroIndex == 2)
            {
                meleePlayables[2].Play();
            }
            yield return new WaitForSeconds((float)meleePlayables[0].duration);

            // damage attached to animation in timeline for strike sync

            /*
            float defAdjusted = (float)actionTarget.defBonusPercent / 100f + 1f;
            float defX = defAdjusted * actionTarget.def;

            float powerAdjusted = (float)powerBonusPercent / 100f + 1f;
            float powerX = powerAdjusted * power;
            float powerStatusBoost = 0f;
            if (statusC.boost)
            {
                powerStatusBoost = statusC.boostAmount;
            }
            powerX = powerX + powerStatusBoost;
            int damageAmount = Mathf.RoundToInt(powerX) - Mathf.RoundToInt(defX);
            if (damageAmount < 0)
            {
                damageAmount = 0;
            }

            Debug.Log(modelName + " attacking " + actionTarget.modelName + " for " + damageAmount + " damage");
            actionTarget.TakeDamage(damageAmount, this);

            transform.position = returnPos;
            */
        }
        afterAction.Invoke();
        afterAction = null;
    }


    public void PunchDamage()
    {
        float defAdjusted = (float)actionTarget.defBonusPercent / 100f + 1f;
        float defX = defAdjusted * actionTarget.def;

        float powerAdjusted = (float)powerBonusPercent / 100f + 1f;
        float powerX = powerAdjusted * power;
        float powerStatusBoost = 0f;
        if (statusC.boost)
        {
            powerStatusBoost = statusC.boostAmount;
        }
        powerX = powerX + powerStatusBoost;
        int damageAmount = Mathf.RoundToInt(powerX) - Mathf.RoundToInt(defX);
        if (damageAmount < 0)
        {
            damageAmount = 0;
        }

        Debug.Log(modelName + " attacking " + actionTarget.modelName + " for " + damageAmount + " damage");
        actionTarget.GetHit(this);
        actionTarget.impactFX.StandardImpact();
        actionTarget.TakeDamage(damageAmount, this);
    }

    public void PickUp()
    {
        actionTarget.transform.SetParent(handParent.transform);
        actionTarget.transform.position = handParent.transform.position;
        actionTarget.anim.SetBool("floating", true);
        actionTarget.activeWeapon.gameObject.SetActive(false);
        throwCam.m_Priority = 20;
        pickedUp = true;
    }

    public void Throw()
    {
        throwCam.m_LookAt = actionTarget.hitTarget.transform;

        pickedUp = false;
        actionTarget.transform.SetParent(null);
        actionTarget.anim.SetBool("floating", false);
        IEnumerator MoveOverTime()
        {
            float elapsedTime = 0;
            float duration = 0.4f;

            Vector3 startPosition = actionTarget.transform.position;
            Quaternion startRotation = actionTarget.transform.rotation;
            Vector3 endPosition = actionTarget.spawnPoint;
            Quaternion endRotation = Quaternion.LookRotation(transform.up);

            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                actionTarget.transform.position = Vector3.Lerp(startPosition, endPosition, t);
                actionTarget.transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            actionTarget.transform.position = endPosition;
            actionTarget.transform.rotation = endRotation;

            actionTarget.transform.LookAt(transform.position);
            int throwDamage = power - actionTarget.def;
            if (throwDamage < 0)
            {
                throwDamage = 0;
            }

            throwCam.m_LookAt = null;
            throwCam.transform.position = battleC.activeRoom.mainCam.transform.position;
            throwCam.transform.rotation = battleC.activeRoom.mainCam.transform.rotation;
            throwCam.m_Priority = -5;

            actionTarget.GetHit(this);
            actionTarget.TakeDamage(throwDamage, this);
            actionTarget.impactFX.StandardImpact();
        }
        StartCoroutine(MoveOverTime());

    }

    public void Update()
    {
        if (pickedUp)
        {
            actionTarget.transform.position = handParent.transform.position;
        }
    }
}
