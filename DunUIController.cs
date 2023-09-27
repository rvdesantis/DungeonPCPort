using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    private bool isSpacebarDown = false;
    private float timeHeldDown = 0f;

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
                    Button reset = startButtons[1];
                    reset.gameObject.SetActive(true);
                    reset.Select();

                    startButtons[2].gameObject.SetActive(true);
                    uiActive = true;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;

                    if (controller.activePlayable != null)
                    {
                        controller.activePlayable.Pause();
                    }
                    StartCoroutine(ToggleTimer());
                }
                if (controller.activePlayable != null)
                {
                    if (controller.activePlayable.state == UnityEngine.Playables.PlayState.Playing)
                    {
                        if (Input.GetKey(KeyCode.Space))
                        {
                            if (!isSpacebarDown)
                            {
                                isSpacebarDown = true;
                                timeHeldDown = 0f;
                            }
                            else
                            {
                                timeHeldDown += Time.deltaTime;
                                if (timeHeldDown >= 1f)
                                {   
                                    if (controller.endAction != null)
                                    {
                                        controller.endAction.Invoke();
                                    }
                                }
                            }
                        }
                        else
                        {
                            isSpacebarDown = false;
                            timeHeldDown = 0f;
                        }
                    }
                }

            }

            if (uiActive && !isToggling)
            {
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7))
                {
                    isToggling = true;
                    Button reset = startButtons[1];
                    reset.gameObject.SetActive(false);
                    startButtons[2].gameObject.SetActive(false);
                    uiActive = false;
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    StartCoroutine(ToggleTimer());
                    if (controller.activePlayable != null)
                    {
                        controller.activePlayable.Play();
                    }
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
