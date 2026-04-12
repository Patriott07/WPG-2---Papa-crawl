using UnityEngine;
using data.structs;
using System.Collections.Generic;
public class ObjectPollingGame : MonoBehaviour
{
    [Header("==== UI FLOATS ====")]
    [SerializeField] ObjectPolling TextUIFloats;
    [Header("==== Exp particle ====")]
    [SerializeField] ObjectPolling particleExps;
    List<GameObject> list_TextUIFloats = new List<GameObject>();
    List<GameObject> list_ParticleExps = new List<GameObject>();

    public GameObject BigDagger;

    public static ObjectPollingGame Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Debug.Log($"Object Pooling was attached on {gameObject.name}");
        Setup_UITextFloats();
        Setup_ParticleExps();
    }

    void Setup_ParticleExps()
    {
        list_ParticleExps = new List<GameObject>();

        for (int i = 0; i < particleExps.count; i++)
        {
            GameObject obj = Instantiate(particleExps.gameObject, transform);
            obj.SetActive(false);

            list_ParticleExps.Add(obj);
        }
    }
    void Setup_UITextFloats()
    {
        list_TextUIFloats = new List<GameObject>();

        for (int i = 0; i < TextUIFloats.count; i++)
        {
            GameObject obj = Instantiate(TextUIFloats.gameObject, transform);
            obj.SetActive(false);

            list_TextUIFloats.Add(obj);
        }
    }


    public GameObject GetParticleExp()
    {
        for (int i = 0; i < list_ParticleExps.Count; i++)
        {
            if (!list_ParticleExps[i].activeInHierarchy)
            {
                return list_ParticleExps[i];
            }
        }

        // kalau semua object sedang dipakai
        GameObject obj = Instantiate(particleExps.gameObject, transform);
        obj.SetActive(false);

        list_ParticleExps.Add(obj);
        return obj;
    }

    public GameObject GetUITextFloat()
    {
        for (int i = 0; i < list_TextUIFloats.Count; i++)
        {
            if (!list_TextUIFloats[i].activeInHierarchy)
            {
                return list_TextUIFloats[i];
            }
        }

        // kalau semua object sedang dipakai
        GameObject obj = Instantiate(TextUIFloats.gameObject, transform);
        obj.SetActive(false);

        list_TextUIFloats.Add(obj);

        return obj;
    }
}
