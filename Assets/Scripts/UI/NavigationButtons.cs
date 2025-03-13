using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavigationButtons : MonoBehaviour
{
    public Button statusButton;
    public Button inventoryButton;
    public Button skillTreeButton;
    public Button mapButton;
    public Button bestiaryButton;
    public Button questLogWindowButton;

    private GameObject currFrame;
    private Button currButton;

    private UiMaster uiMaster;

    private void Start()
    {
        uiMaster = GlobalStatus.mainPlayer.GetComponent<UiMaster>();
    }

    public void InitializeButtons()
    {
        statusButton.onClick.AddListener(() => OnButtonClicked(statusButton));
        inventoryButton.onClick.AddListener(() => OnButtonClicked(inventoryButton));
        skillTreeButton.onClick.AddListener(() => OnButtonClicked(skillTreeButton));
        mapButton.onClick.AddListener(() => OnButtonClicked(mapButton));
        bestiaryButton.onClick.AddListener(() => OnButtonClicked(bestiaryButton));
        questLogWindowButton.onClick.AddListener(() => OnButtonClicked(questLogWindowButton));

        DeactivateAllFrames();
    }

    public void OnButtonClicked(Button button)
    {
        if (currButton == button) return;

        if (button == statusButton) uiMaster.OnOffStatusMenu();
        else if (button == inventoryButton) uiMaster.OnOffInventoryMenu();
        else if (button == skillTreeButton) uiMaster.OnOffSkillMenu();
        else if (button == mapButton) uiMaster.OnOffMap();
        else if (button == bestiaryButton) uiMaster.OnOffBestiary();
        //else if (button == questLogWindowButton) uiMaster.OnOffQuestLogWindow();

        UpdateFrame(button);
    }

    public void UpdateButtonState(Button button)
    {
        if (currButton == button) return;
        DeactivateAllFrames();
        currButton = button;
        UpdateFrame(button);
    }

    public void UpdateFrame(Button button)
    {
        GameObject frame = button.transform.Find("Frame")?.gameObject;

        if (frame == null)
        {
            Debug.LogWarning($"No frame found for button: {button.name}");
            return;
        }

        if (currFrame == frame)
        {
            return;
        }

        if (currFrame != null)
        {
            currFrame.SetActive(false);
        }

        frame.SetActive(true);
        currFrame = frame;
        
    }

    public void DeactivateAllFrames()
    {
        SetFrameActive(statusButton, false);
        SetFrameActive(inventoryButton, false);
        SetFrameActive(skillTreeButton, false);
        SetFrameActive(mapButton, false);
        SetFrameActive(bestiaryButton, false);
        SetFrameActive(questLogWindowButton, false);

        currFrame = null;
    }

    private void SetFrameActive(Button button, bool isActive)
    {
        GameObject frame = button.transform.Find("Frame")?.gameObject;
        if (frame != null)
        {
            frame.SetActive(isActive);
        }
    }
}
