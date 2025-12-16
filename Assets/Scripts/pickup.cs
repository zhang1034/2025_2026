using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public ItemData itemData;

    public void Pickup()
    {
        InventoryManager.Instance.AddItem(itemData);
        Destroy(gameObject);
        if (GetComponent<GhostFloatWander>() != null) GetComponent<GhostFloatWander>().enabled = false;
        gameObject.SetActive(false);
    }
}
