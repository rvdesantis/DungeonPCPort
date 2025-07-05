using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BugTrap : DunTrap
{
    public BugTrapNPC bugNPC;
    public DunModel bugModel0;
    public DunModel bugModel1;
    public DunModel bugModel2;

    public PlayableDirector bugIdlePlayable;

    public override void SetSideTrap(SideExtenderCube sideCube)
    {
        MonsterController monsterController = FindObjectOfType<MonsterController>();    
        bugModel0 = Instantiate(monsterController.enemyMasterList[24], sideCube.floorPoint.transform);
        bugModel1 = Instantiate(monsterController.enemyMasterList[24], sideCube.floorPoint.transform);
        bugModel2 = Instantiate(monsterController.enemyMasterList[24], sideCube.floorPoint.transform);
        base.SetSideTrap(sideCube);
        DistanceController distance = FindObjectOfType<DistanceController>();
        distance.npcS.Add(bugNPC);
        bugModel0.AssignSpecificDirector(bugIdlePlayable, 0);
        bugModel1.AssignSpecificDirector(bugIdlePlayable, 1);
        bugModel2.AssignSpecificDirector(bugIdlePlayable, 2);
        bugIdlePlayable.Play();
    }

}
