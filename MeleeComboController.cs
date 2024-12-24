using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class MeleeComboController : MonoBehaviour
{
    public BattleController battleC;
    public int comboBoost;
    public List<PlayableDirector> comboPlayables;
    public bool triggered;

    public void StartMeleeCombo(int comboNum)
    {
        StartCoroutine(MeleeComboTimer(comboNum));
    }

    IEnumerator MeleeComboTimer(int ComboNum)
    {
        Debug.Log("MeleeComboTimer Start");
        BattleModel hero0 = battleC.heroParty[0];
        BattleModel hero1 = battleC.heroParty[1];
        BattleModel hero2 = battleC.heroParty[2];

        BattleModel comboTarget = hero0.actionTarget;

        PlayableDirector comboPlayable = comboPlayables[ComboNum];
        comboPlayable.transform.position = battleC.activeRoom.playerSpawnPoints[0].transform.position;
        comboPlayable.transform.rotation = battleC.activeRoom.playerSpawnPoints[0].transform.rotation;

        foreach (BattleModel mod in battleC.heroParty)
        {
            mod.transform.parent = comboPlayable.transform;
            mod.transform.position = comboPlayable.transform.position;
        }
        foreach (BattleModel enMod in battleC.enemyParty)
        {
            if (enMod != comboTarget)
            {
                enMod.gameObject.SetActive(false);
            }
            if (enMod == comboTarget)
            {
                Vector3 backwardMovement = -battleC.activeRoom.enemySpawnPoints[0].transform.forward * enMod.strikeDistance;
                enMod.transform.position = battleC.activeRoom.enemySpawnPoints[0].transform.position;
                enMod.transform.position += backwardMovement;
            }
        }


        comboPlayable.Play();
        yield return new WaitForSeconds((float)comboPlayable.duration + .1f);


        foreach (BattleModel enMod in battleC.enemyParty)
        {
            int x = battleC.enemyParty.IndexOf(enMod);
            enMod.transform.position = battleC.activeRoom.enemySpawnPoints[x].transform.position;
            enMod.gameObject.SetActive(true);
        }
        float powerAdjusted0 = (float)hero0.powerBonusPercent / 100f + 1f;
        float powerX0 = (powerAdjusted0 * hero0.power) + hero0.statusC.boostAmount;

        float powerAdjusted1 = (float)hero1.powerBonusPercent / 100f + 1f;
        float powerX1 = (powerAdjusted1 * hero1.power) + hero1.statusC.boostAmount;

        float powerAdjusted2 = (float)hero2.powerBonusPercent / 100f + 1f;
        float powerX2 = (powerAdjusted2 * hero2.power) + hero2.statusC.boostAmount;

        float totalDam = powerX0 + powerX1 + powerX2;
        int damage = Mathf.RoundToInt(totalDam) + comboBoost;

        comboTarget.TakeDamage(damage, hero0);


        foreach (BattleModel mod in battleC.heroParty)
        {
            int x = battleC.heroParty.IndexOf(mod);
            mod.transform.parent = battleC.activeRoom.playerSpawnPoints[x].transform;
            mod.transform.position = battleC.activeRoom.playerSpawnPoints[x].transform.position;
            mod.statusC.ActivateBoost(false);
        }
        battleC.StartPostHeroTimer();
    }

}
