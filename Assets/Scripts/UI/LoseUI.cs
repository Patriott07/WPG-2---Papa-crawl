using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
public class LoseUI : MonoBehaviour
{
    public TMP_Text text_tips, text_time;
    public CanvasGroup canvasGroupContent, canvasGroupText;
    List<string> tips_texts_content = new List<string>
    {
        "Better prepare yourself before fight",
        "Every Map Have Different Enemy, Becarefull",
        "Choose Right Weapon Will Bring You Win.",
        "Sometime maps diffuculty its not fit on you.",
        "You can back to last map to collect item and gain exp.",
        "Focus on Enemy, move, and shoot.",
    };

    void Awake()
    {
        canvasGroupContent.alpha = 0;
    }

    void DiedUIShow()
    {
        canvasGroupContent.DOFade(1f, 0.6f);
        text_tips.text = tips_texts_content[Random.Range(0, tips_texts_content.Count)];
        StartCoroutine(StartCountDown());
    }

    IEnumerator StartCountDown(int d = 5)
    {
        text_time.text = d.ToString();
        for (int i = d; i > 0; i--)
        {
            yield return new WaitForSeconds(1f);
            text_time.text = (i).ToString();
        }
        yield return new WaitForSeconds(1f);
        text_time.text = (0).ToString();
    }

    void OnEnable()
    {
        GameEvents.OnPlayerDead += DiedUIShow;
    }

    void OnDisable()
    {
        GameEvents.OnPlayerDead -= DiedUIShow;
    }
}
