using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public enum MapInventoryStatus { sketched, outlined, full, secret}
    public MapInventoryStatus mapstatus;
    public MapItem mapItemPrefab;
    public int gold;
    public List<DunItem> dungeonItems;
    public List<DunItem> battleItems;
    public List<DunItem> keyItems;


}
