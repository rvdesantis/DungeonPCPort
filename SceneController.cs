using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SceneController : MonoBehaviour
{
    public SceneBuilder builder;
    public Cube sanctuary;
    public MapController mapController;
    public PlayerController playerController;
    public PartyController party;
    public DunUIController uiController;
    public DistanceController distance;
    public bool active;
    public CinemachineVirtualCamera characterCam;


    public void SceneStart()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        active = true;
        Debug.Log("Scene Controller Start");
        StartCoroutine(SceneStarter());
    }

    private IEnumerator SceneStarter()
    {
        characterCam.m_Priority = -1;
        playerController.firstPersonCam.depth = 1;
        playerController.cinPersonCam.m_Priority = 5;
        playerController.active = true;
        playerController.playerLight.enabled = true;

        party.EndOpening();

        uiController.startButtons[1].gameObject.SetActive(false);
        uiController.titleObj.SetActive(false);
        uiController.uiActive = false;
        uiController.compassObj.SetActive(true);

        Debug.Log("Importing Map Info");
        mapController.GatherMap(); 
        yield return new WaitForSeconds(.5f);
        mapController.GatherLayers();
        Debug.Log("Filling Rooms...");
        StartCoroutine(RoomFiller()); 
        // triggers SwapHallwayCubes() at end owf RoomFiller()
    }

    private IEnumerator RoomFiller()
    {
        CubeRoom targetRoom = null;
        foreach (CubeRoom room in builder.createdRooms)
        {
            if (!room.roomAssigned)
            {
                targetRoom = room;
                break;
            }
        }
        if (targetRoom != null)
        {
            // fill target room
            targetRoom.roomAssigned = true;
            int x = Enum.GetValues(typeof(CubeRoom.RoomType)).Length;
            int roomNum = UnityEngine.Random.Range(0, x);
            targetRoom.roomType = (CubeRoom.RoomType)roomNum;
            targetRoom.mapIcon.icon.sprite = mapController.iconMasterList[roomNum];

            targetRoom.FillRoom();
            yield return new WaitForSeconds(.1f);
            StartCoroutine(RoomFiller());
        }
        if (targetRoom == null)
        {
            Debug.Log("All Rooms Full.  Starting Hallways");
            StartCoroutine(SwapHallwayCubes());
        }

    }

    private IEnumerator SwapTrapCubes()
    {
        Debug.Log("Starting Traps...");
        Cube targetHall = null;
        HallStarterCube starter = null;
        List<HallStarterCube> openHalls = new List<HallStarterCube>();

        Vector3 hallPos = new Vector3(0, 0, 0);
        Quaternion hallRot = new Quaternion(0, 0, 0, 0);
        foreach (Cube starterC in builder.createdStarters)
        {
            HallStarterCube comp = starterC.GetComponent<HallStarterCube>();
            if (comp.generatedHallway.Count > 0)
            {
                if (comp.hallType != HallStarterCube.HallType.boss)
                {
                    openHalls.Add(comp);
                }
            }
        }
        int y = openHalls.Count;
        int x = UnityEngine.Random.Range(0, y);

        starter = openHalls[x];
        int z = starter.generatedHallway.Count;

        targetHall = starter.generatedHallway[UnityEngine.Random.Range(1, z - 1)];

        if (targetHall == null)
        {
            Debug.Log("Hall Cube Select Invalid");
        }
        if (targetHall.filled || targetHall.cubeType == Cube.CubeType.bHallway)
        {
            targetHall = null;
            StartCoroutine(SwapTrapCubes());  
        }
        if (targetHall != null)
        {
            if (!targetHall.filled || targetHall.cubeType == Cube.CubeType.hallway)
            {
                if (targetHall.FallBoxChecker())
                {
                    Debug.Log("Fall Box Checker True, rerolling hallway");
                    targetHall = null;
                    StartCoroutine(SwapTrapCubes());
                }
                else
                {
                    hallPos = targetHall.transform.position;
                    hallRot = targetHall.transform.rotation;
                    TrapHallCube trapHallCube = Instantiate(builder.trapHallCube, hallPos, hallRot);
                    builder.createdTrapHalls.Add(trapHallCube);
                    starter.generatedHallway.Add(trapHallCube);
                    trapHallCube.filled = true;
                    foreach (GameObject tube in trapHallCube.fallTubes)
                    {
                        tube.SetActive(true);
                    }
                    trapHallCube.fallRoom.SetActive(true);
                    distance.fakeFloors.Add(trapHallCube.fakeFloor);        
                    targetHall.filled = true;
                    int zz = builder.createdTrapHalls.Count;
                    targetHall.gameObject.SetActive(false);

                    yield return new WaitForSeconds(.1f);
                    if (zz < (builder.createdHallCubes.Count / 30))
                    {
                        StartCoroutine(SwapTrapCubes());
                    }
                    if (zz == (builder.createdHallCubes.Count / 30))
                    {
                        Debug.Log("Trap Swap Finished");
                        foreach (GameObject fog in sanctuary.fogWalls)
                        {
                            fog.GetComponent<ParticleSystem>().Stop();
                            fog.GetComponent<BoxCollider>().enabled = false;
                            playerController.audioSource.PlayOneShot(playerController.audioClips[0]);
                        }
                    }
                }
            }
        }
    }

    private IEnumerator SwapHallwayCubes() // lowers fog walls at end
    {
        Cube targetHall = null;
        HallStarterCube starter = null;
        List<HallStarterCube> openHalls = new List<HallStarterCube>();

        Vector3 hallPos = new Vector3(0, 0, 0);
        Quaternion hallRot = new Quaternion(0, 0, 0, 0);

        bool left = false;
        bool right = false;

        foreach (Cube starterC in builder.createdStarters)
        {
            HallStarterCube comp = starterC.GetComponent<HallStarterCube>();
            if (comp.generatedHallway.Count > 0)
            {
                if (comp.hallType != HallStarterCube.HallType.boss)
                {
                    openHalls.Add(comp);
                }
            }
        }
        int y = openHalls.Count;
        int x = UnityEngine.Random.Range(0, y);

        starter = openHalls[x];

        int z = starter.generatedHallway.Count;

        targetHall = starter.generatedHallway[UnityEngine.Random.Range(1, z - 1)];

        if (targetHall == null)
        {
            Debug.Log("Hall Cube Select Invalid");
        }
        if (targetHall.filled || targetHall.cubeType == Cube.CubeType.bHallway)
        {
            StartCoroutine(SwapHallwayCubes());
            targetHall = null;
        }
        if (targetHall != null && !targetHall.filled)
        {
            if (targetHall.LeftBoxChecker())
            {
                left = true;
                Debug.Log("TargetHall Left Box Collision");
            }
            if (targetHall.RightBoxChecker())
            {
                right = true;
                Debug.Log("TargetHall Left Box Collision");
            }

            if (!left || !right)
            {
                hallPos = targetHall.transform.position;
                hallRot = targetHall.transform.rotation;
                SideExtenderCube sideExtender = Instantiate(builder.sideCubeExtender, hallPos, hallRot);
                builder.createdHallSideCubes.Add(sideExtender);
                starter.generatedHallway.Add(sideExtender);
                sideExtender.filled = true;
                targetHall.filled = true;
                int zz = builder.createdHallSideCubes.Count;
                targetHall.gameObject.SetActive(false);

                if (!left && !right)
                {
                    int dice = UnityEngine.Random.Range(0, 2);
                    if (dice == 0)
                    {
                        sideExtender.SetWalls();
                    }
                    if (dice == 1)
                    {
                        sideExtender.SetWalls(false);
                    }
                }
                if (!left && right)
                {
                    sideExtender.SetWalls();
                }
                if (left && !right)
                {
                    sideExtender.SetWalls(false);
                }
                yield return new WaitForSeconds(.1f);
                if (zz < (builder.createdHallCubes.Count / 20))
                {
                    StartCoroutine(SwapHallwayCubes());
                }
                if (zz == (builder.createdHallCubes.Count / 20))
                {
                    Debug.Log("Hallway Swap Finished, Starting Traps");
                    StartCoroutine(SwapTrapCubes());

                }
            }
            if (left && right)
            {
                Debug.Log("both L and R blocked, restarting side loop");
                StartCoroutine(SwapHallwayCubes());                
            }
        }
        
    }
}
