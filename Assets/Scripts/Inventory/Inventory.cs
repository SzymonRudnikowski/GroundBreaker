using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Component encapsulating inventory logic. Only one instance per game (on Player).
/// </summary>
public class Inventory : MonoBehaviour
{

    #region Singleton

    public static Inventory instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one inventory instance");
            return;
        }
        instance = this;
    }

    #endregion

    public delegate void InventoryChangedHandler();

    /// <summary>
    /// Called when inventory changes, eg. item is added removed consumed
    /// </summary>
    public InventoryChangedHandler OnInventoryChanged;

    /// <summary>
    /// List of all items in inventory
    /// </summary>
    public List<IInventoryItem> items = new List<IInventoryItem>();

    /// <summary>
    /// List containing information about how many items there are in the corresponding slot in items
    /// (some items may be stackable)
    /// </summary>
    public List<int> itemsCount = new List<int>();

    /// <summary>
    /// Inventory capacity
    /// </summary>
    /// <remarks>IF ALTERED UI NEED TO BE UPDATED, CURRENTLY UI WORKS FOR CAP = 8</remarks>
    public const int capacity = 8;

    /// <summary>
    /// Adds item into the inventory and returns whether it was successfull
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Add(IInventoryItem item)
    {
        if (item == null)
        {
            Debug.LogWarning("Trying to add null into inventory!");
            return false;
        }

        // No way to add not stackable item into the inventory
        if (items.Count >= capacity && !item.Stackable)
        {
            Debug.Log("Not enough space!");
            return false;
        }

        if (item.Stackable)
        {
            // For stackable items first check if they can be added into stack
            for (int i = 0; i < items.Count; i++)
            {
                var inventoryItem = items[i];
                var itemCount = itemsCount[i];
                if (inventoryItem == item && itemCount < item.StackSize)
                {
                    itemsCount[i]++;
                    if (OnInventoryChanged != null)
                    {
                        OnInventoryChanged();
                    }
                    return true;
                }
            }
        }

        // No stack found or item is not stackable - add as new element if theres space
        if (items.Count < capacity)
        {
            items.Add(item);
            itemsCount.Add(1);
            if (OnInventoryChanged != null)
            {
                OnInventoryChanged();
            }
            return true;
        }

        return false;
    }

    /// <summary>
    /// Removes item from the inventory. 
    /// If stackable decreases number of items or deletes if only one left.
    /// </summary>
    /// <param name="item"></param>
    public void Remove(IInventoryItem item)
    {
        if (item == null)
        {
            Debug.LogWarning("Trying to remove null from inventory!");
            return;
        }

        if (item.Stackable)
        {
            // Again for stackable items try to remove from the LAST stack
            for (int i = items.Count - 1; i >= 0; i--)
            {
                var inventoryItem = items[i];
                var itemCount = itemsCount[i];
                if (inventoryItem == item && itemCount > 1)
                {
                    itemsCount[i]--;
                    if (OnInventoryChanged != null)
                    {
                        OnInventoryChanged();
                    }
                    return;
                }
            }
        }

        // Stack not found or item not stackable - just remove 
        int idx = items.LastIndexOf(item);
        if (idx >= 0 && idx < capacity)
        {
            items.RemoveAt(idx);
            itemsCount.RemoveAt(idx);

            if (OnInventoryChanged != null)
            {
                OnInventoryChanged();
            }
        }
    }
}