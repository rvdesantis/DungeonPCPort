using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapIcon : MonoBehaviour
{
    public MapController mapController;
    public GameObject attachedObject;
    public SpriteRenderer iconBase;
    public SpriteRenderer visitedBase;
    public SpriteRenderer icon;
    public SpriteRenderer visitedFrame;


    public bool onMap;
    
    /*
    private void Update()
    {
        if (icon.gameObject.activeSelf)
        {
            if (icon.sprite != null)
            {
                
            }
        }
    }
    */

}
