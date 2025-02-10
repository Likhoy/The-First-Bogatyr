using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour {
	public Animator mainSprite;
	public Transform deathPrefab;

	public string characterName = "";
	public int characterId = 0;
	public int level = 1;
	public int atk = 0;
	public int def = 0;
	public int matk = 0;
	public int mdef = 0;
	public int exp = 0;
	public int maxExp = 100;
	public int maxHealth = 100;
	public int health = 100;
	public int maxMana = 100;
	public int mana = 100;
	public int statusPoint = 0;
    public int divinityPoint = 0;
    public int skillPoint = 0;
    public int darknessPoint = 0;
    private bool dead = false;
	public bool immortal = false;
	public bool stability = false;
	public Transform levelUpEffect;

	//Effect
	public GameObject poisonEffect;
	public GameObject silenceEffect;
	public GameObject stunEffect;
	public GameObject frozenEffect;

	[HideInInspector]
	public MainStatus additionStat; //For Equipment Bonus
	[HideInInspector]
	public MainStatus buffsStat; //For Buffs Bonus
	[HideInInspector]
	public MainStatus totalStat;
	[HideInInspector]
	public HiddenStat hiddenStatus;

	[HideInInspector]
	public string spawnPointName = ""; //Store the name for Spawn Point When Change Scene
	//Negative Buffs
	[HideInInspector]
	public bool poison = false;
	[HideInInspector]
	public bool silence = false;
	[HideInInspector]
	public bool stun = false;
	[HideInInspector]
	public bool frozen = false;

	public bool freeze = false; // Use for Freeze Character
	[HideInInspector]
	public bool dodge = false;
	[HideInInspector]
	public bool flinch = false;

	//Positive Buffs
	[HideInInspector]
	public bool brave = false; //Can be use for Weaken
	[HideInInspector]
	public bool barrier = false;
	[HideInInspector]
	public bool mbarrier = false;
	[HideInInspector]
	public bool faith = false; //Can be use for Clumsy
	[HideInInspector]
	public bool block = false;

	public ElementalStat elementalEffective;
	public Resist statusResist;

	[HideInInspector]
	public Resist eqResist;
	[HideInInspector]
	public Resist totalResist;
	[HideInInspector]
	public ElementalResist eqElemental;
	[HideInInspector]
	public int[] totalElemental = new int[9];

	public string sendMsgWhenDead = "";
	[HideInInspector]
	public bool canControl = true;

	public AudioClip hurtVoice;
	private GameObject showHpBar;

	void Start(){
		if(characterName != ""){
			gameObject.name = characterName;
		}
		if(!mainSprite && GetComponent<Animator>()){
			mainSprite = GetComponent<Animator>();
		}
		CalculateStatus();
		if(GetComponentInChildren<MonsterHealthBar>()){
			showHpBar = GetComponentInChildren<MonsterHealthBar>().gameObject;
			showHpBar.SetActive(false);
		}

	}

	public void CalculateStatus(){
		totalStat.atk = atk + additionStat.atk + buffsStat.atk;
		totalStat.def = def + additionStat.def + buffsStat.def;
		totalStat.matk = matk + additionStat.matk + buffsStat.matk;
		totalStat.mdef = mdef + additionStat.mdef + buffsStat.mdef;

		totalStat.health = maxHealth + additionStat.health + buffsStat.health;
		totalStat.mana = maxMana + additionStat.mana + buffsStat.mana;
		//addMdef += mdef;
		if(health >= totalStat.health){
			health = totalStat.health;
		}
		
		if(mana >= totalStat.mana){
			mana = totalStat.mana;
		}
		totalResist.poisonResist = statusResist.poisonResist + eqResist.poisonResist;
		totalResist.silenceResist = statusResist.silenceResist + eqResist.silenceResist;
		totalResist.stunResist = statusResist.stunResist + eqResist.stunResist;
		totalResist.frozenResist = statusResist.frozenResist + eqResist.frozenResist;
		
		totalElemental[0] = elementalEffective.normal - eqElemental.normal;
		totalElemental[1] = elementalEffective.fire - eqElemental.fire;
		totalElemental[2] = elementalEffective.ice - eqElemental.ice;
		totalElemental[3] = elementalEffective.thunder - eqElemental.thunder;
		totalElemental[4] = elementalEffective.earth - eqElemental.earth;
		totalElemental[5] = elementalEffective.poison - eqElemental.poison;
		totalElemental[6] = elementalEffective.wind - eqElemental.wind;
		totalElemental[7] = elementalEffective.holy - eqElemental.holy;
		totalElemental[8] = elementalEffective.darkness - eqElemental.darkness;
	}

	public string OnDamage(int amount , int element , bool isMagic){	
		if(!dead){
			if(GlobalStatus.freezeAll){
				return "";
			}
			if(dodge){
				return "Evaded";
			}
			if(immortal){
				return "Invulnerable";
			}
			if(block){
				return "Guard";
			}
			if(hiddenStatus.autoGuard > 0){
				int ran = Random.Range(0 , 100);
				if(ran <= hiddenStatus.autoGuard){
					return "Guard";
				}
			}
			if(!isMagic){
				amount -= totalStat.def;
			}else{
				amount -= totalStat.mdef;
			}

			//Calculate Element Effective
			amount *= totalElemental[element];
			amount /= 100;
			
			if(amount < 1){
				amount = 1;
			}
			health -= amount;
			if(hurtVoice && GetComponent<AudioSource>()){
				GetComponent<AudioSource>().PlayOneShot(hurtVoice);
			}
			if(showHpBar){
				showHpBar.SetActive(true);
			}
			if(health <= 0){
				health = 0;
				Death();
			}
		}
		return amount.ToString();
	}

	public void GainEXP(int gain){
		exp += gain;
		if(exp >= maxExp){
			int remain = exp - maxExp;
			LevelUp(remain);
		}
	}
	
	public void LevelUp(int remainingEXP){
		exp = 0;
		exp += remainingEXP;
		level++;
		statusPoint += 5;
		skillPoint++;
		//Extend the Max EXP, Max Health and Max Mana
		maxExp = 125 * maxExp  / 100;
		maxHealth += 20;
		maxMana += 10;
		//Recover Health and Mana
		CalculateStatus();
		health = totalStat.health;
		mana = totalStat.mana;
		GainEXP(0);
		if(levelUpEffect){
			Instantiate(levelUpEffect , transform.position , Quaternion.identity);
		}
		if(GetComponent<UiMaster>()){
			GetComponent<UiMaster>().ShowLevelUpWarning();
		}
	}

	public void Heal(int hp , int mp){
		health += hp;
		if(health >= totalStat.health){
			health = totalStat.health;
		}
		
		mana += mp;
		if(mana >= totalStat.mana){
			mana = totalStat.mana;
		}
	}

	public void Death(){
		if(dead){
			return;
		}
		dead = true;
		if(sendMsgWhenDead != ""){
			SendMessage(sendMsgWhenDead , SendMessageOptions.DontRequireReceiver);
		}
		if(gameObject.tag == "Player"){
            if(GetComponent<UiMaster>() && GetComponent<UiMaster>().eventSystem) {
				Destroy(GetComponent<UiMaster>().eventSystem.gameObject);
            }
			GlobalStatus.SavePlayerStatus(this.gameObject);
		}
		Destroy(gameObject);
		if(deathPrefab){
			Instantiate(deathPrefab, transform.position , transform.rotation);
		}else{
			print("This Object didn't assign the Death Body");
		}
	}

	//----------Abnormal Status--------
	public void ApplyAbnormalStat(int statId , float dur){
		if(GlobalStatus.freezePlayer){
			return;
		}
		if(statId == 0){
			StartCoroutine(OnPoison(Mathf.FloorToInt(dur)));
		}
		if(statId == 1){
			StartCoroutine(OnSilence(dur));
		}
		if(statId == 2){
			StartCoroutine(OnStun(dur));
		}
		if(statId == 3){
			StartCoroutine(OnFrozen(dur));
		}
	}

	private GameObject poisonEff;
	public IEnumerator OnPoison(int hurtTime){
		int amount = 0;
		if(poison){
			yield break;
		}
		int chance = 100 - totalResist.poisonResist;
		if(chance > 0){
			int per = Random.Range(0, 100);
			if(per <= chance){
				poison = true;
				amount = totalStat.health * 3 / 100; // Hurt 3% of Max HP
				if(poisonEffect){ //Show Poison Effect
					poisonEff = Instantiate(poisonEffect, transform.position, poisonEffect.transform.rotation) as GameObject;
					poisonEff.transform.parent = transform;
				}
			}
		}
		//--------------------
		while(poison && hurtTime > 0){
			yield return new WaitForSeconds(1); // Reduce HP  Every 0.7f Seconds
			health -= amount;
			
			if(health <= 1){
				health = 1;
			}
			hurtTime--;
			if(hurtTime <= 0){
				poison = false;
				if(poisonEff){ //Destroy Effect if it still on a map
					Destroy(poisonEff.gameObject);
				}
			}
		}
	}
	
	private GameObject silenceEff;
	public IEnumerator OnSilence(float dur){
		if(silence){
			yield break;
		}
		int chance = 100 - totalResist.silenceResist;
		if(chance > 0){
			int per = Random.Range(0, 100);
			if(per <= chance){
				silence = true;
				if(silenceEffect){
					silenceEff = Instantiate(silenceEffect, transform.position, transform.rotation) as GameObject;
					silenceEff.transform.parent = transform;
				}
				yield return new WaitForSeconds(dur);
				if(silenceEff){ //Destroy Effect if it still on a map
					Destroy(silenceEff.gameObject);
				}
				silence = false;
			}
		}
	}

	private GameObject frozenEff;
	public IEnumerator OnFrozen(float dur){
		if(frozen){
			yield break;
		}
		int chance = 100 - totalResist.frozenResist;
		if(chance > 0){
			int per = Random.Range(0, 100);
			if(per <= chance){
				frozen = true;
				freeze = true; // Freeze Character On (Character cannot do anything)
				if(frozenEffect){
					frozenEff = Instantiate(frozenEffect, transform.position, transform.rotation) as GameObject;
					frozenEff.transform.parent = transform;
				}
				if(mainSprite){
					mainSprite.GetComponent<Animator>().enabled = false;
				}

				yield return new WaitForSeconds(dur);
				if(frozenEff){ //Destroy Effect if it still on a map
					Destroy(frozenEff.gameObject);
				}
				if(mainSprite){
					mainSprite.GetComponent<Animator>().enabled = true;
				}
				freeze = false; // Freeze Character Off
				frozen = false;
			}
		}
	}

	private GameObject stunEff;
	public IEnumerator OnStun(float dur){
		if(!stun){
			int chance= 100;
			chance -= totalResist.stunResist;
			if(chance > 0){
				int per= Random.Range(0, 100);
				if(per <= chance){
					stun = true;
					freeze = true; // Freeze Character On (Character cannot do anything)
					if(stunEffect){
						stunEff = Instantiate(stunEffect, transform.position, stunEffect.transform.rotation) as GameObject;
						stunEff.transform.parent = transform;
					}
					/*if(mainSprite){
						mainSprite.SetTrigger("stunOn");
					}*/

					yield return new WaitForSeconds(dur);
					if(stunEff){ //Destroy Effect if it still on a map
						Destroy(stunEff.gameObject);
					}
					/*if(mainSprite){
						mainSprite.SetTrigger("stunOff");
					}*/

					freeze = false; // Freeze Character Off
					stun = false;
				}
			}
		}
	}

	//Buffs
	public void ApplyBuff(int statId , float dur , int amount){
		if(statId == 1){
			StartCoroutine(OnBarrier(amount , dur));
		}
		if(statId == 2){
			StartCoroutine(OnMagicBarrier(amount , dur));
		}
		if(statId == 3){
			StartCoroutine(OnBrave(amount , dur));
		}
		if(statId == 4){
			StartCoroutine(OnFaith(amount , dur));
		}
	}

	public IEnumerator OnBarrier (int amount , float dur){
		//Increase Defense
		if(!barrier){
			barrier = true;
			buffsStat.def = amount;
			CalculateStatus();
			yield return new WaitForSeconds(dur);
			buffsStat.def = 0;
			barrier = false;
			CalculateStatus();
		}
	}
	
	public IEnumerator OnMagicBarrier(int amount , float dur){
		//Increase Magic Defense
		if(!mbarrier){
			mbarrier = true;
			buffsStat.mdef = amount;
			CalculateStatus();
			yield return new WaitForSeconds(dur);
			buffsStat.mdef = 0;
			mbarrier = false;
			CalculateStatus();
		}
	}
	
	public IEnumerator OnBrave(int amount , float dur){
		//Increase Attack
		if(!brave){
			brave = true;
			buffsStat.atk = amount;
			CalculateStatus();
			yield return new WaitForSeconds(dur);
			buffsStat.atk = 0;
			brave = false;
			CalculateStatus();
		}
	}
	
	public IEnumerator OnFaith (int amount , float dur){
		//Increase Magic Attack
		if(!faith){
			faith = true;
			buffsStat.matk = amount;
			CalculateStatus();
			yield return new WaitForSeconds(dur);
			buffsStat.matk = 0;
			faith = false;
			CalculateStatus();
		}
	}
	
	public void GuardUp(string anim){
		if(block){
			return;
		}
		block = true;
		if(anim != ""){
			mainSprite.SetTrigger(anim);
		}
	}
	
	public void GuardBreak(string anim){
		block = false;
		if(anim != ""){
			mainSprite.SetTrigger(anim);
		}
	}

	[HideInInspector]
	public Vector3 knock = Vector3.zero;
	[HideInInspector]
	public float knockForce = 700;

	public void Flinch(Vector3 dir){
		if(GlobalStatus.freezePlayer || stability){
			return;
		}
		knock = dir;
		//canControl = false;
		StartCoroutine(KnockBack());
		if(mainSprite && !block){
			mainSprite.SetTrigger("hurt");
		}
		//canControl = true;
	}
	
	IEnumerator KnockBack(){
		flinch = true;
		yield return new WaitForSeconds(0.2f);
		flinch = false;
	}
}

[System.Serializable]
public class Resist{
	public int poisonResist = 0;
	public int silenceResist = 0;
	public int stunResist = 0;
	public int frozenResist = 0;
}

[System.Serializable]
public class HiddenStat{
	public bool doubleJump = false;
	public int drainTouch = 0;
	public int autoGuard = 0;
	public int mpReduce = 0;
}

[System.Serializable]
public class ElementalStat{
	public int normal = 100;
	public int fire = 100;
	public int ice = 100;
	public int thunder = 100;
	public int earth = 100;
	public int poison = 100;
	public int wind = 100;
	public int holy = 100;
	public int darkness = 100;
}

[System.Serializable]
public class ElementalResist{
	public int normal = 0;
	public int fire = 0;
	public int ice = 0;
	public int thunder = 0;
	public int earth = 0;
	public int poison = 0;
	public int wind = 0;
	public int holy = 0;
	public int darkness = 0;
}

public enum ElementalAtk{
	Normal = 0,
	Fire = 1,
	Ice = 2,
	Thunder = 3,
	Earth = 4,
	Poison = 5,
	Wind = 6,
	Holy = 7,
	Darkness = 8
}

[System.Serializable]
public class MainStatus{
	public int atk = 0;
	public int def = 0;
	public int matk = 0;
	public int mdef = 0;
	public int health = 0;
	public int mana = 0;
}
