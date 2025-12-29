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
        DistanceController distance = FindAnyObjectByType<DistanceController>();
        Instantiate(shopPrefab, transform.position, transform.rotation);
        distance.npcS.Add(this);
        Debug.Log("Battle Shop Spawned", thisModel.gameObject);
        uIController = FindAnyObjectByType<DunUIController>();
    }
    
}
