using UnityEngine;
using UnityEngine.EventSystems;
public class DragHandlerSlotInventory : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static GameObject itemBeingDragged;
    Vector3 startPosition;
    Transform startParent;
    CanvasGroup canvasGroup;

    public ItemData itemData; // Isi saat slot diupdate

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        itemBeingDragged = gameObject;
        startPosition = transform.position;
        startParent = transform.parent;

        // Agar ikon bisa melewati UI lain tanpa terhalang
        transform.SetParent(transform.root);
        canvasGroup.blocksRaycasts = false; // Supaya mouse bisa mendeteksi objek di bawahnya (Slot tujuan)
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        itemBeingDragged = null;
        canvasGroup.blocksRaycasts = true;

        if (transform.parent == transform.root)
        {
            transform.position = startPosition;
            transform.SetParent(startParent);
        }
    }
}
