using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class HiddenMeadow : HiddenEndCube
{
    public GameObject treantSpawnPoint;
    public PlayableDirector treantEnterPlayable;
    public PlayableDirector treantCombatPlayable;
    public bool opened;

    public DunChest cornerChest;

    public GameObject volumeObject;


    public void FirstEnter()
    {
        PartyController party = FindObjectOfType<PartyController>();
        PlayerController player = FindObjectOfType<PlayerController>();
        MonsterController monsters = FindAnyObjectByType<MonsterController>();
        DistanceController distance = FindObjectOfType<DistanceController>();

        party.AssignCamBrain(treantEnterPlayable, 3);
        foreach (DunModel model in party.activeParty)
        {
            model.AssignToDirector(treantEnterPlayable);        
        }     
        DunModel Treant = Instantiate(monsters.enemyMasterList[2], treantSpawnPoint.transform.position, treantSpawnPoint.transform.rotation);
        Treant.AssignToDirector(treantEnterPlayable, 4);
        distance.chests.Add(cornerChest);
        cornerChest.gameObject.SetActive(true);
        volumeObject.SetActive(true);
        
        
    }

    private void Update()
    {
        if (fakeWall.wallBroken && !opened)
        {
            opened = true;
            FirstEnter();
            Debug.Log("Trigger Enter Hidden Meadow");
        }
    }
}
