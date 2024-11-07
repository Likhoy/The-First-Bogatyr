#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class SpawnPlayerInEditor{
	[MenuItem("2D ARPG Kit/Instantiate Example Player")]
	static void InstantiatePrefab(){
		Object prefab = AssetDatabase.LoadAssetAtPath("Assets/2D-ARPG/Prefab/Player/Knight-Player.prefab", typeof(GameObject));
		PrefabUtility.InstantiatePrefab(prefab as GameObject);
	}

	[MenuItem("2D ARPG Kit/Instantiate Example Monster")]
	static void InstantiatePrefabMon(){
		Object prefab = AssetDatabase.LoadAssetAtPath("Assets/2D-ARPG/Prefab/Monster/Goblin.prefab", typeof(GameObject));
		PrefabUtility.InstantiatePrefab(prefab as GameObject);
	}

	[MenuItem("2D ARPG Kit/Instantiate Example Ally")]
	static void InstantiatePrefabAlly(){
		Object prefab = AssetDatabase.LoadAssetAtPath("Assets/2D-ARPG/Prefab/AllyNPC/AllyArcher.prefab", typeof(GameObject));
		PrefabUtility.InstantiatePrefab(prefab as GameObject);
	}

	[MenuItem("2D ARPG Kit/Instantiate Player Spawnpoint")]
	static void InstantiatePrefabSp(){
		Object prefab = AssetDatabase.LoadAssetAtPath("Assets/2D-ARPG/Prefab/Player/PlayerSpawnPoint.prefab", typeof(GameObject));
		PrefabUtility.InstantiatePrefab(prefab as GameObject);
	}

	[MenuItem("2D ARPG Kit/Instantiate Teleporter")]
	static void InstantiateTeleporter(){
		Object prefab = AssetDatabase.LoadAssetAtPath("Assets/2D-ARPG/Prefab/Other/Teleporter.prefab", typeof(GameObject));
		GameObject t = (GameObject)PrefabUtility.InstantiatePrefab(prefab as GameObject);
		PrefabUtility.UnpackPrefabInstance(t.gameObject , PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
	}

	[MenuItem("2D ARPG Kit/Instantiate Example Merchant")]
	static void InstantiateSampleMerchant(){
		Object prefab = AssetDatabase.LoadAssetAtPath("Assets/2D-ARPG/Prefab/Other/Merchant.prefab", typeof(GameObject));
		GameObject t = (GameObject)PrefabUtility.InstantiatePrefab(prefab as GameObject);
		PrefabUtility.UnpackPrefabInstance(t.gameObject , PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
	}

	[MenuItem("2D ARPG Kit/Instantiate Example Crafter")]
	static void InstantiateSampleCrafter(){
		Object prefab = AssetDatabase.LoadAssetAtPath("Assets/2D-ARPG/Prefab/Other/Crafter.prefab", typeof(GameObject));
		GameObject t = (GameObject)PrefabUtility.InstantiatePrefab(prefab as GameObject);
		PrefabUtility.UnpackPrefabInstance(t.gameObject , PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
	}
}
#endif
