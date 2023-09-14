using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;


public class DunModel : MonoBehaviour
{
    public Animator anim;
    public AudioSource audioSource;
    public String modelName;
    public GameObject activeWeapon;
    public List<GameObject> weaponMasterList;
    public GameObject torch;
    public enum SpawnArea { fallRoom, sideHall, smallRoom, largeRoom, massive}
    public SpawnArea spawnArea;
    public int spawnPlayableInt;

    public void AssignToDirector(PlayableDirector dir, int pos = 0, bool activeTorch = false, bool weapon = false) 
    {
        PartyController partyController = FindObjectOfType<PartyController>();
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
        }

        if (weapon)
        {
            torch.SetActive(false);
            activeWeapon.SetActive(true);
        }
    }



}
