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

    public void AssignToDirector(PlayableDirector dir)
    {
        PartyController partyController = FindObjectOfType<PartyController>();
        int posNum = partyController.activeParty.IndexOf(this);

        Debug.Log("Assigning " + modelName + " to position " + posNum);

        PlayableBinding playableBinding = dir.playableAsset.outputs.ElementAt(posNum);
        dir.SetGenericBinding(playableBinding.sourceObject, anim);
        
    }



}
