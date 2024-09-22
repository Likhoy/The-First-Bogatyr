using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AttackTrigger))]
[RequireComponent(typeof(Status))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlatformerTransformation:MonoBehaviour {
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

	void Awake() {
		rb = GetComponent<Rigidbody2D>();
		originalGravity = rb.gravityScale;
		rb = GetComponent<Rigidbody2D>();
		//rb.gravityScale = 0;
		rb.constraints = RigidbodyConstraints2D.FreezeRotation;
		stat = GetComponent<Status>();
		atk = GetComponent<AttackTrigger>();
		if(!anim && stat.mainSprite) {
			anim = stat.mainSprite;
		}
		if(!anim && GetComponent<Animator>()) {
			anim = GetComponent<Animator>();
		}
		if(dropItemPrefab) {
			atk.dropItemPrefab = dropItemPrefab;
		}
	}

	void Update() {
		UpdateIsGrounded();
		if(Time.timeScale == 0.0f || stat.freeze || GlobalStatus.freezeAll || GlobalStatus.freezePlayer || stat.flinch || !stat.canControl) {
			if(onDashing) {
				CancelDash();
			}
			if(anim) {
				anim.SetBool("run",false);
			}
			//rb.velocity = Vector2.zero;
			rb.velocity = new Vector2(0,rb.velocity.y);
			return;
		}
		if(onDashing) {
			return;
		}

		if(joyStick) {
			if(Input.GetButton("Horizontal") || Input.GetButton("Vertical")) {
				moveHorizontal = Input.GetAxis("Horizontal");
				//moveVertical = Input.GetAxis("Vertical");
			} else {
				moveHorizontal = joyStick.position.x;
				//moveVertical = joyStick.position.y;
			}
		} else {
			moveHorizontal = Input.GetAxis("Horizontal");
			//moveVertical = Input.GetAxis("Vertical");
		}
		//Flip Right Side
		if(moveHorizontal > 0.1 && !atk.facingRight) {
			atk.facingRight = true;
			Vector3 rot = transform.eulerAngles;
			rot.y = 0;
			transform.eulerAngles = rot;
		}
		//Flip Left Side
		if(moveHorizontal < -0.1 && atk.facingRight) {
			atk.facingRight = false;
			Vector3 rot = transform.eulerAngles;
			rot.y = 180;
			transform.eulerAngles = rot;
		}
	}

	void FixedUpdate() {
		if(Time.timeScale == 0.0f || stat.freeze || GlobalStatus.freezeAll || GlobalStatus.freezePlayer || stat.flinch || !stat.canControl || stat.block) {
			moveHorizontal = 0;
			return;
		}
		if(onDashing) {
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

		rb.velocity = new Vector2(dirX,rb.velocity.y);

		if(moveHorizontal != 0) {
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

	public void DashButton() {
		if(onDashing) {
			return;
		}
		if(canDash && isGrounded) {
			StartCoroutine("Dash");
		}
		if(canAirDash && !isGrounded && !airMove) {
			StartCoroutine("AirDash");
		}
	}

	public void JumpButton() {
		if(isGrounded) {
			anim.SetTrigger("jump");
			rb.velocity = new Vector2(rb.velocity.x,0);
			rb.AddForce(Vector2.up * jumpForce);
		}

		if(canDoubleJump && !airMove2 || stat.hiddenStatus.doubleJump && !airMove2) {
			if(!isGrounded) {
				DoubleJump();
			}
		}
	}

	void DoubleJump() {
		anim.SetTrigger("jump");
		rb.velocity = new Vector2(rb.velocity.x,0.1f);
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

			if(leftFoot.collider == null && rightFoot.collider == null) {
				isGrounded = false;
			}else{
				isGrounded = true;
			}
		}else{
			if(rb.velocity.y == 0){
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

	void OnDrawGizmos(){
		Bounds colliderBounds = GetComponent<BoxCollider2D>().bounds;

		// Get the bottom-left and bottom-right points
		Vector2 bottomLeft = new Vector2(colliderBounds.min.x,colliderBounds.min.y - 0.01f);
		Vector2 bottomRight = new Vector2(colliderBounds.max.x,colliderBounds.min.y - 0.01f);

		// Draw Gizmos for the bottom-left and bottom-right points
		Gizmos.color = Color.red; // You can change the color as needed


		Gizmos.DrawRay(bottomLeft,-Vector2.up*0.05f); // แก้เป็น drawray
		Gizmos.DrawRay(bottomRight,-Vector2.up * 0.05f); // แก้เป็น drawray 
	}

	public IEnumerator Dash() {
		if(!onDashing) {
			if(stat.block) {
				stat.GuardBreak("cancelGuard");
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
		rb.gravityScale = originalGravity;
		onDashing = false;
	}

	public IEnumerator AirDash() {
		if(!onDashing && !airMove) {
			if(stat.block) {
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

	//Add On
	[System.Serializable]
	public class TransformationStat {
		public Animator newSprite;
		public Transform transformationEffectPrefab;

		public float speed = 6;
		public float jumpForce = 500;
		public bool canDoubleJump = false;

		public bool canDash = false;
		public float dashSpeed = 15;
		public float dashDuration = 0.5f;
		public bool canAirDash = false;

		public bool replaceAttackSetting = false;
		public BulletStatus[] attackPrefab = new BulletStatus[1];
		public string[] attackAnimationTrigger = new string[3];
		public string blockingAnimationTrigger;
		public bool canBlock = false;

		public WhileAtk whileAttack = WhileAtk.Immobile;
		public float attackCast = 0.18f;
		public float attackDelay = 0.12f;
		public AudioClip attackSoundEffect;
		public ChargeAtk[] chargeAttack;

		public bool changeColliderSize = false;
		public Vector2 newOffset;
		public Vector2 newSize;
	}
	public TransformationStat[] transformationSetting;
	public static int currentForm = 0;

	public bool transformLock = false;

	public void Transformation(int id) {
		if(id >= transformationSetting.Length || id == currentForm || transformLock){
			return;
		}
		currentForm = id;

		if(transformationSetting[id].transformationEffectPrefab) {
			Instantiate(transformationSetting[id].transformationEffectPrefab,transform.position,transform.rotation);
		}
		speed = transformationSetting[id].speed;
		jumpForce = transformationSetting[id].jumpForce;
		canDoubleJump = transformationSetting[id].canDoubleJump;
		canDash = transformationSetting[id].canDash;
		dashSpeed = transformationSetting[id].dashSpeed;
		dashDuration = transformationSetting[id].dashDuration;
		canAirDash = transformationSetting[id].canAirDash;

		if(transformationSetting[id].newSprite){
			anim.gameObject.SetActive(false);
			transformationSetting[id].newSprite.gameObject.SetActive(true);
			anim = transformationSetting[id].newSprite;
			stat.mainSprite = transformationSetting[id].newSprite;

            if(transformationSetting[id].changeColliderSize) {
				GetComponent<BoxCollider2D>().size = transformationSetting[id].newSize;
				GetComponent<BoxCollider2D>().offset = transformationSetting[id].newOffset;
			}
		}

		//Replace Attack
		GetComponent<Inventory>().replaceAttack = transformationSetting[id].replaceAttackSetting;

		if(transformationSetting[id].replaceAttackSetting) {
			GetComponent<AttackTrigger>().attackAnimationTrigger = transformationSetting[id].attackAnimationTrigger;
			GetComponent<AttackTrigger>().blockingAnimationTrigger = transformationSetting[id].blockingAnimationTrigger;
			GetComponent<AttackTrigger>().whileAttack = transformationSetting[id].whileAttack;
			GetComponent<AttackTrigger>().canBlock = transformationSetting[id].canBlock;
			GetComponent<AttackTrigger>().charge = transformationSetting[id].chargeAttack;
			GetComponent<AttackTrigger>().attackCast = transformationSetting[id].attackCast;
			GetComponent<AttackTrigger>().attackDelay = transformationSetting[id].attackDelay;
			GetComponent<AttackTrigger>().attackSoundEffect = transformationSetting[id].attackSoundEffect;

			if(transformationSetting[id].attackPrefab.Length > 0) {
				GetComponent<AttackTrigger>().attackPrefab = new BulletStatus[transformationSetting[id].attackPrefab.Length];
				for(int a = 0; a < transformationSetting[id].attackPrefab.Length; a++) {
					GetComponent<AttackTrigger>().attackPrefab[a] = transformationSetting[id].attackPrefab[a];
				}
			}
		}else if(GetComponent<Inventory>().weaponEquip > 0) {
			int tempEq = GetComponent<Inventory>().weaponEquip;
			GetComponent<Inventory>().weaponEquip = 0;
			GetComponent<Inventory>().EquipItem(tempEq,9999);
		}

	}
}

