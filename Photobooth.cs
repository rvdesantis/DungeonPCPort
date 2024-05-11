using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

public class Photobooth : MonoBehaviour
{
    public Camera photoCam;
    public DunModel currentHero;
    public Transform playerSpawnTransform;
    public Light photoLight;
    public MeshCollider wallCollider;
    public List<DunModel> masterPhotoModels;



    public void SayCheese(DunModel partyHero)
    {
        wallCollider.enabled = false;
        if (!photoLight.gameObject.activeSelf)
        {
            photoLight.gameObject.SetActive(true);
        }
        if (currentHero != null && currentHero != partyHero)
        {
            currentHero.gameObject.SetActive(false);   
            currentHero = null;
        }
        foreach (DunModel master in masterPhotoModels)
        {
            if (master.modelName == partyHero.modelName)
            {
                currentHero = master;
                break;
            }
        }

        currentHero.transform.position = playerSpawnTransform.position;
        if (!currentHero.gameObject.activeSelf)
        {
            currentHero.gameObject.SetActive(true);
        } 
    }

    public void ResetBooth()
    {
        if (currentHero != null)
        {
            currentHero.gameObject.SetActive(false);
        }
        photoLight.gameObject.SetActive(false);
        wallCollider.enabled = true;
    }

}


