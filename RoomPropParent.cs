using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPropParent : MonoBehaviour
{
    public DistanceController distanceController;
    public CubeRoom roomParent;
    public GameObject openWall;
    public GameObject openWallSpawnPoint;
    public List<GameObject> inactiveList;
    public List<GameObject> activeList;

    public List<GameObject> agentSpawn;
    public List<GameObject> NPCSpawn;
    public List<GameObject> portalSpawn;
    public List<DunChest> treasureSpawn;
    public List<GameObject> portalAList;
    public List<AudioDistance> audioDistanceList;
    public DunPortal portA;
    public GameObject portbGameObject; // portal b
    public DunPortal portB;

    public float portalBossReq;
    public float portalWinReq;

    public bool active;
    
    public void AvailableWall()
    {
        int x = 0;
        foreach (GameObject cover in roomParent.wallCovers)
        {
            if (cover.activeSelf)
            {
                x = roomParent.wallCovers.IndexOf(cover);
                break;
            }
        }
        openWallSpawnPoint = roomParent.wallSpawnPoints[x];
    }

    public GameObject PortalShuffle() // odds set by portalBossRequired & portalWinRequired
    {
        GameObject randomPortal = null;
        List<GameObject> openPorts = new List<GameObject>();
        HallStarterCube bossStarter = null;
        SceneController controller = FindObjectOfType<SceneController>();
        SceneBuilder builder = FindObjectOfType<SceneBuilder>();
        SanctuaryCube sanct = builder.sanctuary.GetComponent<SanctuaryCube>();


        int portalRoll = Random.Range(0, 100);
        if (portalRoll >= portalWinReq)
        {
            if (!sanct.eventCubes.treasureChest.opened)
            {
                openPorts.Add(sanct.eventCubes.enterPortalSMRoom.gameObject);
            }
            foreach (Cube endCube in controller.builder.createdSecretEnds)
            {
                HiddenEndCube hidden = endCube.GetComponent<HiddenEndCube>();
                if (hidden != null)
                {
                    if (hidden.secretPortal != null)
                    {
                        openPorts.Add(hidden.secretPortal.gameObject);
                    }
                }
            }
        }
        if (portalRoll < portalWinReq && portalRoll >= portalBossReq)
        {
            foreach (HallStarterCube starter in builder.createdStarters)
            {
                if (starter.hallType == HallStarterCube.HallType.boss)
                {
                    bossStarter = starter;
                    break;
                }
            }
            BossHallCube targetCube = bossStarter.generatedHallway[1].GetComponent<BossHallCube>();
            openPorts.Add(targetCube.bossPortal);
        }
        if (portalRoll < portalBossReq)
        {
            openPorts.Add(sanct.returnPortal.gameObject);
        }
        
        int ranPortalNum = Random.Range(0, openPorts.Count);
        randomPortal = openPorts[ranPortalNum];

        if (randomPortal == sanct.eventCubes.enterPortalSMRoom)
        {
            sanct.eventCubes.SetTreasureRoom(randomPortal.GetComponent<DunPortal>(), true);
        }

        return randomPortal;
    }

    public virtual void EnvFill()
    {
        StartCoroutine(SetActives());
        AvailableWall();
        roomParent.activeENV = this;
        if (roomParent.roomType == CubeRoom.RoomType.portal)
        {

        }
        if (roomParent.roomType == CubeRoom.RoomType.NPC)
        {
            int count = NPCSpawn.Count;
            if (count > 0)
            {
                int xx = Random.Range(0, count);
                NPCSpawn[xx].SetActive(true);

                if (distanceController == null)
                {
                    distanceController = FindAnyObjectByType<DistanceController>();
                }

                DunNPC npc = NPCSpawn[xx].GetComponent<DunNPC>();
                distanceController.npcS.Add(npc);
            }
        }
    }

    IEnumerator SetActives()
    {
        yield return new WaitForSeconds(.0f);
        foreach (GameObject obj in inactiveList)
        {
            if (obj.activeSelf)
            {
                obj.SetActive(false);
            }
        }
        foreach (GameObject obj in activeList)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
            }
        }   
    }


    public void SetPortal()
    {
        int x = 0;
        foreach (GameObject wall in roomParent.wallCovers)
        {
            if (wall.activeSelf)
            {
                x = roomParent.wallCovers.IndexOf(wall);
            }
        }
        portA = portalAList[x].GetComponent<DunPortal>();
        portB = portbGameObject.GetComponent<DunPortal>();
        if (distanceController == null)
        {
            distanceController = FindAnyObjectByType<DistanceController>();
        }
        distanceController.portals.Add(portA);
        portA.ConnectPortals(portB);
        portA.gameObject.SetActive(true);
        portB.gameObject.SetActive(true);

    }
}
