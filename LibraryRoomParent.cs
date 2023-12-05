using Runemark.DarkFantasyKit.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using DTT.PlayerPrefsEnhanced;

public class LibraryRoomParent : RoomPropParent 
{
    public bool vMageUnlocked;
    public SkullNPC skull;
    public bool skulltest;
    public PlayableDirector templarIdlePlayable;

    public override void EnvFill()
    {
        StartCoroutine(SetActives());
        AvailableWall();
        roomParent.activeENV = this;

        vMageUnlocked = EnhancedPrefs.GetPlayerPref("voidMageUnlock", false);


        if (!vMageUnlocked)
        {
            skull.gameObject.SetActive(true);
            if (distanceController == null)
            {
                distanceController = FindAnyObjectByType<DistanceController>();
            }
            distanceController.npcS.Add(skull);
            skull.player = FindObjectOfType<PlayerController>();
            skull.activeTarget = skull.player.transform;
            skull.roomParent = roomParent;
        }
 
        if (roomParent.roomType == CubeRoom.RoomType.NPC)
        {
            if (!vMageUnlocked)
            {
                NPCController npcController = FindObjectOfType<NPCController>();
                DistanceController distance = FindObjectOfType<DistanceController>();
                bool inDun = false;
                foreach (DunNPC npc in distance.npcS)
                {
                    if (npc.GetComponent<TemplarNPC>() != null)
                    {
                        inDun = true;
                        Debug.Log("Templar already in dungeon", gameObject);
                        break;
                    }
                }
                if (!inDun)
                {
                    DunModel target = npcController.npcMasterList[1];
                    DunModel templar = Instantiate(target, templarIdlePlayable.transform);
                    templar.AssignToDirector(templarIdlePlayable, 3);

                    TemplarNPC templarNPC = templar.GetComponent<TemplarNPC>();
                    templarNPC.idlePlayableLoop = templarIdlePlayable;
                    templarNPC.roomSkull = skull;
                    skull.activeTemplar = templarNPC;
                    skull.activeTarget = templar.transform;

                    distance.npcS.Add(templarNPC);
                    templarNPC.idlePlayableLoop.Play();
                    Debug.Log("Templar added to Library", gameObject);
                }
            }
        }
        if (roomParent.roomType == CubeRoom.RoomType.battle)
        {
            if (vMageUnlocked)
            {
                Debug.Log("Trigger Templar Battle");
            }
        }
    }
}
