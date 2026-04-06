using UnityEngine;
using TMPro;
using System.Collections;
public class TextUIFloatingDamage : MonoBehaviour
{
    public TMP_Text textTMP;
    public Color colorText;
    public string textInput;
    public Animator animator;
    public float dDestroy = 0.2f;
    

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }
    public void OnEnable()
    {
        textTMP.fontStyle = FontStyles.Normal;
        textTMP.rectTransform.sizeDelta = new Vector2(200, 50);
        textTMP.color = colorText;
        textTMP.text = textInput;
        animator.Play("StartFade", 0, 0f);
        StartCoroutine(DestroyInSec());
    }

    IEnumerator DestroyInSec()
    {
        yield return new WaitForSeconds(dDestroy);
        dDestroy = 0.2f;
        gameObject.SetActive(false);
    }



}
