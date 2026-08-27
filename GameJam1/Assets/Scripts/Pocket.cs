using NUnit.Framework.Interfaces;
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

    public event Action OnInventoryChanged;
    public InventorySlot[] Slots => slots;

    public bool AddItem(Items item)
    {
        foreach (var slot in slots)
        {
            if (slot.item == item)
            {
                slot.quantity++;
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        return false;
    }
}