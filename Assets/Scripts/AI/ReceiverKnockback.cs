using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
// using UnityEngine

public class ReceiverKnockback : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] UnityEvent OnStartKnock, OnEndKnock;
    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }
    void OnEnable()
    {
        GameEvents.OnEnemyGetKnockBack += ReceiverKnockBack;
    }

    void OnDisable()
    {
        GameEvents.OnEnemyGetKnockBack -= ReceiverKnockBack;
    }

    void ReceiverKnockBack(string instanceID, float strength, float speedbullet, Vector2 dir)
    {
        if (gameObject.GetInstanceID().ToString() != instanceID) return;
        Debug.Log($"{gameObject.name} kena knockback");
        if (strength <= 0) return;
        OnStartKnock?.Invoke();

        rb.linearVelocity = Vector2.zero; // reset dulu biar konsisten
        rb.AddForce(dir.normalized * strength * speedbullet, ForceMode2D.Impulse); // dorong

        StartCoroutine(StopKnockBack(0.1f));
    }
    IEnumerator StopKnockBack(float duration)
    {
        yield return new WaitForSeconds(duration);
        OnEndKnock?.Invoke();
        rb.linearVelocity = Vector2.zero;
    }
}
