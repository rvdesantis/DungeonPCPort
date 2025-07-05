using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHintsUI : MonoBehaviour
{
    public TextMeshProUGUI tipDetailTextMP;
    public List<string> tipStrings;
    public List<string> usedTips;

    public bool isToggling;

    private void OnEnable()
    {
        StartCycle();
    }

    private void OnDisable()
    {
        isToggling = false;
    }


    public void StartCycle()
    {
        if (!isToggling)
        {
            StartCoroutine(MessageTimer());
        }       
    }

    IEnumerator MessageTimer()
    {
        Debug.Log("Starting Tip Loop"); 
        isToggling = true;
        if (tipStrings.Count == 0)
        {
            Debug.Log("Resetting Tip Loop Tips");
            foreach (string tip in usedTips)
            {
                tipStrings.Add(tip);
            }
            usedTips.Clear();
        }
        int x = Random.Range(0, tipStrings.Count);
        tipDetailTextMP.text = tipStrings[x];
        Debug.Log("Tip Set");
        yield return new WaitForSeconds(.25f);       
        usedTips.Add(tipStrings[x]);
        tipStrings.Remove(tipStrings[x]);

        yield return new WaitForSeconds(8);
        isToggling = false;
        StartCycle();
    }

    void Update()
    {
        
    }
}
