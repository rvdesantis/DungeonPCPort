using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HiddenEndCube : DeadEndCube
{
    public List<DunChest> chests;
    public DunPortal secretPortal;
    public List<DunNPC> npcs;
    public List<DunItem> items;
    public enum SecretSize { end, room, massive}
    public SecretSize secretSize;
    public List<BoxCollider> customSecretColliders;


    void Start()
    {
        if (distanceC == null)
        {
            distanceC = FindAnyObjectByType<DistanceController>();
        }
        if (fakeWall != null)
        {
            if (fakeWall.gameObject.activeSelf)
            {
                distanceC.fakeWalls.Add(fakeWall);
            }
        }  
        if (chests.Count > 0)
        {
            foreach (DunChest chest in chests)
            {
                distanceC.chests.Add(chest);
            }
        }
        if (npcs.Count > 0)
        {
            foreach (DunNPC npc in npcs)
            {
                distanceC.npcS.Add(npc);
            }
        }
        if (items.Count > 0)
        {
            foreach (DunItem item in items)
            {
                distanceC.dunItems.Add(item);
            }
        }
    }


    public bool CustomSecretChecker()
    {
        bool blocked = false;

        foreach (BoxCollider boxC in customSecretColliders)
        {
            if (!blocked)
            {    
                boxC.gameObject.SetActive(true);
                boxC.enabled = true;

                Collider[] colliders = Physics.OverlapBox(boxC.bounds.center, boxC.bounds.extents);
                if (colliders.Length > 1)
                {
                    Debug.Log("Custom Secret Checker Blocked", boxC.gameObject);
                    blocked = true;
                }

                boxC.enabled = false;
                boxC.gameObject.SetActive(false);
            }
        }
        return blocked;
    }

    public virtual void SecretSetUp()
    {

    }

}
