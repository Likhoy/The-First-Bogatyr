using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(EventActivator))]
[AddComponentMenu("Easy Event Maker/Add Event")]
public class EventSetting : MonoBehaviour {
    public static GameObject mainPlayer;

    public static bool[] globalBoolean = new bool[300];
    public static int[] globalInt = new int[300];

    public ParametersType type = ParametersType.Dialogue;

    //[TextArea(3,10)]
    public string message = "";
    public Transform target;
    public Transform target2;
    public bool targetIsPlayer;
    public float floatVar = 2;
    public float floatVar2;
    public float floatVar3 = 1;
    public float floatVar4 = 50;
    public float floatVar5 = 50;
    public Vector3 vector3Var = Vector3.zero;
    public Vector3 vector3Var2 = Vector3.zero;
    public Vector2 vector2Var = new(100, 100);
    public Transform objectVar;
    public EventActivator eventVar;

    public string msgString = "";
    public string msgString2 = "";
    public int intVar;
    public int intVar2;
    public int intVar3;
    public int intVar4;

    public bool boolVar;
    public bool boolVar2;
    public bool boolVar3;
    public bool boolVar4 = true;
    public bool boolVar5;
    public bool sendBack;
    public bool stopMoving;
    public AnimationClip animClip;
    public AudioClip audiClip;
    public string stringVar = "";
    public string stringVar2 = "";
    public Color colorVar = Color.white;
    public Color colorVar2 = Color.black;
    public Color colorVar3 = Color.white;
    public Material matVar;
    public Light lightVar;
    public float lightIntesity = 0.5f;
    public Image imageVar;
    public Image imageVar2;
    public Image textBoxVar;
    public Text showText;
    public Text nameText;
    public Sprite spriteVar;
    public IntValueSet math = IntValueSet.Equal;
    public ConditionChecker condi = ConditionChecker.GreaterOrEqual;
    public CondiCheckForm condiForm = CondiCheckForm.Variable;

    public string note = "";
    public string noteId = "";

    public EventActivator ev1;
    public EventActivator ev2;

    //public static string spawnPointName = "";

    public void Activate() {
        if (type == ParametersType.Dialogue) StartDialogue();
        if (type == ParametersType.Wait) StartCoroutine(Waiting());
        if (type == ParametersType.ActivateOtherEvent) {
            eventVar.ActivateEvent();
            NextEvent();
        }

        if (type == ParametersType.SpawnPrefabAtObject) {
            if (targetIsPlayer)
                SpawnPrefab(2);
            else
                SpawnPrefab(1);
        }

        if (type == ParametersType.GoToEvent) GoToEvent(intVar);
        if (type == ParametersType.EnableObject) {
            var t = SetTarget();
            if (!t) {
                NextEvent();
                return;
            }

            t.gameObject.SetActive(true);
            NextEvent();
        }

        if (type == ParametersType.DisableObject) {
            var t = SetTarget();
            if (!t) {
                NextEvent();
                return;
            }

            t.gameObject.SetActive(false);
            NextEvent();
        }

        if (type == ParametersType.DeleteObject) {
            var t = SetTarget();
            if (!t) {
                NextEvent();
                return;
            }

            if (t == gameObject)
                //End Event if Delete this object
                GetComponent<EventActivator>().EndEvent();
            Destroy(t.gameObject);
            NextEvent();
        }

        if (type == ParametersType.PlayMecanimAnimation) {
            var t = SetTarget();
            if (!t) {
                NextEvent();
                return;
            }

            if (t.GetComponent<Status>() && t.GetComponent<Status>().mainSprite)
                t = t.GetComponent<Status>().mainSprite.transform;
            if (!t.GetComponent<Animator>()) {
                print("Target not have Animator Component");
                NextEvent();
                return;
            }

            if (boolVar)
                t.GetComponent<Animator>().SetTrigger(stringVar);
            else
                t.GetComponent<Animator>().Play(animClip.name);

            NextEvent();
        }

        if (type == ParametersType.ControlBoolean) {
            globalBoolean[intVar] = boolVar;
            NextEvent();
        }

        if (type == ParametersType.ControlVariable) {
            if (math == IntValueSet.Equal)
                globalInt[intVar] = intVar2;
            else if (math == IntValueSet.Plus)
                globalInt[intVar] += intVar2;
            else if (math == IntValueSet.Minus)
                globalInt[intVar] -= intVar2;
            else if (math == IntValueSet.Multiply)
                globalInt[intVar] *= intVar2;
            else if (math == IntValueSet.Divide) globalInt[intVar] /= intVar2;
            //print(globalInt[intVar]);
            NextEvent();
        }

        if (type == ParametersType.PlayBGM) {
            if (!target) {
                print("No Audio Source");
                NextEvent();
                return;
            }

            if (!target.GetComponent<AudioSource>()) target.gameObject.AddComponent<AudioSource>();
            if (boolVar) {
                target.GetComponent<AudioSource>().Stop();
            }
            else {
                target.GetComponent<AudioSource>().clip = audiClip;
                target.GetComponent<AudioSource>().loop = boolVar2;
                target.GetComponent<AudioSource>().Play();
            }

            NextEvent();
        }

        if (type == ParametersType.PlayOneShotAudio) {
            if (!target) {
                print("No Audio Source");
                NextEvent();
                return;
            }

            if (!target.GetComponent<AudioSource>()) target.gameObject.AddComponent<AudioSource>();
            target.GetComponent<AudioSource>().PlayOneShot(audiClip);
            NextEvent();
        }

        if (type == ParametersType.ChangeScene) {
            if (!mainPlayer) mainPlayer = GameObject.FindWithTag("Player");
            if (!mainPlayer) print("No Player in the scene");
            if (boolVar) mainPlayer.GetComponent<Status>().spawnPointName = stringVar2;
            //Application.LoadLevel(stringVar);
            SceneManager.LoadScene(stringVar, LoadSceneMode.Single);
            NextEvent();
        }

        if (type == ParametersType.SetObjectLocation) {
            var t = SetTarget();
            if (!t) {
                NextEvent();
                return;
            }

            target.localPosition = vector3Var;
            NextEvent();
        }

        if (type == ParametersType.SetObjectRotation) {
            var t = SetTarget();
            if (!t) {
                NextEvent();
                return;
            }

            if (target2) vector3Var = target2.eulerAngles;
            target.eulerAngles = vector3Var;
            NextEvent();
        }

        if (type == ParametersType.SetObjectAtObjectPosition) {
            var t = SetTarget();
            if (!t || !target2) {
                print("You didn't assign Target yet");
                NextEvent();
                return;
            }

            target.position = target2.position;
            target.rotation = target2.rotation;
            NextEvent();
        }

        if (type == ParametersType.FreezeAll) {
            GlobalStatus.freezeAll = boolVar;
            NextEvent();
        }

        if (type == ParametersType.MoveObjectTowards) {
            var t = SetTarget();
            if (boolVar5) target2 = GameObject.FindWithTag("Player").transform;
            if (!t || !target2) {
                print("You didn't assign Destination Object at Target yet");
                NextEvent();
                return;
            }

            if (!t.GetComponent<MovingLookingEvent>()) t.gameObject.AddComponent<MovingLookingEvent>();
            t.GetComponent<MovingLookingEvent>().destinationObject = target2;
            t.GetComponent<MovingLookingEvent>().moveSpeed = floatVar;
            t.GetComponent<MovingLookingEvent>().useCharacterController = boolVar;
            t.GetComponent<MovingLookingEvent>().reachDistance = floatVar2;
            t.GetComponent<MovingLookingEvent>().sendMsgWhenReact = msgString;
            t.GetComponent<MovingLookingEvent>().stopWhenReach = stopMoving;

            t.GetComponent<MovingLookingEvent>().onMoving = true;
            if (boolVar2) {
                t.GetComponent<MovingLookingEvent>().lookAtObject = target2;
                t.GetComponent<MovingLookingEvent>().lockZAngle = boolVar3;
                t.GetComponent<MovingLookingEvent>().onLooking = true;
            }

            if (sendBack) {
                var ev = GetComponent<EventActivator>();
                t.GetComponent<MovingLookingEvent>().source = GetComponents<EventSetting>()[ev.runEvent];
                t.GetComponent<MovingLookingEvent>().stopSending = false;
            }
            else {
                NextEvent();
            }
        }

        if (type == ParametersType.LookAtTarget) {
            var t = SetTarget();
            if (!t || (!target2 && !boolVar3)) {
                print("You didn't assign Look At Object at Target yet");
                NextEvent();
                return;
            }

            if (boolVar3) {
                //Look At Player
                if (!mainPlayer) mainPlayer = GameObject.FindWithTag("Player");
                if (!mainPlayer) {
                    print("No Player in the scene");
                    NextEvent();
                    return;
                }

                target2 = mainPlayer.transform;
            }

            if (boolVar) {
                if (!t.GetComponent<MovingLookingEvent>()) t.gameObject.AddComponent<MovingLookingEvent>();
                t.gameObject.GetComponent<MovingLookingEvent>().lookAtObject = target2;
                t.gameObject.GetComponent<MovingLookingEvent>().lockZAngle = boolVar2;
                t.gameObject.GetComponent<MovingLookingEvent>().lookSpeed = floatVar;
                t.gameObject.GetComponent<MovingLookingEvent>().onLooking = true;
            }
            else {
                if (t.GetComponent<MovingLookingEvent>())
                    t.gameObject.GetComponent<MovingLookingEvent>().onLooking = false;
                var lookPos = target2.position;
                if (boolVar2) {
                    //lookPos.y = t.position.y;
                    var delta = lookPos - t.position;

                    if (delta.x >= 0) {
                        var rot = t.eulerAngles;
                        rot.y = 0;
                        t.eulerAngles = rot;
                    }
                    else if (delta.x < 0) {
                        var rot = t.eulerAngles;
                        rot.y = 180;
                        t.eulerAngles = rot;
                    }
                }
                else {
                    t.right = lookPos - t.position;
                }
                //t.LookAt(lookPos);
            }

            NextEvent();
        }

        if (type == ParametersType.SetParent) {
            if (boolVar && GlobalStatus.mainPlayer) target2 = GlobalStatus.mainPlayer.transform;
            if (target && target2) target.parent = target2;
            NextEvent();
        }

        if (type == ParametersType.Unparent) {
            if (target) target.parent = null;
            NextEvent();
        }

        if (type == ParametersType.AmbientEditor) SetSkyFog();

        if (type == ParametersType.LightEditor) SetLight();

        if (type == ParametersType.SetTimeScale) {
            Time.timeScale = floatVar3;
            NextEvent();
        }

        if (type == ParametersType.SendMessage) {
            if (target)
                target.SendMessage(msgString, SendMessageOptions.DontRequireReceiver);
            else
                SendMessage(msgString, SendMessageOptions.DontRequireReceiver);
            NextEvent();
        }

        if (type == ParametersType.StopMovingAndLooking) {
            var t = SetTarget();
            if (!t) {
                NextEvent();
                return;
            }

            if (!t.GetComponent<MovingLookingEvent>()) {
                NextEvent();
                return;
            }

            if (boolVar) t.gameObject.GetComponent<MovingLookingEvent>().onMoving = false;
            if (boolVar2) t.gameObject.GetComponent<MovingLookingEvent>().onLooking = false;
            NextEvent();
        }

        if (type == ParametersType.BreakEvent) GetComponent<EventActivator>().EndEvent();

        if (type == ParametersType.ScreenShake) ShakeCamera();

        if (type == ParametersType.FadeImage) {
            if (!imageVar) {
                print("You didn't assign Image");
                NextEvent();
                return;
            }

            if (!imageVar.GetComponent<FadeImage>()) imageVar.gameObject.AddComponent<FadeImage>();
            //imageVar.color = colorVar;
            imageVar.GetComponent<FadeImage>().SetFirstColor(colorVar);
            imageVar.GetComponent<FadeImage>().startColor = colorVar;
            imageVar.GetComponent<FadeImage>().endColor = colorVar2;
            imageVar.GetComponent<FadeImage>().lerpSpeed = floatVar3;
            imageVar.GetComponent<FadeImage>().fading = true;
            NextEvent();
        }

        if (type == ParametersType.ConditionCheck) {
            var pass = false;
            if (boolVar)
                pass = globalBoolean[intVar];
            else
                pass = CheckCondition();
            if (pass) {
                //Condition Pass
                if (ev1) {
                    ev1.ActivateEvent();
                    GetComponent<EventActivator>().runEvent = 0;
                    GetComponent<EventActivator>().eventRunning = false;
                }
                else {
                    NextEvent();
                }
            }
            else {
                //Condition Fail
                if (ev2) {
                    ev2.ActivateEvent();
                    GetComponent<EventActivator>().runEvent = 0;
                    GetComponent<EventActivator>().eventRunning = false;
                }
                else {
                    NextEvent();
                }
            }
        }

        if (type == ParametersType.MoveImage) {
            if (!imageVar) {
                print("You didn't assign Image source yet");
                NextEvent();
                return;
            }

            imageVar.gameObject.SetActive(true);
            if (!imageVar.GetComponent<MoveImage>()) imageVar.gameObject.AddComponent<MoveImage>();
            if (boolVar4) {
                imageVar.GetComponent<MoveImage>().destinationPosition = vector3Var;
                imageVar.GetComponent<MoveImage>().speed = floatVar4;
            }

            imageVar.GetComponent<MoveImage>().onMoving = boolVar4;
            if (boolVar2) {
                imageVar.GetComponent<MoveImage>().size = vector2Var;
                imageVar.GetComponent<MoveImage>().scalingSpeed = floatVar5;
            }

            imageVar.GetComponent<MoveImage>().onScaling = boolVar2;
            NextEvent();
        }

        if (type == ParametersType.SetCameraTarget) {
            var t = SetTarget();
            if (!t) {
                NextEvent();
                return;
            }

            /*if(Camera.main.GetComponent<ARPGcamera>()){
                Camera.main.GetComponent<ARPGcamera>().enabled = !boolVar2;
            }*/
            GlobalStatus.freezeCam = boolVar2;
            if (Camera.main.GetComponent<CameraFollowPlayer2D>())
                Camera.main.GetComponent<CameraFollowPlayer2D>().SetNewTarget(t);
            else
                Camera.main.SendMessage("SetNewTarget", t, SendMessageOptions.DontRequireReceiver);
            if (boolVar3) {
                Camera.main.transform.position = t.position;
                Camera.main.transform.rotation = t.rotation;
            }

            NextEvent();
        }

        if (type == ParametersType.LockCamera) {
            GlobalStatus.freezeCam = boolVar;
            NextEvent();
        }

        if (type == ParametersType.AddCash) {
            if (GlobalStatus.mainPlayer) GlobalStatus.mainPlayer.GetComponent<Inventory>().cash += intVar;
            NextEvent();
        }

        if (type == ParametersType.AddSkillPoint) {
            if (GlobalStatus.mainPlayer) GlobalStatus.mainPlayer.GetComponent<Status>().skillPoint += intVar;
            NextEvent();
        }

        if (type == ParametersType.AddStatusPoint) {
            if (GlobalStatus.mainPlayer) GlobalStatus.mainPlayer.GetComponent<Status>().statusPoint += intVar;
            NextEvent();
        }

        if (type == ParametersType.AddItem) {
            if (GlobalStatus.mainPlayer) GlobalStatus.mainPlayer.GetComponent<Inventory>().AddItem(intVar, intVar2);
            NextEvent();
        }

        if (type == ParametersType.AddEquipment) {
            if (GlobalStatus.mainPlayer) GlobalStatus.mainPlayer.GetComponent<Inventory>().AddEquipment(intVar);
            NextEvent();
        }

        if (type == ParametersType.AddQuest) {
            if (GlobalStatus.mainPlayer) GlobalStatus.mainPlayer.GetComponent<QuestStat>().AddQuest(intVar);
            NextEvent();
        }
    }

    public void GoToEvent(int id) {
        GetComponent<EventActivator>().runEvent = id;
        GetComponents<EventSetting>()[GetComponent<EventActivator>().runEvent].Activate();
    }

    private Transform SetTarget() {
        if (targetIsPlayer) {
            if (!GlobalStatus.mainPlayer) GlobalStatus.mainPlayer = GameObject.FindWithTag("Player");
            if (!GlobalStatus.mainPlayer) {
                print("No Player in the scene");
                return null;
            }

            return GlobalStatus.mainPlayer.transform;
        }

        if (!target) {
            print("You didn't assign Target yet");
            return null;
        }

        return target;
    }

    private IEnumerator Waiting() {
        yield return new WaitForSeconds(floatVar);
        NextEvent();
    }

    //Mode 0 = Spawn Prefab , Mode 1 = Spawn Prefab at Object , Mode 2 = Spawn at Player
    private void SpawnPrefab(int mode) {
        if (!objectVar) {
            print("You didn't assign Prefab yet");
            NextEvent();
            return;
        }

        if (mode == 0) {
            var obj = Instantiate(objectVar, vector3Var, transform.rotation);
            obj.eulerAngles = vector3Var2;
            if (msgString != "") obj.SendMessage(msgString, SendMessageOptions.DontRequireReceiver);
        }
        else if (mode == 1) {
            if (!target) {
                print("You didn't assign Spawn at Target yet");
                NextEvent();
                return;
            }

            var obj = Instantiate(objectVar, target.position, target.rotation);
            if (msgString != "") obj.SendMessage(msgString, SendMessageOptions.DontRequireReceiver);
        }
        else {
            if (!mainPlayer) mainPlayer = GameObject.FindWithTag("Player");
            if (!mainPlayer) {
                print("No Player in the scene");
                NextEvent();
                return;
            }

            var obj = Instantiate(objectVar, mainPlayer.transform.position, mainPlayer.transform.rotation);
            if (msgString != "") obj.SendMessage(msgString, SendMessageOptions.DontRequireReceiver);
        }

        NextEvent();
    }


    public void SetSkyFog() {
        RenderSettings.fogColor = colorVar;
        RenderSettings.fogDensity = floatVar;
        if (matVar) RenderSettings.skybox = matVar;
        NextEvent();
    }

    public void SetLight() {
        if (!lightVar) {
            print("No Light Source");
            NextEvent();
            return;
        }

        lightVar.color = colorVar;
        lightVar.intensity = lightIntesity;
        lightVar.bounceIntensity = floatVar2;
        lightVar.shadowStrength = floatVar3;
        NextEvent();
    }

    public void NextEvent() {
        var ev = GetComponent<EventActivator>();
        if (ev.runEvent >= GetComponents<EventSetting>().Length - 1) {
            //Done
            ev.EndEvent();
        }
        else {
            ev.runEvent++;
            GetComponents<EventSetting>()[ev.runEvent].Activate();
        }
    }

    private void ShakeCamera() {
        //Shake
        if (Camera.main.GetComponent<CameraFollowPlayer2D>())
            Camera.main.GetComponent<CameraFollowPlayer2D>().Shake(floatVar, floatVar2);
        else
            Camera.main.SendMessage("Shake", floatVar2, SendMessageOptions.DontRequireReceiver);
        NextEvent();
    }

    private bool CheckCondition() {
        var pass = false;

        if (condi == ConditionChecker.GreaterOrEqual) {
            if (globalInt[intVar] >= intVar2) pass = true;
        }
        else if (condi == ConditionChecker.Greater) {
            if (globalInt[intVar] > intVar2) pass = true;
        }
        else if (condi == ConditionChecker.Equal) {
            if (globalInt[intVar] == intVar2) pass = true;
        }
        else if (condi == ConditionChecker.LessOrEqual) {
            if (globalInt[intVar] <= intVar2) pass = true;
        }
        else if (condi == ConditionChecker.Less) {
            if (globalInt[intVar] < intVar2) pass = true;
        }
        else if (condi == ConditionChecker.NotEqual) {
            if (globalInt[intVar] != intVar2) pass = true;
        }

        return pass;
    }

    private void StartDialogue() {
        if (!showText) {
            print("You didn't Text source yet");
            return;
        }

        showText.gameObject.SetActive(true);
        if (imageVar) {
            imageVar.color = colorVar;
            imageVar.gameObject.SetActive(true);
        }

        if (imageVar2) {
            imageVar2.color = colorVar3;
            imageVar2.gameObject.SetActive(true);
        }

        if (textBoxVar)
            textBoxVar.gameObject.SetActive(true);
        if (nameText) {
            nameText.text = stringVar;
            nameText.gameObject.SetActive(true);
        }

        if (!showText.GetComponent<AnimateText>()) showText.gameObject.AddComponent<AnimateText>();
        var ev = GetComponent<EventActivator>();

        showText.text = "";
        showText.GetComponent<AnimateText>().enabled = true;
        showText.GetComponent<AnimateText>().source = GetComponents<EventSetting>()[ev.runEvent];
        showText.GetComponent<AnimateText>().StartAnimate(message);
    }

    public void FinishDialogue() {
        if (imageVar)
            imageVar.gameObject.SetActive(false);
        if (imageVar2)
            imageVar2.gameObject.SetActive(false);
        if (textBoxVar)
            textBoxVar.gameObject.SetActive(false);
        if (showText)
            showText.gameObject.SetActive(false);
        if (nameText) nameText.gameObject.SetActive(false);
        NextEvent();
    }
}

public enum ParametersType {
    Dialogue,
    Wait,
    ActivateOtherEvent,
    SpawnPrefabAtObject,
    GoToEvent,
    FreezeAll,
    MoveObjectTowards,
    EnableObject,
    DisableObject,
    DeleteObject,
    PlayMecanimAnimation,
    ControlBoolean,
    ControlVariable,
    ConditionCheck,
    PlayBGM,
    PlayOneShotAudio,
    ChangeScene,
    SetObjectLocation,
    SetObjectRotation,
    SetObjectAtObjectPosition,
    LookAtTarget,
    StopMovingAndLooking,
    SetParent,
    Unparent,
    AmbientEditor,
    LightEditor,
    FadeImage,
    MoveImage,
    ScreenShake,
    BreakEvent,
    SetCameraTarget,
    LockCamera,
    AddCash,
    AddSkillPoint,
    AddStatusPoint,
    AddItem,
    AddEquipment,
    SetTimeScale,
    AddQuest,
    SendMessage
}

public enum IntValueSet {
    Equal,
    Plus,
    Minus,
    Multiply,
    Divide
}

public enum ConditionChecker {
    GreaterOrEqual,
    Greater,
    Equal,
    LessOrEqual,
    Less,
    NotEqual
}

public enum CondiCheckForm {
    Variable,
    Boolean
}