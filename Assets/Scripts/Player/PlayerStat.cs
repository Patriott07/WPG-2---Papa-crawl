using UnityEngine;
using data.structs;

public class PlayerStat : MonoBehaviour
{
    public PlayerStatus playerStatus;

    void Awake()
    {
        playerStatus = new PlayerStatus(30, 5, 0.3f, 5, 30);
    }
}

