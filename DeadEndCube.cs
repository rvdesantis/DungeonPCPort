using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadEndCube : Cube
{
    public DistanceController distanceC;
    public enum EndType { cap, small, med, large, room, drop}
    public EndType endType;
    public Transform spawnPoint;
    public FakeWall fakeWall;


    private void Start()
    {

    }
}
