using UnityEngine;

public class AutoHideSprite : MonoBehaviour
{
    SpriteRenderer spR;
    void Start()
    {
        spR = gameObject.GetComponent<SpriteRenderer>();
        spR.color = new Color32(255, 255, 255, 0);
    }
}
