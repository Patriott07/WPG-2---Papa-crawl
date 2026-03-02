using UnityEngine;
using System;
public class GameEvents : MonoBehaviour
{
    public static Action<bool> OnPlayerMove;
    public static GameEvents Instance;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
