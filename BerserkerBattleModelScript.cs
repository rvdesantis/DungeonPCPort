using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerserkerBattleModelScript : BattleModel
{
    public GameObject activeSecondWeapon;
    public List<GameObject> secondWeaponList;

    public override void ActivateWeapon()
    {
        if (activeWeapon != null)
        {
            activeWeapon.gameObject.SetActive(true);
        }
        if (activeSecondWeapon != null)
        {
            activeSecondWeapon.gameObject.SetActive(true);
        }
    }
}
