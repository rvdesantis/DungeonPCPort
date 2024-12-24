using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class StatusController : MonoBehaviour
{
    public BattleModel attachedModel;
    public bool boost;
    public int boostAmount;
    public bool DEF;
    public int DEFboost;
    public bool burn;
    public bool freeze;
    public bool poison;
    public int poisonAmount;
    public BattleModel poisonModel;
    public bool shock;
    public bool dark;
    public BattleModel darkModel;
    public int darkCount;
    public List<ParticleSystem> statusCircleFX; // boost - 0, STR - 1, DEF - 2, BURN - 3, FREEZE - 4, POISON - 5, SHOCK - 6, Dark - 7   
    public List<AudioClip> FXSounds;
    public PlayableDirector necroPlayable;

    public void ActivateBoost(bool activate)
    {
        if (activate)
        {
            boost = true;
            statusCircleFX[0].gameObject.SetActive(true);
            statusCircleFX[0].Play();
            attachedModel.audioSource.PlayOneShot(FXSounds[0]);
        }
        if (!activate)
        {
            boost = false;
            statusCircleFX[0].Stop();
            statusCircleFX[0].gameObject.SetActive(false);
            boostAmount = 0;
        }
    }

    public void ActivateDEF(bool activate)
    {
        if (activate)
        {
            DEF = true;
            statusCircleFX[2].gameObject.SetActive(true);
            statusCircleFX[2].Play();
            attachedModel.audioSource.PlayOneShot(FXSounds[2]);
        }
        if (!activate)
        {
            DEF = false;
            statusCircleFX[2].Stop();
            statusCircleFX[2].gameObject.SetActive(false);
            DEFboost = 0;
        }
    }

    public void AddPoison(int poisonCount, BattleModel poisonSource)
    {
        poisonAmount = poisonAmount + poisonCount;
        poisonModel = poisonSource;
        statusCircleFX[5].gameObject.SetActive(true);
        statusCircleFX[5].Play();
        poison = true;
    }

    public void AddDark(int voidCount, BattleModel darkMod)
    {
        darkModel = darkMod;
        darkCount = darkCount + voidCount;
        statusCircleFX[7].gameObject.SetActive(true);
        statusCircleFX[7].Play();
    }

    public void DarkTurn()
    {
        if (darkCount != 0)
        {
            int darkChance = Random.Range(0, 10);
            Debug.Log("Dark Count = " + darkCount + " / Dark Chance = " + darkChance, gameObject);
            if (darkChance < darkCount)
            {
                StartCoroutine(DarkKill());
            }
            darkCount--;       
        }

        if (darkCount == 0)
        {
            statusCircleFX[7].Stop();
            statusCircleFX[7].gameObject.SetActive(false);
            
        }
    }

    IEnumerator DarkKill()
    {
        necroPlayable.Play();
        yield return new WaitForSeconds((float)necroPlayable.duration);
        attachedModel.actionType = BattleModel.ActionType.spell;
        attachedModel.TakeDamage(attachedModel.health, darkModel);
    }

    public void PoisonTurn()
    {
        if (poisonAmount > 0)
        {
            attachedModel.impactFX.elementalImpactFXs[2].gameObject.SetActive(true);
            attachedModel.impactFX.ElementalImpact(2);

            attachedModel.TakeDamage(poisonAmount, poisonModel);
            attachedModel.GetHit(poisonModel);
        }

        poisonAmount = poisonAmount - 10;
        if (poisonAmount <= 0)
        {
            poisonAmount = 0;
            statusCircleFX[5].Stop();
            statusCircleFX[5].gameObject.SetActive(false);
            poison = false;
        }
    }

    public void PostActionCheck()
    {
        StartCoroutine(PostStatusTimer());
    }

    IEnumerator PostStatusTimer()
    {
        PoisonTurn();
        yield return new WaitForSeconds(.25f);
        DarkTurn();
        yield return new WaitForSeconds(.25f);
    }

    public void ResetAll()
    {
        boost = false;
        DEF = false;
        burn = false;
        freeze = false;
        poison = false;
        shock = false;
        dark = false;

        boostAmount = 0;
        DEFboost = 0;
        poisonAmount = 0;
        poisonModel = null;
        darkCount = 0;
        
        IEnumerator ResetTimer()
        {
            yield return new WaitForSeconds(3);
            foreach (ParticleSystem particle in statusCircleFX)
            {
                particle.Stop();
                particle.gameObject.SetActive(false);
            }
        }
        StartCoroutine(ResetTimer());
       
    }


}
