using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletStatus : MonoBehaviour {
	public int damage = 10;
	public int damageMax = 20;
	
	[HideInInspector]
	public int playerAttack = 5;
	public int totalDamage = 0;
	public int variance = 15;
	[Range(0, 100)]
	public int criticalChance = 0;

	public enum AbStat{
		Poison = 0,
		Silence = 1,
		Stun = 2,
		Frozen = 3,
	}
	[System.Serializable]
	public class AbnormalStatAtk{
		public AbStat status = AbStat.Poison;
		[Range(1 , 100)]
		public int chance = 100;
		public float duration = 5f;
	}
	public AbnormalStatAtk[] inflictStatus;

	public string shooterTag = "Player";
	[HideInInspector]
	public GameObject shooter;
	
	public Transform Popup;
	
	public GameObject hitEffect;
	public bool effectAtTarget = false;

	public bool flinch = false;
	[Tooltip("Used when flinch was mark on.")]
	public float knockBackForces = 4;
	public bool penetrate = false;

	[Tooltip("Prevent Bullet to damage same enemy multiple times when mark on.")]
	public bool oneHitPerEnemy = false;
	private string popDamage = "";

	public enum AtkType{
		Physic = 0,
		Magic = 1,
	}
	public AtkType AttackType = AtkType.Physic;
	public ElementalAtk element = ElementalAtk.Normal;
	[Range(0 , 100)]
	public int drainHp = 0;

	[System.Serializable]
	public class BombHit{
		public bool enable = false;
		public GameObject bombEffect;
		public float bombRadius = 20;
	}
	public BombHit bombHitSetting;


	[System.Serializable]
	public class BulletChildStat{
		public bool enable = false;
		public BulletStatus prefab;
		public int maximumSpawn = 1;
	}
	public BulletChildStat spawnChildWhenHit;
	private int alreadySpawn = 0;

	public float fwdPlusAfterSpawn = 0;

	private List<GameObject> alreadyDmg = new List<GameObject>();

	//-----------------------------
	void Start(){
		if(variance >= 100){
			variance = 100;
		}
		if(variance <= 1){
			variance = 1;
		}
		if(fwdPlusAfterSpawn != 0 ){
			Vector3 absoluteDirection = transform.rotation * Vector3.right;
			transform.position += absoluteDirection * fwdPlusAfterSpawn;
		}
	}
	
	public void Setting(int str , int mag , string tag , GameObject owner){
		if(AttackType == AtkType.Physic){
			playerAttack = str;
		}else{
			playerAttack = mag;
		}
		shooterTag = tag;
		shooter = owner;
		int varMin = 100 - variance;
		int varMax = 100 + variance;
		int randomDmg = Random.Range(damage, damageMax);
		totalDamage = (randomDmg + playerAttack) * Random.Range(varMin ,varMax) / 100;
	}

	void OnTriggerEnter2D(Collider2D other){  	
		//When Player Shoot at Enemy
		if(shooterTag == "Player" && other.tag == "Enemy"){
			if(oneHitPerEnemy){
				//Prevent Bullet to damage same enemy multiple times.
				if(!alreadyDmg.Contains(other.gameObject)){
					alreadyDmg.Add(other.gameObject);
				}else{
					return;
				}
			}
			Transform dmgPop = Instantiate(Popup, other.transform.position , Popup.rotation) as Transform;
			if(effectAtTarget){
				dmgPop.position = other.transform.position;
			}
			
			if(criticalChance >= 1){
				if(criticalChance >= 100){
					criticalChance = 100;
				}
				int per = Random.Range(0, 100);
				if(per <= criticalChance){
					dmgPop.GetComponent<DamagePopup>().critical = true;
					totalDamage *= 2;
				}
			}
			
			if(AttackType == AtkType.Physic){
				popDamage = other.GetComponent<Status>().OnDamage(totalDamage , (int)element , false);
			}else{
				popDamage = other.GetComponent<Status>().OnDamage(totalDamage , (int)element , true);
			}
			dmgPop.GetComponent<DamagePopup>().damage = popDamage;	
			
			if(hitEffect){
				Vector3 spPos = transform.position;
				if(effectAtTarget){
					spPos = other.transform.position;
				}
				Instantiate(hitEffect, spPos , transform.rotation);
			}
			if(flinch){
				Vector3 dir = (other.transform.position - transform.position).normalized;
				other.GetComponent<Status>().knockForce = knockBackForces;
				other.GetComponent<Status>().Flinch(dir);
			}
			if(bombHitSetting.enable){
				ExplosionDamage();
			}
			if(spawnChildWhenHit.enable){
				SpawnBulletChild();
			}
			//Drain HP
			if(drainHp > 0 && shooter && popDamage != "Miss" && popDamage != "Evaded" && popDamage != "Guard" && popDamage != "Invulnerable"){
				int lf = int.Parse(popDamage) * drainHp;
				lf /= 100;
				if(lf < 1){
					lf = 1;
				}
				Vector3 hpos = shooter.transform.position;
				hpos.y += 0.75f;
				Transform hpPop = Instantiate(Popup, hpos , transform.rotation) as Transform;
				hpPop.GetComponent<DamagePopup>().damage = lf.ToString();
				hpPop.GetComponent<DamagePopup>().fontColor = Color.green;
				shooter.GetComponent<Status>().Heal(lf , 0);
			}
			if(inflictStatus.Length > 0){
				for(int a = 0; a < inflictStatus.Length; a++){
					int ran = Random.Range(0,100);
					if(ran <= inflictStatus[a].chance){
						//Call Function ApplyAbnormalStat in Status Script
						other.GetComponent<Status>().ApplyAbnormalStat((int)inflictStatus[a].status , inflictStatus[a].duration);
					}
				}
			}
			//----------------------------
			
			if(!penetrate){
				Destroy(gameObject);
			}
			//When Enemy Shoot at Player
		}else if(shooterTag == "Enemy" && other.tag == "Player" || shooterTag == "Enemy" && other.tag == "Ally"){
			if(oneHitPerEnemy){
				//Prevent Bullet to damage same enemy multiple times.
				if(!alreadyDmg.Contains(other.gameObject)){
					alreadyDmg.Add(other.gameObject);
				}else{
					return;
				}
			}
			if(AttackType == AtkType.Physic){
				popDamage = other.GetComponent<Status>().OnDamage(totalDamage , (int)element , false);
			}else{
				popDamage = other.GetComponent<Status>().OnDamage(totalDamage , (int)element , true);
			}
			Transform dmgPop = Instantiate(Popup, transform.position , Popup.rotation) as Transform;
			if(effectAtTarget){
				dmgPop.position = other.transform.position;
			}
			dmgPop.GetComponent<DamagePopup>().damage = popDamage;
			dmgPop.GetComponent<DamagePopup>().fontColor = Color.red;
			
			if(hitEffect){
				Vector3 spPos = transform.position;
				if(effectAtTarget){
					spPos = other.transform.position;
				}
				Instantiate(hitEffect, spPos , transform.rotation);
			}

			if(flinch){
				Vector3 dir = (other.transform.position - transform.position).normalized;
				other.GetComponent<Status>().knockForce = knockBackForces;
				other.GetComponent<Status>().Flinch(dir);
			}
			if(bombHitSetting.enable){
				ExplosionDamage();
			}
			if(spawnChildWhenHit.enable){
				SpawnBulletChild();
			}
			//Drain HP
			if(drainHp > 0 && shooter && popDamage != "Miss" && popDamage != "Evaded" && popDamage != "Guard" && popDamage != "Invulnerable"){
				int lf = int.Parse(popDamage) * drainHp;
				lf /= 100;
				if(lf < 1){
					lf = 1;
				}
				Vector3 hpos = shooter.transform.position;
				hpos.y += 0.75f;
				Transform hpPop = Instantiate(Popup, hpos , transform.rotation) as Transform;
				hpPop.GetComponent<DamagePopup>().damage = lf.ToString();
				hpPop.GetComponent<DamagePopup>().fontColor = Color.green;
				shooter.GetComponent<Status>().Heal(lf , 0);
			}
			if(inflictStatus.Length > 0){
				for(int a = 0; a < inflictStatus.Length; a++){
					int ran = Random.Range(0,100);
					if(ran <= inflictStatus[a].chance){
						//Call Function ApplyAbnormalStat in Status Script
						other.GetComponent<Status>().ApplyAbnormalStat((int)inflictStatus[a].status , inflictStatus[a].duration);
					}
				}
			}
			//----------------------------
			if(!penetrate){
				Destroy (gameObject);
			}
		}
	}
	
	public void ExplosionDamage(){
		//Collider[] hitColliders = Physics.OverlapSphere(transform.position, bombHitSetting.bombRadius);
		Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position , bombHitSetting.bombRadius);
		if(bombHitSetting.bombEffect){
			Instantiate(bombHitSetting.bombEffect , transform.position , transform.rotation);
		}
		
		for(int i= 0; i < hitColliders.Length; i++){
			if(shooterTag == "Player" && hitColliders[i].tag == "Enemy"){	  
				DealDamageOnly(hitColliders[i].transform , false);
			}else if(shooterTag == "Enemy" && hitColliders[i].tag == "Player" || shooterTag == "Enemy" && hitColliders[i].tag == "Ally"){  	
				DealDamageOnly(hitColliders[i].transform , true);
			}
		}
		bombHitSetting.enable = false;
	}
	
	public void DealDamageOnly(Transform other , bool red){
		if(oneHitPerEnemy){
			//Prevent Bullet to damage same enemy multiple times.
			if(!alreadyDmg.Contains(other.gameObject)){
				alreadyDmg.Add(other.gameObject);
			}else{
				return;
			}
		}
		if(AttackType == AtkType.Physic){
			popDamage = other.GetComponent<Status>().OnDamage(totalDamage , (int)element , false);
		}else{
			popDamage = other.GetComponent<Status>().OnDamage(totalDamage , (int)element , true);
		}
		Transform dmgPop = Instantiate(Popup, other.position , other.rotation) as Transform;	
		dmgPop.GetComponent<DamagePopup>().damage = popDamage;
		if(red){
			dmgPop.GetComponent<DamagePopup>().fontColor = Color.red;
		}
		
		if(hitEffect){
			Instantiate(hitEffect, other.position , other.rotation);
		}
		if(flinch){
			Vector3 dir = (other.transform.position - transform.position).normalized;
			other.GetComponent<Status>().knockForce = knockBackForces;
			other.GetComponent<Status>().Flinch(dir);
		}
		if(inflictStatus.Length > 0){
			for(int a = 0; a < inflictStatus.Length; a++){
				int ran = Random.Range(0,100);
				if(ran <= inflictStatus[a].chance){
					//Call Function ApplyAbnormalStat in Status Script
					other.GetComponent<Status>().ApplyAbnormalStat((int)inflictStatus[a].status , inflictStatus[a].duration);
				}
			}
		}
	}

	public void SpawnBulletChild(){
		if(alreadySpawn >= spawnChildWhenHit.maximumSpawn){
			return;
		}
		alreadySpawn++;

		Transform c = Instantiate(spawnChildWhenHit.prefab.transform , transform.position, transform.rotation) as Transform;
		c.GetComponent<BulletStatus>().Setting(playerAttack , playerAttack , shooterTag , shooter);
	}
}
