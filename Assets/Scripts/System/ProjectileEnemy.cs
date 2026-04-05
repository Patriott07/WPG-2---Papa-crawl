using UnityEngine;

public class ProjectileEnemy : MonoBehaviour
{
    [SerializeField] float speed = 10f; //speed of bulletx
    [SerializeField] float lifeTime = 5f;
    [SerializeField] public float damage, critDamage;

    private Vector3 direction;

    public void Init(Vector2 dir, float damage, float speed, float angleChange = 0)
    {
        this.direction = dir;
        this.damage = damage;
        this.speed = speed;
        
        // change dir 
        this.direction = RotateDirection(this.direction, angleChange);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameEvents.OnPlayerGetDamage?.Invoke(this.damage);
            Destroy(gameObject);
        }
    }
    Vector3 RotateDirection(Vector3 dir, float angle)
    {
        return Quaternion.AngleAxis(angle, Vector3.forward) * dir;
    }
    void Update()
    {
        transform.position += this.direction * speed * Time.deltaTime;
    }
}
