using UnityEngine;
using System;
using data.structs;
public class GameEvents : MonoBehaviour
{
    // =====================================
    // Player
    public static Action<bool> OnPlayerMove;
    public static Action<float> OnPlayerGetDamage;
    public static Action OnPlayerDead;
    // =====================================

    // =====================================
    // System
    public static Action<bool> OnStart;
    public static Action<float> LevelStart;
    public static Action<GameState> SaveManagerLoaded;
    public static Action<int, int> CalculateEnemyStatByMapLevel;
    public static Action<Transform> GetCollideWithPlayer;
    public static Action MapClear;
    public static Action<string> StartRotatingDagger;
    // =====================================


    // =====================================
    // UI
    
    // =====================================



    // =====================================
    // Enemy
    public static Action<string, float, float, Vector2> OnEnemyGetKnockBack;
    public static Action<string, float> OnEnemyGetDamage;

    // =====================================


}
