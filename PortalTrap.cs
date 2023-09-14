using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTrap : MonoBehaviour
{
    public SanctuaryCube sanct;
    public BossHallCube bossHall;
    public SceneController controller;
    public SceneBuilder builder;
    public DistanceController distance;
    public List<DunPortal> randomPortals;

    public bool roomActive;

    void Start()
    {
        controller = FindObjectOfType<SceneController>();
        builder = FindObjectOfType<SceneBuilder>();
        sanct = controller.sanctuary.GetComponent<SanctuaryCube>();
        distance = FindObjectOfType<DistanceController>();

        foreach(HallStarterCube starter in builder.createdStarters)
        {
            if (starter.hallType == HallStarterCube.HallType.boss)
            {
                bossHall = starter.generatedHallway[1].GetComponent<BossHallCube>();
                break;
            }
        }

        AssignPortals();
    }

    public void AssignPortals()
    {
        foreach(DunPortal portal in randomPortals)
        {
            portal.sceneController = controller;
            portal.closeOnJump = true;
        }
        randomPortals[0].ConnectPortals(sanct.returnPortal);

        // random portal 1 between boss, or attack events
        randomPortals[1].ConnectPortals(bossHall.bossPortal.GetComponent<DunPortal>());

        // random portal 2 random between treasure room or hidden room portal

        List<DunPortal> mystPortals = new List<DunPortal>();
        mystPortals.Add(sanct.eventCubes.enterPortalSMRoom); // SM Sact Event Room set first to activate environment if triggered.  
        foreach (Cube endCube in builder.createdSecretEnds)
        {
            HiddenEndCube hidden = endCube.GetComponent<HiddenEndCube>();
            if (hidden != null)
            {
                if (hidden.secretPortal != null)
                {
                    mystPortals.Add(hidden.secretPortal);
                }
            }
        }

        int mystNum = Random.Range(0, mystPortals.Count);
        if (mystNum == 0)
        {
            sanct.eventCubes.SetTreasureRoom(randomPortals[2]);
            randomPortals[2].ConnectPortals(sanct.eventCubes.enterPortalSMRoom);
        }
        if (mystNum != 0)
        {
            randomPortals[2].ConnectPortals(mystPortals[mystNum]);
        }

        foreach (DunPortal portal in randomPortals)
        {
            portal.gameObject.SetActive(true);
            portal.closeOnJump = true;
            if (!portal.connectedPortal.gameObject.activeSelf)
            {
                portal.connectedPortal.gameObject.SetActive(true);
            }
            distance.portals.Add(portal);
        }
    }

    private void Update()
    {
        if (randomPortals[2].gameObject.activeSelf)
        {
            roomActive = true;
        }

        if (roomActive)
        {
            bool closeRoom = false;
            foreach (DunPortal randomPort in randomPortals)
            {
                if (!randomPort.gameObject.activeSelf)
                {
                    closeRoom = true;
                    break;
                }
            }

            if (closeRoom)
            {
                foreach (DunPortal randomPort in randomPortals)
                {
                    randomPort.gameObject.SetActive(false);                    
                }
                roomActive = false;
            }
        }

        
    }
}
