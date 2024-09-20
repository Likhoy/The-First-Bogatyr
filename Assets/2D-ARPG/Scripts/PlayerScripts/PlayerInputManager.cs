using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour{
    private Status stat;
    private AttackTrigger atk;
    private UiMaster ui;
    private SaveLoad save;
    private PlayerController topDown2d;
    private TopDownFourDirection topDown4d;
    private PlatformerController2D platormer2d;
    private PlatformerTransformation platormerTransformation;
    void Start(){
        atk = GetComponent<AttackTrigger>();
        stat = GetComponent<Status>();
        if(GetComponent<UiMaster>()){
            ui = GetComponent<UiMaster>();
        }
        if(GetComponent<SaveLoad>()) {
            save = GetComponent<SaveLoad>();
        }
        if(GetComponent<PlayerController>()) {
            topDown2d = GetComponent<PlayerController>();
        }
        if(GetComponent<TopDownFourDirection>()) {
            topDown4d = GetComponent<TopDownFourDirection>();
        }
        if(GetComponent<PlatformerController2D>()) {
            platormer2d = GetComponent<PlatformerController2D>();
        }
        if(GetComponent<PlatformerTransformation>()) {
            platormerTransformation = GetComponent<PlatformerTransformation>();
        }
    }

    void Update(){
        //Attack Trigger Activate Event
        if(Input.GetKeyDown("e")) {
            atk.Activator();
        }
        //Attack Trigger Shortcuts
        if(Input.GetKeyDown("1")){
            atk.UseShortcut(0);
        }
        if(Input.GetKeyDown("2")){
            atk.UseShortcut(1);
        }
        if(Input.GetKeyDown("3")){
            atk.UseShortcut(2);
        }
        if(Input.GetKeyDown("4")){
            atk.UseShortcut(3);
        }
        if(Input.GetKeyDown("5")){
            atk.UseShortcut(4);
        }
        if(Input.GetKeyDown("6")){
            atk.UseShortcut(5);
        }
        if(Input.GetKeyDown("7")){
            atk.UseShortcut(6);
        }
        if(Input.GetKeyDown("8")){
            atk.UseShortcut(7);
        }

        //Attack Trigger Attack
        if(Input.GetKeyDown("f")){
            atk.GuardUp();
        }
        if(stat.block && Input.GetKeyUp("f")){
            stat.GuardBreak("cancelGuard");
        }
        if(!atk.mobileMode) {
            if(Input.GetButtonUp("Fire1")) {
                atk.ReleaseCharge();
            }
            if(Input.GetButton("Fire1")) {
                atk.TriggerAttack();
            }
        }

        //TopDown2D
        if(topDown2d) {
            if(Input.GetKeyUp(KeyCode.Mouse1)){
                topDown2d.CancelDash();
            }
            if(Input.GetKeyDown(KeyCode.Mouse1)) {
                topDown2d.DashButton();
            }
        }

        //TopDown 4 Directions
        if(topDown4d) {
            if(Input.GetKeyUp(KeyCode.Mouse1)) {
                topDown4d.CancelDash();
            }
            if(Input.GetKeyDown(KeyCode.Mouse1)) {
                topDown4d.DashButton();
            }
        }

        if(platormer2d){
            if(Input.GetKeyUp(KeyCode.Mouse1)){
                platormer2d.CancelDash();
            }
            if(Input.GetKeyDown(KeyCode.Mouse1)) {
                platormer2d.DashButton();
            }
            if(Input.GetButtonDown("Jump")){
                platormer2d.JumpButton();
            }
        }

        if(platormerTransformation) {
            if(Input.GetKeyUp(KeyCode.Mouse1)) {
                platormerTransformation.CancelDash();
            }
            if(Input.GetKeyDown(KeyCode.Mouse1)) {
                platormerTransformation.DashButton();
            }
            if(Input.GetButtonDown("Jump")) {
                platormerTransformation.JumpButton();
            }

            //Transformation
            if(Input.GetKeyDown(KeyCode.F1) || Input.GetKeyDown(KeyCode.Keypad1)) {
                platormerTransformation.Transformation(0);
            }
            if(Input.GetKeyDown(KeyCode.F2) || Input.GetKeyDown(KeyCode.Keypad2)) {
                platormerTransformation.Transformation(1);
            }
            if(Input.GetKeyDown(KeyCode.F3) || Input.GetKeyDown(KeyCode.Keypad3)) {
                platormerTransformation.Transformation(2);
            }
            if(Input.GetKeyDown(KeyCode.F4) || Input.GetKeyDown(KeyCode.Keypad4)) {
                platormerTransformation.Transformation(3);
            }
        }

        if(save){
            if(Input.GetKeyDown(KeyCode.Escape)){
                //Open Save Load Menu
                save.OnOffMenu();
            }
        }
    }
}
