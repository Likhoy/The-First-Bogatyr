using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer2D : MonoBehaviour {
	public Transform player;
	public Vector3 offset = new Vector3(0 , 0 , -10);

	[HideInInspector]
	public float shakeValue = 0.0f;
	[HideInInspector]
	public bool onShaking = false;
	private float shakingv = 0.0f;

	void Start(){
		DontDestroyOnLoad(transform.gameObject);
		if(!player){
			player = GameObject.FindWithTag("Player").transform;
		}
	}
	
	void Update(){
		if(onShaking && GlobalStatus.freezeCam){
			shakeValue = Random.Range(-shakingv , shakingv)* 0.2f;
			transform.position += new Vector3(0,shakeValue,0);
		}
		if(!player || GlobalStatus.freezeCam){
			return;
		}
		
		if(Time.timeScale == 0.0f){
			return;
		}
		transform.position = new Vector3(player.position.x + offset.x, player.position.y + offset.y, offset.z);
		if(onShaking){
			shakeValue = Random.Range(-shakingv , shakingv)* 0.2f;
			transform.position += new Vector3(0 , shakeValue , 0);
		}
	}

	public void Shake(float val , float dur){
		if(onShaking){
			return;
		}
		shakingv = val;
		StartCoroutine(Shaking(dur));
	}
	
	public IEnumerator Shaking(float dur){
		onShaking = true;
		yield return new WaitForSeconds(dur);
		shakingv = 0;
		shakeValue = 0;
		onShaking = false;
	}
	
	public void SetNewTarget(Transform p){
		player = p;
	}
	
	void OnEnable(){
		shakingv = 0;
		shakeValue = 0;
		onShaking = false;
	}
}
