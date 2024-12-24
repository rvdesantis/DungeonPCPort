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
        ParticleSystem imp = Instantiate(standardImpactFX, standardImpactFX.transform.position, standardImpactFX.transform.rotation);
        imp.gameObject.SetActive(true);
        imp.Play();
    } 

    public void ElementalImpact(int elementIndex)
    {
        if (elementIndex == 0)
        {
            ParticleSystem fireImp = Instantiate(elementalImpactFXs[0], elementalImpactFXs[0].transform.position, elementalImpactFXs[0].transform.rotation);
            fireImp.gameObject.SetActive(true);
            fireImp.Play();
            IEnumerator Destroyer()
            {
                yield return new WaitForSeconds(3);
                fireImp.gameObject.SetActive(false);
                Destroy(fireImp.gameObject);
            }
            StartCoroutine(Destroyer());
        }
        if (elementIndex == 1)
        {
            ParticleSystem voidImp = Instantiate(elementalImpactFXs[1], elementalImpactFXs[1].transform.position, elementalImpactFXs[1].transform.rotation);
            voidImp.gameObject.SetActive(true);
            voidImp.Play();
            IEnumerator Destroyer()
            {
                yield return new WaitForSeconds(3);
                voidImp.gameObject.SetActive(false);
                Destroy(voidImp.gameObject);
            }
            StartCoroutine(Destroyer());
        }
        if (elementIndex == 2)
        {
            ParticleSystem poisonImp = Instantiate(elementalImpactFXs[2], elementalImpactFXs[2].transform.position, elementalImpactFXs[2].transform.rotation);
            poisonImp.gameObject.SetActive(true);
            poisonImp.Play();
            IEnumerator Destroyer()
            {
                yield return new WaitForSeconds(3);
                poisonImp.gameObject.SetActive(false);
                Destroy(poisonImp.gameObject);
            }
            StartCoroutine(Destroyer());
        }
        if (elementIndex == 3)
        {
            ParticleSystem iceImp = Instantiate(elementalImpactFXs[3], elementalImpactFXs[3].transform.position, elementalImpactFXs[3].transform.rotation);
            iceImp.gameObject.SetActive(true);
            iceImp.Play();
            IEnumerator Destroyer()
            {
                yield return new WaitForSeconds(3);
                iceImp.gameObject.SetActive(false);
                Destroy(iceImp.gameObject);
            }
            StartCoroutine(Destroyer());
        }
    }       
}
