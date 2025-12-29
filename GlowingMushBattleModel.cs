using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Playables;

public class GlowingMushBattleModel : EnemyBattleModel
{
    public PlayableDirector poisonHop;
    public GameObject poisonDart;
    public FireMageBattleModel fireMage;

    private void Start()
    {
        anim.SetTrigger("goMonster");
        spawnPoint = transform.position;
        if (fireMage != null )
        {
            fireMage.afterAction = null;
            StartCoroutine(MageBattleStart());
        }
    }

    public IEnumerator MageBattleStart()
    {
        yield return new WaitForSeconds(2);
        fireMage.actionTarget = this;
        fireMage.selectedSpell = fireMage.activeSpells[0];
        StartCoroutine(fireMage.CastTimer(this, fireMage.activeSpells[0]));
    }

    public override void Attack(BattleModel target)
    {
        audioSource.PlayOneShot(actionSounds[2]);
        base.Attack(target);
    }

    public override void Die(BattleModel damSource)
    {
        audioSource.PlayOneShot(actionSounds[2]);
        base.Die(damSource);
    }

    public override void GetHit(BattleModel modelSource)
    {
        audioSource.PlayOneShot(actionSounds[1]);
        base.GetHit(modelSource);
    }

    public override void StartAction()
    {
        if (battleC == null)
        {
            battleC = FindAnyObjectByType<BattleController>();
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
            if (dead)
            {
                foreach (GameObject bodyPart in bodyObjects)
                {
                    bodyPart.SetActive(false);
                }
            }
            skip = false;
            battleC.enemyIndex++;
            afterAction.Invoke();
            afterAction = null;
            return;
        }
        if (!skip && !dead)
        {
            SelectRandomTarget();
            int actionNum = Random.Range(0, 2);

            if (actionNum == 0)
            {
                Attack(actionTarget);
            }

            if (actionNum == 1)
            {
                actionType = ActionType.spell;
                StartCoroutine(PoisonHop());
            }
        }
    }


    IEnumerator PoisonHop()
    {
        if (battleC == null)
        {
            battleC = FindAnyObjectByType<BattleController>();
        }
        float time = (float)poisonHop.duration;
        poisonHop.Play();
        yield return new WaitForSeconds(time + 05f);
        transform.position = battleC.enemySpawnPoints[0].transform.position;
        afterAction.Invoke();
        afterAction = null;
    }

    public void PoisonAll() // attacahed to timeline
    {
        if (battleC == null)
        {
            battleC = FindAnyObjectByType<BattleController>();
        }
        foreach (BattleModel hero in battleC.heroParty)
        {
            GameObject bolt = Instantiate(poisonDart, poisonDart.transform.position, poisonDart.transform.rotation);
            
            IEnumerator MoveBolt()
            {
                Vector3 startPosition = bolt.transform.position;
                Vector3 targetPosition = hero.hitTarget.position;
                float elapsedTime = 0;
                bolt.gameObject.SetActive(true);
                while (elapsedTime < .5f)
                {
                    bolt.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / .5f);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                bolt.transform.position = targetPosition;
                hero.statusC.AddPoison(10, this);
            }  StartCoroutine(MoveBolt());
        }
    }

}
