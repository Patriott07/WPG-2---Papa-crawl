using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Player.script;
using data.structs;


public class ETikusBanditMele : MonoBehaviour
{
    [Header("=== STATS ===")]
    [SerializeField] EnemyStatus statEnemy, baseInit;

    [Header("=== References ===")]
    public GameObject trail, indicator;
    public Transform target;

    // STATE
    bool isAlive = true;
    bool isAttacking = false;
    bool isPerform = false;

    float radiusPathfinderPos = 1.2f;
    float distancePerform = 6f;
    float performDistance = 7.5f;

    int level;

    Vector2 direction;

    // COMPONENT
    [SerializeField] Rigidbody2D rb;
    AIPath aiPath;
    EnemyPathFinderCust enemyPathFinderCust;
      EnemyDropSystem itemDropScript;

    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        aiPath = GetComponent<AIPath>();
        itemDropScript = gameObject.GetComponent<EnemyDropSystem>();
        enemyPathFinderCust = GetComponent<EnemyPathFinderCust>();
        Init();
    }

    void Start()
    {
        target = PlayerHit.Instance.transform;
        enemyPathFinderCust.aIDestinationSetter.target = target;
        trail.SetActive(false);
    }

    void Update()
    {
        if (isAlive && CheckRange() <= distancePerform && !isPerform && !isAttacking)
        {
            StartCoroutine(DoAttack());
        }
    }

    float CheckRange()
    {
        return Vector3.Distance(transform.position, target.position);
    }

    void CalculatedStatEnemy(int minL, int maxL)
    {
        level = Random.Range(minL, maxL);
        statEnemy.hp = baseInit.hp + (level * 40);
        statEnemy.att = baseInit.att + (level * 10);
        statEnemy.moveSpeed = baseInit.moveSpeed + (level * 0.01f);

        Debug.Log($"{gameObject.name} Sudah dihitung berdasarkan level");
    }

    void TurnOffEnemy()
    {
        // isCanChasing = false;
        target = null;
        isAlive = false;
        SetMovePathF(false);
        aiPath.maxSpeed = 0;
        aiPath.enabled = false;
    }

    void OnEnable()
    {
        GameEvents.OnEnemyGetDamage += GetDamage;
        GameEvents.OnPlayerDead += TurnOffEnemy;
        GameEvents.CalculateEnemyStatByMapLevel += CalculatedStatEnemy;
    }

    void OnDisable()
    {
        GameEvents.OnEnemyGetDamage -= GetDamage;
        GameEvents.OnPlayerDead -= TurnOffEnemy;
        GameEvents.CalculateEnemyStatByMapLevel -= CalculatedStatEnemy;
    }


    // ==========================
    // ATTACK
    // ==========================
    IEnumerator DoAttack()
    {
        if (isAttacking) yield break;
        isAttacking = true;

        yield return StartCoroutine(PerformDashAttack());

        yield return new WaitForSeconds(2f); // cooldown
                                             // back to normal
        aiPath.enabled = true;
        isAttacking = false;
    }

    IEnumerator PerformDashAttack()
    {
        Vector3 dir = (target.position - transform.position).normalized;
        direction = dir;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // show indicator
        indicator.SetActive(true);

        aiPath.enabled = false; //Stop movement for charge
        yield return new WaitForSeconds(0.6f);

        // perform
        isPerform = true;
        trail.SetActive(true);
        rb.AddForce(dir * 25 * rb.mass, ForceMode2D.Impulse);
        Debug.Log(rb.linearVelocity);
        // hide indicator
        indicator.SetActive(false);

        yield return new WaitForSeconds(0.3f);
        Debug.Log(rb.linearVelocity);

        // stop perform
        isPerform = false;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        trail.SetActive(false);

    }

    // ==========================
    // DAMAGE SYSTEM
    // ==========================
    void GetDamage(string instanceID, float d)
    {
        if (!isAlive) return;
        if (gameObject.GetInstanceID().ToString() != instanceID) return;

        statEnemy.hp -= d;

        if (statEnemy.hp <= 0)
            Dead();
    }

    void Dead()
    {
        if (!isAlive) return;
        isAlive = false;
        aiPath.canMove = false;
        GetComponent<SpriteRenderer>().color = Color.blue;

        MapIdentity.Instance.DecreaseEnemyCount();
        if (itemDropScript != null) StartCoroutine(itemDropScript.StartDrop()); // drop item
        MapIdentity.Instance.SpawnObjectExp(transform, (baseInit.hp + (level * 14)) / 4);

        StopAllCoroutines();
        Destroy(gameObject);
    }

    // ==========================
    // INIT
    // ==========================
    void Init()
    {
        statEnemy.hp = 250;
        statEnemy.att = 60;
        statEnemy.moveSpeed = 2.4f;

        baseInit = statEnemy;

        aiPath.maxSpeed = statEnemy.moveSpeed;
    }

    // ==========================
    // RANDOM MOVE TARGET
    // ==========================
    // Vector3 GetDirection(float radius)
    // {
    //     return new Vector3(
    //         transform.position.x + Random.Range(-radius, radius),
    //         transform.position.y + Random.Range(-radius, radius),
    //         0
    //     );
    // }

    // ==========================
    // DEBUG
    // ==========================
    void OnDrawGizmos()
    {
        Gizmos.color = Color.purple;
        Gizmos.DrawWireSphere(transform.position, distancePerform);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, performDistance);
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isPerform)
        {
            // knockback
            float knockbackStrenth = 80f;
            target.GetComponent<Rigidbody2D>().AddForce(direction * knockbackStrenth, ForceMode2D.Impulse);

            // reset
            isPerform = false;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            trail.SetActive(false);

            // damage
            GameEvents.OnPlayerGetDamage?.Invoke(statEnemy.att);
            // yield return new WaitForSeconds()
        }

    }

    public void SetMovePathF(bool state)
    {
        aiPath.canMove = state;
    }
}