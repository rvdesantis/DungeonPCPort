using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DunUIController : MonoBehaviour
{
    public SceneController controller;
    public bool uiStart;
    public bool uiActive;
    public bool isToggling;
    public List<Button> startButtons;
    public GameObject compassObj;
    public GameObject titleObj;
    public bool joystick;
    public LoadingBarUI loadingBar;


    IEnumerator ToggleTimer()
    {
        yield return new WaitForSeconds(.25f);
        isToggling = false;
    }

    public void UIExitGame()
    {
        // saves
        Application.Quit();
    }

    private void Update()
    {
        if (!isToggling)
        {
            if (!uiActive)
            {
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7))
                {
                    isToggling = true;
                    compassObj.SetActive(false);
                    Button reset = startButtons[1];
                    reset.gameObject.SetActive(true);
                    reset.Select();

                    startButtons[2].gameObject.SetActive(true);
                    uiActive = true;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    StartCoroutine(ToggleTimer());
                }
            }

            if (uiActive && !isToggling)
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    bool highlighted = false;
                    foreach (Button button in startButtons)
                    {
                        if (EventSystem.current.currentSelectedGameObject == button)
                        {
                            highlighted = true;
                        }
                    }

                    if (!highlighted)
                    {
                        if (startButtons[0].gameObject.activeSelf)
                        {
                            startButtons[0].Select();
                        }
                        else
                        {
                            startButtons[1].Select();
                        }
                    }
                }
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7))
                {
                    isToggling = true;
                    Button reset = startButtons[1];
                    reset.gameObject.SetActive(false);
                    startButtons[2].gameObject.SetActive(false);
                    uiActive = false;
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    compassObj.SetActive(true);
                    StartCoroutine(ToggleTimer());
                }
            }
        }

        if (!uiStart)
        {
            if (startButtons[0].gameObject.activeSelf)
            {
                uiStart = true;
                startButtons[0].Select();
            }
        }

        if (!joystick)
        {
            if (Input.GetKeyDown(KeyCode.JoystickButton0))
            {
                joystick = true;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }
}
