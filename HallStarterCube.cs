using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class HallStarterCube : Cube
{
    public SceneBuilder builder;
    public SceneController controller;
    public BoxCollider smallHallwayCollider; 
    public BoxCollider mediumHallwayCollider; 
    public BoxCollider largeHallwayCollider;
    public BoxCollider smallSecretCollider;
    public BoxCollider largeSecretCollider;
    public List<BoxCollider> massiveSecretColliders;


    public bool hallBuildFin;
    public bool secret;
    public bool hallSecret;
    public List<Cube> generatedHallway;

    public bool testColliders;

    public bool testSmallCollider;
    public bool testLargeCollider;
    public bool testMassiveCollider;

    public enum HallType { deadEnd, small, med, large, boss}
    public HallType hallType;
    public TurnCube endTurn;
    public CubeRoom endRoom;
    public TurnCube attachedTurn;
    public int hallwayNumber;
    public DeadEndCube deadEnd;

    private void Start()
    {
        if (builder == null)
        {
            builder = FindAnyObjectByType<SceneBuilder>();
        }
    }

    public bool HallwayCheck()
    {
        bool blocked = false;
        foreach (Cube hallbox in generatedHallway)
        {
            hallbox.positioner.SetActive(false);
            hallbox.collisionChecker.enabled = true;       
            if (hallbox.BoxChecker())
            {
                hallbox.collisionChecker.enabled = false;
                hallbox.positioner.SetActive(true);
                blocked = true;
              
            }
            if (!hallbox.BoxChecker())
            {
                hallbox.collisionChecker.enabled = false;
                hallbox.positioner.SetActive(true);
            }
        }

        if (blocked)
        {
            Debug.Log("Hallway Check Returning as Blocked");
            return true;

        }
        else
        {
            Debug.Log("Hallway Check Returning as Not Blocked");
            return false;
        }
    }

    public bool MassiveSecretChecker() 
    {
        bool blocked = false;
    
        foreach (BoxCollider boxC in massiveSecretColliders)
        {
            if (!blocked)
            {
                IEnumerator CheckTimer()
                {
                    boxC.gameObject.SetActive(true);
                    boxC.enabled = true;
                    yield return new WaitForNextFrameUnit();

                    Collider[] colliders = Physics.OverlapBox(boxC.bounds.center, boxC.bounds.extents);
                    if (colliders.Length > 1)
                    {
                        blocked = true;
                    }                    
                    yield return new WaitForNextFrameUnit();
                    boxC.enabled = false;
                    boxC.gameObject.SetActive(false);
                } 
                StartCoroutine(CheckTimer());
            }
        }  
        return blocked;
    }

    public bool SecretEndChecker()
    {
        bool blocked = false;

        smallHallwayCollider.gameObject.SetActive(true);
        Collider[] colliders = Physics.OverlapBox(smallSecretCollider.bounds.center, smallSecretCollider.bounds.extents);
        if (colliders.Length > 1)
        {
            blocked = true;
        }
        smallHallwayCollider.gameObject.SetActive(false);
        Debug.Log("HallStarter Cube Checked for Secret End (" + blocked + ")", gameObject);
        return blocked;
    }

    private void Update()
    {

    }
}
