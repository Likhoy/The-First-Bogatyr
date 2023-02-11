/*using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogUI : MonoBehaviour
{
    #region Header OBJECT REFERENCES
    [Space(10)]
    [Header("OBJECT REFERENCES")]
    #endregion Header OBJECT REFERENCES

    #region Tooltip
    [Tooltip("Populate with the TextMeshPro-Text component on the child DialogText gameobject")]
    #endregion Tooltip
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private NodeGraphSO nodeGraph;
    [SerializeField] private Button[] optionButtonsArray;

    private Player player;
    private Animator spaceAnimator;

    private List<NodeSO> currentNodes = null;

    private void Awake()
    {
        // Load components
        player = GameManager.Instance.GetPlayer();
        // Get animator
        spaceAnimator = this.gameObject.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        // Subscribe to dialog started event
        player.dialogStartedEvent.OnStartDialog += DialogStartedEvent_OnStartDialog;

        player.dialogProceededEvent.OnProceedDialog += DialogProceededEvent_OnProceedDialog;
    }

    private void OnDisable()
    {
        // Unsubscribe from dialog started event
        player.dialogStartedEvent.OnStartDialog -= DialogStartedEvent_OnStartDialog;

        player.dialogProceededEvent.OnProceedDialog -= DialogProceededEvent_OnProceedDialog;
    }

    private void DialogProceededEvent_OnProceedDialog(DialogProceededEvent arg1, DialogProceededEventArgs arg2)
    {
        ProceedDialog();
    }

    private void ProceedDialog()
    {
        UpdateDialogText();
    }

    /// <summary>
    /// Handle dialog started event
    /// </summary>
    private void DialogStartedEvent_OnStartDialog(DialogStartedEvent dialogStartedEvent, DialogStartedEventArgs dialogStartedEventArgs)
    {
        StartDialog();
    }

    /// <summary>
    /// start dialog - update UI
    /// </summary>
    private void StartDialog()
    {
        DisableOptionButtons();
        UpdateDialogSpace();
        DisplayFirstStatementOptions();
    }

    private void DisableOptionButtons()
    {
        foreach (Button button in optionButtonsArray)
        {
            button.gameObject.SetActive(false);
        }
        DialogManager.Instance.optionButtonsAreBeingDisplayed = false;
    }

    private void DisplayFirstStatementOptions()
    {
        // for simplicity we consider that our hero always has a choice of statement
        // get options for the first statement
        currentNodes = nodeGraph.nodeList.FindAll(node => node.parentNodeIDList.Count == 0).ToList();

        if (currentNodes != null)
        {
            StopAllCoroutines();
            // NPC speaking
            if (currentNodes.Count() == 1)
            {
                StartCoroutine(StatementAppearingRoutine(currentNodes[0].nodeText));
            }
            // player speaking
            else
            {
                DisplayStatementOptions(currentNodes.Select(node => node.nodeText).ToList());
            }
        }
    }

    /// <summary>
    /// update dialog space UI (fading of screen at the bottom)
    /// </summary>
    private void UpdateDialogSpace()
    {
        spaceAnimator.SetBool(Settings.spaceOpen, true);
    }

    /// <summary>
    /// update dialog text UI - display a statement (or statement options)
    /// </summary>
    private void UpdateDialogText()
    {
        if (currentNodes != null)
        { 
            currentNodes = nodeGraph.nodeList.FindAll(node => currentNodes[0].childNodeIDList.Contains(node.id)).ToList();

            StopAllCoroutines();

            if (currentNodes.Count() == 0)
            {
                StopDialog();
                return;
            }

            // NPC speaking
            if (currentNodes.Count() == 1)
            {
                StartCoroutine(StatementAppearingRoutine(currentNodes[0].nodeText));
            }
            // player speaking
            else 
            {
                DisplayStatementOptions(currentNodes.Select(node => node.nodeText).ToList());
            }
        }
    }

    private void StopDialog()
    {
        DialogManager.Instance.StopDialogStage();
        spaceAnimator.SetBool(Settings.spaceOpen, false);
        dialogText.text = "";
        currentNodes = null;
        player.dialogEndedEvent.CallDialogEndedEvent();
    }

    private void DisplayStatementOptions(List<string> statementOptions)
    {
        if (statementOptions.Count > 4)
            Debug.Log("More then 4 statement options can't be processed");

        DialogManager.Instance.optionButtonsAreBeingDisplayed = true;
        dialogText.text = "";

        int i = 0;
        foreach (string statement in statementOptions)
        {
            Button button = optionButtonsArray[i];
            button.gameObject.SetActive(true);
            button.GetComponentInChildren<TextMeshProUGUI>().text = statement;
            i++;
        }
        for (int j = i; j < optionButtonsArray.Length; j++)
        {
            optionButtonsArray[j].gameObject.SetActive(false);
        }
    }

    public void ChooseOption(int optionIndex)
    {
        string chosenStatement = optionButtonsArray[optionIndex].GetComponentInChildren<TextMeshProUGUI>().text;
        DisableOptionButtons();
        
        StopAllCoroutines();
        StartCoroutine(StatementAppearingRoutine(chosenStatement));
        NodeSO chosenNode = currentNodes.Find(node => node.nodeText == chosenStatement);
        currentNodes.Clear();
        currentNodes.Add(chosenNode);
    }

    /// <summary>
    /// demonstrate the current statement
    /// </summary>
    IEnumerator StatementAppearingRoutine(string statement)
    {
        dialogText.text = "";
        foreach (char symbol in statement)
        {
            dialogText.text += symbol;
            yield return new WaitForSeconds(0.03f);
        }
    }
}
*/