using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DunTrap : MonoBehaviour
{




    public virtual void SetSideTrap(SideExtenderCube sideCube)
    {
        sideCube.activeTrap = this;
        sideCube.sideType = SideExtenderCube.SideType.trap;
    }

    public virtual void TriggerTrap()
    {

    }
}
