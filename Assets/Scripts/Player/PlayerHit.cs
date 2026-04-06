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
        bool canShoot = false;
        private Weapon weapon;
        [SerializeField] GameObject projectile;
        PlayerStat playerStat;

        public static PlayerHit Instance;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            // weapon = new Weapon("Bow", 5, 3, 10, 4, Weapons.Bow);
            playerStat = gameObject.GetComponent<PlayerStat>();
        }

        void Awake()
        {
            Instance = this;
            // weapon = ModelsData.weaponsModel1.Find((val) => val.ID == 1);
        }

        public void SetWeapon(Weapon newWeapon) => weapon = newWeapon;
        void ChangeWeapon()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Weapon lastWeapon = weapon;
                if (lastWeapon.ID >= ModelsData.weaponsModel1.Count)
                {
                    weapon = ModelsData.weaponsModel1.Find((val) => val.ID == 1);
                }
                else
                {
                    weapon = ModelsData.weaponsModel1.Find((val) => val.ID == lastWeapon.ID + 1);
                }
            }
        }

        public Weapon GetCurrentWeapon() => weapon;
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
        }

        void Attack()
        {
            attackTimer += Time.deltaTime;

            Transform target = FindClosestEnemy();

            if (target == null)
            {
                // Debug.Log("Tidak ada enemy dalam range");
            }

            if (target != null && attackTimer >= CalculateAttackSpeed())
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
            float impactAttackSpeed = weapon.attackSpeedImpact;

            float result = playerStat.playerStatus.attackSpeed + impactAttackSpeed;

            if (result < maxAttackSpeed) return maxAttackSpeed;
            return result;
        }

        // calculate Damage
        float CalculateDamage()
        {
            float result = playerStat.playerStatus.attackPoint + weapon.damage;
            return result;
        }

        // calculate movement speed 
        public float GetMovementImpact()
        {
            float result = weapon.movementImpact / 2;
            return result;
        }

        // calculate range (stat, level)
        float CalculateRange()
        {
            float result = playerStat.playerStatus.attackPoint + weapon.range;
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

            GameObject arrow = Instantiate(projectiles[weapon.ID - 1], transform.position, Quaternion.identity);
            Vector3 direction = (target.position - transform.position).normalized;
            ArrowSystem projectile = arrow.GetComponent<ArrowSystem>();

            Camera.main.DOShakePosition(0.08f, 0.1f + ((weapon.damage / 50) * 0.3f));
            projectile.Init(direction, weapon.lifetime, CalculateDamage(), weapon.speed, weapon.knockbackStrength);

            PlaySound();
        }

        void PlaySound()
        {
            switch (weapon.ID)
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