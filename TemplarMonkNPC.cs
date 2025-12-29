using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemplarMonkNPC : DunNPC
{
    public MessagePanelUI messagePanel;
    public bool isToggling;
    public string dialog;


    public override void NPCTrigger()
    {
        if (inRange && !isToggling)
        {
            dialog = "Templar Monk:\nPlease speak with the head of our order.";
            isToggling = true;
            if (messagePanel == null)
            {
                messagePanel = FindAnyObjectByType<DunUIController>().messagePanelUI;
            }
            StartCoroutine(MessageTimer());
        }
    }


    IEnumerator MessageTimer()
    {       
        messagePanel.OpenMessage(dialog);
        yield return new WaitForSeconds(3);
        if (messagePanel.currentString == dialog)
        {
            messagePanel.gameObject.SetActive(false);
        }
        isToggling = false;       
    }

}
