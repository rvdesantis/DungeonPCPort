using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedusaBattleModel : EnemyBattleModel
{
    public float desiredHairWeight = 1.0f;                  // Weight of hair animations
    public float hairWeightChangeSpeed = 5f;                // Change per second
    public List<ParticleSystem> eyeParticles;

    public void SetDesiredHairWeight(float newValue)
    {
        desiredHairWeight = newValue;
    }
    public override void StartAction()
    {
        if (battleC == null)
        {
            battleC = FindObjectOfType<BattleController>();
        }

        if (battleC.enemyIndex == 0) // works for Enemy side
        {
            afterAction = null;
            afterAction = battleC.enemyParty[1].StartAction;
        }
        if (battleC.enemyIndex == 1)
        {
            afterAction = null;
            afterAction = battleC.enemyParty[2].StartAction;
        }
        if (battleC.enemyIndex == 2)
        {
            afterAction = null;
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
            int action = Random.Range(0, 2);
            if (action == 0)
            {
                Debug.Log("Bone Dragon Attack");
                Attack(actionTarget);
            }
            if (action == 1)
            {
                bool unfrozenHero = false;
                if (!actionTarget.statusC.freeze)
                {
                    Debug.Log("Medusa Cast Start");
                    Cast(actionTarget);
                }
                if (actionTarget.statusC.freeze)
                {
                    foreach (BattleModel hero in battleC.heroParty)
                    {
                        if (!hero.statusC.freeze && hero.statusC.freezeCount == 0)
                        {
                            unfrozenHero = true;
                            actionTarget = hero;
                            break;
                        }
                    }
                    if (unfrozenHero)
                    {
                        Debug.Log("Medusa Cast Start");
                        Cast(actionTarget);
                    }
                    if (!unfrozenHero)
                    {
                        Debug.Log("Bone Dragon Attack");
                        Attack(actionTarget);
                    }
                }
            }
        }
    }
    

    public override void Cast(BattleModel target, Spell spell = null)
    {
        battleC.enemyIndex++;
        StartCoroutine(FreezeTimer());
    }


    public IEnumerator FreezeTimer()
    {

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
        anim.SetTrigger("taunt");
        yield return new WaitForSeconds(1.5f);

        foreach(ParticleSystem eyePart in eyeParticles)
        {
            if (!eyePart.gameObject.activeSelf)
            {
                eyePart.gameObject.SetActive(true);
            }
            eyePart.Play();
        }
        if (attCam != null)
        {
            attCam.m_Priority = 20;
        }
        actionTarget.GetHit(this);
        actionTarget.skip = true;
        actionTarget.stoneSelf.statusC.freezeCount = 2;
        actionTarget.anim.SetBool("stoneOnHit", true);
        actionTarget.anim.SetTrigger("hit");
        yield return new WaitForSeconds(1);

        afterAction.Invoke();
        afterAction = null;
        transform.position = spawnPoint;
        transform.LookAt(battleC.activeRoom.playerSpawnPoints[0].transform);
    }

    private void Update()
    {
        if (anim.GetLayerWeight(1) != desiredHairWeight)
        {
            anim.SetLayerWeight(1, Mathf.MoveTowards(anim.GetLayerWeight(1), desiredHairWeight, hairWeightChangeSpeed * Time.deltaTime));
        }
    }
}
