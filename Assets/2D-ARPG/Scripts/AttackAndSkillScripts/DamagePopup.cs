using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamagePopup : MonoBehaviour {
	public Text textSource;
	Vector3 targetScreenPosition;
	public string damage = "";
	public Color fontColor = Color.white;
	
	public float duration = 0.5f;
	private float wait = 0;
	
	[HideInInspector]
	public bool critical = false;
	
	public GameObject criticalImage;
	public Vector3 velocity = new Vector3(1 , 5 , 0);
	public bool normalFloat = false;

	void Start(){
		transform.rotation = Quaternion.identity;
		SetText();
		Destroy(gameObject, duration);

		if(!normalFloat){
			textSource.fontSize += 10;
		}
	}

	public void SetText(){
		if(!textSource){
			return;
		}
		textSource.text = damage;
		textSource.color = fontColor;
		if(critical && criticalImage){
			criticalImage.SetActive(true);
		}
	}

	void Update(){
		if(normalFloat){
			transform.Translate(Vector3.up * Time.deltaTime);
			return;
		}
		transform.Translate(velocity * Time.deltaTime);
		if(wait >= 0.05f){
			velocity.y -= 1;
			textSource.fontSize -= 1;
			wait = 0;
		}else{
			wait += Time.deltaTime;
		}
	}
}
