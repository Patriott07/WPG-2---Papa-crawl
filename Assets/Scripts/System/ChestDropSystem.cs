using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Player.script;

[System.Serializable]
public struct ItemChest
{
    public GameObject itemPrefab;
    public int min;
    public int max;
    private int itemAmount;

    public int GetItemAmount() => itemAmount;
    public void SetItemAmount(int v) => itemAmount = v;
}
public class ChestDropSystem : MonoBehaviour
{
    [Header("Settings")]
    public List<ItemChest> ItemsInsideChest;
    // public int itemAmount = 3;
    public float dropDelay = 2f;
    public float dropRadius = 1.5f;
    public float travelDuration = 0.5f; // Durasi meluncur ke posisi aman

    [Header("Setting For Exp Drop")]
    public float minExp = 50, maxExp = 200;

    [Header("Collision Check")]
    public LayerMask obstacleLayer; // Masukkan layer Tembok/Obs di sini
    public float checkRadius = 0.2f; // Ukuran pengecekan agar tidak mepet tembok

    private bool isOpened = false, isNearPlayer = false, isCanOpen = false;

    // Bisa dipanggil saat Chest diklik atau kena interaksi
    public void OpenChest()
    {
        if (isOpened || !isCanOpen) return;
        isOpened = true;


        StartCoroutine(DropItemsRoutine());
    }

    void SetCanOpen()
    {
        isCanOpen = true;
    }

    void OnEnable()
    {
        GameEvents.MapClear += SetCanOpen;
    }

    void OnDisable()
    {
        GameEvents.MapClear -= SetCanOpen;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isNearPlayer && isCanOpen)
            OpenChest();
    }

    IEnumerator DropItemsRoutine()
    {
        foreach (ItemChest item in ItemsInsideChest)
        {
            item.SetItemAmount(Random.Range(item.min, item.max));

            for (int i = 0; i < item.GetItemAmount(); i++)
            {
                // 1. Cari posisi aman
                Vector2 targetPos = GetSafeSpawnPosition();

                // 2. Spawn item tepat di posisi chest
                GameObject obj = Instantiate(item.itemPrefab, transform.position, Quaternion.identity);

                // Ambil collider item dan collider chest ini
                Collider2D itemCol = obj.GetComponent<Collider2D>();
                Collider2D chestCol = gameObject.GetComponent<Collider2D>();

                // check item == exp?
                CheckSpawnItemExp(obj);

                if (itemCol != null && chestCol != null)
                {
                    // Beritahu Unity: "Khusus dua objek ini, jangan saling tabrak"
                    Physics2D.IgnoreCollision(itemCol, chestCol);
                }

                // 3. Gerakkan item dari chest ke targetPos dengan efek melengkung (Jump)
                // .DOJump(posisi_tujuan, tinggi_lompatan, jumlah_lompatan, durasi)
                obj.transform.DOJump(targetPos, 0.8f, 1, travelDuration).SetEase(Ease.OutQuad);

                // Beri jeda sedikit antar item agar tidak keluar berbarengan (opsional)
                yield return new WaitForSeconds(0.1f);
            }
        }

        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        Destroy(gameObject, 2f);
    }

    void CheckSpawnItemExp(GameObject g)
    {
        ExpParticle expParticleScript = g.GetComponent<ExpParticle>();
        if (expParticleScript != null)
        {
            expParticleScript.expAffect = Random.Range(minExp, maxExp);
            expParticleScript.StartCollide(PlayerHit.Instance.transform);
        }
    }

    Vector2 GetSafeSpawnPosition()
    {
        int maxAttempts = 20;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 randomDir = Random.insideUnitCircle * dropRadius;

            // PAKSA jarak minimal 0.6f agar item PASTI keluar dari badan chest
            if (randomDir.magnitude < 0.6f) randomDir = randomDir.normalized * 0.8f;

            Vector2 potentialPos = (Vector2)transform.position + randomDir;

            // Tambahkan pengecekan: Apakah posisi ini menabrak TEMBOK?
            Collider2D hit = Physics2D.OverlapCircle(potentialPos, checkRadius, obstacleLayer);

            if (hit == null) return potentialPos;
        }

        // Fallback: Jika semua sisi tembok, paksa lempar ke arah acak
        return (Vector2)transform.position + (Random.insideUnitCircle.normalized * Random.Range(dropRadius - 1f, dropRadius));
    }

    // Visualisasi radius drop di Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.darkBlue;
        Gizmos.DrawWireSphere(transform.position, dropRadius);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            isNearPlayer = true;
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            isNearPlayer = false;
    }


}
