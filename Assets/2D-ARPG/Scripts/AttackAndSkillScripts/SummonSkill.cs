using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BulletStatus))]

public class SummonSkill : MonoBehaviour {
	public AllyAi summonPrefab;

	[Tooltip("Summon can get stronger by player's level.")]
	public bool autoSetLvStatus = false; //Summon can get stronger by player's level.
	private int currentLv = 1;
	public int maxLevel = 100;
	
	public StatusParam minStatus;
	public StatusParam maxStatus;
	
	private int min = 0;
	private int max = 0;
	
	void Start(){
		GameObject source = GetComponent<BulletStatus>().shooter;
		if(source.GetComponent<AttackTrigger>().minion){
			source.GetComponent<AttackTrigger>().minion.GetComponent<Status>().Death();
		}
		Vector3 spawnPoint = transform.position;
		spawnPoint.x += Random.Range(-3 , 3);
		spawnPoint.y += Random.Range(1 , 1.5f);
		transform.position = spawnPoint;

		Transform mn = Instantiate(summonPrefab.transform , transform.position , source.transform.rotation) as Transform;
		if(mn.GetComponent<AllyAi>()){
			mn.GetComponent<AllyAi>().master = source.transform;
			mn.GetComponent<AllyAi>().deadIfNoMaster = true;
		}
		Physics2D.IgnoreCollision(source.GetComponent<Collider2D>(), mn.GetComponent<Collider2D>());

		source.GetComponent<AttackTrigger>().minion = mn;
		if(autoSetLvStatus){
			currentLv = source.GetComponent<Status>().level;
			CalculateStatLv(mn);
		}
		Destroy(gameObject , 0.1f);
	}
	
	public void CalculateStatLv(Transform mn){
		Status stat = mn.GetComponent<Status>();
		//[min_stat*(max_lv-lv)/(max_lv- 1)] + [max_stat*(lv- 1)/(max_lv- 1)]
		
		//Atk
		min = minStatus.atk * (maxLevel - currentLv)/(maxLevel - 1);
		max = maxStatus.atk * (currentLv - 1)/(maxLevel - 1);
		stat.atk = min + max;
		//Def
		min = minStatus.def * (maxLevel - currentLv)/(maxLevel - 1);
		max = maxStatus.def * (currentLv - 1)/(maxLevel - 1);
		stat.def = min + max;
		//Matk
		min = minStatus.matk * (maxLevel - currentLv)/(maxLevel - 1);
		max = maxStatus.matk * (currentLv - 1)/(maxLevel - 1);
		stat.matk = min + max;
		//Mdef
		min = minStatus.mdef * (maxLevel - currentLv)/(maxLevel - 1);
		max = maxStatus.mdef * (currentLv - 1)/(maxLevel - 1);
		stat.mdef = min + max;
		
		//HP
		min = minStatus.maxHealth * (maxLevel - currentLv)/(maxLevel - 1);
		max = maxStatus.maxHealth * (currentLv - 1)/(maxLevel - 1);
		stat.maxHealth = min + max;
		stat.health = stat.maxHealth;
		//MP
		min = minStatus.maxMana * (maxLevel - currentLv)/(maxLevel - 1);
		max = maxStatus.maxMana * (currentLv - 1)/(maxLevel - 1);
		stat.maxMana = min + max;
		stat.mana = stat.maxMana;
	}
}

[System.Serializable]
public class StatusParam{
	public int atk = 5;
	public int def = 5;
	public int matk = 5;
	public int mdef = 5;
	public int maxHealth = 100;
	public int maxMana = 100;
}
