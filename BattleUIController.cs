using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using System.Linq;
using UnityEngine.UI;
using Unity.VisualScripting;
using TMPro;
using CryptUI.Scripts;

public class BattleUIController : MonoBehaviour
{

    public BattleController battleC;
    public BattlePhaseUI phaseUI;

    public GameObject fadeImageObj;
    public Animator fadeImageAnim;
    public PlayableDirector fadeDarkPlayable;
    public AudioSource audioSource;
    public List<AudioClip> audioClips;

    public List<Button> lActionButtons;
    public List<Button> lSpellButtons;
    public List<TextMeshProUGUI> lSpellText;
    public List<Button> lItemButtons;
    public List<TextMeshProUGUI> lItemText;
    public GameObject lInfoFrame;
    public TextMeshProUGUI lInfoTXT;

    public List<Button> rActionButtons;
    public List<Button> rSpellButtons;
    public List<TextMeshProUGUI> rSpellText;
    public List<Button> rItemButtons;
    public List<TextMeshProUGUI> rItemText;
    public GameObject rInfoFrame;
    public TextMeshProUGUI rInfoTXT;

    public List<Button> enemySelectButtons;
    public List<Button> partySelectButtons;

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
                }
                if (index == 2)
                {
                    Debug.Log("Setting Buttons for Hero 2");
                    rHealthSlider.maxValue = battleC.heroParty[2].maxH;
                    rHealthSlider.value = battleC.heroParty[2].health;
                    rInfoTXT.text = battleC.heroParty[2].modelName;
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
        foreach (Button bt in rSpellButtons)
        {
            if (bt.gameObject.activeSelf)
            {
                bt.gameObject.SetActive(false);
            }
        }


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

        if (rActionButtons[1].gameObject.activeSelf)
        {
            rActionButtons[1].Select();
        }
        if (lActionButtons[1].gameObject.activeSelf)
        {
            lActionButtons[1].Select();
        }         
        int x = battleC.heroIndex;
        BattleModel activeBModel = battleC.heroParty[x];

        if (activeBModel.activeSpells.Count > 0)
        {          
            if (x == 1)
            {
                for (int i = 0; i < activeBModel.activeSpells.Count; i++)
                {
                    lSpellButtons[i].gameObject.SetActive(true);
                    lSpellText[i].text = activeBModel.activeSpells[i].spellName;
                    if (i == 0)
                    {
                        lSpellButtons[i].Select();
                    }
                }
            }
            if (x != 1)
            {
                for (int i = 0; i < activeBModel.activeSpells.Count; i++)
                {
                    rSpellButtons[i].gameObject.SetActive(true);
                    rSpellText[i].text = activeBModel.activeSpells[i].spellName;
                    if (i == 0)
                    {
                        rSpellButtons[i].Select();
                    }
                }
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

    public void SpellSelectBT(int btNum)
    {
        int x = battleC.heroIndex;
        BattleModel activeHero = battleC.heroParty[x];
        activeHero.actionType = BattleModel.ActionType.spell;
        activeHero.selectedSpell = battleC.heroParty[x].activeSpells[btNum];
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
    }


    public void ItemActionBT()
    {
        foreach (Button bt in lSpellButtons)
        {
            bt.gameObject.SetActive(false);
        }
        foreach (Button bt in rSpellButtons)
        {
            bt.gameObject.SetActive(false);
        }

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


        InventoryController inventory = battleC.inventory;
        int itemCount = battleC.inventory.battleItems.Count;
        if (itemCount > 0)
        {
            for (int i = 0; i < itemCount; i++)
            {
                if (lActionButtons[2].gameObject.activeSelf)
                {
                    lItemButtons[i].gameObject.SetActive(true);
                    lItemText[i].text = inventory.battleItems[i].itemName;
                    if (i == 0)
                    {
                        lItemButtons[0].Select();
                    }
                }
                if (rActionButtons[2].gameObject.activeSelf)
                {
                    rItemButtons[i].gameObject.SetActive(true);
                    rItemText[i].text = inventory.battleItems[i].itemName;
                    if (i == 0)
                    {
                        rItemButtons[0].Select();
                    }
                }
            }
        }
        if (itemCount == 0)
        {
            audioSource.PlayOneShot(audioClips[0]);
            return;
        }
    }

    public void ItemSelectBT(int btNum)
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

        battleC.heroParty[x].selectedItem = battleC.inventory.battleItems[btNum];

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
        foreach (Button bt in rSpellButtons)
        {
            bt.gameObject.SetActive(false);
        }
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
        foreach (Button bt in rSpellButtons)
        {
            bt.gameObject.SetActive(false);
        }
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
            battleC.heroIndex = 1;
            battleC.HeroOneSelect();
            rBackBT.gameObject.SetActive(false);
            return;
        }


    } 
}
