using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    int id;
    [SerializeField]
    int slot_number;
    public int ID { get { return id; } }
    Item item;
    [SerializeField] GameObject textObject;
    [SerializeField] GameObject imageObject;
    [SerializeField] UnityEngine.UI.Image image;
    [SerializeField] TextMeshProUGUI text;
    Animator animator;
    int count = 0;
    bool isEmpty;
    public bool IsEmpty { get { return isEmpty; } }

    //Костыль (исправить)

    private void Start()
    {
        id = -1;
        animator = GetComponent<Animator>();
        text = textObject.GetComponent<TextMeshProUGUI>();
        image = imageObject.GetComponent<UnityEngine.UI.Image>();
        SetEmpty();
    }

    private void Update()
    {
        if (Input.GetKeyDown((KeyCode)(slot_number + 48)))
            if (!isEmpty)
                UseItem();
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (!isEmpty)
            UseItem();
    }

    private void UseItem()
    {
        item.UseItem();
        count--;
        text.text = count.ToString();
        if (count == 0)
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
        isEmpty = false;
        id = item.itemID;
        textObject.SetActive(true);
        imageObject.SetActive(true);
        image.sprite = item.sprite;
        text.text = Convert.ToString(count);
        //Костыль (исправить)
        item.isTaken = true;
        this.item = item;
    }

    public void IncCount()
    {
        count++;
        text.text = count.ToString();
    }

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
