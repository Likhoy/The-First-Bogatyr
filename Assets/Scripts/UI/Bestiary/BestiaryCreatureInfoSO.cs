using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BestiaryCreatureInfoSO : ScriptableObject
{
    public string name;
    public string description;
    public Sprite icon;
    public Sprite picture;

    public bool isDiscovered;
}


