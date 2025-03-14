using System.Collections.Generic;
using UnityEngine;
using System;


public class CubeRoom : TurnCube
{
    public List<GameObject> wallCovers;
    public List<GameObject> wallSpawnPoints;

    public enum RoomType { quest, battle, chest, shop, NPC, health, portal }
    public RoomType roomType;
    public GameObject roofParent;
    public List<RoomPropParent> environments;
    public RoomPropParent activeENV;
    public bool roomAssigned;
    public GameObject openWall;
    public bool inRoom;
    public bool eventTriggered;
    public GameObject roomCenter;
    public float enterDistance;




    public void FillRoom()
    {
        if (roomType == RoomType.battle)
        {
            List<RoomPropParent> fillList = new List<RoomPropParent>();  
            foreach (RoomPropParent env in environments)
            {
                // add rooms with agent fills
                fillList.Add(env);
                if (openWall !=null)
                {
                    env.openWall = openWall;
                }
            }
            int roomNum = UnityEngine.Random.Range(0, fillList.Count);
            RoomPropParent targetRoom = fillList[roomNum];  
            targetRoom.gameObject.SetActive(true);
            targetRoom.EnvFill();
        }
        if (roomType == RoomType.chest)
        {
            List<RoomPropParent> treasureList = new List<RoomPropParent>();
            foreach (RoomPropParent env in environments)
            {
                if (env.treasureSpawn.Count > 0)
                {
                    treasureList.Add(env);
                    if (openWall != null)
                    {
                        env.openWall = openWall;
                    }
                }
            }

            int roomNum = UnityEngine.Random.Range(0, treasureList.Count);
            RoomPropParent targetRoom = treasureList[roomNum];
            targetRoom.gameObject.SetActive(true);
            targetRoom.EnvFill();

            int treas = UnityEngine.Random.Range(0, targetRoom.treasureSpawn.Count);
            DunChest targetChest = targetRoom.treasureSpawn[treas];
            targetChest.gameObject.SetActive(true);
            FindAnyObjectByType<DistanceController>().chests.Add(targetChest);
            
        }
        if (roomType == RoomType.NPC)
        {
            List<RoomPropParent> fillList = new List<RoomPropParent>();
            foreach (RoomPropParent env in environments)
            {
                if (env.NPCSpawn.Count > 0)
                {
                    fillList.Add(env);
                    if (openWall != null)
                    {
                        env.openWall = openWall;
                    }
                }
            }
            if (fillList.Count > 0)
            {
                int roomNum = UnityEngine.Random.Range(0, fillList.Count);
                RoomPropParent targetRoom = fillList[roomNum];
                targetRoom.gameObject.SetActive(true);
                targetRoom.EnvFill();
            }
            if (fillList.Count == 0)
            {
                roomType = RoomType.battle;
                FillRoom();
                return;
            }
        }
        if (roomType == RoomType.health)
        {
            List<RoomPropParent> fillList = new List<RoomPropParent>();
            foreach (RoomPropParent env in environments)
            {
                // add rooms with Trap fills
                fillList.Add(env);
                if (openWall != null)
                {
                    env.openWall = openWall;
                }
            }
            int roomNum = UnityEngine.Random.Range(0, fillList.Count);
            RoomPropParent targetRoom = fillList[roomNum];
            targetRoom.gameObject.SetActive(true);
            targetRoom.EnvFill();
        }
        if (roomType == RoomType.quest)
        {
            List<RoomPropParent> fillList = new List<RoomPropParent>();
            foreach (RoomPropParent env in environments)
            {
                // add rooms with Mystery fills
                fillList.Add(env);
                if (openWall != null)
                {
                    env.openWall = openWall;
                }
            }
            int roomNum = UnityEngine.Random.Range(0, fillList.Count);
            RoomPropParent targetRoom = fillList[roomNum];
            targetRoom.gameObject.SetActive(true);
            targetRoom.EnvFill();
        }
        if (roomType == RoomType.portal)
        {
            List<RoomPropParent> fillList = new List<RoomPropParent>();
            foreach (RoomPropParent env in environments)
            {
                if (env.portalSpawn.Count > 0)
                {
                    fillList.Add(env);
                    if (openWall != null)
                    {
                        env.openWall = openWall;
                    }
                }
            }
            int roomNum = UnityEngine.Random.Range(0, fillList.Count);
            RoomPropParent targetRoom = fillList[roomNum];
            targetRoom.gameObject.SetActive(true);
            targetRoom.EnvFill(); // portal set up through EnvFill
        }
        if (roomType == RoomType.shop)
        {
            List<RoomPropParent> fillList = new List<RoomPropParent>();
            foreach (RoomPropParent env in environments)
            {
                // add rooms with Mystery fills
                fillList.Add(env);
                if (openWall != null)
                {
                    env.openWall = openWall;
                }
            }
            int roomNum = UnityEngine.Random.Range(0, fillList.Count);
            RoomPropParent targetRoom = fillList[roomNum];
            targetRoom.gameObject.SetActive(true);
            targetRoom.EnvFill();
        }
    }


    private void Update()
    {
        if (inRoom && !eventTriggered)
        {
            if (roomType == RoomType.quest)
            {

            }
            if (roomType == RoomType.battle)
            {
                
            }
            if (roomType == RoomType.chest)
            {

            }
            if (roomType == RoomType.shop)
            {

            }
            if (roomType == RoomType.NPC)
            {

            }
            if (roomType == RoomType.portal)
            {
                if (activeENV.portbGameObject == null)
                {
                    activeENV.portbGameObject = activeENV.PortalShuffle();
                    activeENV.SetPortal();
                }
            }
            if (roomType == RoomType.health)
            {

            }
        }
    }
}
