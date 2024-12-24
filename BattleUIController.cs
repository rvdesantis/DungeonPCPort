using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using System.Linq;
using UnityEngine.UI;
using Unity.VisualScripting;
using TMPro;
using CryptUI.Scripts;
using UnityEngine.EventSystems;
using DTT.Utils.Extensions;

public class BattleUIController : MonoBehaviour
{

    public BattleController battleC;
    public BattlePhaseUI phaseUI;

    public GameObject fadeImageObj;
    public Animator fadeImageAnim;
    public PlayableDirector fadeDarkPlayable;
    public AudioSource audioSource;
    public List<AudioClip> audioClips;

    public List<Sprite> actionIconsGray;
    public List<Sprite> actionIconsColor;
    public List<Image> lActionIcons;
    public List<Image> rActionIcons;

    public List<Button> lActionButtons;
    public List<Button> lSpellButtons;
    public List<TextMeshProUGUI> lSpellText;   

    public List<Button> rActionButtons;
    public List<Button> rSpellButtons;
    public List<TextMeshProUGUI> rSpellText;
    public GameObject lSpellObj;
    public GameObject rSpellObj;
    public Image lSpellIcon;
    public Image rSpellIcon;
    public TextMeshProUGUI lSpellTXT;
    public TextMeshProUGUI rSpellTXT;
    public TextMeshProUGUI lSpellTypeTXT;
    public TextMeshProUGUI rSpellTypeTXT;

    public GameObject lItemFrame;
    public GameObject rItemFrame;
    public Image lItemIcon;
    public Image rItemIcon;

    public Button leftLArrowItemBT;
    public Button leftRArrowItemBT;
    public Button rightLArrowItemBT;
    public Button rightRArrowItemBT;

    public List<Button> lItemButtons;
    public List<Button> rItemButtons;

    public TextMeshProUGUI lItemTXT;
    public TextMeshProUGUI rItemTXT;

    public GameObject lInfoFrame;
    public TextMeshProUGUI lInfoTXT;

    public GameObject rInfoFrame;
    public TextMeshProUGUI rInfoTXT;

    public int itemIndex;
    public int spellIndex;  

    public List<Button> enemySelectButtons;
    public List<TargetButtonFrameUI> enemyButtonFrames;
    public List<Button> partySelectButtons;
    public List<TargetButtonFrameUI> partyButtonFrames;

    public List<Slider> enemySelectSliders;
    public List<Slider> partySelectSliders;


    public List<TextMeshProUGUI> enemyBTText;
    public List<TextMeshProUGUI> partyBTText;

    public Slider lHealthSlider;
    public Slider rHealthSlider;

    public DefeatUI defeatUI;
    public VictoryUI victoryUI;


    public void SetActionButtons(bool left, bool close = false)
    {
        if (!close)
        {
            if (left)
            {
                foreach (Button bt in lActionButtons)
                {
                    int x = lActionButtons.IndexOf(bt);
                    bt.gameObject.SetActive(true);
                    if (x == 0)
                    {
                        bt.Select();
                    }
                }
                foreach (Button bt in rActionButtons)
                {
                    bt.gameObject.SetActive(false);
                }
                rInfoFrame.SetActive(false);
                rHealthSlider.gameObject.SetActive(false);
                lHealthSlider.gameObject.SetActive(true);
                lHealthSlider.maxValue = battleC.heroParty[1].maxH;
                lHealthSlider.value = battleC.heroParty[1].health;

                lInfoFrame.SetActive(true);
                lInfoTXT.text = battleC.heroParty[1].modelName;
                Debug.Log("Setting Buttons for Hero 1");
                battleC.activeRoom.targetingCams[0].m_Priority = 0;
                battleC.activeRoom.targetingCams[1].m_Priority = 10;
                battleC.activeRoom.targetingCams[2].m_Priority = 0;
            }
            if (!left)
            {
                foreach (Button bt in lActionButtons)
                {
                    bt.gameObject.SetActive(false);
                }
                int index = battleC.heroIndex;

                rHealthSlider.gameObject.SetActive(true);
                rInfoFrame.SetActive(true);
                lInfoFrame.SetActive(false);
                if (index == 0)
                {
                    Debug.Log("Setting Buttons for Hero 0");
                    rHealthSlider.maxValue = battleC.heroParty[0].maxH;
                    rHealthSlider.value = battleC.heroParty[0].health;                    
                    rInfoTXT.text = battleC.heroParty[0].modelName;
                    battleC.activeRoom.targetingCams[0].m_Priority = 10;
                    battleC.activeRoom.targetingCams[1].m_Priority = 0;
                    battleC.activeRoom.targetingCams[2].m_Priority = 0;
                }
                if (index == 2)
                {
                    Debug.Log("Setting Buttons for Hero 2");
                    rHealthSlider.maxValue = battleC.heroParty[2].maxH;
                    rHealthSlider.value = battleC.heroParty[2].health;
                    rInfoTXT.text = battleC.heroParty[2].modelName;
                    battleC.activeRoom.targetingCams[0].m_Priority = 0;
                    battleC.activeRoom.targetingCams[1].m_Priority = 0;
                    battleC.activeRoom.targetingCams[2].m_Priority = 10;
                }
                lHealthSlider.gameObject.SetActive(false);
                foreach (Button bt in rActionButtons)
                {
                    int x = rActionButtons.IndexOf(bt);
                    if (x == 3)
                    {
                        if (battleC.heroIndex == 2)
                        {
                            bt.gameObject.SetActive(true);
                        }
                    }
                    if (x != 3)
                    {
                        bt.gameObject.SetActive(true);
                    }
               
                    if (x == 0)
                    {
                        rActionButtons[3].gameObject.SetActive(false);
                        bt.Select();
                    }
                }

            }
        }
    }

    public void ColorSpellType(bool left, Spell targetSpell)
    {        
        if (!left)
        {
            if (targetSpell.spellType == Spell.SpellType.fire)
            {
                rSpellTypeTXT.color = Color.red;
            }
            if (targetSpell.spellType == Spell.SpellType.voidMag)
            {
                rSpellTypeTXT.color = Color.magenta;
            }
            if (targetSpell.spellType == Spell.SpellType.defense)
            {
                rSpellTypeTXT.color = Color.gray;
            }
            if (targetSpell.spellType == Spell.SpellType.offense)
            {
                rSpellTypeTXT.color = Color.white;
            }
            if (targetSpell.spellType == Spell.SpellType.ice)
            {
                rSpellTypeTXT.color = Color.cyan;
            }
            if (targetSpell.spellType == Spell.SpellType.thunder)
            {
                rSpellTypeTXT.color = Color.yellow;
            }
        }
        if (left)
        {
            if (targetSpell.spellType == Spell.SpellType.fire)
            {
                lSpellTypeTXT.color = Color.red;
            }
            if (targetSpell.spellType == Spell.SpellType.voidMag)
            {
                lSpellTypeTXT.color = Color.magenta;
            }
            if (targetSpell.spellType == Spell.SpellType.defense)
            {
                lSpellTypeTXT.color = Color.gray;
            }
            if (targetSpell.spellType == Spell.SpellType.offense)
            {
                lSpellTypeTXT.color = Color.white;
            }
            if (targetSpell.spellType == Spell.SpellType.ice)
            {
                lSpellTypeTXT.color = Color.cyan;
            }
            if (targetSpell.spellType == Spell.SpellType.thunder)
            {
                lSpellTypeTXT.color = Color.yellow;
            }
        }
    }

    public void StrikeActionBT()
    {
        foreach (Button bt in enemySelectButtons)
        {
            if (bt.gameObject.activeSelf)
            {
                bt.gameObject.SetActive(false);
            }
        }

        foreach (Button bt in partySelectButtons)
        {
            if (bt.gameObject.activeSelf)
            {
                bt.gameObject.SetActive(false);
            }
        }


        foreach (Button bt in lSpellButtons)
        {
            if (bt.gameObject.activeSelf)
            {
                bt.gameObject.SetActive(false);
            }
        }
        lSpellObj.SetActive(false);
        foreach (Button bt in rSpellButtons)
        {
            if (bt.gameObject.activeSelf)
            {
                bt.gameObject.SetActive(false);
            }
        }
        rSpellObj.SetActive(false);

        foreach (Button bt in lItemButtons)
        {
            if (bt.gameObject.activeSelf)
            {
                bt.gameObject.SetActive(false);
            }
        }
        foreach (Button bt in rItemButtons)
        {
            if (bt.gameObject.activeSelf)
            {
                bt.gameObject.SetActive(false);
            }
        }
        rItemFrame.SetActive(false);
        lItemFrame.SetActive(false);

        int x = battleC.heroIndex;
        battleC.heroParty[x].actionType = BattleModel.ActionType.melee;        
 
        if (!battleC.enemyParty[0].dead)
        {
            string name = battleC.enemyParty[0].modelName;
            enemySelectButtons[0].gameObject.SetActive(true);
            enemyBTText[0].text = name;
            enemySelectButtons[0].Select();

            enemySelectSliders[0].maxValue = battleC.enemyParty[0].maxH;
            enemySelectSliders[0].value = battleC.enemyParty[0].health;
        }
        if (!battleC.enemyParty[1].dead)
        {
            string name = battleC.enemyParty[1].modelName;
            enemySelectButtons[1].gameObject.SetActive(true);
            enemyBTText[1].text = name;
            enemySelectSliders[1].maxValue = battleC.enemyParty[1].maxH;
            enemySelectSliders[1].value = battleC.enemyParty[1].health;

            if (!enemySelectButtons[0].gameObject.activeSelf)
            {
                enemySelectButtons[1].Select();
            }
        }
        if (!battleC.enemyParty[2].dead)
        {
            string name = battleC.enemyParty[2].modelName;
            enemySelectButtons[2].gameObject.SetActive(true);
            enemyBTText[2].text = name;

            enemySelectSliders[2].maxValue = battleC.enemyParty[2].maxH;
            enemySelectSliders[2].value = battleC.enemyParty[2].health;

            if (!enemySelectButtons[0].gameObject.activeSelf && !enemySelectButtons[1].gameObject.activeSelf)
            {
                enemySelectButtons[2].Select();
            }
        }
        
    }

    public void SpellActionBT()
    {
        foreach (Button bt in enemySelectButtons)
        {
            if (bt.gameObject.activeSelf)
            {
                bt.gameObject.SetActive(false);
            }
        }
        foreach (Button bt in partySelectButtons)
        {
            if (bt.gameObject.activeSelf)
            {
                bt.gameObject.SetActive(false);
            }
        }

        foreach (Button bt in lSpellButtons)
        {
            bt.gameObject.SetActive(false);
        }
        foreach (Button bt in rSpellButtons)
        {
            bt.gameObject.SetActive(false);
        }
        foreach (Button bt in rItemButtons)
        {
            bt.gameObject.SetActive(false);
        }
        foreach (Button bt in lItemButtons)
        {
            bt.gameObject.SetActive(false);
        }
        rItemFrame.SetActive(false);
        lItemFrame.SetActive(false);

        int x = battleC.heroIndex;
        BattleModel activeBModel = battleC.heroParty[x];

        if (activeBModel.activeSpells.Count > 0)
        {
            spellIndex = 0;
            if (x == 1)
            {
                lSpellObj.SetActive(true);
                lSpellTXT.text = activeBModel.activeSpells[0].spellInfo;
                lSpellTypeTXT.text = activeBModel.activeSpells[0].spellType.ToString();
                ColorSpellType(true, activeBModel.activeSpells[0]);
                lSpellIcon.sprite = activeBModel.activeSpells[0].spellIcon;
                lSpellText[0].text = activeBModel.activeSpells[0].spellName;
                lSpellButtons[0].gameObject.SetActive(true);
                lSpellButtons[0].Select();
            }
            if (x != 1)
            {
                rSpellObj.SetActive(true);
                rSpellTXT.text = activeBModel.activeSpells[0].spellInfo;
                rSpellTypeTXT.text = activeBModel.activeSpells[0].spellType.ToString();
                ColorSpellType(false, activeBModel.activeSpells[0]);
                rSpellIcon.sprite = activeBModel.activeSpells[0].spellIcon;
                rSpellButtons[0].gameObject.SetActive(true);
                rSpellText[0].text = activeBModel.activeSpells[0].spellName;
                rSpellButtons[0].gameObject.SetActive(true);
                rSpellButtons[0].Select();
            }
            return;
        }
        if (activeBModel.activeSpells.Count == 0)
        {
            audioSource.PlayOneShot(audioClips[0]);
            return;
        }
    }

    public void EnemySelectBT(int btNum)
    {
        int x = battleC.heroIndex;
        battleC.heroParty[x].actionTarget = battleC.enemyParty[btNum];
        foreach(Button bt in enemySelectButtons)
        {
            bt.gameObject.SetActive(false);
        }        
        foreach (Button bt in partySelectButtons)
        {
            bt.gameObject.SetActive(false);
        }
        if (x == 0)
        {
            battleC.heroIndex++;
            battleC.HeroOneSelect();
            return;
        }
        if (x == 1)
        {
            battleC.heroIndex++;
            battleC.HeroTwoSelect();
            return;
        }
        if (x == 2)
        {
            battleC.activeRoom.targetingCams[2].m_Priority = -1;
            battleC.activeRoom.mainCam.m_Priority = 20;
            CloseAllButtons();
            battleC.heroIndex = 0;
            battleC.StartPreHeroTimer();
            return;
        }

    }

    public void HeroSelectBT(int btNum)
    {
        int x = battleC.heroIndex;
        battleC.heroParty[x].actionTarget = battleC.heroParty[btNum];
        foreach (Button bt in partySelectButtons)
        {
            bt.gameObject.SetActive(false);
        }
        foreach (Button bt in enemySelectButtons)
        {
            bt.gameObject.SetActive(false);
        }
        if (x == 0)
        {
            battleC.heroIndex++;
            battleC.HeroOneSelect();
            return;
        }
        if (x == 1)
        {
            battleC.heroIndex++;
            battleC.HeroTwoSelect();
            return;
        }
        if (x == 2)
        {
            battleC.activeRoom.targetingCams[2].m_Priority = -1;
            battleC.activeRoom.mainCam.m_Priority = 20;
            CloseAllButtons();
            battleC.heroIndex = 0;
            battleC.StartPreHeroTimer();
            return;
        }
    }

    public void SpellSelectBT()
    {
        int x = battleC.heroIndex;
        BattleModel activeHero = battleC.heroParty[x];
        activeHero.actionType = BattleModel.ActionType.spell;
        activeHero.selectedSpell = battleC.heroParty[x].activeSpells[spellIndex];
        if (activeHero.selectedSpell.spellTargeting == Spell.SpellTargeting.enemies)
        {
            if (!battleC.enemyParty[0].dead)
            {
                string name = battleC.enemyParty[0].modelName;
                enemySelectButtons[0].gameObject.SetActive(true);
                enemyBTText[0].text = name;
                enemySelectButtons[0].Select();

                enemySelectSliders[0].maxValue = battleC.enemyParty[0].maxH;
                enemySelectSliders[0].value = battleC.enemyParty[0].health;
            }
            if (!battleC.enemyParty[1].dead)
            {
                string name = battleC.enemyParty[1].modelName;
                enemySelectButtons[1].gameObject.SetActive(true);
                enemyBTText[1].text = name;

                enemySelectSliders[1].maxValue = battleC.enemyParty[1].maxH;
                enemySelectSliders[1].value = battleC.enemyParty[1].health;

                if (!enemySelectButtons[0].gameObject.activeSelf)
                {
                    enemySelectButtons[1].Select();
                }
            }
            if (!battleC.enemyParty[2].dead)
            {
                string name = battleC.enemyParty[2].modelName;
                enemySelectButtons[2].gameObject.SetActive(true);
                enemyBTText[2].text = name;

                enemySelectSliders[2].maxValue = battleC.enemyParty[2].maxH;
                enemySelectSliders[2].value = battleC.enemyParty[2].health;

                if (!enemySelectButtons[0].gameObject.activeSelf && !enemySelectButtons[1].gameObject.activeSelf)
                {
                    enemySelectButtons[2].Select();
                }
            }
        }
        if (activeHero.selectedSpell.spellTargeting == Spell.SpellTargeting.party)
        {
            if (!battleC.heroParty[0].dead)
            {
                string name = battleC.heroParty[0].modelName;
                partySelectButtons[0].gameObject.SetActive(true);
                partyBTText[0].text = name;

                partySelectSliders[0].maxValue = battleC.heroParty[0].maxH;
                partySelectSliders[0].value = battleC.heroParty[0].health;
                if (x == 0)
                {
                    partySelectButtons[0].Select();
                }
            }
            if (!battleC.heroParty[1].dead)
            {
                string name = battleC.heroParty[1].modelName;
                partySelectButtons[1].gameObject.SetActive(true);
                partyBTText[1].text = name;

                partySelectSliders[1].maxValue = battleC.heroParty[1].maxH;
                partySelectSliders[1].value = battleC.heroParty[1].health;

                if (x == 1)
                {
                    partySelectButtons[1].Select();
                }
            }
            if (!battleC.heroParty[2].dead)
            {
                string name = battleC.heroParty[2].modelName;
                partySelectButtons[2].gameObject.SetActive(true);
                partyBTText[2].text = name;

                partySelectSliders[2].maxValue = battleC.heroParty[2].maxH;
                partySelectSliders[2].value = battleC.heroParty[2].health;

                if (x == 2)
                {
                    partySelectButtons[2].Select();
                }
            }
        }
        if (activeHero.selectedSpell.spellTargeting == Spell.SpellTargeting.all)
        {
            if (!battleC.enemyParty[0].dead)
            {
                string name = battleC.enemyParty[0].modelName;
                enemySelectButtons[0].gameObject.SetActive(true);
                enemyBTText[0].text = name;
                enemySelectButtons[0].Select();

                enemySelectSliders[0].maxValue = battleC.enemyParty[0].maxH;
                enemySelectSliders[0].value = battleC.enemyParty[0].health;
            }
            if (!battleC.enemyParty[1].dead)
            {
                string name = battleC.enemyParty[1].modelName;
                enemySelectButtons[1].gameObject.SetActive(true);
                enemyBTText[1].text = name;

                enemySelectSliders[1].maxValue = battleC.enemyParty[1].maxH;
                enemySelectSliders[1].value = battleC.enemyParty[1].health;

                if (!enemySelectButtons[0].gameObject.activeSelf)
                {
                    enemySelectButtons[1].Select();
                }
            }
            if (!battleC.enemyParty[2].dead)
            {
                string name = battleC.enemyParty[2].modelName;
                enemySelectButtons[2].gameObject.SetActive(true);
                enemyBTText[2].text = name;

                enemySelectSliders[2].maxValue = battleC.enemyParty[2].maxH;
                enemySelectSliders[2].value = battleC.enemyParty[2].health;

                if (!enemySelectButtons[0].gameObject.activeSelf && !enemySelectButtons[1].gameObject.activeSelf)
                {
                    enemySelectButtons[2].Select();
                }
            }

            if (!battleC.heroParty[0].dead)
            {
                string name = battleC.heroParty[0].modelName;
                partySelectButtons[0].gameObject.SetActive(true);
                partyBTText[0].text = name;

                partySelectSliders[0].maxValue = battleC.heroParty[0].maxH;
                partySelectSliders[0].value = battleC.heroParty[0].health;
                if (x == 0)
                {
                    partySelectButtons[0].Select();
                }
            }
            if (!battleC.heroParty[1].dead)
            {
                string name = battleC.heroParty[1].modelName;
                partySelectButtons[1].gameObject.SetActive(true);
                partyBTText[1].text = name;

                partySelectSliders[1].maxValue = battleC.heroParty[1].maxH;
                partySelectSliders[1].value = battleC.heroParty[1].health;

                if (x == 1)
                {
                    partySelectButtons[1].Select();
                }
            }
            if (!battleC.heroParty[2].dead)
            {
                string name = battleC.heroParty[2].modelName;
                partySelectButtons[2].gameObject.SetActive(true);
                partyBTText[2].text = name;

                partySelectSliders[2].maxValue = battleC.heroParty[2].maxH;
                partySelectSliders[2].value = battleC.heroParty[2].health;

                if (x == 2)
                {
                    partySelectButtons[2].Select();
                }
            }
        }

        lSpellObj.SetActive(false);
        rSpellObj.SetActive(false);
    }


    public void ItemActionBT()
    {
        foreach (Button bt in lSpellButtons)
        {
            bt.gameObject.SetActive(false);
        }
        lSpellObj.SetActive(false);
        foreach (Button bt in rSpellButtons)
        {
            bt.gameObject.SetActive(false);
        }
        rSpellObj.SetActive(false);
        foreach (Button bt in enemySelectButtons)
        {
            if (bt.gameObject.activeSelf)
            {
                bt.gameObject.SetActive(false);
            }
        }

        foreach (Button bt in partySelectButtons)
        {
            if (bt.gameObject.activeSelf)
            {
                bt.gameObject.SetActive(false);
            }
        }
        itemIndex = 0;
        int x = battleC.heroIndex;
        InventoryController inventory = battleC.inventory;
        int itemCount = battleC.inventory.battleItems.Count;
        if (itemCount > 0)
        {
            List<BattleItem> activeBattleItems = battleC.inventory.battleItems;
            if (x == 1)
            {
                lItemFrame.SetActive(true);
                lItemTXT.text = activeBattleItems[0].itemInfo;
                lItemIcon.sprite = activeBattleItems[0].icon;

                lItemButtons[0].gameObject.SetActive(true);
                lItemButtons[0].Select();               
            }
            if (x != 1)
            {
                rItemFrame.SetActive(true);
                rItemTXT.text = activeBattleItems[0].itemInfo;
                rItemIcon.sprite = activeBattleItems[0].icon;

                rItemButtons[0].gameObject.SetActive(true);
                rItemButtons[0].Select();
            }
            return;
        }
        if (itemCount == 0)
        {
            audioSource.PlayOneShot(audioClips[0]);
            return;
        }
    }



    public void ItemSelectBT()
    {
        int x = battleC.heroIndex;
        battleC.heroParty[x].actionType = BattleModel.ActionType.item;
        foreach (Button bt in lItemButtons)
        {
            bt.gameObject.SetActive(false);
        }
        foreach (Button bt in rItemButtons)
        {
            bt.gameObject.SetActive(false);
        } 
        BattleItem selectedItem = null;
        selectedItem = battleC.inventory.battleItems[itemIndex];
        battleC.heroParty[x].selectedItem = selectedItem;

        if (selectedItem.itemTarget == BattleItem.BattleTarget.heroes)
        {
            lItemFrame.gameObject.SetActive(false);
            rItemFrame.gameObject.SetActive(false);
            if (!battleC.heroParty[0].dead)
            {
                string name = battleC.heroParty[0].modelName;
                partySelectButtons[0].gameObject.SetActive(true);
                partyBTText[0].text = name;

                partySelectSliders[0].maxValue = battleC.heroParty[0].maxH;
                partySelectSliders[0].value = battleC.heroParty[0].health;
                if (x == 0)
                {
                    partySelectButtons[0].Select();
                }
            }

            if (!battleC.heroParty[1].dead)
            {
                string name = battleC.heroParty[1].modelName;
                partySelectButtons[1].gameObject.SetActive(true);
                partyBTText[1].text = name;

                partySelectSliders[1].maxValue = battleC.heroParty[1].maxH;
                partySelectSliders[1].value = battleC.heroParty[1].health;

                if (x == 1)
                {
                    partySelectButtons[1].Select();
                }
            }

            if (!battleC.heroParty[2].dead)
            {
                string name = battleC.heroParty[2].modelName;
                partySelectButtons[2].gameObject.SetActive(true);
                partyBTText[2].text = name;

                partySelectSliders[2].maxValue = battleC.heroParty[2].maxH;
                partySelectSliders[2].value = battleC.heroParty[2].health;

                if (x == 2)
                {
                    partySelectButtons[2].Select();
                }
            }
        }
        if (selectedItem.itemTarget == BattleItem.BattleTarget.enemies)
        {
            lItemFrame.gameObject.SetActive(false);
            rItemFrame.gameObject.SetActive(false);
            if (!battleC.enemyParty[0].dead)
            {
                string name = battleC.enemyParty[0].modelName;
                enemySelectButtons[0].gameObject.SetActive(true);
                enemyBTText[0].text = name;

                enemySelectSliders[0].maxValue = battleC.enemyParty[0].maxH;
                enemySelectSliders[0].value = battleC.enemyParty[0].health;
            }
            if (!battleC.enemyParty[1].dead)
            {
                string name = battleC.enemyParty[1].modelName;
                enemySelectButtons[1].gameObject.SetActive(true);
                enemyBTText[1].text = name;

                enemySelectSliders[1].maxValue = battleC.enemyParty[1].maxH;
                enemySelectSliders[1].value = battleC.enemyParty[1].health;
            }
            if (!battleC.enemyParty[2].dead)
            {
                string name = battleC.enemyParty[2].modelName;
                enemySelectButtons[2].gameObject.SetActive(true);
                enemyBTText[2].text = name;

                enemySelectSliders[2].maxValue = battleC.enemyParty[2].maxH;
                enemySelectSliders[2].value = battleC.enemyParty[2].health;
            }
        }
        if (selectedItem.itemTarget == BattleItem.BattleTarget.all)
        {
            lItemFrame.gameObject.SetActive(false);
            rItemFrame.gameObject.SetActive(false);
            if (!battleC.enemyParty[0].dead)
            {
                string name = battleC.enemyParty[0].modelName;
                enemySelectButtons[0].gameObject.SetActive(true);
                enemyBTText[0].text = name;

                enemySelectSliders[0].maxValue = battleC.enemyParty[0].maxH;
                enemySelectSliders[0].value = battleC.enemyParty[0].health;
            }
            if (!battleC.enemyParty[1].dead)
            {
                string name = battleC.enemyParty[1].modelName;
                enemySelectButtons[1].gameObject.SetActive(true);
                enemyBTText[1].text = name;

                enemySelectSliders[1].maxValue = battleC.enemyParty[1].maxH;
                enemySelectSliders[1].value = battleC.enemyParty[1].health;
            }
            if (!battleC.enemyParty[2].dead)
            {
                string name = battleC.enemyParty[2].modelName;
                enemySelectButtons[2].gameObject.SetActive(true);
                enemyBTText[2].text = name;

                enemySelectSliders[2].maxValue = battleC.enemyParty[2].maxH;
                enemySelectSliders[2].value = battleC.enemyParty[2].health;
            }

            if (!battleC.heroParty[0].dead)
            {
                string name = battleC.heroParty[0].modelName;
                partySelectButtons[0].gameObject.SetActive(true);
                partyBTText[0].text = name;

                partySelectSliders[0].maxValue = battleC.heroParty[0].maxH;
                partySelectSliders[0].value = battleC.heroParty[0].health;
                if (x == 0)
                {
                    partySelectButtons[0].Select();
                }
            }
            if (!battleC.heroParty[1].dead)
            {
                string name = battleC.heroParty[1].modelName;
                partySelectButtons[1].gameObject.SetActive(true);
                partyBTText[1].text = name;

                partySelectSliders[1].maxValue = battleC.heroParty[1].maxH;
                partySelectSliders[1].value = battleC.heroParty[1].health;

                if (x == 1)
                {
                    partySelectButtons[1].Select();
                }
            }
            if (!battleC.heroParty[2].dead)
            {
                string name = battleC.heroParty[2].modelName;
                partySelectButtons[2].gameObject.SetActive(true);
                partyBTText[2].text = name;

                partySelectSliders[2].maxValue = battleC.heroParty[2].maxH;
                partySelectSliders[2].value = battleC.heroParty[2].health;

                if (x == 2)
                {
                    partySelectButtons[2].Select();
                }
            }
        }
        if (selectedItem.itemTarget == BattleItem.BattleTarget.dead)
        {
            int deadCount = 0;
            foreach (BattleModel bMod in battleC.heroParty)
            {
                if (bMod.dead && !bMod.pHolder)
                {
                    deadCount++;
                }
            }
            foreach (BattleModel bMod in battleC.enemyParty)
            {
                if (bMod.dead && !bMod.pHolder)
                {
                    deadCount++;
                }
            }
            if (deadCount > 0)
            {
                lItemFrame.gameObject.SetActive(false);
                rItemFrame.gameObject.SetActive(false);

                if (battleC.enemyParty[0].dead && !battleC.enemyParty[0].pHolder)
                {
                    string name = battleC.enemyParty[0].modelName;
                    enemySelectButtons[0].gameObject.SetActive(true);
                    enemySelectButtons[0].Select();
                    enemyBTText[0].text = name;

                    enemySelectSliders[0].maxValue = battleC.enemyParty[0].maxH;
                    enemySelectSliders[0].value = battleC.enemyParty[0].health;
                }
                if (battleC.enemyParty[1].dead && !battleC.enemyParty[1].pHolder)
                {
                    string name = battleC.enemyParty[1].modelName;
                    enemySelectButtons[1].gameObject.SetActive(true);
                    enemySelectButtons[1].Select();
                    enemyBTText[1].text = name;

                    enemySelectSliders[1].maxValue = battleC.enemyParty[1].maxH;
                    enemySelectSliders[1].value = battleC.enemyParty[1].health;
                }
                if (battleC.enemyParty[2].dead && !battleC.enemyParty[2].pHolder)
                {
                    string name = battleC.enemyParty[2].modelName;
                    enemySelectButtons[2].gameObject.SetActive(true);
                    enemySelectButtons[2].Select();
                    enemyBTText[2].text = name;

                    enemySelectSliders[2].maxValue = battleC.enemyParty[2].maxH;
                    enemySelectSliders[2].value = battleC.enemyParty[2].health;
                }

                if (battleC.heroParty[0].dead && !battleC.heroParty[0].pHolder)
                {
                    string name = battleC.heroParty[0].modelName;
                    partySelectButtons[0].gameObject.SetActive(true);
                    partySelectButtons[0].Select();
                    partyBTText[0].text = name;

                    partySelectSliders[0].maxValue = battleC.heroParty[0].maxH;
                    partySelectSliders[0].value = battleC.heroParty[0].health;
                }
                if (battleC.heroParty[1].dead && !battleC.heroParty[1].pHolder)
                {
                    string name = battleC.heroParty[1].modelName;
                    partySelectButtons[1].gameObject.SetActive(true);
                    partySelectButtons[1].Select();
                    partyBTText[1].text = name;

                    partySelectSliders[1].maxValue = battleC.heroParty[1].maxH;
                    partySelectSliders[1].value = battleC.heroParty[1].health;
                }
                if (battleC.heroParty[2].dead && !battleC.heroParty[2].pHolder)
                {
                    string name = battleC.heroParty[2].modelName;
                    partySelectButtons[2].gameObject.SetActive(true);
                    partyBTText[2].text = name;

                    partySelectSliders[2].maxValue = battleC.heroParty[2].maxH;
                    partySelectSliders[2].value = battleC.heroParty[2].health;
                    partySelectButtons[2].Select();

                }
            }
            if (deadCount == 0)
            {
                audioSource.PlayOneShot(audioClips[1]);
                if (lItemFrame.activeSelf)
                {
                    lItemFrame.gameObject.SetActive(false);
                    lActionButtons[2].Select();
                }
                if (rItemFrame.activeSelf)
                {
                    rItemFrame.gameObject.SetActive(false);
                    rActionButtons[2].Select();
                }
   
                
            }

        }

    }

    public void ItemArrowLeft(bool leftPanel)
    {
        int itemCount = battleC.inventory.battleItems.Count;  
        if (itemCount != 0)
        {
            bool reduced = false;
            if (itemIndex != 0)
            {
                itemIndex--;
                reduced = true;
            }
            if (itemIndex == 0 && !reduced)
            {
                itemIndex = itemCount - 1;
            }

            if (leftPanel)
            {
                lItemIcon.sprite = battleC.inventory.battleItems[itemIndex].icon;
                lItemTXT.text = battleC.inventory.battleItems[itemIndex].itemInfo;
            }
            if (!leftPanel)
            {
                rItemIcon.sprite = battleC.inventory.battleItems[itemIndex].icon;
                rItemTXT.text = battleC.inventory.battleItems[itemIndex].itemInfo;
            }
        }   
    }

    public void ItemArrowRight(bool leftPanel)
    {
        int itemCount = battleC.inventory.battleItems.Count;
        if (itemCount != 0)
        {
            bool increased = false;
            if (itemIndex != itemCount - 1)
            {
                itemIndex++;
                increased = true;
            }
            if (itemIndex == itemCount - 1 && !increased)
            {
                itemIndex = 0;
            }

            if (leftPanel)
            {
                lItemIcon.sprite = battleC.inventory.battleItems[itemIndex].icon;
                lItemTXT.text = battleC.inventory.battleItems[itemIndex].itemInfo;
            }
            if (!leftPanel)
            {
                rItemIcon.sprite = battleC.inventory.battleItems[itemIndex].icon;
                rItemTXT.text = battleC.inventory.battleItems[itemIndex].itemInfo;
            }
        }
    }

    public void SpellArrowLeft(bool leftPanel)
    {
        bool clicked = false;
        BattleModel activeBModel = battleC.heroParty[battleC.heroIndex];
        if (activeBModel.activeSpells.Count == 1)
        {
            Debug.Log("Only 1 Spell in " + activeBModel.modelName + " active Spells");
        }
        if (activeBModel.activeSpells.Count > 1)
        {
            if (spellIndex != 0)
            {
                clicked = true;
                spellIndex--;
            }
            if (spellIndex == 0 & !clicked)
            {
                clicked = true;
                spellIndex = activeBModel.activeSpells.Count - 1;

            }

            if (leftPanel)
            {
                lSpellTXT.text = activeBModel.activeSpells[spellIndex].spellInfo;
                lSpellTypeTXT.text = activeBModel.activeSpells[0].spellType.ToString();
                ColorSpellType(true, activeBModel.activeSpells[0]);
                lSpellIcon.sprite = activeBModel.activeSpells[spellIndex].spellIcon;
                lSpellText[0].text = activeBModel.activeSpells[spellIndex].spellName;
            }
            if (!leftPanel)
            {
                rSpellTXT.text = activeBModel.activeSpells[spellIndex].spellInfo;
                rSpellTypeTXT.text = activeBModel.activeSpells[0].spellType.ToString();
                ColorSpellType(false, activeBModel.activeSpells[0]);
                rSpellIcon.sprite = activeBModel.activeSpells[spellIndex].spellIcon;
                rSpellText[0].text = activeBModel.activeSpells[spellIndex].spellName;
            }
        }
    }

    public void SpellArrowRight(bool leftPanel)
    {
        bool clicked = false;
        BattleModel activeBModel = battleC.heroParty[battleC.heroIndex];
        if (activeBModel.activeSpells.Count == 1)
        {
            Debug.Log("Only 1 Spell in " + activeBModel.modelName + " active Spells");
        }
        if (activeBModel.activeSpells.Count > 1)
        {
            if (spellIndex != activeBModel.activeSpells.Count - 1)
            {
                clicked = true;
                spellIndex++;
            }
            if (spellIndex == activeBModel.activeSpells.Count - 1 & !clicked)
            {
                clicked = true;
                spellIndex = 0;
            }

            if (leftPanel)
            {
                lSpellTXT.text = activeBModel.activeSpells[spellIndex].spellInfo;
                lSpellTypeTXT.text = activeBModel.activeSpells[0].spellType.ToString();
                ColorSpellType(true, activeBModel.activeSpells[0]);
                lSpellIcon.sprite = activeBModel.activeSpells[spellIndex].spellIcon;
                lSpellText[0].text = activeBModel.activeSpells[spellIndex].spellName;
            }
            if (!leftPanel)
            {
                rSpellTXT.text = activeBModel.activeSpells[spellIndex].spellInfo;
                rSpellTypeTXT.text = activeBModel.activeSpells[0].spellType.ToString();
                ColorSpellType(false, activeBModel.activeSpells[0]);
                rSpellIcon.sprite = activeBModel.activeSpells[spellIndex].spellIcon;
                rSpellText[0].text = activeBModel.activeSpells[spellIndex].spellName;
            }
        }
    }

    public void TogglePhase(bool active)
    {
        phaseUI.gameObject.SetActive(active);
    }

    public void AssignFadeImage(PlayableDirector dir) // always assign fade animator to last output position
    {
        PlayableGraph graph = dir.playableGraph;
        if (graph.IsValid())
        {
            Debug.Log("Assigning Fade Screen");
            int graphCount = graph.GetOutputCount();

            int posNum = graphCount - 1;
            PlayableBinding playableBinding = dir.playableAsset.outputs.ElementAt(posNum);
            dir.SetGenericBinding(playableBinding.sourceObject, fadeImageAnim);
        }
        else
        {
            Debug.LogError("The PlayableGraph is not valid. Make sure it is properly initialized.");
        }
    }

    public void CloseAllButtons()
    {
        foreach (Button bt in enemySelectButtons)
        {
            bt.gameObject.SetActive(false);
        }
        foreach (Button bt in partySelectButtons)
        {
            bt.gameObject.SetActive(false);
        }
        foreach (Button bt in lActionButtons)
        {
            bt.gameObject.SetActive(false);
        }
        foreach (Button bt in rActionButtons)
        {
            bt.gameObject.SetActive(false);
        }
        foreach (Button bt in lSpellButtons)
        {
            bt.gameObject.SetActive(false);
        }
        lSpellObj.SetActive(false);
        foreach (Button bt in rSpellButtons)
        {
            bt.gameObject.SetActive(false);
        }
        rSpellObj.SetActive(false);
        lHealthSlider.gameObject.SetActive(false);
        rHealthSlider.gameObject.SetActive(false);
        lInfoFrame.SetActive(false);
        rInfoFrame.SetActive(false);

    }

    public void BackButton()
    {
        int index = battleC.heroIndex;
        Button lBackBT = lActionButtons[3];
        Button rBackBT = rActionButtons[3];

        foreach (Button bt in enemySelectButtons)
        {
            bt.gameObject.SetActive(false);
        }
        foreach (Button bt in partySelectButtons)
        {
            bt.gameObject.SetActive(false);
        }

        foreach (Button bt in lItemButtons)
        {
            bt.gameObject.SetActive(false);
        }
        foreach (Button bt in rItemButtons)
        {
            bt.gameObject.SetActive(false);
        }

        foreach (Button bt in lSpellButtons)
        {
            bt.gameObject.SetActive(false);
        }
        lSpellObj.SetActive(false);
        foreach (Button bt in rSpellButtons)
        {
            bt.gameObject.SetActive(false);
        }
        rSpellObj.SetActive(false);
        lInfoFrame.SetActive(false);
        rInfoFrame.SetActive(false);
        

        if (index == 0)
        {
            Debug.Log("ERROR: Back Button Clicked while heroindex set to 0");
            return;
        }
        if (index == 1)
        {
            battleC.heroIndex = 0;
            battleC.HeroZeroSelect();
            rBackBT.gameObject.SetActive(false);
            lBackBT.gameObject.SetActive(false);
            return;
        }
        if (index == 2)
        {
            if (!battleC.heroParty[1].dead)
            {
                battleC.heroIndex = 1;
                battleC.HeroOneSelect();
                rBackBT.gameObject.SetActive(false);
                return;
            }
            if (battleC.heroParty[1].dead)
            {
                battleC.heroIndex = 0;
                battleC.HeroZeroSelect();
                rBackBT.gameObject.SetActive(false);
                return;
            }
        }


    } 

    public void ButtonChecker()
    {
        if (lActionButtons[0].gameObject.activeSelf || rActionButtons[0].gameObject.activeSelf)
        {
            if (EventSystem.current.currentSelectedGameObject == lActionButtons[0].gameObject)
            {
                if (lActionIcons[0].sprite != actionIconsColor[0])
                {
                    lActionIcons[0].sprite = actionIconsColor[0];
                }
            }
            if (EventSystem.current.currentSelectedGameObject == lActionButtons[1].gameObject)
            {
                if (lActionIcons[1].sprite != actionIconsColor[1])
                {
                    lActionIcons[1].sprite = actionIconsColor[1];
                }
            }
            if (EventSystem.current.currentSelectedGameObject == lActionButtons[2].gameObject)
            {
                if (lActionIcons[2].sprite != actionIconsColor[2])
                {
                    lActionIcons[2].sprite = actionIconsColor[2];
                }
            }

            if (EventSystem.current.currentSelectedGameObject != lActionButtons[0].gameObject)
            {
                if (lActionIcons[0].sprite != actionIconsGray[0])
                {
                    lActionIcons[0].sprite = actionIconsGray[0];
                }
            }
            if (EventSystem.current.currentSelectedGameObject != lActionButtons[1].gameObject)
            {
                if (lActionIcons[1].sprite != actionIconsGray[1])
                {
                    lActionIcons[1].sprite = actionIconsGray[1];
                }
            }
            if (EventSystem.current.currentSelectedGameObject != lActionButtons[2].gameObject)
            {
                if (lActionIcons[2].sprite != actionIconsGray[2])
                {
                    lActionIcons[2].sprite = actionIconsGray[2];
                }
            }


            if (EventSystem.current.currentSelectedGameObject == rActionButtons[0].gameObject)
            {
                if (rActionIcons[0].sprite != actionIconsColor[0])
                {
                    rActionIcons[0].sprite = actionIconsColor[0];
                }
            }
            if (EventSystem.current.currentSelectedGameObject == rActionButtons[1].gameObject)
            {
                if (rActionIcons[1].sprite != actionIconsColor[1])
                {
                    rActionIcons[1].sprite = actionIconsColor[1];
                }
            }
            if (EventSystem.current.currentSelectedGameObject == rActionButtons[2].gameObject)
            {
                if (rActionIcons[2].sprite != actionIconsColor[2])
                {
                    rActionIcons[2].sprite = actionIconsColor[2];
                }
            }

            if (EventSystem.current.currentSelectedGameObject != rActionButtons[0].gameObject)
            {
                if (rActionIcons[0].sprite != actionIconsGray[0])
                {
                    rActionIcons[0].sprite = actionIconsGray[0];
                }
            }
            if (EventSystem.current.currentSelectedGameObject != rActionButtons[1].gameObject)
            {
                if (rActionIcons[1].sprite != actionIconsGray[1])
                {
                    rActionIcons[1].sprite = actionIconsGray[1];
                }
            }
            if (EventSystem.current.currentSelectedGameObject != rActionButtons[2].gameObject)
            {
                if (rActionIcons[2].sprite != actionIconsGray[2])
                {
                    rActionIcons[2].sprite = actionIconsGray[2];
                }
            }
        }

    }

    private void FixedUpdate()
    {
        ButtonChecker();
    }
}
