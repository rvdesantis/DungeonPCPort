using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleShopNPC : ShopNPC
{
    public DunModel thisModel;
    public DunUIController uIController;
    public GameObject shopPrefab;

    private void Start()
    {
        DistanceController distance = FindObjectOfType<DistanceController>();
        Instantiate(shopPrefab, transform.position, transform.rotation);
        distance.npcS.Add(this);
        Debug.Log("Battle Shop Spawned", thisModel.gameObject);
    }

    public override void OpenUI()
    {
        DunUIController uiController = FindObjectOfType<DunUIController>();
        PlayerController player = FindObjectOfType<PlayerController>();

        if (!uiController.isToggling && !uiController.uiActive)
        {
            opened = true;

            ShopUI shopUI = uiController.shopUI;

            if (shopUI != null)
            {
                player.controller.enabled = false;
                faceCam.gameObject.SetActive(true);
                faceCam.m_Priority = 20;
                shopUI.OpenShopUI(this);
            }
            if (shopUI == null)
            {
                Debug.Log("ERROR: Did not capture Shop UI from uiController", gameObject);
            }
        }
    }

    
}
