using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DefeatUI : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI body;
    public Button restartButton;
    public Button exitButton;
    public DunUIController dunUI;


    public void OpenDefeat(int goldLoss)
    {
        restartButton.Select();
        if (dunUI == null)
        {
            FindObjectOfType<DunUIController>();
        }
        string lineZero = "You have dropped half your gold: (" + goldLoss + ")";
        if (GoldenChestTrinket.goldenChestActive)
        {
            lineZero = "No Gold Dropped (Golden Chest)";
        }  
        string lineOne = "Experience Gained: (xxxx)";
        string lineTwo = "Total Battles: (xxxx)";
        string lineThree = "Bosses Defeated: (x)";
        string lineFour = "Total Chests Opened: (xxxx)";
        string lineFive = "False Walls Opened: (xx)";

        body.text = lineZero + "\n" + lineOne + "\n" + lineTwo + "\n" + lineThree + "\n" + lineFour + "\n" + lineFive;
    }

    public void RestartButton()
    {        
        dunUI.controller.builder.ReLoadScene();
    }

    public void ExitButton()
    {
        dunUI.UIExitGame();
    }

}
