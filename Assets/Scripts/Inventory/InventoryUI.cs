using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component connecting Inventory object with UI
/// </summary>
public class InventoryUI : MonoBehaviour
{
    /// <summary>
    /// Parent panel of all slots
    /// </summary>
    public Transform slotsParent;

    /// <summary>
    /// Cached inventory instance
    /// </summary>
    Inventory inventory;

    /// <summary>
    /// List of all slots in the UI
    /// </summary>
    InventorySlot[] slots;

    void Start()
    {
        inventory = Inventory.instance;
        inventory.OnInventoryChanged += UpdateUI;

        slots = transform.GetComponentsInChildren<InventorySlot>();
    }

    /// <summary>
    /// Called when inventory changes
    /// </summary>
    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            var slot = slots[i];
            if (i < inventory.items.Count)
            {
                var item = inventory.items[i];
                if (item.Stackable)
                {
                    var count = inventory.itemsCount[i];
                    slot.AddItem(item, count);
                }
                else
                {
                    slot.AddItem(item);
                }
            }
            else
            {
                slot.ClearSlot();
            }
        }

    }
}
