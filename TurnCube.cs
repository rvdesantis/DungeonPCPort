using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnCube : Cube
{
    public List<HallStarterCube> starterCubes;
    public BoxCollider turnCollider;


    public bool TurnChecker()
    {
        bool collision = false;
        Collider[] colliders = Physics.OverlapBox(turnCollider.bounds.center, turnCollider.bounds.extents);

        // Exclude child colliders
        foreach (Transform child in transform)
        {
            Collider childCollider = child.GetComponent<Collider>();
            if (childCollider != null)
            {         
                colliders = Array.FindAll(colliders, c => c.transform.parent != transform);
            }
        }

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
}
