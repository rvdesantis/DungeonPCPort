using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnCube : Cube
{
    public List<HallStarterCube> starterCubes;  
    public BoxCollider turnCollider;
    public int hallwayNum;

    public bool TurnChecker()
    {
        bool collision = false;
        Collider[] colliders = Physics.OverlapBox(turnCollider.bounds.center, turnCollider.bounds.extents);
        if (colliders.Length > 1)
        {
            collision = true;
            Debug.Log("BoxChecker Collision with (GameObject) " + colliders[0].gameObject.name, gameObject);
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
