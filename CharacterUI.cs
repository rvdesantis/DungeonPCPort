using DTT.PlayerPrefsEnhanced;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    public enum UIType { CSelect, CMenu}
    public UIType uiType;
    public Photobooth photoBooth;
    public DunUIController uiController;
    public SelectController selectController;
    public PartyController party;

    public TextMeshProUGUI titleTXT;

    public Image armorImage;
    public Button armorBT;
    public TextMeshProUGUI armorTXT;
    public TextMeshProUGUI armorPERPERTXT;

    public Image weaponImage;
    public Button weaponBT;
    public TextMeshProUGUI weaponTXT;
    public TextMeshProUGUI weaponPERTXT;

    public Image skillImage;
    public Button skillBT;
    public TextMeshProUGUI skillTXT;
    public TextMeshProUGUI skillPerTXT;

    public List<Sprite> armorSprites; // 0 - mage, 1 - med, 2 - heavy
    public List<Sprite> weaponSprites; // 0 - sword, 1 - staff, 2 - heavy,3 - bow

    public Button rArrowBT;
    public Button lArrowBT;

    public Image spell0Image;
    public Image spell1Image;
    public Image spell2Image;

    public TextMeshProUGUI spellTXT0;
    public TextMeshProUGUI spellTXT1;
    public TextMeshProUGUI spellTXT2;

    public Button exitBT;
    public Button selectBT;

    public bool toggling;
    public bool firstOpen;
    public int partyIndex;

    public GameObject infoTab;
    public TextMeshProUGUI heroInfoTabTXT;
    public TextMeshProUGUI heroLoreTabTXT;
    public List<GameObject> selectParents;
    public List<TextMeshProUGUI> selectParentTXTs;
    public List<AudioClip> uiSounds;
    IEnumerator ToggleTimer()
    {
        yield return new WaitForSeconds(.25f);
        toggling = false;
    }


    IEnumerator CloseTimer()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        toggling = true;
        firstOpen = true;
        uiController.isToggling = true;
        photoBooth.ResetBooth();
        yield return new WaitForSeconds(.15f);
        toggling = false;
        uiController.RemoteToggleTimer();
        uiController.uiActive = false;
        player.enabled = true;

        if (uiType == UIType.CSelect)
        {
            heroInfoTabTXT.text = "";
            infoTab.gameObject.SetActive(false);
            uiType = UIType.CMenu;
        }
        gameObject.SetActive(false);
    }


    public void CloseUI()
    {
        StartCoroutine(CloseTimer());
    }


    public void LoadStats(DunModel activeHero, bool select = false)
    {
        if (firstOpen)
        {
            partyIndex = 0;
            rArrowBT.Select();
            firstOpen = false;
        }
        if (!select)
        {
            if (!exitBT.gameObject.activeSelf)
            {
                exitBT.gameObject.SetActive(true);
            }
            if (selectBT.gameObject.activeSelf)
            {
                selectBT.gameObject.SetActive(false);
            }
        }

        photoBooth.SayCheese(activeHero);
        BattleModel battleMod = null;
        foreach (BattleModel battle in party.combatMaster)
        {
            if (battle.modelName == activeHero.modelName)
            {
                battleMod = battle;
                break;
            }
        }

        int x = party.activeParty.IndexOf(activeHero);

        string nameStats = "";
        nameStats = nameStats + activeHero.modelName + "\n";
        int health = battleMod.health;
        int maxHealth = battleMod.maxH;
        int mana = battleMod.mana;
        int maxMana = battleMod.maxM;

        nameStats = nameStats + "HP: " + health + " / " + maxHealth + "\n";
        nameStats = nameStats + "MP: " + mana + " / " + maxMana;

        titleTXT.text = nameStats;

        BattleModel targetBattle = null;

        foreach (BattleModel battle in party.combatMaster)
        {
            if (battle.modelName == activeHero.modelName)
            {
                targetBattle = battle;
                break;
            }
        }

        float armorP = 0;   
        float weaponP = 0;
        float spellP = 0;

        armorTXT.text = targetBattle.def.ToString();
        armorP = EnhancedPrefs.GetPlayerPref(activeHero.modelName + "DefPercent", 0f);
        armorPERPERTXT.text = "+ " + armorP.ToString() + "%";

        weaponTXT.text = targetBattle.power.ToString();
        weaponP = EnhancedPrefs.GetPlayerPref(activeHero.modelName + "PowPercent", 0f);
        weaponPERTXT.text = "+ " + weaponP.ToString() + "%";

        spellP = EnhancedPrefs.GetPlayerPref(activeHero.modelName + "SpellPercent", 0f);
        skillPerTXT.text = "+ " + spellP.ToString() + "%";

        if (activeHero.modelClass == DunModel.ModelClass.Warrior)
        {
            armorImage.sprite = armorSprites[2];
            weaponImage.sprite = weaponSprites[0];
        }

        if (activeHero.modelClass == DunModel.ModelClass.Rogue)
        {
            armorImage.sprite = armorSprites[1];
            weaponImage.sprite = weaponSprites[0];
        }

        if (activeHero.modelClass == DunModel.ModelClass.Mage)
        {
            armorImage.sprite = armorSprites[0];
            weaponImage.sprite = weaponSprites[1];
        }

        

        if (select)
        {
            if (uiType == UIType.CMenu)
            {
                uiType = UIType.CSelect;               
            }
            infoTab.SetActive(true);
            heroInfoTabTXT.text = activeHero.modelInfo;
            heroLoreTabTXT.text = activeHero.modelLore;
            exitBT.gameObject.SetActive(false);
            selectBT.gameObject.SetActive(true);

            BattleModel activeBattle = null;
            foreach (BattleModel bModel in party.combatMaster)
            {
                if (bModel.modelName == activeHero.modelName)
                {
                    activeBattle = bModel;
                    break;
                }
            }
            int spellCount = battleMod.masterSpells.Count;
            if (spellCount > 0)
            {
                spell0Image.sprite = activeBattle.activeSpells[0].spellIcon;
                spellTXT0.text = activeBattle.activeSpells[0].spellName;
                spell1Image.gameObject.SetActive(false);
                spell2Image.gameObject.SetActive(false);
            }
            if (spellCount > 1)
            {
                spell1Image.gameObject.SetActive(true);
                spellTXT1.text = activeBattle.activeSpells[1].spellName;
                spell1Image.sprite = activeBattle.activeSpells[1].spellIcon;
            }
            if (spellCount > 2)
            {
                spell2Image.gameObject.SetActive(true);
                spellTXT2.text = activeBattle.activeSpells[2].spellName;
                spell2Image.sprite = activeBattle.activeSpells[2].spellIcon;
            }
        }
        if (!select)
        {
            // set Spells
            BattleModel activeBattle = null;
            foreach (BattleModel bModel in party.combatParty)
            {
                if (bModel.modelName == activeHero.modelName)
                {
                    activeBattle = bModel;
                    break;
                }
            }
            int spellCount = battleMod.activeSpells.Count;
            if (spellCount > 0)
            {
                spell0Image.sprite = activeBattle.activeSpells[0].spellIcon;
                spellTXT0.text = activeBattle.activeSpells[0].spellName;
                spell1Image.gameObject.SetActive(false);
                spell2Image.gameObject.SetActive(false);
            }
            if (spellCount > 1)
            {
                spell1Image.gameObject.SetActive(true);
                spellTXT1.text = activeBattle.activeSpells[1].spellName;
                spell1Image.sprite = activeBattle.activeSpells[1].spellIcon;
            }
            if (spellCount > 2)
            {
                spell2Image.gameObject.SetActive(true);
                spellTXT2.text = activeBattle.activeSpells[2].spellName;
                spell2Image.sprite = activeBattle.activeSpells[2].spellIcon;
            }
        }

        StartCoroutine(ToggleTimer());

    }



    public void LeftButton()
    {

        if (!toggling)
        {
            uiController.uiAudioSource.PlayOneShot(uiSounds[1]);
            if (uiType == UIType.CMenu)
            {
                if (partyIndex == 0)
                {
                    partyIndex = 2;
                    toggling = true;
                    LoadStats(party.activeParty[2]);
                    return;
                }
                if (partyIndex == 1)
                {
                    partyIndex = 1;
                    toggling = true;
                    LoadStats(party.activeParty[1]);
                    return;
                }
                if (partyIndex == 2)
                {
                    partyIndex = 0;
                    toggling = true;
                    LoadStats(party.activeParty[0]);
                    return;
                }
            }
           
            if (uiType == UIType.CSelect)
            {
                selectController.LeftArrowAdd();                          
            } 
        }

    }

    public void RightButton()
    {    
        if (!toggling)
        {
            uiController.uiAudioSource.PlayOneShot(uiSounds[1]);
            if (uiType == UIType.CMenu)
            {
                if (partyIndex == 0)
                {
                    partyIndex = 1;
                    toggling = true;
                    LoadStats(party.activeParty[1]);
                    return;
                }
                if (partyIndex == 1)
                {
                    partyIndex = 2;
                    toggling = true;
                    LoadStats(party.activeParty[2]);
                    return;
                }
                if (partyIndex == 2)
                {
                    partyIndex = 0;
                    toggling = true;
                    LoadStats(party.activeParty[0]);
                    return;
                }
            }

            if (uiType == UIType.CSelect)
            {
                selectController.RightArrowAdd();              
            }
        }
    }




}
