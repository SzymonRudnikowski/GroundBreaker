using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for all items which can be added into inventory
/// </summary>
public interface IInventoryItem : ITradeable {

    /// <summary>
    /// Icon to display in the inventory
    /// </summary>
    Sprite InventoryIcon { get; set; }

    /// <summary>
    /// Indicates whether item can be stacked
    /// This means that one inventory slot can hold more than one item
    /// </summary>
    bool Stackable { get; set; }

    /// <summary>
    /// How many items can be stacked
    /// </summary>
    int StackSize { get; set; }
}
