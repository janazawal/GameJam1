using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class InventorySlot
{
    public Items item;      
    public int quantity=0;       // starts at 0
}

public class Pocket : MonoBehaviour
{
    [SerializeField] private InventorySlot[] slots;
    [SerializeField] private InventorySlot extraSlot = new InventorySlot();

    public event Action OnInventoryChanged;
    public InventorySlot[] Slots => slots;
    public InventorySlot ExtraSlot => extraSlot;

    public bool AddItem(Items item)
    {
        if (item == null) return false;

        foreach (var slot in slots)
        {
            if (slot.item == item)
            {
                slot.quantity++;
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        // If it's an extra item, accumulate it into the single extra slot
        extraSlot.quantity++;
        OnInventoryChanged?.Invoke();
        return true;
    }
}