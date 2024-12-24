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
    public List<int> combatHealthTracker;

    public List<int> PowerUpCounters;
    public List<int> DEFUpCounters;
    public List<int> SpellUpCounters;

    public void AssignCamBrain(PlayableDirector dir, int pos = 0)
    {
 
        CinemachineBrain cinemachineBrain = player.cinBrain;
        PlayableBinding playableBinding = dir.playableAsset.outputs.ElementAt(pos);
        dir.SetGenericBinding(playableBinding.sourceObject, cinemachineBrain);
        Debug.Log("CamBrain assigned to slot " + pos);

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

        combatHealthTracker[0] = combatParty[0].health;
        combatHealthTracker[1] = combatParty[1].health;
        combatHealthTracker[2] = combatParty[2].health;
    }

    public void DungeonHeal(int healAmount = 0, bool healAll = false, int healPosition = 0)
    {
        PartyController party = FindObjectOfType<PartyController>();
        if (healAll)
        {
            foreach (BattleModel battleModel in combatParty)
            {
                int index = combatParty.IndexOf(battleModel);
                int currentHealth = party.combatHealthTracker[index];
                int newHealth = currentHealth + healAmount;
                if (newHealth > battleModel.maxH)
                {
                    newHealth = battleModel.maxH;
                }

                party.combatHealthTracker[index] = newHealth;
            }
   
            player.vfxLIST[1].gameObject.SetActive(true);
            player.vfxLIST[1].Play();
        }
        else
        {
            BattleModel battleModel = combatParty[healPosition];
            int currentHealth = party.combatHealthTracker[healPosition];
            int newHealth = currentHealth + healAmount;
            if (newHealth > battleModel.maxH)
            {
                newHealth = battleModel.maxH;
            }

            party.combatHealthTracker[healPosition] = newHealth;
        }
    }

    public void LoadCounters()
    {
        foreach (DunModel partyMod in activeParty)
        {
            int x = EnhancedPrefs.GetPlayerPref(partyMod.modelName + "PowerUpCount", 0);
            Debug.Log(partyMod.modelName + " PowerUpCount = " + x);
            int y = EnhancedPrefs.GetPlayerPref(partyMod.modelName + "DEFUpCount", 0);
            Debug.Log(partyMod.modelName + " DEFUpCount = " + y);
            int z = EnhancedPrefs.GetPlayerPref(partyMod.modelName + "SpellUpCount", 0);
            Debug.Log(partyMod.modelName + " SpellUpCount = " + z);

            PowerUpCounters.Add(x);
            DEFUpCounters.Add(y);
            SpellUpCounters.Add(z);
        }
    }

}



