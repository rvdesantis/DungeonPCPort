using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedusaTrap : DunTrap
{
    public MedusaDunModel medusaNPC;

    public override void SetSideTrap(SideExtenderCube sideCube)
    {
        MonsterController monsterController = FindObjectOfType<MonsterController>();
        DunModel medusaModel = null;
        medusaModel = Instantiate(monsterController.enemyMasterList[23], sideCube.floorPoint.transform);
        medusaNPC = medusaModel.GetComponent<MedusaDunModel>();

        base.SetSideTrap(sideCube);
        DistanceController distance = FindObjectOfType<DistanceController>();
        medusaNPC.gameObject.SetActive(true);
        distance.npcS.Add(medusaNPC);
        medusaNPC.hallPointA = sideCube.starter.generatedHallway[0].floorPoint.transform;
        medusaNPC.hallPointB = sideCube.starter.generatedHallway[sideCube.starter.generatedHallway.Count - 1].floorPoint.transform;

        medusaNPC.targetPoint = medusaNPC.hallPointA;
        medusaNPC.transform.parent = null;
    }

}
