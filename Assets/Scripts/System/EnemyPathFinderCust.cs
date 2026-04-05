using Pathfinding;
using Player.script;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyPathFinderCust : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool isCastPlayerOnStart = false;
    public AIDestinationSetter aIDestinationSetter;
    public AIPath aIPath;

    
    void Start()
    {
        if (isCastPlayerOnStart) aIDestinationSetter.target = PlayerHit.Instance.transform;
    }

    

}
