using DG.Tweening;
using data.structs;
using UnityEngine;

public class KunaiRotationSystem : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Kecepatan rotasi. Positif = Berlawanan jarum jam, Negatif = Searah jarum jam.")]
    public float rotationSpeed = 100f;

    [Tooltip("Gunakan Unscaled Time jika ingin tetap berputar saat game di-pause.")]
    public bool useUnscaledTime = false;
    public float deadTime = 3f;
    public float damage;
    void Start()
    {
        transform.localScale = new Vector3(1, 1, 1);
        transform.DOScale(2.6f, 0.8f);
        Destroy(gameObject, deadTime);
    }

    void Update()
    {
        // Hitung Delta Time
        float deltaTime = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        // Untuk game 2D, kita memutar sumbu Z
        transform.Rotate(0, 0, rotationSpeed * deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log($"damage : {damage} : {collision.GetInstanceID()}");
            GameObject textUIDamage = ObjectPollingGame.Instance.GetUITextFloat();
            GameEvents.OnEnemyGetDamage?.Invoke(collision.gameObject.GetInstanceID().ToString(), damage);

            // set color, text, pos
            textUIDamage.transform.position = new Vector2(collision.transform.position.x + Random.Range(-0.8f, 0.8f), collision.transform.position.y + 1.2f);
            TextUIFloatingDamage scriptText = textUIDamage.GetComponent<TextUIFloatingDamage>();
            scriptText.colorText = Color.darkBlue;
            scriptText.textInput = Mathf.RoundToInt(damage).ToString();
            textUIDamage.SetActive(true);
        }


    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            GameEvents.OnEnemyGetDamage?.Invoke(collision.gameObject.GetInstanceID().ToString(), damage / 10);
        }
    }
}