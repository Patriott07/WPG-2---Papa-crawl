using UnityEngine;
using DG.Tweening;
using System.Collections;
using data.structs;
using System;
public class LoadingUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] CanvasGroup canvasGroupLoading;

    public static LoadingUI Instance;
    void Start()
    {
        Instance = this;
        // canvasGroupLoading = gameObject.GetComponent<CanvasGroup>();
        // GameState gameState = 
        GameEvents.LevelStart?.Invoke(0f);
        StartCoroutine(WaitForXBeforeFade());
    }

    IEnumerator WaitForXBeforeFade()
    {
        yield return new WaitForSeconds(1f);
        canvasGroupLoading.DOFade(0, 1f);
    }

    public IEnumerator HideForX(float d, Action action)
    {
        canvasGroupLoading.DOFade(1, 0.6f);
        yield return new WaitForSeconds( d + 0.6f);
        action?.Invoke();
    }


}
