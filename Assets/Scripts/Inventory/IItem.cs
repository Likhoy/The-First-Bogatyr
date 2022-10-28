using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItem
{
    [SerializeField]
    int id { get; }

    void TakeItem();
    void UseItem();
    void DropItem();
}
