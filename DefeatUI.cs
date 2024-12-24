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
    public StatsTracker stats;

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
        string lineOne = "Experience Gained: (" + stats.totalXP + ")";
        string lineTwo = "Total Battles: (" + stats.battles + ")";
        string lineThree = "Bosses Defeated: (" + stats.bosses + ")";
        string lineFour = "Total Chests Opened: (" + stats.openedChests + ")";
        string lineFive = "False Walls Opened: (" + stats.openedWalls + ")";
        string lineSix = "Traps Triggered: (" + stats.trapsTotal + ")";

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
