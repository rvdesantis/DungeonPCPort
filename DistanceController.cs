using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class DistanceController : MonoBehaviour
{
    public PlayerController playerController;
    public DunUIController uiController;
    public SceneBuilder builder;
    public SceneController sceneController;
    public MapController mapController;
    public List<DunPortal> portals;
    public List<FakeWall> fakeWalls;
    public List<FakeFloor> fakeFloors;
    public List<DunChest> chests;
    public List<DunNPC> npcS;
    public List<CubeRoom> rooms;
    public List<CubeRoom> bossRooms;
    public List<DunSwitch> switches;
    public List<DunItem> dunItems;
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
                        if (starterComp.generatedHallway.Count > 0)
                        {
                            foreach (Cube hallway in starterComp.generatedHallway)
                            {
                                mapController.LayerOnMap(hallway, 6, false);
                            }
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
        foreach (Cube end in builder.createdDeadEnds)
        {
            if (!end.mapIcon.onMap)
            {
                if (Vector3.Distance(playerPosition, end.floorPoint.transform.position) < 6)
                {
                    mapController.LayerOnMap(end, 6, true);        
                }
            }
        }
        foreach (Cube room in builder.createdRooms)
        {
            if (Vector3.Distance(playerPosition, room.floorPoint.transform.position) < 6)
            {
                mapController.LayerOnMap(room, 6, true);
                room.mapIcon.visitedFrame.gameObject.SetActive(true);
                room.mapIcon.iconBase.gameObject.SetActive(false);
                room.mapIcon.visitedBase.gameObject.SetActive(true);
                // trigger room event
            }
            CubeRoom roomComp = room.GetComponent<CubeRoom>();
            foreach (HallStarterCube starter2 in roomComp.starterCubes)
            {
                if (Vector3.Distance(playerPosition, starter2.floorPoint.transform.position) < 4)
                {
                    mapController.LayerOnMap(room, 6, true);
                    // trigger room event
                }
            }            
        }
        foreach (Cube boss in builder.createdBossRooms)
        {
            if (Vector3.Distance(playerPosition, boss.floorPoint.transform.position) < 6) // floorpoint set to door
            {
                mapController.LayerOnMap(boss, 6, true);
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
            if (Vector3.Distance(playerPosition, portal.transform.position) < 15 && portal.assigned)
            {
                if (!portal.gameObject.activeSelf)
                {
                    if (!portal.closeOnJump)
                    {
                        portal.gameObject.SetActive(true);
                        portal.connectedPortal.gameObject.SetActive(true);
                    }
                    if (portal.closeOnJump)
                    {
                        if (portal.jumpCount == 0)
                        {
                            portal.gameObject.SetActive(true);
                            portal.connectedPortal.gameObject.SetActive(true);
                        }
                    }
                }
                if (Vector3.Distance(playerPosition, portal.transform.position) < 3)
                {
                    if (!portal.inRange)
                    {
                        portal.inRange = true;
                        uiController.rangeImage.sprite = uiController.rangeSprites[1];
                        uiController.rangeImage.gameObject.SetActive(true);
                        uiController.ToggleKeyUI(portal.gameObject, true);
                    }
                }
                if (Vector3.Distance(playerPosition, portal.transform.position) > 4)
                {
                    if (portal.inRange)
                    {
                        portal.inRange = false;
                        uiController.rangeImage.gameObject.SetActive(false);
                        if (uiController.interactUI.activeObj == portal.gameObject)
                        {
                            uiController.ToggleKeyUI(portal.gameObject, false);
                        }
                    }
                }
            }
            if (Vector3.Distance(playerPosition, portal.transform.position) > 15)
            {
                if (portal.gameObject.activeSelf)
                {
                    portal.gameObject.SetActive(false);
                    portal.connectedPortal.gameObject.SetActive(false);
                }
            }
        }
    }
    public void FakeWallDistance(Vector3 playerPosition)
    {
        if (playerController.enabled)
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
        
    }
    public void AudioDistance(Vector3 playerPosition)
    {
        if (audioDistanceControllers.Count > 0)
        {
            foreach (AudioDistance audioD in audioDistanceControllers)
            {
                if (audioD.gameObject.activeSelf)
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
    }
    public void ChestDistance(Vector3 playerPosition)
    {
        foreach (DunChest chest in chests)
        {
            if (!chest.opened)
            {
                if (Vector3.Distance(playerPosition, chest.transform.position) < 4 && !chest.inRange && !chest.opened)
                {
                    chest.inRange = true;
                    if (chest.locked)
                    {
                        if (chest.fakeWall == null || chest.fakeWall.wallBroken)
                        {
                            uiController.rangeImage.sprite = uiController.rangeSprites[4];
                            uiController.customImage.sprite = uiController.customSprites[1];
                            uiController.rangeImage.gameObject.SetActive(true);
                            uiController.customImage.gameObject.SetActive(true);
                            uiController.ToggleKeyUI(chest.gameObject, true);
                        }
                    }
                    if (!chest.locked)
                    {
                        if (chest.fakeWall == null || chest.fakeWall.wallBroken)
                        {
                            uiController.rangeImage.sprite = uiController.rangeSprites[0];
                            uiController.rangeImage.gameObject.SetActive(true);
                            uiController.ToggleKeyUI(chest.gameObject, true);
                        }
                    }
                }
                if (Vector3.Distance(playerPosition, chest.transform.position) >= 4 && chest.inRange)
                {
                    chest.inRange = false;
                    if (uiController.interactUI.activeObj == chest.gameObject)
                    {
                        uiController.rangeImage.gameObject.SetActive(false);
                        uiController.customImage.gameObject.SetActive(false);
                        uiController.ToggleKeyUI(chest.gameObject, false);
                    }
                }
                if (chest.inRange && !sceneController.uiController.uiActive)
                {
                    if (chest.fakeWall == null || chest.fakeWall.wallBroken)
                    {
                        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0))
                        {
                            chest.OpenChest();                            
                        }
                    }
                }
            }         
            if (chest.opened && chest.inRange)
            {
                chest.inRange = false;
                uiController.rangeImage.gameObject.SetActive(false);
                uiController.customImage.gameObject.SetActive(false);
            }
        }
    }
    public void NPCDistance(Vector3 playerPosition)
    {
        foreach (DunNPC npc in npcS)
        {
            if (npc.gameObject.activeSelf)
            {
                if (npc.idlePlayableLoop != null)
                {
                    if (Vector3.Distance(playerPosition, npc.transform.position) < 15)
                    {
                        if (!npc.engaged)
                        {
                            if (npc.idlePlayableLoop.state != UnityEngine.Playables.PlayState.Playing)
                            {
                                npc.idlePlayableLoop.Play();
                            }
                        }
                    }
                    if (Vector3.Distance(playerPosition, npc.transform.position) > 16)
                    {
                        npc.idlePlayableLoop.Stop();
                        npc.idlePlayableLoop.time = 0;
                    }
                }
                if (Vector3.Distance(playerPosition, npc.transform.position) < 4 && !npc.inRange)
                {
                    npc.inRange = true;
                    if (npc.icon == null)
                    {
                        uiController.rangeImage.sprite = uiController.rangeSprites[2];
                    }
                    if (npc.icon != null)
                    {
                        uiController.rangeImage.sprite = uiController.rangeSprites[4];
                        uiController.customImage.sprite = npc.icon;
                        uiController.rangeImage.gameObject.SetActive(true);
                        uiController.customImage.gameObject.SetActive(true);
                    }

                    uiController.rangeImage.gameObject.SetActive(true);
                    uiController.ToggleKeyUI(npc.gameObject, true);
                }
                if (Vector3.Distance(playerPosition, npc.transform.position) >= 4 && npc.inRange)
                {
                    npc.inRange = false;
                    uiController.rangeImage.gameObject.SetActive(false);
                    uiController.customImage.gameObject.SetActive(false);

                    if (uiController.interactUI.activeObj == npc.gameObject)
                    {
                        uiController.ToggleKeyUI(npc.gameObject, false);
                    }
                }
                if (npc.inRange && !sceneController.uiController.uiActive)
                {
                    if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0))
                    {
                        npc.NPCTrigger();
                    }
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
                    bool back = false;

                    float frontDis = Vector3.Distance(playerPosition, floor.front.transform.position);
                    float backDis = Vector3.Distance(playerPosition, floor.back.transform.position);

                    if (backDis < frontDis)
                    {
                        back = true;
                    }
                    Debug.Log("Trap Door: Front Distance - " + frontDis + "/ Back Distance - " + backDis, floor.gameObject);
                    floor.StartBreak(back);                   
                }
            }
        }
    }
    public void RoomDistance(Vector3 playerPosition)
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
    public void EnemyTriggerDistance(Vector3 playerPosition)
    {
        if (playerController.controller.enabled && !sceneController.uiController.uiActive)
        {
            foreach (Cube cube in builder.createdHallSideCubes)
            {
                SideExtenderCube side = cube.GetComponent<SideExtenderCube>();
                if (side != null && !side.triggered)
                {
                    if (side.sideType == SideExtenderCube.SideType.enemy)
                    {
                        if (Vector3.Distance(playerPosition, side.triggerSpot.transform.position) < 4)
                        {
                            side.TriggerEnemy();
                        }
                    }
                }
            }
        }
    }
    public void SwitchDistance(Vector3 playerPosition)
    {
        if (playerController.controller.enabled && !sceneController.uiController.uiActive)
        {
            foreach (DunSwitch dunSwit in switches)
            {
                if (Vector3.Distance(playerPosition, dunSwit.transform.position) < 4 && !dunSwit.inRange && !dunSwit.flipping)
                {
                    if (dunSwit.locked)
                    {
                        dunSwit.inRange = true;
                        uiController.rangeImage.sprite = uiController.rangeSprites[4];
                        uiController.customImage.sprite = uiController.customSprites[1];
                        uiController.rangeImage.gameObject.SetActive(true);
                        uiController.customImage.gameObject.SetActive(true);
                        uiController.ToggleKeyUI(dunSwit.gameObject, true);
                    }
                    else
                    {
                        dunSwit.inRange = true;
                        uiController.rangeImage.sprite = uiController.rangeSprites[1];
                        uiController.rangeImage.gameObject.SetActive(true);
                        uiController.ToggleKeyUI(dunSwit.gameObject, false);
                    }
                }
                if (dunSwit.inRange)
                {
                    if (Vector3.Distance(playerPosition, dunSwit.transform.position) >= 4 || dunSwit.flipping)
                    {
                        dunSwit.inRange = false;
                        uiController.rangeImage.gameObject.SetActive(false);
                        uiController.customImage.gameObject.SetActive(false);
                        if (uiController.interactUI.activeObj == dunSwit.gameObject)
                        {
                            uiController.ToggleKeyUI(dunSwit.gameObject, false);
                        }
                    }
                }
            }
        }
    }
    public void DunItemDistance(Vector3 playerPosition)
    {
        if (playerController.controller.enabled && !sceneController.uiController.uiActive)
        {
            foreach (DunItem item in dunItems)
            {
                if (item.gameObject.activeSelf)
                {
                    if (Vector3.Distance(playerPosition, item.transform.position) <= 4 && !item.inRange)
                    {
                        item.inRange = true;
                        uiController.rangeImage.sprite = uiController.rangeSprites[4];
                        uiController.customImage.sprite = item.icon;
                        uiController.rangeImage.gameObject.SetActive(true);
                        uiController.customImage.gameObject.SetActive(true);
                        uiController.ToggleKeyUI(item.gameObject, true);
                    }
                    if (item.inRange)
                    {
                        if (Vector3.Distance(playerPosition, item.transform.position) >= 4 && item.inRange)
                        {
                            item.inRange = false;
                            uiController.rangeImage.gameObject.SetActive(false);
                            uiController.customImage.gameObject.SetActive(false);
                            if (uiController.interactUI.activeObj == item.gameObject)
                            {
                                uiController.ToggleKeyUI(item.gameObject, false);
                            }        
                        }
                    }
                }    
            }
        }
    }

    private void Update()
    {
        Vector3 playerPosition = playerController.transform.position;
        if (playerController.active && playerController.enabled && sceneController.active)
        {
            if (!sceneController.uiController.uiActive && !sceneController.uiController.isToggling)
            {
                MapDistance(playerPosition);
                PortalDistance(playerPosition);
                FakeWallDistance(playerPosition);
                FakeFloorDistance(playerPosition);
                AudioDistance(playerPosition);
                ChestDistance(playerPosition);
                NPCDistance(playerPosition);
                RoomDistance(playerPosition);
                EnemyTriggerDistance(playerPosition);
                SwitchDistance(playerPosition);
                DunItemDistance(playerPosition);
            }
        }
    }
}
