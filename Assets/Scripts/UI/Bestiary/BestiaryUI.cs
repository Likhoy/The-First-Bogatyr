using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BestiaryUI : MonoBehaviour
{
    public ScrollRect listScrollView;
    public GameObject infoPanel;
    public TMP_Text creatureNameTMP;
    public Image creatureImage;
    public GameObject descriptionPanel;
    public TMP_Text creatureDescriptionTMP;
    public GameObject namePanel;
    public GameObject vulnerabilitiesPanel;
    public Transform vulnerabilitiesContainer;

    public Button groupButtonPrefab;
    public Button creatureButtonPrefab;
    public GameObject vulnerabilityCellPrefab;

    private Dictionary<Button, List<Button>> groupToCreatureButtonsMap = new Dictionary<Button, List<Button>>();

    public void UpdateCreatureUI(BestiaryCreatureInfoSO creature, string groupName)
    {
        if (creature.isDiscovered)
        {
            ShowCreatureButton(creature, groupName);
        }
    }

    private void ShowCreatureButton(BestiaryCreatureInfoSO creature, string groupName)
    {
        foreach (var group in groupToCreatureButtonsMap)
        {
            if (group.Key.GetComponentInChildren<TMP_Text>().text == groupName) 
            {
                foreach (Button creatureButton in group.Value)
                {
                    if (creatureButton.GetComponentInChildren<TMP_Text>().text == creature.name) 
                    {
                        creatureButton.gameObject.SetActive(true);
                        break;
                    }
                }
            }
        }
    }
    public void CreateCreatureButtons(List<CreaturesGroup> creaturesGroups)
    {
        foreach (CreaturesGroup group in creaturesGroups)
        {
            CreateButtonsGroup(group);
        }
    }

    private void CreateButtonsGroup(CreaturesGroup group)
    {
        bool hasDiscoveredCreatures = group.creatures.Any(creature => creature.isDiscovered);

        if (!hasDiscoveredCreatures)
        {
            return;
        }

        Button groupButton = Instantiate(groupButtonPrefab, listScrollView.content);
        groupButton.GetComponentInChildren<TMP_Text>().text = group.groupName;
        groupButton.onClick.AddListener(() => ToggleCreatureList(groupButton, group));

        List<Button> creatureButtons = new List<Button>();

        foreach (BestiaryCreatureInfoSO creature in group.creatures)
        {
            if (creature.isDiscovered)
            {
                Button creatureButton = CreateCreatureButton(creature, listScrollView.content);
                creatureButtons.Add(creatureButton);
            }
        }

        groupToCreatureButtonsMap[groupButton] = creatureButtons;

        SetCreatureButtonsVisibility(creatureButtons, false);
    }

    private void ToggleCreatureList(Button groupButton, CreaturesGroup group)
    {
        List<Button> creatureButtons = groupToCreatureButtonsMap[groupButton];
        bool isActive = creatureButtons[0].gameObject.activeSelf; 
        SetCreatureButtonsVisibility(creatureButtons, !isActive);
    }

    private void SetCreatureButtonsVisibility(List<Button> creatureButtons, bool isVisible)
    {
        foreach (Button button in creatureButtons)
        {
            button.gameObject.SetActive(isVisible);
        }
    }

    private Button CreateCreatureButton(BestiaryCreatureInfoSO creature, Transform parent)
    {
        Button newButton = Instantiate(creatureButtonPrefab, parent);
        newButton.GetComponentInChildren<TMP_Text>().text = creature.name;
        newButton.GetComponentsInChildren<Image>()[1].sprite = creature.icon;
        newButton.onClick.AddListener(() => DisplayCreatureInfo(creature, newButton));
        return newButton;
    }

    private Button lastSelectedButton;

    private void DisplayCreatureInfo(BestiaryCreatureInfoSO creature,Button selectedButton)
    {
        if (lastSelectedButton == selectedButton)
        {
            infoPanel.SetActive(false);
            descriptionPanel.SetActive(false);
            namePanel.SetActive(false);
            ClearVulnerabilitiesPanel();
            vulnerabilitiesPanel.SetActive(false);
            lastSelectedButton = null;
        }
        else
        {
            lastSelectedButton = selectedButton;
            infoPanel.SetActive(true);
            descriptionPanel.SetActive(true);
            namePanel.SetActive(true);

            if (creatureImage != null) creatureImage.sprite = creature.picture;
            if (creatureNameTMP != null) creatureNameTMP.text = creature.name;
            if (creatureDescriptionTMP != null) creatureDescriptionTMP.text = creature.description;

            if (creature is EnemyBestiaryInfoSO enemy)
            {
                ShowVulnerabilities(enemy.Vulnerabilities);
            }
            else
            {
                ClearVulnerabilitiesPanel();
            }

        }

    }

    private void ShowVulnerabilities(List<Vulnerability> vulnerabilities)
    {
        vulnerabilitiesPanel.SetActive(true);
        ClearVulnerabilitiesPanel();

        foreach (Vulnerability vulnerability in vulnerabilities)
        {
            GameObject vulnerabilityCell = Instantiate(vulnerabilityCellPrefab, vulnerabilitiesContainer);

            Image icon = vulnerabilityCell.GetComponentsInChildren<Image>()[1];
            if (icon != null)
            {
                icon.sprite = vulnerability.icon;
            }

            TMP_Text text = vulnerabilityCell.GetComponentInChildren<TMP_Text>();
            if (text != null)
            {
                text.text = vulnerability.name;
            }

            VulnerabilityTooltip tooltip = vulnerabilityCell.GetComponent<VulnerabilityTooltip>();
            if (tooltip != null)
            {
                tooltip.Initialize(vulnerability.name);
            }
        }
    }

    private void ClearVulnerabilitiesPanel()
    {
        foreach (Transform child in vulnerabilitiesContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public void ClosePanel()
    {
        foreach (var group in groupToCreatureButtonsMap)
        {
            SetCreatureButtonsVisibility(group.Value, false);
        }

        infoPanel.SetActive(false);
        descriptionPanel.SetActive(false);
        namePanel.SetActive(false);
        vulnerabilitiesPanel.SetActive(false);

        ClearVulnerabilitiesPanel();

        this.gameObject.SetActive(false);
    }
}
