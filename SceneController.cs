using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Playables;

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
    public PlayableDirector activePlayable;
    public Action endAction;

    public void SceneStart()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        active = true;
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

        uiController.loadingBar.skullSlider.value = .5f;
        uiController.loadingBar.text.text = "Building Map...";
        mapController.GatherMap(); 
        yield return new WaitForSeconds(.1f);
        mapController.GatherLayers();
        yield return new WaitForSeconds(.1f);
        Debug.Log("Filling Rooms...");
        StartCoroutine(RoomFiller()); 
        // triggers SwapHallwayCubes() at end owf RoomFiller()
    }

    private IEnumerator RoomFiller()
    {
        uiController.loadingBar.skullSlider.value = .6f;
        uiController.loadingBar.text.text = "Filling Rooms...";
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
            int roomTypeNum = UnityEngine.Random.Range(0, x);

            // portal distance check
            if (roomTypeNum == x - 1)
            {
                int roomIndex = builder.createdRooms.IndexOf(targetRoom);
                if (roomIndex == 0 || roomIndex == 1 || roomIndex == 2) // checks to make sure room isn't connected to Sanctuary
                {
                    Debug.Log("Portal closed, too close to Sanctuary.  Set to Battle");
                    roomTypeNum = 1;
                }

                Cube bossRoom = builder.createdBossRooms[0];
                if (Vector3.Distance(targetRoom.transform.position, bossRoom.transform.position) < 60)
                {
                    Debug.Log("Portal closed, too close to Boss Room.  Set to Battle");
                    roomTypeNum = 1;
                }
            }

            targetRoom.roomType = (CubeRoom.RoomType)roomTypeNum;
            targetRoom.mapIcon.icon.sprite = mapController.iconMasterList[roomTypeNum];
            distance.rooms.Add(targetRoom);
            targetRoom.FillRoom(); // portal room set to last option for distance check
            yield return new WaitForSeconds(.15f);
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
        uiController.loadingBar.skullSlider.value = .9f;
        uiController.loadingBar.text.text = "Filling Traps...";

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

        int hallIndex = UnityEngine.Random.Range(1, z - 2);

        targetHall = starter.generatedHallway[hallIndex];

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

                    starter.generatedHallway.RemoveAt(hallIndex);
                    starter.generatedHallway.Insert(hallIndex, trapHallCube);

                    trapHallCube.filled = true;
                    foreach (GameObject tube in trapHallCube.fallTubes)
                    {
                        tube.SetActive(true);
                    }
                    trapHallCube.fallRoom.SetActive(true);
                    distance.fakeFloors.Add(trapHallCube.fakeFloor);        
                    targetHall.filled = true;
                    int zz = builder.createdTrapHalls.Count;
                    // end cap checker
                    if (targetHall.cap.gameObject.activeSelf)
                    {
                        trapHallCube.cap.gameObject.SetActive(true);
                    }
                    targetHall.gameObject.SetActive(false);

                    yield return new WaitForSeconds(.1f);
                    if (zz < (builder.createdHallCubes.Count / 30))
                    {
                        StartCoroutine(SwapTrapCubes());
                    }
                    if (zz == (builder.createdHallCubes.Count / 30))
                    {
                        Debug.Log("Trap Swap Finished");
                        uiController.loadingBar.skullSlider.value = 1;
                        uiController.loadingBar.text.text = "Opening...";

                        foreach (GameObject fog in sanctuary.fogWalls)
                        {
                            fog.GetComponent<ParticleSystem>().Stop();
                            fog.GetComponent<BoxCollider>().enabled = false;
                            playerController.audioSource.PlayOneShot(playerController.audioClips[0]);
                        }
                    }

                    yield return new WaitForSeconds(3);
                    uiController.loadingBar.gameObject.SetActive(false);
                }
            }
        }
    }

    private IEnumerator SwapHallwayCubes() // lowers fog walls at end
    {
        uiController.loadingBar.skullSlider.value = .75f;
        uiController.loadingBar.text.text = "Adding Secrets...";

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

        int hallIndex = UnityEngine.Random.Range(1, z - 2);

        targetHall = starter.generatedHallway[hallIndex];

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
            }
            if (targetHall.RightBoxChecker())
            {
                right = true;
            }

            if (!left || !right)
            {
                hallPos = targetHall.transform.position;
                hallRot = targetHall.transform.rotation;
                SideExtenderCube sideExtender = Instantiate(builder.sideCubeExtender, hallPos, hallRot);
                builder.createdHallSideCubes.Add(sideExtender);

                starter.generatedHallway.RemoveAt(hallIndex);
                starter.generatedHallway.Insert(hallIndex, sideExtender);

                sideExtender.filled = true;
                targetHall.filled = true;
                int zz = builder.createdHallSideCubes.Count;

                if (targetHall.cap.gameObject.activeSelf)
                {
                    sideExtender.cap.gameObject.SetActive(true);
                }

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
