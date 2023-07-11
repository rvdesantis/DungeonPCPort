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
    public GameObject bosshallPortal; // portal b
    public DunPortal portB;
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

    public virtual void EnvFill()
    {
        StartCoroutine(SetActives());
        AvailableWall();
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
        portB = bosshallPortal.GetComponent<DunPortal>();
        if (distanceController == null)
        {
            distanceController = FindAnyObjectByType<DistanceController>();
        }
        distanceController.portals.Add(portA);

        portA.connectedPortal = portB;
    }
}
