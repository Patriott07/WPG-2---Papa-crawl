using UnityEngine;
using data.structs;
using System.Collections.Generic;
using DG.Tweening;

namespace Player.script
{
    public class PlayerHit : MonoBehaviour
    {
        [SerializeField] List<GameObject> projectiles;
        [SerializeField] LayerMask enemyLayer;
        float attackTimer;
        public bool canShoot = false, isNearWithBlckSmith = false;
        public Weapon? weapon;
        [SerializeField] GameObject projectile;
        PlayerStat playerStat;
        public Transform transformWeapon;
        public List<Sprite> spritesOfWeapon;
        public List<Sprite> spritesOfArmor;

        [Header("Debug Info")]
        [SerializeField] private Weapon weaponDebug; // Ini yang bakal tampil di Inspector

        public static PlayerHit Instance;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            // weapon = new Weapon("Bow", 5, 3, 10, 4, Weapons.Bow);
            playerStat = gameObject.GetComponent<PlayerStat>();
            SetWeapon(ModelsData.GetListOfWeapon1()[0]);
        }

        void Awake()
        {
            Instance = this;
            // weapon = ModelsData.weaponsModel1.Find((val) => val.ID == 1);
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("BlackSmith"))
            {
                isNearWithBlckSmith = true;
            }
        }
        void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("BlackSmith"))
            {
                isNearWithBlckSmith = false;
            }

        }

        void ChangeModelWeapon(int i)
        {
            PlayerMovement.Instance.spWeapon.sprite = spritesOfWeapon[i];
            HUDUI.Instance.UpdateHUD();
        }

        public void SetCanShoot(bool state) => canShoot = state;
        public void SetWeapon(Weapon? newWeapon) => weapon = newWeapon;
        void ChangeWeapon()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Weapon? lastWeapon = weapon;
                Debug.Log($"OKE {lastWeapon?.name} || {lastWeapon?.ID}");
                // Cara 1: Menggunakan Value (Paling aman untuk Nullable)

                if (lastWeapon?.ID >= ModelsData.weaponsModel1.Count)
                {
                    Debug.Log($"OKE 1");
                    weapon = ModelsData.weaponsModel1.Find((val) => val.ID == 1);
                    ChangeModelWeapon(weapon != null ? weapon.Value.ID - 1 : 0);
                }
                else
                {
                    Debug.Log($"OKE 2");
                    weapon = ModelsData.weaponsModel1.Find((val) => val.ID == lastWeapon?.ID + 1);
                    ChangeModelWeapon(weapon != null ? weapon.Value.ID - 1 : 0);
                    // weapon = ModelsData.weaponsModel1[lastWeapon?.ID + 1 ?? ];
                }
            }
        }

        public Weapon? GetCurrentWeapon() => weapon;
        void SetIsCanShot(bool stateMove)
        {
            if (stateMove)
            {
                canShoot = false;
            }
            else canShoot = true;
        }

        // suscribe event 
        void OnEnable()
        {
            GameEvents.OnPlayerMove += SetIsCanShot;
        }
        void OnDisable()
        {
            GameEvents.OnPlayerMove -= SetIsCanShot;
        }

        void Update()
        {
            Attack();
            ChangeWeapon();

            // Kalau weapon lagi null, kita pakai default (kosong)
            weaponDebug = weapon ?? default;

            // Panggil fungsi rotasi setiap frame agar halus
            RotateWeaponToTarget();

            if (Input.GetKeyDown(KeyCode.G) && isNearWithBlckSmith)
                CraftingManager.Instance.OpenClosePanel();
        }

        void RotateWeaponToTarget()
        {
            Transform target = FindClosestEnemy();

            if (target != null && transformWeapon != null && weapon.HasValue)
            {
                Vector3 direction = target.position - transformWeapon.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                // TAMBAHKAN OFFSET DI SINI
                float finalAngle = angle + weapon.Value.offsetAngleView;

                transformWeapon.rotation = Quaternion.Euler(0, 0, finalAngle);

                // Flip logic agar tidak terbalik
                if (direction.x < 0)
                {
                    // Jika di-flip, offset juga harus disesuaikan sedikit atau pakai cara ini:
                    transformWeapon.localScale = new Vector3(0.8f, -0.8f, 0.8f);
                }
                else
                {
                    transformWeapon.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                }
            }
        }

        void Attack()
        {
            attackTimer += Time.deltaTime;

            Transform target = FindClosestEnemy();

            if (target == null)
            {
                // Debug.Log("Tidak ada enemy dalam range");
            }

            if (weapon != null && target != null && attackTimer >= CalculateAttackSpeed())
            {
                Shoot(target);
                attackTimer = 0f;
            }
        }

        // calculate attack speed character base + weapon attack speed
        float CalculateAttackSpeed()
        {
            float maxAttackSpeed = 0.3f;

            // jika nilainya positif maka (makin lambat)
            // jika nilainya negatif maka (makin cepat)
            float impactAttackSpeed = weapon?.attackSpeedImpact ?? 0;

            float result = playerStat.playerStatus.attackSpeed + impactAttackSpeed;

            if (result < maxAttackSpeed) return maxAttackSpeed;
            return result;
        }

        // calculate Damage
        float CalculateDamage()
        {
            float result = playerStat.playerStatus.attackPoint + weapon?.damage ?? 0;
            return result;
        }

        // calculate movement speed 
        public float GetMovementImpact()
        {
            float result = weapon?.movementImpact / 2 ?? 0;
            return result;
        }

        // calculate range (stat, level)
        float CalculateRange()
        {
            // float result = playerStat.playerStatus.attackPoint + weapon?.range ?? 0;
            float result = weapon?.range ?? 0;
            return result;
        }

        Transform FindClosestEnemy()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, CalculateRange(), enemyLayer);

            Transform closest = null;
            float minDistance = Mathf.Infinity;

            foreach (var hit in hits)
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closest = hit.transform;
                }
            }

            return closest;
        }

        void Shoot(Transform target)
        {
            if (!canShoot) return;

            Vector3 shootPos = transform.position;
            shootPos.y -= 0.15f;

            GameObject arrow = Instantiate(projectiles[weapon?.ID - 1 ?? 0], shootPos, Quaternion.identity);
            Vector3 direction = (target.position - shootPos).normalized;
            ArrowSystem projectile = arrow.GetComponent<ArrowSystem>();

            projectile.Init(direction, weapon?.lifetime ?? 0, CalculateDamage(), weapon?.speed ?? 0, weapon?.knockbackStrength ?? 0);
            PlaySound();
        }

        void PlaySound()
        {
            switch (weapon?.ID)
            {
                case 1:
                    PlayerSounds.Instance.slingshot.Stop();
                    PlayerSounds.Instance.slingshot.pitch = Random.Range(0.95f, 1.05f);
                    PlayerSounds.Instance.slingshot.Play();
                    break;
                case 2:
                    PlayerSounds.Instance.bow.Stop();
                    PlayerSounds.Instance.bow.pitch = Random.Range(0.95f, 1.05f);
                    PlayerSounds.Instance.bow.Play();
                    break;
                case 3:
                    PlayerSounds.Instance.dagger.Stop();
                    PlayerSounds.Instance.dagger.pitch = Random.Range(0.95f, 1.05f);
                    PlayerSounds.Instance.dagger.Play();
                    break;
                case 4:
                    PlayerSounds.Instance.bow.Stop();
                    PlayerSounds.Instance.bow.pitch = Random.Range(0.95f, 1.05f);
                    PlayerSounds.Instance.bow.Play();
                    break;

            }
        }

        void OnDrawGizmos()
        {
            if (Instance == null) return;

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, CalculateRange());
        }
    }




}