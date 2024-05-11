using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class RootsRoomParent : RoomPropParent
{
    public GameObject entSpawnPointA;
    public GameObject entSpawnPointB;
    public PlayableDirector treantAttackPlayable;
    public List<DunModel> treants;
    public GameObject playerSpawnPoint;


    public override void EnvFill()
    {
        base.EnvFill();
        
        if (roomParent.roomType == CubeRoom.RoomType.battle || roomParent.roomType == CubeRoom.RoomType.chest || roomParent.roomType == CubeRoom.RoomType.quest)
        {
            MonsterController monsters = FindObjectOfType<MonsterController>();
            DunModel targetTree = null;

            foreach (DunModel monster in monsters.enemyMasterList)
            {
                if (monster.spawnArea == DunModel.SpawnArea.smallRoom)
                {
                    if (monster.spawnPlayableInt == 4)
                    {
                        targetTree = monster;
                        break;
                    }
                }
            }

            DunModel treantA = Instantiate(targetTree, entSpawnPointA.transform);
            DunModel treantB = Instantiate(targetTree, entSpawnPointB.transform);

            treants.Add(treantA);
            treants.Add(treantB);

            treantA.AssignToDirector(treantAttackPlayable, 4);
            treantB.AssignToDirector(treantAttackPlayable, 5);
        }
    }

    public void StartTreantAttack()
    {
        PartyController party = FindObjectOfType<PartyController>();
        PlayerController player = FindObjectOfType<PlayerController>();
        SceneController controller = FindObjectOfType<SceneController>();

        controller.activePlayable = treantAttackPlayable;
        controller.endAction = AttackFinish;

        player.enabled = false;

        
        party.AssignCamBrain(treantAttackPlayable, 3);

     
        foreach (DunModel model in party.activeParty)
        {            
            int x = party.activeParty.IndexOf(model);
            model.AssignToDirector(treantAttackPlayable);
            model.gameObject.SetActive(true);
            model.transform.parent = treantAttackPlayable.transform;
            model.transform.position = treantAttackPlayable.transform.position;
            model.transform.rotation = treantAttackPlayable.transform.rotation;
            if (model.torch != null)
            {
                model.torch.SetActive(false);
            }
            if (model.activeWeapon != null)
            {
                model.activeWeapon.SetActive(false);
            }
        }
        player.transform.position = playerSpawnPoint.transform.position;
        StartCoroutine(AttackTimer());
    }

    IEnumerator AttackTimer()
    {
        SceneController controller = FindObjectOfType<SceneController>();
        treantAttackPlayable.Play();        
        yield return new WaitForSeconds((float)treantAttackPlayable.duration);
        if (controller.activePlayable == treantAttackPlayable)
        {
            AttackFinish();
        }
    }

    public void AttackFinish()
    {
        PartyController party = FindObjectOfType<PartyController>();
     
        SceneController controller = FindObjectOfType<SceneController>();
        BattleController battleC = FindObjectOfType<BattleController>();

        controller.activePlayable = null;
        controller.endAction = null;

        battleC.afterBattleAction = BattleFinish;
        battleC.SetBattle(10);

        foreach (DunModel model in party.activeParty)
        {          
            model.gameObject.SetActive(false);
        }
        if (treasureSpawn[0].gameObject.activeSelf)
        {
            RootChest rChest = treasureSpawn[0].GetComponent<RootChest>();
            if (rChest == null)
            {
                Debug.Log("Failed to Grab Root Chest Component on treasureSpawn 0", rChest.gameObject);
            }
            rChest.rootBlocker.SetActive(false);
            rChest.locked = false;
        }
        treants[0].gameObject.SetActive(false);
        treants[1].gameObject.SetActive(false);


    }

    public void BattleFinish()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        player.enabled = true;
    }

    private void Update()
    {       
        if (roomParent.inRoom && !battleTriggered)
        {
            if (roomParent.roomType == CubeRoom.RoomType.battle || roomParent.roomType == CubeRoom.RoomType.quest)
            {
                battleTriggered = true;
                StartTreantAttack();
            }
        }
    }



}
