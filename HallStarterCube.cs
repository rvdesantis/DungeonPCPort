using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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
    public bool buildMirrorFin;
    public List<Cube> generatedHallway;

    public bool testColliders;

    public bool testSmallCollider;
    public bool testLargeCollider;
    public bool testMassiveCollider;

    public enum HallType { deadEnd, small, med, large, boss}
    public HallType hallType;

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
            hallbox.collisionChecker.gameObject.SetActive(true);
            if (hallbox.BoxChecker())
            {
                hallbox.collisionChecker.gameObject.SetActive(false);
                hallbox.positioner.SetActive(true);
                blocked = true;
              
            }
            if (!hallbox.BoxChecker())
            {
                hallbox.collisionChecker.gameObject.SetActive(false);
                hallbox.positioner.SetActive(true);

                
            }
        }

        if (blocked)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool ForwardChecker()
    {
        Vector3 direction = transform.forward;

        // Perform the sphere cast
        RaycastHit hit;
        bool hasHit = Physics.SphereCast(transform.position, 2, direction, out hit, 100);


        if (hasHit)
        {
            GameObject hitObject = hit.collider.gameObject;
        }

        return hasHit;
    }

    public bool MassiveSecretChecker() // listed small to large
    {
        bool blocked = false;
    
        foreach (BoxCollider boxC in massiveSecretColliders)
        {
            if (!blocked)
            {
                boxC.gameObject.SetActive(true);
                boxC.enabled = true;

                blocked = Physics.CheckBox(boxC.bounds.center, boxC.bounds.extents/2, quaternion.identity, 0);

                boxC.enabled = false;
                boxC.gameObject.SetActive(false);
            }
        }       

        if (!blocked)
        {
            blocked = ForwardChecker();
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

        return blocked;
    }

    private void Update()
    {
        if (buildMirrorFin != hallBuildFin && hallBuildFin == true)
        {
            int x = builder.createdStarters.IndexOf(this);      
            buildMirrorFin = true;
        }

        if (testMassiveCollider)
        {
            testMassiveCollider = false;
            if (MassiveSecretChecker())
            {
                Debug.Log("Massive Checker Collision");
            }
        }
    }
}
