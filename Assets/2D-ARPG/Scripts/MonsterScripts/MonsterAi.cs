using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Status))]
[RequireComponent(typeof (BoxCollider2D))]
[RequireComponent(typeof (Rigidbody2D))]
[AddComponentMenu("2D Action-RPG Kit/Create Monster")]

public class MonsterAi : MonoBehaviour {
	public enum AIState { Moving = 0, Pausing = 1 , Idle = 2}

	[HideInInspector]
	public Transform followTarget;
	public float approachDistance = 2.0f;
	public float detectRange = 7.0f;
	public float lostSight = 40.0f;
	public float speed = 4.0f;
	public float gravity = 0;

	public string[] attackAnimationTrigger = new string[1];
	public BulletStatus attackPrefab;
	public Transform attackPoint;
	
	public float attackCast = 0.3f;
	public float attackDelay = 0.5f;

	[HideInInspector]
	public AIState followState = AIState.Idle;
	private float distance = 0.0f;

	[HideInInspector]
	public bool cancelAttack = false;
	private bool attacking = false;
	//private bool castSkill = false;
	private GameObject[] gos;

	public AudioClip attackVoice;
	public AudioClip hurtVoice;

	public enum WhileAtkMon{
		MeleeFwd = 0,
		Immobile = 1
	}
	public WhileAtkMon whileAttack = WhileAtkMon.Immobile;
	
	public bool aimAtTarget = true;
	private Status stat;
	[HideInInspector]
	public bool facingRight = true;
	private Animator anim;
	private bool meleefwd = false;

	[System.Serializable]
	public class PatrollingSetting{
		public bool enable = false;
		public float patrolSpeed = 4;
		public float idleDuration = 1.5f;
		public float moveDuration = 1.5f;
	}
	public PatrollingSetting patrolSetting;
	private int patrolState = 0; //0 = Idle , 1 = Moving.
	private float patrolWait = 0;
	private int c = 0;
	private Vector3 pfwd = Vector3.right;
	private float density = 1;

	[System.Serializable]
	public class SkillSetting{
		public string skillName;
		public BulletStatus skillPrefab;
		public string skillAnimationTrigger;
		public GameObject castEffect;
		public float castEffectTime = 0.5f;
		public string castAnimationTrigger;
		public float castTime = 0.5f;
		public float delayTime = 1.5f;
		public float allSkillDelay = 10.5f;
		public bool spawnAtPlayer = false;
		public int repeatBullet = 1;
		public float repeatDelay = 0.3f;
		public DirectionSet moveDirection = DirectionSet.None;
		public float moveSpeed = 7.5f;
		public string moveAnimTrigger;
		public float moveDuration = 0.5f;
	}
	
	public enum DirectionSet{
		None = 0,
		Forward = 1,
		Backward = 2,
		Up = 3,
		Down = 4
	}
	
	public SkillSetting[] skill;
	private Transform playerPointer;
	private Rigidbody2D rb;
	public bool use4DirectionSprite = false;
	void Start(){
		gameObject.tag = "Enemy";
		gameObject.layer = 8; //Set to Character Layer
		rb = GetComponent<Rigidbody2D>();

		//Create new Attack Point if you didn't have one.
		if(!attackPoint){
			attackPoint = new GameObject().transform;
			attackPoint.position = transform.position;
			attackPoint.rotation = transform.rotation;
			attackPoint.parent = this.transform;
			attackPoint.name = "AttackPoint";
		}
		stat = GetComponent<Status>();
		if(!anim && stat.mainSprite){
			anim = stat.mainSprite;
		}
		if(!anim && GetComponent<Animator>()){
			anim = GetComponent<Animator>();
		}
		stability = stat.stability; //For Skill.
		rb.gravityScale = gravity;
		rb.constraints = RigidbodyConstraints2D.FreezeRotation;
		if(transform.eulerAngles.y == 0 && !use4DirectionSprite){
			facingRight = true;
		}
		if(use4DirectionSprite) {
			currentDir = 3;
		}
		if(GetComponent<Collider2D>()){
			density = GetComponent<Collider2D>().density;
		}
		if(!GetComponent<AudioSource>()){
			gameObject.AddComponent<AudioSource>();
		}
		playerPointer = new GameObject().transform;
		playerPointer.position = transform.position;
		playerPointer.rotation = transform.rotation;
		playerPointer.parent = this.transform;
		playerPointer.name = "PlayerPointer";

		//Set Z Axis to 0
		Vector3 pos = transform.position;
		pos.z = 0;
		transform.position = pos;
	}
	
	void Update(){
		if(Time.timeScale == 0.0f || stat.freeze || GlobalStatus.freezeAll){
			followState = AIState.Idle;
			if(rb.gravityScale > 0){
				rb.velocity = new Vector2(0 , rb.velocity.y);
			}else{
				rb.velocity = Vector2.zero;
			}
			if(anim){
				anim.SetBool("run" , false);
			}
			return;
		}
		FindClosestEnemy();
		
		if(meleefwd){
			if(rb.gravityScale > 0){
				rb.velocity = new Vector2(0 , rb.velocity.y);
			}else{
				rb.velocity = Vector2.zero;
			}
			if(rb.gravityScale == 0){
				Vector3 dir = attackPoint.TransformDirection(Vector3.right);
				rb.velocity = dir * 4;
			}else{
				Vector3 dir = attackPoint.TransformDirection(Vector3.right);
				rb.AddForce(dir * 3200 * density * Time.deltaTime);
			}
		}

		if(!followTarget){
			if(rb.gravityScale > 0){
				rb.velocity = new Vector2(0 , rb.velocity.y);
			}else{
				rb.velocity = Vector2.zero;
			}
			if(followState == AIState.Moving || followState == AIState.Pausing){
				followState = AIState.Idle;
				if(anim){
					anim.SetBool("run" , false);
				}
			}
			return;
		}
		//-----------------------------------
		distance = (transform.position - followTarget.position).magnitude;
		
		if(skill.Length > 0){
			if(!onSkill && followState != AIState.Idle && distance <= skillDistance){
				if(wait >= skillDelay){
					CancelAttack();
					StartCoroutine("UseSkill");
					wait = 0;
				}else{
					wait += Time.deltaTime;
				}
			}
		}
		
		if(stat.flinch){
			cancelAttack = true;
			rb.velocity = stat.knock * stat.knockForce;
			return;
		}

		if(onMoving){
			if(fwdSkill && distance <= approachDistance){
				rb.velocity = Vector2.zero;
				return;
			}
			if(rb.gravityScale > 0) {
				movDir.y = 0;
			}
			rb.velocity = movDir * movSpd;
		}

		if(attacking){
			return;
		}
		//------------------------------------
		if(followState == AIState.Moving){
			if(anim){
				anim.SetBool("run" , true);
			}
			LookAtTarget();
			if(distance <= approachDistance) {
				followState = AIState.Pausing;
			}else if(distance >= lostSight){
				//Lost Sight
				stat.health = stat.maxHealth;
				followState = AIState.Idle;
				if(anim){
					anim.SetBool("run" , false);
				}
			}else{
				//transform.position = Vector2.MoveTowards(transform.position, followTarget.position, speed * Time.deltaTime);
				if(rb.gravityScale > 0){
					Vector3 mov = transform.TransformDirection(Vector3.right) * speed;
					mov.y = -gravity;
					rb.velocity = mov;
				} else{
					if(playerPointer){
						Vector3 dir = followTarget.position - playerPointer.position;
						float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
						playerPointer.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
						rb.velocity = playerPointer.TransformDirection(Vector3.right) * speed;
					}else{
						Vector2 destination = new Vector2((transform.position.x - followTarget.position.x), (transform.position.y - followTarget.position.y)) * speed * 10 * Time.deltaTime;
						rb.velocity = -destination;
					}
				}

			}
		}else if(followState == AIState.Pausing){
			rb.velocity = Vector2.zero;
			if(anim){
				anim.SetBool("run" , false);
			}
			//----Attack----
			StartCoroutine("Attack");
			
			if(distance > approachDistance){
				followState = AIState.Moving;
			}
		}else if(followState == AIState.Idle){
			if(patrolSetting.enable){
				if(patrolState == 1){//Moving
					rb.velocity = pfwd * patrolSetting.patrolSpeed;
				}
				//----------------------------
				if(patrolWait >= patrolSetting.idleDuration && patrolState == 0){
					//Set to Moving Mode.
					Vector3 delta = transform.position;
					delta.x += Random.Range(-2.0f,2.0f);
					delta.y += Random.Range(-2.0f,2.0f);

					Vector3 dir = delta - attackPoint.position;
					float angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg;
					attackPoint.rotation = Quaternion.AngleAxis(angle,Vector3.forward);

					if(use4DirectionSprite){
						delta -= transform.position;
						//print(delta);
						if(delta.y > 1 && Mathf.Abs(delta.x) < 0.5f){
							SetDirection(2);
						}else if(delta.y < 1 && Mathf.Abs(delta.x) < 0.5f){
							SetDirection(3);
						}else if(delta.x >= 0){
							SetDirection(0);
						}else if(delta.x < 0){
							SetDirection(1);
						}
						pfwd = attackPoint.TransformDirection(Vector3.right);
					}else{
						//-------------------
						int r = Random.Range(0,10);
						if(r >= 5){
							Vector3 rot = transform.eulerAngles;
							rot.y = 0;
							transform.eulerAngles = rot;
							facingRight = true;
						}else{
							Vector3 rot = transform.eulerAngles;
							rot.y = 180;
							transform.eulerAngles = rot;
							facingRight = false;
						}
						if(anim){
							anim.SetBool("run",true);
						}
						//Random Movement Direction
						pfwd = transform.TransformDirection(Vector3.right);
					}
					
					if(gravity == 0){
						pfwd.y = Random.Range(-1.1f , 1.1f);
					}
					patrolWait = 0; // Reset wait time.
					patrolState = 1; // Change State to Move.
				}
				if(patrolWait >= patrolSetting.moveDuration && patrolState == 1){
					//Set to Idle Mode.
					if(anim){
						anim.SetBool("run" , false);
					}
					rb.velocity = Vector2.zero;
					patrolWait = 0;
					patrolState = 0;
				}
				patrolWait += Time.deltaTime;
				//-----------------------------
			}else{
				rb.velocity = Vector2.zero;
			}
			//----------------Idle Mode--------------
			int getHealth = stat.maxHealth - stat.health;
			
			if(distance < detectRange || getHealth > 0){
				followState = AIState.Moving;
			}
		}
	}

	void LookAtTarget(){
		if(!followTarget){
			return;
		}
		Vector3 delta = followTarget.position - transform.position;

		if(use4DirectionSprite) {
			if(delta.y > 1 && Mathf.Abs(delta.x) < 2){
				SetDirection(2);
			}else if(delta.y < -1 && Mathf.Abs(delta.x) < 2){
				SetDirection(3);
			}else if(delta.x >= 0){
				SetDirection(0);
			}else if(delta.x < 0){
				SetDirection(1);
			}
			return;
		}

		if(delta.x >= 0 && !facingRight){
			Vector3 rot = transform.eulerAngles;
			rot.y = 0;
			transform.eulerAngles = rot;
			facingRight = true;
		}else if (delta.x < 0 && facingRight){
			Vector3 rot = transform.eulerAngles;
			rot.y = 180;
			transform.eulerAngles = rot;
			facingRight = false;
		}
	}

	private int currentDir = 0;
	public void SetDirection(int dir){
		if(dir == currentDir){
			return;
		}
		if(dir == 0){
			//Right
			Vector3 rot = attackPoint.eulerAngles;
			if(rot != new Vector3(0,0,0) || use4DirectionSprite) {
				anim.SetTrigger("startMoving");
			}
			anim.SetInteger("runDir",0);
		}else if(dir == 1){
			//Left
			Vector3 rot = attackPoint.eulerAngles;
			if(rot != new Vector3(0,180,0) || use4DirectionSprite) {
				anim.SetTrigger("startMoving");
			}
			anim.SetInteger("runDir",1);
		}else if(dir == 2){
			//Up
			Vector3 rot = attackPoint.eulerAngles;
			if(rot != new Vector3(0,0,90) || use4DirectionSprite) {
				anim.SetTrigger("startMoving");
			}
			anim.SetInteger("runDir",2);
		}else if(dir == 3){
			//Down
			Vector3 rot = attackPoint.eulerAngles;
			if(rot != new Vector3(0,0,270) || use4DirectionSprite) {
				anim.SetTrigger("startMoving");
			}
			anim.SetInteger("runDir",3);
		}
		currentDir = dir;
	}

	void FindClosestEnemy(){ 
		// Find Closest Player   
		List<GameObject> gosList = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
		gosList.AddRange(GameObject.FindGameObjectsWithTag("Ally"));
		
		gos = gosList.ToArray() as GameObject[];
		
		if(gos.Length > 0){
			float distance = Mathf.Infinity; 
			Vector3 position = transform.position; 
			
			foreach(GameObject go in gos) { 
				Vector3 diff = (go.transform.position - position); 
				float curDistance = diff.sqrMagnitude; 
				if(curDistance < distance) { 
					//------------
					followTarget = go.transform; 
					distance = curDistance;
				} 
			} 
		}
	}

	void CancelAttack(){
		c = 0;
		attacking = false;
		StopCoroutine("Attack");
	}

	IEnumerator Attack(){
		cancelAttack = false;
		if(!stat.flinch && !stat.freeze && !attacking){
			rb.velocity = Vector2.zero;
			attacking = true;
			if(whileAttack == WhileAtkMon.MeleeFwd){
				StartCoroutine(MeleeDash());
			}
			if(anim && attackAnimationTrigger[c] != ""){
				anim.SetTrigger(attackAnimationTrigger[c]);
			}
			LookAtTarget();
			if(aimAtTarget && followTarget){
				//attackPoint.LookAt(followTarget.position);
				Vector3 dir = followTarget.position - attackPoint.position;
				float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
				attackPoint.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
			}
			yield return new WaitForSeconds(attackCast);
			
			if(!cancelAttack){
				if(attackVoice && !stat.flinch){
					GetComponent<AudioSource>().PlayOneShot(attackVoice);
				}
				Transform bulletShootout = Instantiate(attackPrefab.transform, attackPoint.position , attackPoint.rotation) as Transform;
				bulletShootout.gameObject.SetActive(true);
				bulletShootout.GetComponent<BulletStatus>().Setting(stat.atk , stat.matk , "Enemy" , this.gameObject);
				c++;
				if(c >= attackAnimationTrigger.Length){
					c = 0;
				}
				yield return new WaitForSeconds(attackDelay);
				attacking = false;
				CheckDistance();
				/*if(distance > approachDistance + 0.55f){
					c = 0;
				}*/
			}else{
				c = 0;
				attacking = false;
			}
		}
	}

	void CheckDistance(){
		if(!followTarget || GlobalStatus.freezeAll){
			followState = AIState.Idle;
			if(anim){
				anim.SetBool("run" , false);
			}
			return;
		}
		float distancea = (followTarget.position - transform.position).magnitude;
		if(distancea > approachDistance){
			followState = AIState.Moving;
		}
		if(distancea > approachDistance + 0.5f){
			c = 0;
		}
	}

	IEnumerator MeleeDash(){
		meleefwd = true;
		yield return new WaitForSeconds(0.2f);
		meleefwd = false;
		if(rb.gravityScale > 0){
			rb.velocity = new Vector2(0 , rb.velocity.y);
		}else{
			rb.velocity = Vector2.zero;
		}
	}

	private bool onSkill = false;
	private float wait = 0;
	private GameObject eff;
	private bool onMoving = false;
	private Vector3 movDir = Vector3.zero;
	private float movSpd = 5;
	private bool stability = false;
	public float skillDistance = 5.5f;
	private float skillDelay = 5.5f;
	private bool fwdSkill = false;

	IEnumerator UseSkill(){
		if(!GetComponent<Status>().freeze){
			int s = 0;
			if(skill.Length > 1){
				s = Random.Range(0 , skill.Length);
			}
			onSkill = true;
			attacking = true;
			stat.stability = true;
			cancelAttack = false;
			fwdSkill = false;

			if(aimAtTarget && followTarget){
				Vector3 dir = followTarget.position - attackPoint.position;
				float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
				attackPoint.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
			}
			//Moving
			if(skill[s].moveDirection != DirectionSet.None && followTarget){

				if(anim){
					if(skill[s].moveAnimTrigger != ""){
						anim.SetTrigger(skill[s].moveAnimTrigger);
					}else{
						anim.SetBool("run" , true);
					}
				}
				LookAtTarget();
				if(skill[s].moveDirection == DirectionSet.Forward){
					fwdSkill = true;
					movDir = attackPoint.TransformDirection(Vector3.right);
				}
				if(skill[s].moveDirection == DirectionSet.Backward){
					movDir = attackPoint.TransformDirection(Vector3.left);
				}
				if(skill[s].moveDirection == DirectionSet.Up){
					movDir = Vector3.up;
				}
				if(skill[s].moveDirection == DirectionSet.Down){
					movDir = Vector3.down;
				}
				movSpd = skill[s].moveSpeed;
				
				onMoving = true;
				yield return new WaitForSeconds(skill[s].moveDuration);
				onMoving = false;
				rb.velocity = Vector2.zero;
				if(anim){
					anim.SetBool("run" , false);
				}
			}
			
			//Cast Effect
			if(skill[s].castEffect && followTarget){
				if(anim && skill[s].castAnimationTrigger != ""){
					anim.SetTrigger(skill[s].castAnimationTrigger);
				}
				eff = Instantiate(skill[s].castEffect , transform.position , Quaternion.identity) as GameObject;
				eff.transform.parent = this.transform;
				yield return new WaitForSeconds(skill[s].castEffectTime);
			}
			//Call UseSkill Function in AIsetC Script.

			for(int a = 0; a < skill[s].repeatBullet; a++){
				if(!stat.freeze){
					if(anim && skill[s].skillAnimationTrigger != ""){
						anim.SetTrigger(skill[s].skillAnimationTrigger);
					}
					LookAtTarget();
					yield return new WaitForSeconds(skill[s].castTime);
					if(aimAtTarget && followTarget){
						Vector3 dir = followTarget.position - attackPoint.position;
						float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
						attackPoint.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
					}
					
					Transform bulletShootout = Instantiate(skill[s].skillPrefab.transform , attackPoint.position , attackPoint.rotation) as Transform;
					bulletShootout.gameObject.SetActive(true);
					bulletShootout.GetComponent<BulletStatus>().Setting(stat.atk , stat.matk , "Enemy" , this.gameObject);
					if(skill[s].spawnAtPlayer && followTarget){
						bulletShootout.position = followTarget.position;
					}
					//print(a);
					if(a < skill[s].repeatBullet -1){
						yield return new WaitForSeconds(skill[s].repeatDelay);
					}
				}else{
					onSkill = false;
					attacking = false;
					a = skill[s].repeatBullet;
					skillDelay = skill[s].allSkillDelay;
					fwdSkill = false;
					if(!stability){
						stat.stability = false;
					}
					if(eff){
						Destroy(eff);
					}
					yield break;
				}
			}
			yield return new WaitForSeconds(skill[s].castTime);
			if(eff){
				Destroy(eff);
			}
			yield return new WaitForSeconds(skill[s].delayTime);
			onSkill = false;
			attacking = false;
			fwdSkill = false;
			if(!stability){
				stat.stability = false;
			}
			skillDelay = skill[s].allSkillDelay;
			CheckDistance();
		}
	}
	//-----------
}

