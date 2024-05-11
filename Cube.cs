using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public enum CubeType { home, hallway, bHallway, smallRoom, larRoom, boss, lTurn, rTurn, tTurn, hConnect, secret }
    public CubeType cubeType;
    public MeshRenderer lengthMesh;
    public DeadEndCube cap;
    public bool filled;
    public GameObject positioner;
    public BoxCollider collisionChecker;
    public BoxCollider lcollisionChecker;
    public BoxCollider rcollisionChecker;
    public BoxCollider fallCollisionChecker;
    public MapIcon mapIcon;
    public GameObject floorPoint;
    public List<GameObject> lWalls;
    public List<GameObject> rWalls;
    public List<GameObject> floors;
    public List<GameObject> cealings;
    public List<GameObject> fogWalls;
    public List<GameObject> randomProps;
    public bool BoxChecker()
    {
        bool collision = false;
        Collider[] colliders = Physics.OverlapBox(collisionChecker.bounds.center, collisionChecker.bounds.extents);
        if (colliders.Length > 1)
        {
            collision = true;
        }

        if (collision)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool LeftBoxChecker()
    {
        if (!lcollisionChecker.gameObject.activeSelf)
        {
            lcollisionChecker.gameObject.SetActive(true);
        }
        bool collision = false;
        Collider[] colliders = Physics.OverlapBox(lcollisionChecker.bounds.center, lcollisionChecker.bounds.extents);
        if (colliders.Length > 1)
        {
            collision = true;
        }
        lcollisionChecker.gameObject.SetActive(false);
        if (collision)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool RightBoxChecker()
    {
        if (!rcollisionChecker.gameObject.activeSelf)
        {
            rcollisionChecker.gameObject.SetActive(true);
        }
        bool collision = false;
        Collider[] colliders = Physics.OverlapBox(rcollisionChecker.bounds.center, rcollisionChecker.bounds.extents);
        if (colliders.Length > 1)
        {
            collision = true;
        }
        rcollisionChecker.gameObject.SetActive(false);
        if (collision)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool FallBoxChecker()
    {
        if (!fallCollisionChecker.gameObject.activeSelf)
        {
            fallCollisionChecker.gameObject.SetActive(true);
        }
        bool collision = false;
        Collider[] colliders = Physics.OverlapBox(fallCollisionChecker.bounds.center, fallCollisionChecker.bounds.extents);
        if (colliders.Length > 1)
        {
            collision = true;
        }
        fallCollisionChecker.gameObject.SetActive(false);
        if (collision)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
