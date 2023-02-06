using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProductAI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Transform childPriceText;

    private void Start()
    {
        childPriceText = this.transform.GetChild(0);
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        childPriceText.gameObject.SetActive(true);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData pointerEventData)
    {
        childPriceText.gameObject.SetActive(false);
    }


}
