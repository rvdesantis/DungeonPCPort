using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

public class InputController : MonoBehaviour
{
    public enum ActiveInput { keyboard, joystick}
    public ActiveInput activeInput; 
    public SceneController controller;
    public DunUIController uiController;

    public bool isActionBTDown = false;
    public float timeHeldDown = 0f;
    public bool actionHOLD;

    private void ButtonHoldChecker()
    {
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Joystick1Button0))
        {
            if (!isActionBTDown)
            {
                isActionBTDown = true;
                timeHeldDown = 0f;
                if (uiController.joystick && controller.activePlayable != null)
                {
                    uiController.skipUI.spaceBar.gameObject.SetActive(false);
                    uiController.skipUI.aButton.gameObject.SetActive(true);
                    uiController.skipUI.slider.value = 0;
                    uiController.skipUI.slider.gameObject.SetActive(true);
                }
                if (!uiController.joystick && controller.activePlayable != null)
                {
                    uiController.skipUI.spaceBar.gameObject.SetActive(true);
                    uiController.skipUI.aButton.gameObject.SetActive(false);
                    uiController.skipUI.slider.value = 0;
                    uiController.skipUI.slider.gameObject.SetActive(true);
                }
            }
            else
            {
                timeHeldDown += Time.deltaTime;
                uiController.skipUI.slider.value = timeHeldDown;
                // Check if the spacebar has been held down for the required time.
                if (timeHeldDown >= 1)
                {
                    actionHOLD = true;
                    Debug.Log("Spacebar held down for " + 1 + " seconds.");
                    // Perform your desired action here.
                    if (controller.activePlayable != null)
                    {
                        uiController.skipUI.spaceBar.gameObject.SetActive(false);
                        uiController.skipUI.aButton.gameObject.SetActive(false);
                        uiController.skipUI.slider.value = 0;
                        uiController.skipUI.slider.gameObject.SetActive(false);

                        controller.SkipScene();
                        actionHOLD = false;
                        isActionBTDown = false;
                    }
                }
            }
        }
        else
        {
            isActionBTDown = false;
            timeHeldDown = 0f;
        }
    }

    private void Update()
    {
        ButtonHoldChecker();
    }

}
