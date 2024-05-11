using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FAQUIController : MonoBehaviour
{
    public DunUIController uiController;
    public Button nxtBT;
    public Button extBT;
    public TextMeshProUGUI titleTXT;
    public TextMeshProUGUI bodyTXT;
    public List<string> titleStrings;
    public List<string> bodyStrings;
    public int pageIndex;

    public void OpenFAQ()
    {
        uiController.CloseMenuUI();
        uiController.controller.playerController.enabled = false;
        Cursor.visible = true;
        gameObject.SetActive(true);
        titleTXT.text = titleStrings[0];
        bodyTXT.text = bodyStrings[0];
        pageIndex = 0;
        nxtBT.Select();
    }

    public void NextBT()
    {
        if (pageIndex < titleStrings.Count - 1)
        {
            pageIndex++;
            titleTXT.text = titleStrings[pageIndex];
            bodyTXT.text = bodyStrings[pageIndex];
            if (pageIndex == titleStrings.Count - 1)
            {
                nxtBT.gameObject.SetActive(false);
                extBT.Select();
            }
        }
        
    }

    public void ExitBT()
    {
        uiController.OpenMenuUI();
        uiController.RemoteToggleTimer();
        uiController.controller.playerController.enabled = true;
        gameObject.SetActive(false);
    }
}
