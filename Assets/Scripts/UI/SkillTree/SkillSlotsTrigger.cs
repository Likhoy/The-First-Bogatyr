using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlotsTrigger : MonoBehaviour
{
    public int[] activeSkills = new int[4];
    public Image[] skillUi = new Image[4];

    public Image draggingItemIcon;
    private bool onSkillSlotArea = false;
    private int onSkillSlot = -1;
    private int pickupSkillId = -1;
    private bool onSwapping = false;
    private int swapSlot = -1;

	public SkillTreeUi skillTree;
    private SkillData db;

    private void Start()
    {
        db = skillTree.database;
    }

    private void Update()
    {
        if (draggingItemIcon && draggingItemIcon.gameObject.activeSelf)
        {
            Vector2 dragIconPos = Input.mousePosition;
            dragIconPos.y -= 0.55f;
            draggingItemIcon.transform.position = dragIconPos;
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                SetSkill();
            }
        }
    }
    public void EnterSlotArea(int slot)
    {
        onSkillSlotArea = true;
        onSkillSlot = slot;
    }

    public void ExitSlotArea()
    {
        if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
        {
            onSkillSlotArea = false;
        }
    }

    public void PickupForShortcut(int skillId)
    {
        pickupSkillId = skillId;
        draggingItemIcon.sprite = db.skill[skillId].icon;
        draggingItemIcon.gameObject.SetActive(true);
    }

    public void PickupForSwap(int slot)
    {
        pickupSkillId = activeSkills[slot];
        draggingItemIcon.sprite = skillUi[slot].sprite;
        draggingItemIcon.gameObject.SetActive(true);
        swapSlot = slot;
        onSwapping = true;
    }

    public void SetSkill()
    {
        draggingItemIcon.gameObject.SetActive(false);

        if (!onSkillSlotArea)
        {
            if (onSwapping)
            {
                activeSkills[swapSlot] = -1;
                onSwapping = false;
                skillTree.AssignSkillToShortcut(onSkillSlot, -1);
                UpdateSkillSlots();
            }
            onSwapping = false;
            return;
        }

        if (onSwapping)
        {
            SwapSkills(onSkillSlot, swapSlot);
            onSwapping = false;
            UpdateSkillSlots();
            return;
        }

        activeSkills[onSkillSlot] = pickupSkillId;
        onSwapping = false;
        CheckSameSkill();
        skillTree.AssignSkillToShortcut(onSkillSlot, pickupSkillId);

        pickupSkillId = 0;
        UpdateSkillSlots();
    }

    private void SwapSkills(int onSkillSlot, int swapSlot)
    {
        int tmp = activeSkills[onSkillSlot];
        activeSkills[onSkillSlot] = activeSkills[swapSlot];
        activeSkills[swapSlot] = tmp;

        skillTree.AssignSkillToShortcut(onSkillSlot, activeSkills[onSkillSlot]);
        skillTree.AssignSkillToShortcut(swapSlot, activeSkills[swapSlot]);
    }

    private void UpdateSkillSlots()
    {
        for (int a = 0; a < skillUi.Length; a++)
        {
            if (activeSkills[a] == -1)
            {
                skillUi[a].gameObject.SetActive(false);
            }
            else
            {
                skillUi[a].gameObject.SetActive(true);
                skillUi[a].sprite = db.skill[activeSkills[a]].icon;
            }     
        }
    }

    private void CheckSameSkill()
    {
        int n = 0;
        while (n < activeSkills.Length)
        {
            if (activeSkills[n] == pickupSkillId && n != onSkillSlot)
            {
                activeSkills[n] = -1;
            }
            n++;
        }
    }
}
