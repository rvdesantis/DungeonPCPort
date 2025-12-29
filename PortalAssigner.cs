using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalAssigner : MonoBehaviour
{
    public SideExtenderCube sideExtender;
    public DunPortal lPortal;
    public DunPortal rPortal;


    private void Start()
    {
        AssignPortal();
    }

    public void AssignPortal()
    {
        DistanceController distance = FindAnyObjectByType<DistanceController>();
        if (sideExtender.sideType == SideExtenderCube.SideType.portal)
        {
            bool left = false;
            if (sideExtender.lSmall.activeSelf)
            {
                left = true;
            }

            if (left)
            {
                sideExtender.SetPortal(lPortal);
                lPortal.gameObject.SetActive(true);
                distance.portals.Add(lPortal);
            }
            else
            {
                sideExtender.SetPortal(rPortal);
                rPortal.gameObject.SetActive(true);
                distance.portals.Add(rPortal);
            }
        }
    }
}
