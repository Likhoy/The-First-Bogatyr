using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AttackTrigger))]
[RequireComponent(typeof(BoxCollider2D))]

public class TopDownFourDirection:MonoBehaviour{
	private Animator anim;
	private Rigidbody2D rb;
	public float speed = 6;
	private float dirX, dirY;
	private Status stat;
	private bool moving = false;
	private AttackTrigger atk;

	public bool canDash = false;
	public float dashSpeed = 15;
	public float dashDuration = 0.5f;
	private bool onDashing = false;
	public JoystickCanvas joyStick;// For Mobile
	private float moveHorizontal;
	private float moveVertical;

	public bool forceOneDirection = true;

	void Start() {
		rb = GetComponent<Rigidbody2D>();
		rb.gravityScale = 0;
		rb.constraints = RigidbodyConstraints2D.FreezeRotation;
		stat = GetComponent<Status>();
		atk = GetComponent<AttackTrigger>();
		if(!anim && stat.mainSprite) {
			anim = stat.mainSprite;
		}
		if(!anim && GetComponent<Animator>()) {
			anim = GetComponent<Animator>();
		}
	}

	void Update() {
		if(Time.timeScale == 0.0f || stat.freeze || GlobalStatus.freezeAll || GlobalStatus.freezePlayer || !stat.canControl && !atk.meleefwd){
			rb.velocity = Vector2.zero;
			if(onDashing) {
				CancelDash();
			}
			if(anim) {
				anim.SetBool("run",false);
			}
			return;
		}
		if(onDashing){
			return;
		}

		if(joyStick){
			if(Input.GetButton("Horizontal") || Input.GetButton("Vertical")){
				moveHorizontal = Input.GetAxisRaw("Horizontal");
				moveVertical = Input.GetAxisRaw("Vertical");
			} else {
				moveHorizontal = joyStick.position.x;
				moveVertical = joyStick.position.y;
			}
		}else{
			moveHorizontal = Input.GetAxisRaw("Horizontal");
			moveVertical = Input.GetAxisRaw("Vertical");
		}
        if(forceOneDirection && !joyStick || forceOneDirection && joyStick && !joyStick.gameObject.activeInHierarchy){
			if(moveHorizontal != 0){
				moveVertical = 0;
			}
		}

        if(!stat.canControl && atk.meleefwd){
			return;
		}

		if(moveHorizontal > 0.4f){
			//Right
			SetDirection(0);
		}else if(moveHorizontal < -0.4f){
			//Left
			SetDirection(1);
		}else if(moveVertical > 0.1f){
			//Up
			SetDirection(2);
		}else if(moveVertical < -0.1f){
			//Down
			SetDirection(3);
		}
	}

	void FixedUpdate(){
		if(Time.timeScale == 0.0f || stat.freeze || GlobalStatus.freezeAll || GlobalStatus.freezePlayer || stat.flinch || !stat.canControl || stat.block){
			moveHorizontal = 0;
			moveVertical = 0;
			//rb.velocity = Vector2.zero;
			return;
		}
		if(onDashing){
			if(atk.onAttacking) {
				CancelDash();
			}

			Vector3 dir = atk.attackPoint.TransformDirection(Vector3.right);
			GetComponent<Rigidbody2D>().velocity = dir * dashSpeed;
			return;
		}

		dirX = moveHorizontal * speed;
		dirY = moveVertical * speed;

		rb.velocity = new Vector2(dirX,dirY);
		if(moveHorizontal != 0 || moveVertical != 0) {
			if(!moving) {
				anim.SetTrigger("startMoving");
			}
			moving = true;
			if(anim) {
				anim.SetBool("run",moving);
			}
		} else if(moving) {
			moving = false;
			if(anim) {
				anim.SetBool("run",moving);
			}
		}
	}

	public void DashButton(){
		if(!canDash){
			return;
		}
		StartCoroutine("Dash");
	}

	public IEnumerator Dash() {
		if(!onDashing) {
			if(stat.block) {
				stat.GuardBreak("cancelGuard");
			}
			if(atk.aimAtMouse) {
				atk.LookAtMouse();
			}
			onDashing = true;
			anim.SetTrigger("dash");
			anim.ResetTrigger("cancelDash");
			yield return new WaitForSeconds(dashDuration);
			CancelDash();
		}
	}

	public void CancelDash() {
		StopCoroutine("Dash");
		anim.SetTrigger("cancelDash");
		onDashing = false;
	}

	private int currentDir = 0;
	public void SetDirection(int dir) {
		if(dir == currentDir){
			return;
		}
		if(dir == 0){
			//Right
			Vector3 rot = atk.attackPoint.eulerAngles;
			if(rot != new Vector3(0,0,0)) {
				anim.SetTrigger("startMoving");
			}
			anim.SetInteger("runDir",0);
			if(!atk.aimAtMouse) {
				rot.y = 0;
				rot.z = 0;
				atk.attackPoint.eulerAngles = rot;
			}
		}else if(dir == 1){
			//Left
			Vector3 rot = atk.attackPoint.eulerAngles;
			if(rot != new Vector3(0,180,0)) {
				anim.SetTrigger("startMoving");
			}
			anim.SetInteger("runDir",1);
			if(!atk.aimAtMouse) {
				rot.y = 180;
				rot.z = 0;
				atk.attackPoint.eulerAngles = rot;
			}
		}else if(dir == 2){
			//Up
			Vector3 rot = atk.attackPoint.eulerAngles;
			if(rot != new Vector3(0,0,90)) {
				anim.SetTrigger("startMoving");
			}
			anim.SetInteger("runDir",2);
			if(!atk.aimAtMouse) {
				rot.y = 0;
				rot.z = 90;
				atk.attackPoint.eulerAngles = rot;
			}
		}else if(dir == 3){
			//Down
			Vector3 rot = atk.attackPoint.eulerAngles;
			if(rot != new Vector3(0,0,270)) {
				anim.SetTrigger("startMoving");
			}
			anim.SetInteger("runDir",3);
			if(!atk.aimAtMouse) {
				rot.y = 0;
				rot.z = 270;
				atk.attackPoint.eulerAngles = rot;
			}
		}
		currentDir = dir;
	}
}
