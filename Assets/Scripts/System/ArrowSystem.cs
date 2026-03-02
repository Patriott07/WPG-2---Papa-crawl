using UnityEngine;

public class ArrowSystem : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] float lifeTime = 5f;
    [SerializeField] public float damage;

    private Vector3 direction;

    public void Init(Vector3 shootDirection, float damage, float lifeTime, float damageWeapon)
    {
        // direction = shootDirection.normalized;
        this.direction = shootDirection;
        this.damage = damageWeapon;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
}