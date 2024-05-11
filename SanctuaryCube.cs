using System.Collections;
using System.Collections.Generic;
using DTT.PlayerPrefsEnhanced;
using UnityEngine;
using UnityEngine.Playables;

public class SanctuaryCube : Cube
{
    public DunPortal returnPortal;
    public EventCube eventCubes;
    public GameObject campFire;

    public PlayerController player;
    public DunUIController uiController;
    public UnlockController unlockables;
    public CampfireUI campfireUI;
    public List<DunNPC> sanctVendors;
    public List<PlayableDirector> sanctPlayables;




    private void Update()
    {

    }
}
