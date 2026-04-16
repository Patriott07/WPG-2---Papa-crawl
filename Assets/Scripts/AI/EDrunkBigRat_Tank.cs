using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Player.script;
using data.structs;


public class EDrunkBigRat_Tank : MonoBehaviour
{
    [Header("=== STATS ===")]
    [SerializeField] EnemyStatus statEnemy, baseStat;

    [Header("=== References ===")]
    public GameObject prefabBullet;
    public Transform target;

    // STATE
    bool isAlive = true, canAttack = true;
    float projectileSpeed = 2f;

    int level;

    // COMPONENT
    AIPath aiPath;
    EnemyPathFinderCust enemyPathFinderCust;
    EnemyDropSystem itemDropScript;

    void Awake()
    {
        aiPath = GetComponent<AIPath>();
        enemyPathFinderCust = GetComponent<EnemyPathFinderCust>();
        itemDropScript = gameObject.GetComponent<EnemyDropSystem>();
        Init();
    }

    void TurnOffEnemy()
    {
        // isCanChasing = false;
        target = null;
        canAttack = false;
        SetMovePathF(false);
        aiPath.maxSpeed = 0;
        aiPath.enabled = false;
    }

    void Start()
    {
        target = PlayerHit.Instance.transform;
        StartCoroutine(MainAI());
        // enemyPathFinderCust.aIDestinationSetter.target = target;
    }

    void OnEnable()
    {
        GameEvents.OnEnemyGetDamage += GetDamage;
        GameEvents.CalculateEnemyStatByMapLevel += CalculatedStatEnemy;
        GameEvents.OnPlayerDead += TurnOffEnemy;
    }

    void OnDisable()
    {
        GameEvents.OnEnemyGetDamage -= GetDamage;
        GameEvents.CalculateEnemyStatByMapLevel -= CalculatedStatEnemy;
        GameEvents.OnPlayerDead -= TurnOffEnemy;
    }

    void CalculatedStatEnemy(int minL, int maxL)
    {
        level = Random.Range(minL, maxL);

        statEnemy.hp = baseStat.hp + (level * 60);
        statEnemy.att = baseStat.att + (level * 1);
        statEnemy.moveSpeed = baseStat.moveSpeed + (level * 0.01f);

        Debug.Log($"{gameObject.name} Sudah dihitung berdasarkan level");
    }

    // ==========================
    // ATTACK
    // ==========================
    IEnumerator DoAttack()
    {
        if (!canAttack) yield break;
        yield return StartCoroutine(Attack());
        yield return new WaitForSeconds(0.4f);
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
        if(itemDropScript != null) StartCoroutine(itemDropScript.StartDrop()); // drop item
        GetComponent<SpriteRenderer>().color = new Color32(0, 0, 0, 0);
        StartCoroutine(Explode());
        // Destroy(gameObject, 1.2f);
    }

    // ==========================
    // INIT
    // ==========================
    void Init()
    {
        statEnemy.hp = 700;
        statEnemy.att = 10;
        statEnemy.moveSpeed = 2f;

        aiPath.maxSpeed = statEnemy.moveSpeed;

        baseStat = statEnemy;
    }

    // ==========================
    // RANDOM MOVE TARGET
    // ==========================
    Vector3 GetDirection(float radius)
    {
        return new Vector3(
            target.position.x + Random.Range(-radius, radius),
            target.position.y + Random.Range(-radius, radius),
            0
        );
    }

    // ==========================
    // FINDING PATH
    // ==========================

    IEnumerator DoChase()
    {
        Debug.Log("DOCHASE");
        enemyPathFinderCust.aIDestinationSetter.targetPos = GetDirection(1.5f);

        float time = 0;
        while (!aiPath.reachedDestination && time < 4f)
        {
            time += Time.deltaTime;
            yield return null;
        }

    }

    IEnumerator Explode()
    {
        yield return null;

        MapIdentity.Instance.DecreaseEnemyCount();
        MapIdentity.Instance.SpawnObjectExp(transform, (statEnemy.hp + (level * 14)) / 4);

        //  List<float> angles = new List<float> { 36, 0 };
        for (int i = 0; i < 10; i++)
        {
            GameObject projectile = Instantiate(prefabBullet, transform.position, Quaternion.identity);
            ProjectileEnemy proj = projectile.GetComponent<ProjectileEnemy>();

            Vector3 dir = (target.position - transform.position).normalized;
            proj.Init(dir, statEnemy.att, projectileSpeed, 36 * (i + 1));

            // yield return new WaitForSeconds(0f);
        }


        Destroy(gameObject);
        StopAllCoroutines();
    }

    // ==========================
    // MAIN AI LOOP
    // ==========================
    IEnumerator MainAI()
    {
        while (isAlive)
        {
            yield return StartCoroutine(DoChase());
            yield return new WaitForSeconds(0.6f);
        }
    }

    IEnumerator Attack()
    {
        if (!canAttack) yield break;
        canAttack = false;
        GameEvents.OnPlayerGetDamage?.Invoke(statEnemy.att);
        yield return new WaitForSeconds(0.4f);
        canAttack = true;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            StartCoroutine(DoAttack());
    }

    public void SetMovePathF(bool state)
    {
        aiPath.canMove = state;
    }
}