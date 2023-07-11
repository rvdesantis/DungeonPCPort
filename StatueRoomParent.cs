using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatueRoomParent : RoomPropParent
{
    public Transform crystal;
    public Transform floorGrate;
    public Transform roofGrate;

    public float rotationSpeed = 10f;

    private void Start()
    {
        StartCoroutine(RotateObjects());
    }

    private IEnumerator RotateObjects()
    {
        while (true)
        {
            crystal.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            floorGrate.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            roofGrate.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
