/*using Cinemachine;
using UnityEngine;

public class DialogManager : SingletonMonobehaviour<DialogManager>
{
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    private Player player;

    [HideInInspector] public bool isDialogReady = true; // for testing purposes
    [HideInInspector] public bool isDialogPlaying = false;
    [HideInInspector] public bool optionButtonsAreBeingDisplayed = false;

    protected override void Awake()
    {
        base.Awake();

        // Load components
        player = GameManager.Instance.GetPlayer();
    }

    private void OnEnable()
    {
        // Subscribe to dialog started event
        player.dialogStartedEvent.OnStartDialog += DialogStartedEvent_OnStartDialog;
    }

    private void OnDisable()
    {
        // Unsubscribe from dialog started event
        player.dialogStartedEvent.OnStartDialog -= DialogStartedEvent_OnStartDialog;
    }

    private void DialogStartedEvent_OnStartDialog(DialogStartedEvent dialogStartedEvent, DialogStartedEventArgs dialogStartedEventArgs)
    {
        LaunchDialogStage();
    }

    private void LaunchDialogStage()
    {
        isDialogPlaying = true;
        player.playerControl.DisablePlayer();
        cinemachineVirtualCamera.gameObject.SetActive(false); // disable camera
    }

    public void StopDialogStage()
    {
        isDialogPlaying = false;
        player.playerControl.EnablePlayer();
        cinemachineVirtualCamera.gameObject.SetActive(true); // enable camera
        isDialogReady = false; 
    }
}
*/