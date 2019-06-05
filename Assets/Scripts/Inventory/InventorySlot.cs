using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Component for UI logic of inventory item slot
/// </summary>
public class InventorySlot : MonoBehaviour
{
    /// <summary>
    /// Stackable item count (visible if item is stackable)
    /// </summary>
    public Text countText;

    /// <summary>
    /// Item icon image
    /// </summary>
    public Image icon; 
    
    public Button removeButton; 

    /// <summary>
    /// Item currently sitting in the slot
    /// </summary>
    IInventoryItem item;  

    /// <summary>
    /// Updates slot with stackable item of given count
    /// </summary>
    /// <param name="newItem"></param>
    /// <param name="count"></param>
    public void AddItem(IInventoryItem newItem, int count)
    {
        item = newItem;

        icon.sprite = item.InventoryIcon;
        icon.enabled = true;

        removeButton.interactable = true;

        countText.text = count.ToString();
        countText.enabled = true;        
    }

    /// <summary>
    /// Updates slot with not-stackable item
    /// </summary>
    /// <param name="newItem"></param>
    public void AddItem(IInventoryItem newItem)
    {
        item = newItem;
       
        icon.sprite = item.InventoryIcon;
        icon.enabled = true;
        
        removeButton.interactable = true;

        countText.enabled = false;
    }

    /// <summary>
    /// Removes item from the slot
    /// </summary>
    public void ClearSlot()
    {
        item = null;

        icon.sprite = null;
        icon.enabled = false;

        removeButton.interactable = false;

        countText.text = null;
        countText.enabled = false;
    }

    /// <summary>
    /// Called when remove button is pressed
    /// </summary>
    public void OnRemoveButton()
    {
        Inventory.instance.Remove(item);
    }

   

}