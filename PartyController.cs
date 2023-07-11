using Cinemachine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public class PartyController : MonoBehaviour
{
    public List<DunModel> activeParty;
    public PlayerController controller;
    public List<PlayableDirector> openingPlayables;

    public void AssignCamBrain(PlayableDirector dir, int pos = 0)
    {
 
        CinemachineBrain cinemachineBrain = controller.cinBrain;

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
        foreach (DunModel activeModel in activeParty)
        {
            activeModel.gameObject.SetActive(false);
            activeModel.transform.parent = null;
        }

    }

}



