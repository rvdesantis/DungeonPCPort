using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TrapHallCube;

public class FallRoom : MonoBehaviour
{
    public TrapHallCube trapCube;
    public DunPortal exitPortal;
    public DunPortal returnPortal;


    public void FillTrap()
    {
        if (trapCube.trapType != TrapType.empty)
        {
            if (trapCube.trapType == TrapType.enemy)
            {
                StartCoroutine(FillEnemy());
            }
            if (trapCube.trapType == TrapType.chest)
            {
                StartCoroutine(FillChest());
            }
            if (trapCube.trapType == TrapType.NPC)
            {
                StartCoroutine(FillNPC());
            }
            if (trapCube.trapType == TrapType.other)
            {
                StartCoroutine(FillOther());
            }
        }
    }


    IEnumerator FillEnemy()
    {
        yield return new WaitForSeconds(0);
    }

    IEnumerator FillChest()
    {
        yield return new WaitForSeconds(0);
    }

    IEnumerator FillOther()
    {
        yield return new WaitForSeconds(0);
    }

    IEnumerator FillNPC()
    {
        yield return new WaitForSeconds(0);
    }
}
