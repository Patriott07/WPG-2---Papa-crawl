using UnityEngine;

public class PlayerCollectSystem : MonoBehaviour
{
    private WorldItem currentItem;

    void Update()
    {
        if (currentItem != null && Input.GetKeyDown(KeyCode.E))
        {
            // Play sound
            PlayerSounds.Instance.collect2.Play();
            PlayerInventory inventory = gameObject.GetComponent<PlayerInventory>();

            if (inventory != null)
            {
                currentItem.Collect(inventory);
                ShowTextCollectingItem(currentItem.name);
                currentItem = null;
            }
        }
    }

    void ShowTextCollectingItem(string itemName)
    {
          GameObject text = ObjectPollingGame.Instance.GetUITextFloat();
        // set color, text, pos
        text.transform.position = new Vector2(transform.position.x + Random.Range(-0.4f, 0.4f), transform.position.y + 0.6f);
        TextUIFloatingDamage scriptText = text.GetComponent<TextUIFloatingDamage>();
        scriptText.colorText = Color.white;
        scriptText.textInput = $"+1 {itemName}";
       
        // scriptText.textTMP.rectTransform.sizeDelta = new Vector2(200, 65);
        // scriptText.textTMP.fontStyle = TMPro.FontStyles.Bold;
        scriptText.dDestroy = 2.3f;
        text.SetActive(true);
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
