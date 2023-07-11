using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonicSmallParent : RoomPropParent
{
    public override void EnvFill()
    {
        base.EnvFill(); // sets actives
        if (roomParent.roomType == CubeRoom.RoomType.portal)
        {
            HallStarterCube bossStarter = null;
            SceneBuilder builder = FindObjectOfType<SceneBuilder>();

            foreach (HallStarterCube starter in builder.createdStarters)
            {
                if (starter.hallType == HallStarterCube.HallType.boss)
                {
                    bossStarter = starter;
                    break;
                }
            }

            BossHallCube targetCube = bossStarter.generatedHallway[1].GetComponent<BossHallCube>();
            bosshallPortal = targetCube.bossPortal;
            SetPortal();
        }
    }
}
