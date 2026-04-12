using UnityEngine;
using data.structs;
using Player.script;


public class ArrowSystem : MonoBehaviour
{
    [SerializeField] float speed = 10f; //speed of bulletx
    [SerializeField] float lifeTime = 5f;
    [SerializeField] public float damage, critDamage;

    private Vector3 direction;
    private float strengthKnockBack;

    public void Init(Vector3 shootDirection, float lifeTime, float damageWeapon, float speed, float Knockback)
    {
        // direction = shootDirection.normalized;
        this.direction = shootDirection;
        this.damage = damageWeapon;
        this.speed = speed;
        this.critDamage = damageWeapon * critDamage;
        this.strengthKnockBack = Knockback;
 
        // this.direction.x -= 6;
        // direction.x += 6;
        // this.direction = RotateDirection(this.direction, 8);
        //  
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);


        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log($"damage : {this.damage} : {collision.GetInstanceID()}");
            GameObject textUIDamage = ObjectPollingGame.Instance.GetUITextFloat();
            GameEvents.OnEnemyGetDamage?.Invoke(collision.gameObject.GetInstanceID().ToString(), this.damage);

            // set color, text, pos
            textUIDamage.transform.position = new Vector2(collision.transform.position.x + Random.Range(-0.8f, 0.8f), collision.transform.position.y + 1.2f);
            TextUIFloatingDamage scriptText = textUIDamage.GetComponent<TextUIFloatingDamage>();
            scriptText.colorText = Color.white;
            scriptText.textInput = this.damage.ToString();
            textUIDamage.SetActive(true);

            CheckSpawnDagger(this.damage);

            KnockBackSystem(collision.gameObject.GetInstanceID().ToString());
            Destroy(gameObject);
        }
    }

    void CheckSpawnDagger(float dmg)
    {
        if(PlayerHit.Instance.GetCurrentWeapon()?.name == "Dagger")
        {
            GameObject bigDagger = Instantiate(ObjectPollingGame.Instance.BigDagger, transform.position, Quaternion.identity);
            bigDagger.GetComponent<KunaiRotationSystem>().damage = dmg * 0.3f;
            bigDagger.GetComponent<KunaiRotationSystem>().rotationSpeed += PlayerStat.Instance.level * 1.5f;  
        }
    }


    void KnockBackSystem(string UID)
    {
        if (this.strengthKnockBack <= 0) return;

        Knockback knockback = new Knockback(this.strengthKnockBack, direction);
        GameEvents.OnEnemyGetKnockBack?.Invoke(UID, knockback.knockbackStrength, speed, knockback.direction);

        Debug.Log(UID);
    }

    Vector3 RotateDirection(Vector3 dir, float angle)
    {
        return Quaternion.AngleAxis(angle, Vector3.forward) * dir;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
}