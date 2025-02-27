using System.Collections;
using System.Collections.Generic;
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
    public int bossHallway;
    public List<GameObject> hallwaySigns;

    public void ProcessSigns()
    {
        if (bossHallway == 0)
        {
            hallwaySigns[0].gameObject.SetActive(false);
            hallwaySigns[3].gameObject.SetActive(true);
        }

        if (bossHallway == 1)
        {
            hallwaySigns[1].gameObject.SetActive(false);
            hallwaySigns[4].gameObject.SetActive(true);
        }

        if (bossHallway == 2)
        {
            hallwaySigns[2].gameObject.SetActive(false);
            hallwaySigns[5].gameObject.SetActive(true);
        }

    }

    private void Update()
    {

    }
}
