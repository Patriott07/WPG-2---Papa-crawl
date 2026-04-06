using System.Collections;
using UnityEngine;

public class ExpParticle : MonoBehaviour
{
    private bool isCanCollide = false;
    public float expAffect;
    public float speed = 4f;
    Transform target;


    private Vector3 startPos;
    private Vector3 targetPos;
    private bool hasTarget, isHasInteract;

    public void Init(Vector3 origin)
    {
        startPos = origin;

        Vector3 offset = new Vector3(
            Random.Range(-1.5f, 1.5f),
            Random.Range(-1.5f, 1.5f),
            0
        );

        targetPos = origin + offset;
        hasTarget = true;
    }

    void StartCollide(Transform t)
    {
        isCanCollide = true;
        target = t;
    }

    void Update()
    {
        if (isCanCollide && target != null)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                speed * Time.deltaTime
            );

            if (speed < 8f)
            {
                speed += 0.15f;
            }
            else speed = 8f;
        }
        else
        {
            if (hasTarget)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPos,
                    speed * Time.deltaTime
                );

                // kalau sudah sampai → stop
                if (Vector3.Distance(transform.position, targetPos) < 0.05f)
                {
                    hasTarget = false;
                }
            }
        }
    }

    void OnEnable()
    {
        GameEvents.GetCollideWithPlayer += StartCollide;
    }

    void OnDisable()
    {
        GameEvents.GetCollideWithPlayer -= StartCollide;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (isCanCollide && !isHasInteract)
            {
                isHasInteract = true;
                Debug.Log($"Player Gain Exp {expAffect}");
                PlayerStat.Instance.GainExp(expAffect);
                gameObject.GetComponent<SoundForItem>().PlaySound(false);
                // gameObject.SetActive(false);
                StartCoroutine(SetActiveFalseWithDelay(0.6f));
            }
        }
    }

    IEnumerator SetActiveFalseWithDelay(float d)
    {
        // set function and view turn off
        SpriteRenderer sp = gameObject.GetComponent<SpriteRenderer>();
        BoxCollider2D collider = gameObject.GetComponent<BoxCollider2D>();
        sp.color = new Color(sp.color.r, sp.color.g, sp.color.b, 0);
        collider.enabled = false;

        yield return new WaitForSeconds(d);

        sp.color = new Color(sp.color.r, sp.color.g, sp.color.b, 1f);
        collider.enabled = true;

        gameObject.SetActive(false);
    }
}
