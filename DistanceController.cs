using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class DistanceController : MonoBehaviour
{
    public PlayerController playerController;
    public SceneBuilder builder;
    public SceneController sceneController;
    public MapController mapController;
    public List<DunPortal> portals;
    public List<FakeWall> fakeWalls;
    public List<FakeFloor> fakeFloors;
    public List<DunChest> chests;
    public List<DunNPC> npcS;
    public List<AudioDistance> audioDistanceControllers;

    public void MapDistance(Vector3 playerPosition)
    {
        foreach (Cube starter in builder.createdStarters)
        {
            HallStarterCube starterComp = starter.GetComponent<HallStarterCube>();
            if (starterComp.generatedHallway.Count > 0)
            {
                if (!starterComp.mapIcon.onMap)
                {
                    if (Vector3.Distance(playerPosition, starter.floorPoint.transform.position) < 6)
                    {                        
                        foreach (Cube hallway in starterComp.generatedHallway)
                        {
                            mapController.LayerOnMap(hallway, 6, false);
                        }
                    }
                    if (Vector3.Distance(playerPosition, starterComp.generatedHallway[starterComp.generatedHallway.Count - 1].transform.position) < 6)
                    {
                        foreach (Cube hallway in starterComp.generatedHallway)
                        {
                            mapController.LayerOnMap(hallway, 6, false);
                        }
                    }
                }
            }
        }
        foreach (Cube turn in builder.createdTurns)
        {
            if (!turn.mapIcon.onMap)
            {
                if (Vector3.Distance(playerPosition, turn.floorPoint.transform.position) < 6)
                {
                    mapController.LayerOnMap(turn, 6, false);
                }
            }
        }
        foreach (Cube room in builder.createdRooms)
        {
            if (!room.mapIcon.onMap)
            {
                if (Vector3.Distance(playerPosition, room.floorPoint.transform.position) < 6)
                {
                    mapController.LayerOnMap(room, 6, true);
                    // trigger room event
                }
                CubeRoom roomComp = room.GetComponent<CubeRoom>();
                foreach (HallStarterCube starter2 in roomComp.starterCubes)
                {
                    if (Vector3.Distance(playerPosition, starter2.floorPoint.transform.position) < 6)
                    {
                        mapController.LayerOnMap(room, 6, true);
                        // trigger room event
                    }
                }
            }
        }
        foreach (Cube boss in builder.createdBossRooms)
        {
            if (!boss.mapIcon.onMap)
            {
                if (Vector3.Distance(playerPosition, boss.floorPoint.transform.position) < 6) // floorpoint set to door
                {
                    mapController.LayerOnMap(boss, 6, true);
                }
            }
        }
    }
    public void PortalDistance(Vector3 playerPosition)
    {
        foreach (DunPortal portal in portals)
        {
            if (portal.sceneController == null)
            {
                portal.sceneController = sceneController;
            }
            if (Vector3.Distance(playerPosition, portal.transform.position) < 15)
            {
                if (!portal.gameObject.activeSelf)
                {
                    if (!portal.closeOnJump)
                    {
                        portal.gameObject.SetActive(true);
                        portal.connectedPortal.gameObject.SetActive(true);
                    }
                    if (portal.closeOnJump && portal.jumpCount == 0)
                    {
                        portal.gameObject.SetActive(true);
                        portal.connectedPortal.gameObject.SetActive(true);
                    }
                }
                if (Vector3.Distance(playerPosition, portal.transform.position) < 3)
                {
                    if (!portal.inRange)
                    {
                        portal.inRange = true;
                    }
                }
                if (Vector3.Distance(playerPosition, portal.transform.position) > 4)
                {
                    if (portal.inRange)
                    {
                        portal.inRange = false;
                    }
                }
            }
            if (Vector3.Distance(playerPosition, portal.transform.position) > 15)
            {
                if (portal.gameObject.activeSelf)
                {
                    portal.gameObject.SetActive(false);
                    portal.connectedPortal.gameObject.SetActive(false);
                    portal.inRange = false; 
                }
            }
        }
    }
    public void FakeWallDistance(Vector3 playerPosition)
    {
        foreach (FakeWall fWall in fakeWalls)
        {
            if (fWall.gameObject.activeSelf)
            {
                if (Vector3.Distance(playerPosition, fWall.transform.position) < 5)
                {
                    if (!fWall.inRange)
                    {
                        fWall.inRange = true;
                    }
                }
                if (Vector3.Distance(playerPosition, fWall.transform.position) > 5)
                {
                    if (fWall.inRange)
                    {
                        fWall.inRange = false;
                    }
                }
            }
        }
    }
    public void AudioDistance(Vector3 playerPosition)
    {
        if (audioDistanceControllers.Count > 0)
        {
            foreach (AudioDistance audioD in audioDistanceControllers)
            {
                if (Vector3.Distance(playerPosition, audioD.gameObject.transform.position) > 15)
                {
                    if (audioD.looper)
                    {
                        audioD.LowerVolume();
                    }
                }
                if (Vector3.Distance(playerPosition, audioD.gameObject.transform.position) <= 12)
                {
                    if (audioD.looper)
                    {         
                        if (!audioD.audioSource.isPlaying)
                        {
                            audioD.RaiseVolume();
                        }
                    }
                    if (!audioD.looper && !audioD.triggered)
                    {
                        audioD.triggered = true;
                        audioD.audioSource.volume = audioD.maxVol;
                        audioD.audioSource.Play();
                    }
                }
            }
        }
    }
    public void ChestDistance(Vector3 playerPosition)
    {
        foreach (DunChest chest in chests)
        {
            if (!chest.opened)
            {
                if (Vector3.Distance(playerPosition, chest.transform.position) < 5)
                {
                    chest.inRange = true;
                }

                if (chest.inRange && !sceneController.uiController.uiActive)
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        chest.OpenChest();
                    }
                }
            }
        }
    }
    public void NPCDistance(Vector3 playerPosition)
    {
        foreach (DunNPC npc in npcS)
        {
            if (Vector3.Distance(playerPosition, npc.transform.position) < 5)
            {
                npc.inRange = true;
            }
            if (npc.inRange && !sceneController.uiController.uiActive)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    npc.NPCTrigger();
                }
            }
        }
    }
    public void FakeFloorDistance(Vector3 playerPosition)
    {
        foreach (FakeFloor floor in fakeFloors)
        {
            if (floor.gameObject.activeSelf)
            {
                if (Vector3.Distance(playerPosition, floor.repairFloor.transform.position) < 1.5f)
                {
                    if (!floor.inRange)
                    {
                        floor.inRange = true;
                    }
                }
                if (Vector3.Distance(playerPosition, floor.repairFloor.transform.position) > 1.5f)
                {
                    if (floor.inRange)
                    {
                        floor.inRange = false;
                    }
                }
                if (floor.inRange && !floor.floorBreak)
                {
                    floor.floorBreak = true;

                    if (Vector3.Distance(playerPosition, floor.front.transform.position) < Vector3.Distance(playerPosition, floor.front.transform.position))
                    {
                        Debug.Log("flipping floor");
                        floor.transform.rotation = Quaternion.Euler(floor.transform.rotation.eulerAngles.x, floor.transform.rotation.eulerAngles.y + 180f, floor.transform.rotation.eulerAngles.z);
                    }
                    floor.Fall();                    
                }
            }
        }
    }
    public void FakeRoomDistance(Vector3 playerPosition)
    {
        if (playerController.controller.enabled && !sceneController.uiController.uiActive)
        {
            foreach (CubeRoom room in sceneController.builder.createdRooms)
            {
                float distance = Vector3.Distance(playerPosition, room.roomCenter.transform.position);

                if (!room.inRoom)
                {
                    if (distance < room.enterDistance)
                    {
                        room.inRoom = true;
                    }
                }

                if (room.inRoom)
                {
                    if (distance > room.enterDistance)
                    {
                        room.inRoom = false;
                    }
                }
            }
        }
    }

    private void Update()
    {
        Vector3 playerPosition = playerController.transform.position;
        if (playerController.active && sceneController.active)
        {
            if (!sceneController.uiController.uiActive)
            {
                MapDistance(playerPosition);
                PortalDistance(playerPosition);
                FakeWallDistance(playerPosition);
                FakeFloorDistance(playerPosition);
                AudioDistance(playerPosition);
                ChestDistance(playerPosition);
                NPCDistance(playerPosition);
                FakeRoomDistance(playerPosition);
            }
        }
    }
}
