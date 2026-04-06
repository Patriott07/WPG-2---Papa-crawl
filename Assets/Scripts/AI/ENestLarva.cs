using UnityEngine;
using data.structs;
using System.Collections;
using Pathfinding;
using Player.script;
public class ENestLarva : MonoBehaviour
{
    [SerializeField] EnemyStatus statEnemy, baseInit;
    [SerializeField] GameObject Larva;
    [SerializeField] int minSpawn, maxSpawn;
    [SerializeField] float radiusSpawn = 2f;
    Transform target;
    bool isCanChasing, canAttack = false, isAlive = true;
    float delayHit = 1f;
    int level;
    public int spawnCount;

    // ==========================
    // COMPONENT HELPER
    // ==========================
    AIPath aIPath;
    void Awake()
    {
        aIPath = gameObject.GetComponent<AIPath>();
        Init();
        spawnCount = Random.Range(minSpawn, maxSpawn);
    }
    void Start()
    {
        target = PlayerHit.Instance.transform;
    }

    void CalculatedStatEnemy(int minL, int maxL)
    {
        level = Random.Range(minL, maxL);
        statEnemy.hp = baseInit.hp + (level * 24);
        statEnemy.att = baseInit.att + (level * 20);
        // statEnemy.moveSpeed = baseInit.moveSpeed + (level * 0.03f);

        Debug.Log($"{gameObject.name} Sudah dihitung berdasarkan level");
    }

    void OnEnable()
    {
        GameEvents.OnEnemyGetDamage += GetDamage;
        GameEvents.CalculateEnemyStatByMapLevel += CalculatedStatEnemy;
    }

    void OnDisable()
    {
        GameEvents.OnEnemyGetDamage -= GetDamage;
        GameEvents.CalculateEnemyStatByMapLevel -= CalculatedStatEnemy;
    }

    // calculate stat by level
    void CalculateStatByLevel()
    {

    }

    void GetDamage(string instanceID, float d)
    {
        // Debug.Log($"Instance ID : {gameObject.GetInstanceID()} | name object : {gameObject.name}");
        if (!isAlive) return;
        if (gameObject.GetInstanceID().ToString() != instanceID) return;
        if (statEnemy.hp <= 0) return;
        statEnemy.hp -= d;

        if (statEnemy.hp <= 0)
            Dead();
    }

    void Init()
    {
        statEnemy.hp = 100;
        statEnemy.att = 20;
        statEnemy.rangeAttack = 1.7f;
        statEnemy.moveSpeed = 0f;
        statEnemy.typeEnemy = TypeEnemy.Mele;
        statEnemy.typeMovementEnemy = TypeMovementEnemy.Chasing;

        // setup to main settting
        aIPath.maxSpeed = statEnemy.moveSpeed;
        baseInit = statEnemy;

        CalculateStatByLevel();
    }

    // slow, and will stop after 3s chasing dan 0.8 not move

    void SpawnOnRadius()
    {
        Vector2 randomPos = Random.insideUnitCircle * radiusSpawn;
        Vector3 spawnPos = transform.position + new Vector3(randomPos.x, randomPos.y, 0);

        Instantiate(Larva, spawnPos, Quaternion.identity);

        Destroy(gameObject, 2.4f);
    }

    void CheckRange()
    {
        if (Vector2.Distance(transform.position, target.position) <= statEnemy.rangeAttack)
            Attack();
    }

    void Attack()
    {
        if (!canAttack) return;
        StartCoroutine(DelayBeforeHitAgain());
        GameEvents.OnPlayerGetDamage?.Invoke(statEnemy.att);
    }

    IEnumerator DelayBeforeHitAgain()
    {
        canAttack = false;
        yield return new WaitForSeconds(delayHit);
        canAttack = true;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.purple;
        Gizmos.DrawWireSphere(transform.position, statEnemy.rangeAttack);

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, radiusSpawn);
    }

    void Dead()
    {
        if(!isAlive) return;
        isAlive = false;
        MapIdentity.Instance.DecreaseEnemyCount();
        MapIdentity.Instance.SpawnObjectExp(transform, ((baseInit.hp / 4) + (level * 14)) / 4);
        CheckRange(); // damage radius
        for (int i = 0; i < spawnCount; i++)
        {
            SpawnOnRadius();
        }
    }

}
