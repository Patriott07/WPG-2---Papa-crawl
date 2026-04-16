using UnityEngine;
using data.structs;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class EnemyDropSystem : MonoBehaviour
{
    public ItemDropEnemy itemDrop;
    public IEnumerator StartDrop()
    {
        for (int j = 0; j < itemDrop.itemPrefabs.Count; j++)
        {
            int qty = itemDrop.qtys[j];
            GameObject objPrefab = itemDrop.itemPrefabs[j];

            for (int i = 0; i < qty; i++)
            {
                // 1. Cari posisi aman
                Vector2 targetPos = (Vector2)transform.position + (Random.insideUnitCircle.normalized * 0.8f);
                GameObject obj = Instantiate(objPrefab, transform.position, Quaternion.identity);

                // 3. Gerakkan item dari chest ke targetPos dengan efek melengkung (Jump)
                // .DOJump(posisi_tujuan, tinggi_lompatan, jumlah_lompatan, durasi)
                obj.transform.DOJump(targetPos, 0.8f, 1, 0.8f).SetEase(Ease.OutQuad);

                // Beri jeda sedikit antar item agar tidak keluar berbarengan (opsional)
                yield return new WaitForSeconds(0.005f);
            }
        }
    }


}
