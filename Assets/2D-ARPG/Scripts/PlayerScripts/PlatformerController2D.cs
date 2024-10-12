using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Rigidbody2D))]
[RequireComponent(typeof (AttackTrigger))]
[RequireComponent(typeof (Status))]
[RequireComponent(typeof (BoxCollider2D))]
public class PlatformerController2D : MonoBehaviour {
	public Animator anim;
	public float speed = 6;
	public float jumpForce = 500;
	public bool canDoubleJump = false;

	public bool canDash = false;
	public float dashSpeed = 15;
	public float dashDuration = 0.5f;
	public bool canAirDash = false;

	private Rigidbody2D rb;

	private Status stat;
	private AttackTrigger atk;
	private float dirX;

	private bool isGrounded = false;
	private bool moving = false;
	private bool airMove = false;
	private bool airMove2 = false;
	private bool onDashing = false;
	private float originalGravity = 1;
	public JoystickCanvas joyStick;// For Mobile
	private float moveHorizontal;

	public Transform dropItemPrefab;

	void Awake(){
		rb = GetComponent<Rigidbody2D>();
		originalGravity = rb.gravityScale;
		rb = GetComponent<Rigidbody2D>();
		//rb.gravityScale = 0;
		rb.constraints = RigidbodyConstraints2D.FreezeRotation;
		stat = GetComponent<Status>();
		atk = GetComponent<AttackTrigger>();
		if(!anim && stat.mainSprite){
			anim = stat.mainSprite;
		}
		if(!anim && GetComponent<Animator>()){
			anim = GetComponent<Animator>();
		}
		if(dropItemPrefab){
			atk.dropItemPrefab = dropItemPrefab;
		}
	}
	
	void Update(){
		UpdateIsGrounded();
		if(Time.timeScale == 0.0f || stat.freeze || GlobalStatus.freezeAll || GlobalStatus.freezePlayer || stat.flinch || !stat.canControl){
			if(onDashing){
				CancelDash();
			}
			if(anim){
				anim.SetBool("run" , false);
			}
			//rb.velocity = Vector2.zero;
			rb.velocity = new Vector2(0 , rb.velocity.y);
			return;
		}
		if(onDashing){
			return;
		}

		if(joyStick){
			if(Input.GetButton("Horizontal") || Input.GetButton("Vertical")){
				moveHorizontal = Input.GetAxis("Horizontal");
				//moveVertical = Input.GetAxis("Vertical");
			}else{
				moveHorizontal = joyStick.position.x;
				//moveVertical = joyStick.position.y;
			}
		}else{
			moveHorizontal = Input.GetAxis("Horizontal");
			//moveVertical = Input.GetAxis("Vertical");
		}
		//Flip Right Side
		if(moveHorizontal > 0.1 && !atk.facingRight){
			atk.facingRight = true;
			Vector3 rot = transform.eulerAngles;
			rot.y = 0;
			transform.eulerAngles = rot;
		}
		//Flip Left Side
		if(moveHorizontal < -0.1 && atk.facingRight){
			atk.facingRight = false;
			Vector3 rot = transform.eulerAngles;
			rot.y = 180;
			transform.eulerAngles = rot;
		}
	}
	
	void FixedUpdate(){
		if(Time.timeScale == 0.0f || stat.freeze || GlobalStatus.freezeAll || GlobalStatus.freezePlayer || stat.flinch || !stat.canControl || stat.block){
			moveHorizontal = 0;
			return;
		}
		if(onDashing){
			if(atk.onAttacking){
				CancelDash();
			}
			if(atk.aimAtMouse){
				atk.LookAtMouse();
			}
			Vector3 dir = transform.TransformDirection(Vector3.right);
			GetComponent<Rigidbody2D>().velocity = dir * dashSpeed;
			return;
		}
		
		dirX = moveHorizontal * speed;

		rb.velocity = new Vector2(dirX , rb.velocity.y);

		if(moveHorizontal != 0){
			moving = true;
			if(anim){
				anim.SetBool("run" , moving);
			}
		}else if(moving){
			moving = false;
			if(anim){
				anim.SetBool("run" , moving);
			}
		}
		
	}

	public void DashButton(){
		if(onDashing){
			return;
		}
		if(canDash && isGrounded){
			StartCoroutine("Dash");
		}
		if(canAirDash && !isGrounded && !airMove){
			StartCoroutine("AirDash");
		}
	}

	public void JumpButton(){
		if(isGrounded){
			anim.SetTrigger("jump");
			rb.velocity = new Vector2(rb.velocity.x , 0);
			rb.AddForce(Vector2.up * jumpForce);
		}

		if(canDoubleJump && !airMove2 || stat.hiddenStatus.doubleJump && !airMove2) {
			if(!isGrounded){
				DoubleJump();
			}
		}
	}

	void DoubleJump(){
		anim.SetTrigger("jump");
		rb.velocity = new Vector2(rb.velocity.x , 0.1f);
		rb.AddForce(Vector2.up * jumpForce);
		airMove2 = true;
	}

	void UpdateIsGrounded(){
		if(airMove && onDashing){
			return;
		}

		if(GetComponent<BoxCollider2D>()){
			Bounds colliderBounds = GetComponent<BoxCollider2D>().bounds;

			Vector2 bottomLeft = new Vector2(colliderBounds.min.x,colliderBounds.min.y - 0.01f);
			Vector2 bottomRight = new Vector2(colliderBounds.max.x,colliderBounds.min.y - 0.01f);

			RaycastHit2D leftFoot = Physics2D.Raycast(bottomLeft,-Vector2.up,0.05f);
			RaycastHit2D rightFoot = Physics2D.Raycast(bottomRight,-Vector2.up,0.05f);

			if(leftFoot.collider == null && rightFoot.collider == null){
				isGrounded = false;
			}else{
				isGrounded = true;
			}
		}else{
			if(rb.velocity.y == 0) {
				isGrounded = true;
			}else{
				isGrounded = false;
			}
		}

		anim.SetBool("grounded",isGrounded);
		if(airMove && isGrounded){
			airMove = false;
		}
		if(airMove2 && isGrounded){
			airMove2 = false;
		}
	}

	public IEnumerator Dash(){
		if(!onDashing){
			if(stat.block){
				stat.GuardBreak("cancelGuard");
			}
			onDashing = true;
			anim.SetTrigger("dash");
			anim.ResetTrigger("cancelDash");
			yield return new WaitForSeconds(dashDuration);
			CancelDash();
		}
	}
	
	public void CancelDash(){
		StopCoroutine("Dash");
		anim.SetTrigger("cancelDash");
		rb.gravityScale = originalGravity;
		onDashing = false;
	}
	
	public IEnumerator AirDash(){
		if(!onDashing && !airMove){
			if(stat.block){
				stat.GuardBreak("cancelGuard");
			}
			airMove = true;
			onDashing = true;
			rb.gravityScale = 0.01f;
			anim.SetTrigger("dash");
			anim.ResetTrigger("cancelDash");
			yield return new WaitForSeconds(dashDuration);
			rb.gravityScale = originalGravity;
			CancelDash();
		}
	}

}
