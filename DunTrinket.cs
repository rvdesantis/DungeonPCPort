using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DunTrinket : DunItem
{
    public Action afterActivationAction;

    public virtual void SetTrinket()
    {


    }

    public override void PickUp()
    {
        base.PickUp();
        SetTrinket();
    }

    public virtual void ActivateTrinket()
    {

    }

    public virtual void ActivateDungeonEffect()
    {

    }

    public virtual void ActivateBattleStartEffect()
    {

    }

    public virtual void ActivateBattleTurnEffect()
    {

    }
}
