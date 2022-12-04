using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;

public class InventorySlot : MonoBehaviour
{
    [SerializeField]
    int id;
    public int ID { get { return id; } }
    Item item;
    [SerializeField] GameObject textObject;
    [SerializeField] GameObject imageObject;
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI text;
    Animator animator;
    int count = 0;
    bool isEmpty;
    public bool IsEmpty { get { return isEmpty; } }

    private void Start()
    {
        animator = GetComponent<Animator>();
        text = textObject.GetComponent<TextMeshProUGUI>();
        image = imageObject.GetComponent<Image>();
        SetEmpty();
    }

    public void DropItem()
    {
        //Instantiate(item);
        SetEmpty();
    }

    public void AddNewItem(Item item)
    {
        count = 1;
        id = item.itemID;
        image.sprite = item.Sprite;
        text.text = Convert.ToString(count);
        textObject.SetActive(true);
        imageObject.SetActive(true);
    }

    public void IncCount() => count++;

    private void SetEmpty()
    {
        textObject.SetActive(false);
        imageObject.SetActive(false);
        count = 0;
        id = -1;
        isEmpty = true;
        item = null;
    }

    public void HideInventorySlot() => animator.SetTrigger("HideSlot");

    public void ShowInventorySlot() => animator.SetTrigger("ShowSlot");
}
