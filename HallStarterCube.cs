using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallStarterCube : Cube
{
    public SceneBuilder builder;
    public SceneController controller;
    public BoxCollider smallHallwayCollider; 
    public BoxCollider mediumHallwayCollider; 
    public BoxCollider largeHallwayCollider;
    public BoxCollider smallSecretCollider;

    public bool hallBuildFin;
    public bool buildMirrorFin;
    public List<Cube> generatedHallway;

    public bool testColliders;

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

        if (!blocked)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void Update()
    {
        if (buildMirrorFin != hallBuildFin && hallBuildFin == true)
        {
            int x = builder.createdStarters.IndexOf(this);      
            buildMirrorFin = true;
        }
    }
}
