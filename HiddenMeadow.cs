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
    public DunModel activeTreant;
    public bool battleTrigger;
    public BattleRoom battleRoom;
    public float battleDistance;

    public PlayerController player;

    public void FirstEnter()
    {
        PartyController party = FindObjectOfType<PartyController>();    
        MonsterController monsters = FindAnyObjectByType<MonsterController>();
        DistanceController distance = FindObjectOfType<DistanceController>();

        if (player == null)
        {
            player = FindAnyObjectByType<PlayerController>();
        }

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

        activeTreant = Treant;

        battleRoom.battleC = FindObjectOfType<BattleController>();
    }

    private void Update()
    {
        if (fakeWall.wallBroken && !opened)
        {
            opened = true;
            FirstEnter();
            Debug.Log("Trigger Enter Hidden Meadow");
        }

        if (opened && player != null && !battleTrigger)
        {
            if (Vector3.Distance(activeTreant.transform.position, player.transform.position) < battleDistance)
            {
                battleTrigger = true;
                Debug.Log("Trigger Treant Battle");
                activeTreant.gameObject.SetActive(false);
                battleRoom.battleC.SetBattle(2);
            }
        }
    }
}
