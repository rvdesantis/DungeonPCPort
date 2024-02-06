using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessagePanelUI : MonoBehaviour
{
    public TextMeshProUGUI text;
    public string currentString;

    public void OpenMessage(string message)
    {
        currentString = message;
        text.text = message;
        gameObject.SetActive(true);
    }

    public void CloseMessageTimer(float time)
    {
        StartCoroutine(MessageTimer(time));
    }

    public IEnumerator MessageTimer(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }
}
