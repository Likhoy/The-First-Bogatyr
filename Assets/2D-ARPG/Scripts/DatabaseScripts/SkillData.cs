using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillData : MonoBehaviour {
	public SkillSetting[] skill = new SkillSetting[3];

}

[System.Serializable]
public class SkillSetting{
	public string skillName = "";
	public Sprite icon;
	public BulletStatus skillPrefab;
	public string skillAnimationTrigger = "";
	public int manaCost = 10;
	public float castTime = 0.5f;
	public float skillDelay = 0.3f;
	public int coolDown = 1;
	[TextArea]
	public string description = "";
	public GameObject castEffect;
	public string sendMsg = "";//Send Message calling function when use this skill.
	public WhileAtk whileAttack = WhileAtk.Immobile;
	public BSpawnType skillSpawn = BSpawnType.FromPlayer;
	public AudioClip soundEffect;
	public bool requireWeapon = false;
	public int requireWeaponType = 0;
	public SkillAdditionHit[] multipleHit;
}

public enum BSpawnType{
	FromPlayer = 0,
	AtMouse = 1
}

[System.Serializable]
public class SkillAdditionHit{
	public BulletStatus skillPrefab;
	public string skillAnimationTrigger = "";
	public float castTime = 0.5f;
	public float skillDelay = 0.3f;
	public AudioClip soundEffect;
}