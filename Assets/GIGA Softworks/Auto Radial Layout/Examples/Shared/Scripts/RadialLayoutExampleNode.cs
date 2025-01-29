using GIGA.AutoRadialLayout.QuerySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GIGA.AutoRadialLayout.Examples
{
    [ExecuteInEditMode]
    public class RadialLayoutExampleNode : MonoBehaviour
    {
		private void OnEnable()
		{
			if(this.GetComponent<RadialLayoutQueryTarget>() != null)
				this.transform.Find("Number").GetComponent<Text>().text = this.GetComponent<RadialLayoutQueryTarget>().UniqueId.ToString();
		}
	}
}
