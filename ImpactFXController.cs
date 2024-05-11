using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactFXController : MonoBehaviour
{
    public BattleModel bModel;
    public ParticleSystem standardImpactFX;
    public List<ParticleSystem> elementalImpactFXs;

    public void StandardImpact()
    {
        StartCoroutine(StandardTimer());
    }

    IEnumerator StandardTimer()
    {
        standardImpactFX.gameObject.SetActive(true);
        standardImpactFX.Play();
        yield return new WaitForSeconds(2);
        standardImpactFX.gameObject.SetActive(false);
    }

    public void ElementalImpact(int elementIndex)
    {
        if (elementIndex == 0)
        {
            StartCoroutine(FireTimer());
        }
        if (elementIndex == 1)
        {
            StartCoroutine(VoidTimer());
        }
        if (elementIndex == 2)
        {
            StartCoroutine(PoisonTimer());
        }
        if (elementIndex == 3)
        {
            StartCoroutine(IceTimer());
        }
    }


    IEnumerator FireTimer()
    {
        elementalImpactFXs[0].gameObject.SetActive(true);
        standardImpactFX.Play();
        yield return new WaitForSeconds(2);
        elementalImpactFXs[0].gameObject.SetActive(false);
    }

    IEnumerator VoidTimer()
    {
        elementalImpactFXs[1].gameObject.SetActive(true);
        standardImpactFX.Play();
        yield return new WaitForSeconds(2);
        elementalImpactFXs[1].gameObject.SetActive(false);
    }

    IEnumerator PoisonTimer()
    {
        elementalImpactFXs[2].gameObject.SetActive(true);
        standardImpactFX.Play();
        yield return new WaitForSeconds(2);
        elementalImpactFXs[2].gameObject.SetActive(false);
    }

    IEnumerator IceTimer()
    {
        elementalImpactFXs[3].gameObject.SetActive(true);
        standardImpactFX.Play();
        yield return new WaitForSeconds(2);
        elementalImpactFXs[3].gameObject.SetActive(false);
    }
}
