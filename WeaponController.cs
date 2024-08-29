using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public BattleModel bModel;
    public GameObject activeWeapon;
    public List<GameObject> weapons;
    public List<ParticleSystem> weaponFXs; // 0 - Fire, 
}
