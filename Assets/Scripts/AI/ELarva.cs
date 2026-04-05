using UnityEngine;
using data.structs;
using System.Collections;
using Pathfinding;
using Player.script;
public class ELarva : MonoBehaviour
{
    [SerializeField] EnemyStatus statEnemy, baseInit;
    bool isCanChasing, canAttack, isAlive = true;
    float delayHit = 1f;

    int level;
    Transform target;

    // ==========================
    // COMPONENT HELPER
    // ==========================
    AIPath aIPath;
    void Awake()
    {
        aIPath = gameObject.GetComponent<AIPath>();
        Init();
    }
    void Start()
    {
        StartCoroutine(StopAndChasing());
        target = PlayerHit.Instance.transform;
    }

     void CalculatedStatEnemy(int minL, int maxL)
    {
        level = Random.Range(minL, maxL);
        statEnemy.hp = baseInit.hp + (level * 14);
        statEnemy.att = baseInit.att + (level * 4);
        statEnemy.moveSpeed = baseInit.moveSpeed + (level * 0.03f);

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

    void GetDamage(string instanceID, float d)
    {
        if (!isAlive) return;
        if (gameObject.GetInstanceID().ToString() != instanceID) return;
        if (statEnemy.hp <= 0) return;

        statEnemy.hp -= d;

        if (statEnemy.hp <= 0)
            Dead();
    }

    void Dead()
    {
        if(!isAlive) return;

        isAlive = false;
        SetMovePathF(false);
        gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
        MapIdentity.Instance.DecreaseEnemyCount();
        MapIdentity.Instance.SpawnObjectExp(transform, (baseInit.hp + (level * 14)) / 4);
        Destroy(gameObject);
    }

    void Update()
    {
        CheckRange();
    }


    void Init()
    {
        statEnemy.hp = 30;
        statEnemy.att = 10;
        statEnemy.rangeAttack = 0.8f;
        statEnemy.moveSpeed = 0.8f;
        statEnemy.typeEnemy = TypeEnemy.Mele;
        statEnemy.typeMovementEnemy = TypeMovementEnemy.Chasing;

        // setup to main settting
        aIPath.maxSpeed = statEnemy.moveSpeed;
        baseInit = statEnemy;

    }

    // slow, and will stop after 3s chasing dan 0.8 not move
    IEnumerator StopAndChasing()
    {
        // Do chase
        while (isAlive)
        {
            ChangeSpped(baseInit.moveSpeed);

            isCanChasing = true;
            aIPath.canMove = isCanChasing;

            yield return new WaitForSeconds(7f);

            ChangeSpped(1.1f);
            yield return new WaitForSeconds(1f);

            // Do stop
            isCanChasing = false;
            aIPath.canMove = isCanChasing;

            yield return new WaitForSeconds(0.8f);
        }
    }

    void ChangeSpped(float newSpeed)
    {
        aIPath.maxSpeed = newSpeed;
        statEnemy.moveSpeed = newSpeed;
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
    }

    public void SetMovePathF(bool state)
    {
        aIPath.canMove = state;
    }

}
