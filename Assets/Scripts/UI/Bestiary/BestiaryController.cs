using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BestiaryController : MonoBehaviour
{
    public List<CreaturesGroup> creaturesGroups;
    public BestiaryUI uiManager;

    private void Start()
    {
        uiManager.CreateCreatureButtons(creaturesGroups);
    }

    public void DiscoverCreature(BestiaryCreatureInfoSO creature)
    {
        if (creature != null && !creature.isDiscovered)
        {
            creature.isDiscovered = true;

            string groupName = GetGroupNameForCreature(creature);
            uiManager.UpdateCreatureUI(creature, groupName);
        }
    }

    public void DiscoverCreature(string creatureName)
    {
        BestiaryCreatureInfoSO creature = GetCreatureByName(creatureName);
        if (creature != null && !creature.isDiscovered)
        {
            creature.isDiscovered = true;

            string groupName = GetGroupNameForCreature(creature);
            uiManager.UpdateCreatureUI(creature, groupName);
        }
    }

    private BestiaryCreatureInfoSO GetCreatureByName(string creatureName)
    {
        foreach (CreaturesGroup group in creaturesGroups)
        {
            foreach (BestiaryCreatureInfoSO creature in group.creatures)
            {
                if (creature.name.Equals(creatureName, StringComparison.OrdinalIgnoreCase))
                {
                    return creature;
                }
            }
        }
        return null; 
    }

    private string GetGroupNameForCreature(BestiaryCreatureInfoSO creature)
    {
        foreach (CreaturesGroup group in creaturesGroups)
        {
            if (group.creatures.Contains(creature))
            {
                return group.groupName; 
            }
        }
        return null;
    }
}
