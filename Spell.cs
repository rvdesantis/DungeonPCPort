using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public enum SpellType { heal, fire, thunder, voidMag, ice, nature}
    public SpellType spellType;

    public Transform targetTransform;
 
}
