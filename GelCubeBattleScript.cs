using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GelCubeBattleScript : EnemyBattleModel
{
    public Transform gulpTransform;


    public void Gulp(BattleModel target)
    {
        target.transform.position = gulpTransform.position;
        target.transform.parent = gulpTransform;
    }
}
