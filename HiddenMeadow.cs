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

    public GameObject treasureSpawn0;
    public GameObject treasureSpawn1;
    public GameObject treasureSpawn2;

    public GameObject volumeObject;


    public void FirstEnter()
    {
        PartyController party = FindObjectOfType<PartyController>();
        PlayerController player = FindObjectOfType<PlayerController>();
        MonsterController monsters = FindAnyObjectByType<MonsterController>();

        party.AssignCamBrain(treantEnterPlayable, 3);
        foreach (DunModel model in party.activeParty)
        {
            model.AssignToDirector(treantEnterPlayable);        
        }     
        DunModel Treant = Instantiate(monsters.enemyMasterList[2], treantSpawnPoint.transform.position, treantSpawnPoint.transform.rotation);
        Treant.AssignToDirector(treantEnterPlayable, 4);

        volumeObject.SetActive(true);
    }

    private void Update()
    {
        if (fakeWall.wallBreak && !opened)
        {
            opened = true;
            FirstEnter();
            Debug.Log("Trigger Enter Hidden Meadow");
        }
    }
}
