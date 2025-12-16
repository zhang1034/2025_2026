using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class InventoryManager : MonoBehaviour
{
    
    public static InventoryManager Instance;

    public List<InventorySlot> items = new List<InventorySlot>();

    public GameObject inventoryUI;
    public TMP_Text itemListText;
    public AudioSource audioSource;       
    public AudioClip openClip;
    public AudioClip closeClip;
    private bool isOpen = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    public void AddItem(ItemData item)
    {
        if (item == null)
        {
            Debug.LogError("Tried to add NULL item!");
            return;
        }

        InventorySlot slot = items.Find(i => i.item == item);

        if (slot != null)
        {
            slot.amount++;
        }
        else
        {
            items.Add(new InventorySlot(item, 1));
        }

        UpdateUI();
    }

    void ToggleInventory()
    {
        isOpen = !isOpen;
        inventoryUI.SetActive(isOpen);

        if (audioSource != null)
        {
            audioSource.PlayOneShot(isOpen ? openClip : closeClip);
        }

        inventoryUI.SetActive(isOpen);;
        
    }

    void UpdateUI()
    {
        itemListText.text = "";

        foreach (var slot in items)
        {
            itemListText.text +=
                $"{slot.item.itemName} x{slot.amount} ({slot.item.itemType})\n";
        }
    }
}