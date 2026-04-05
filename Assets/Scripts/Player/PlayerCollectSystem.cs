using UnityEngine;

public class PlayerCollectSystem : MonoBehaviour
{
    private WorldItem currentItem;

    void Update()
    {
        if (currentItem != null && Input.GetKeyDown(KeyCode.E))
        {
            PlayerInventory inventory = gameObject.GetComponent<PlayerInventory>();

            if (inventory != null)
            {
                currentItem.Collect(inventory);
                currentItem = null;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        WorldItem item = other.GetComponent<WorldItem>();

        if (item != null)
        {
            currentItem = item;
            Debug.Log("Tekan E untuk ambil: " + item.itemData.itemName);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        WorldItem item = other.GetComponent<WorldItem>();

        if (item != null && item == currentItem)
        {
            currentItem = null;
        }
    }
}
