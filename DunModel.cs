
using System;

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using DTT.PlayerPrefsEnhanced;


public class DunModel : MonoBehaviour
{
    public Animator anim;
    public AudioSource audioSource;
    public String modelName;
    public String modelLore;
    public String modelInfo;
    
    public enum ModelClass { Warrior, Rogue, Mage}
    public ModelClass modelClass;
    public GameObject activeWeapon;
    public List<GameObject> weaponMasterList;
    public GameObject torch;
    public enum SpawnArea { fallRoom, sideHall, smallRoom, largeRoom, massive, sanct, end}
    public SpawnArea spawnArea;
    public int spawnPlayableInt;

    public void AssignToDirector(PlayableDirector dir, int pos = 0, bool activeTorch = false, bool weapon = false) 
    {
        PartyController partyController = FindAnyObjectByType<PartyController>();
        int posNum = 0;
        if (pos == 0)
        {
            posNum = partyController.activeParty.IndexOf(this);
        }
        if (pos > 2)
        {
            posNum = pos;
        }
    

        Debug.Log("Assigning " + modelName + " to position " + posNum);

        PlayableBinding playableBinding = dir.playableAsset.outputs.ElementAt(posNum);
        dir.SetGenericBinding(playableBinding.sourceObject, anim);

        if (activeTorch)
        {
            activeWeapon.SetActive(false);
            torch.SetActive(true);
            torch.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        if (weapon)
        {
            torch.SetActive(false);
            activeWeapon.SetActive(true);
        }
    }

    public void AssignSpecificDirector(PlayableDirector dir, int pos = 0, bool activeTorch = false, bool weapon = false)
    {
        PartyController partyController = FindAnyObjectByType<PartyController>();
        int posNum = pos;
        Debug.Log("Assigning " + modelName + " to position " + posNum);

        PlayableBinding playableBinding = dir.playableAsset.outputs.ElementAt(posNum);
        dir.SetGenericBinding(playableBinding.sourceObject, anim);

        if (activeTorch)
        {
            activeWeapon.SetActive(false);
            torch.SetActive(true);
            torch.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        if (weapon)
        {
            torch.SetActive(false);
            activeWeapon.SetActive(true);
        }
    }

    public void SaveXP(int XP)
    {
        EnhancedPrefs.SetPlayerPref(modelName + "XP", XP);
        EnhancedPrefs.SavePlayerPrefs();
    }


}
