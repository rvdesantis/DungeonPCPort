using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenCrypt : HiddenEndCube
{
    public DunPortal enterPortal;
    public DunPortal exitPortal;
    bool swapCheck;

    public override void SecretSetUp()
    {
        enterPortal.ConnectPortals(exitPortal);
        SceneBuilder builder = FindObjectOfType<SceneBuilder>();
        if (distanceC == null)
        {
            distanceC = builder.sceneController.distance;
        }
        distanceC.portals.Add(enterPortal);
        distanceC.portals.Add(exitPortal);
    }

    public void FixedUpdate()
    {
        if (fakeWall.wallBroken && !swapCheck)
        {
            if (exitPortal.inRange)
            {
                if (!swapCheck)
                {
                    swapCheck = true;
                    exitPortal.ConnectPortals(enterPortal);
                    exitPortal.SwapOnJump();
                }
            }
        }
    }
}
