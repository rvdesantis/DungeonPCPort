using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DunUIController : MonoBehaviour
{
    public bool uiActive;
    public List<Button> startButtons;
    public GameObject compassObj;
    public GameObject titleObj;

    private void Update()
    {
        if (!uiActive)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Button reset = startButtons[1];
                reset.gameObject.SetActive(true);
                reset.Select();
                uiActive = true;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        if (uiActive)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Button reset = startButtons[1];
                reset.gameObject.SetActive(false);
 
                uiActive = false;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}
