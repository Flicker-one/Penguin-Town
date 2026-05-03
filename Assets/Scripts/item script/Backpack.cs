using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backpack : Item
{
    public int deltaSlotCount = 4;

    public override void useItem()
    {
        ItemController.Instance.ModifySlotCount(deltaSlotCount);
    }
}
