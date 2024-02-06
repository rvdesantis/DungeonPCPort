using Cinemachine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using DTT.PlayerPrefsEnhanced;

public class PartyController : MonoBehaviour
{
    public List<DunModel> activeParty;
    public List<DunModel> masterParty;
    public PlayerController player;
    public List<PlayableDirector> openingPlayables;

    public List<BattleModel> combatParty;
    public List<BattleModel> combatMaster;

    public List<int> PowerUpCounters;
    public List<int> DEFUpCounters;
    public List<int> SpellUpCounters;

    public void AssignCamBrain(PlayableDirector dir, int pos = 0)
    {
 
        CinemachineBrain cinemachineBrain = player.cinBrain;

        if (pos == 0) // if no model assignments, set by default
        {
            PlayableBinding playableBinding = dir.playableAsset.outputs.First();
            dir.SetGenericBinding(playableBinding.sourceObject, cinemachineBrain);
        }

        if (pos == 3)
        {
            PlayableBinding playableBinding = dir.playableAsset.outputs.ElementAt(pos);
            dir.SetGenericBinding(playableBinding.sourceObject, cinemachineBrain);
        }

    }

    public void EndOpening()
    {
        foreach (PlayableDirector openingDir in openingPlayables)
        {
            openingDir.Stop();
            openingDir.gameObject.SetActive(false);
        }
        foreach (DunModel inactive in activeParty)
        {
            inactive.gameObject.SetActive(false);
        }
        foreach(DunModel setModel in masterParty)
        {
            Vector3 refresh = new Vector3(setModel.transform.position.x, 0, setModel.transform.position.z);
            setModel.transform.position = refresh;
        }
    }

    public void DungeonHeal(int healAmount = 0, bool healAll = false, int healPosition = 0)
    {
        if (healAll)
        {
            foreach (BattleModel battleModel in combatParty)
            {
                int currentHealth = EnhancedPrefs.GetPlayerPref(battleModel.modelName + "HP", battleModel.health);
                int newHealth = currentHealth + healAmount;
                if (newHealth > battleModel.maxH)
                {
                    newHealth = battleModel.maxH;
                }
                EnhancedPrefs.SetPlayerPref(battleModel.modelName + "HP", newHealth);
                battleModel.health = newHealth;
            }
            EnhancedPrefs.SavePlayerPrefs();
            player.vfxLIST[1].gameObject.SetActive(true);
            player.vfxLIST[1].Play();
        }
        else
        {
            BattleModel battleModel = combatParty[healPosition];
            int currentHealth = EnhancedPrefs.GetPlayerPref(battleModel.modelName + "HP", battleModel.health);
            int newHealth = currentHealth + healAmount;
            if (newHealth > battleModel.maxH)
            {
                newHealth = battleModel.maxH;
            }
            EnhancedPrefs.SetPlayerPref(battleModel.modelName + "HP", newHealth);
            battleModel.health = newHealth;
        }
    }

    public void LoadCounters()
    {
        foreach (DunModel partyMod in activeParty)
        {
            int x = EnhancedPrefs.GetPlayerPref(partyMod.modelName + "PowerUpCount", 0);
            int y = EnhancedPrefs.GetPlayerPref(partyMod.modelName + "DEFUpCount", 0);
            int z = EnhancedPrefs.GetPlayerPref(partyMod.modelName + "SpellUpCount", 0);

            PowerUpCounters.Add(x);
            DEFUpCounters.Add(y);
            SpellUpCounters.Add(z);
        }
    }

}



