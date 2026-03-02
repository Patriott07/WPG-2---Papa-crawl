
using UnityEngine;
using data.structs;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Setting")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] bool canMove = true;
    Rigidbody2D rb;
    Vector2 velocity;

    public static PlayerMovement Instance;
    PlayerStat pStat;
    CameraFollowPlayer cameraFollowSc;

    void Awake()
    {
        Instance = this;
        rb = gameObject.GetComponent<Rigidbody2D>();
        pStat = gameObject.GetComponent<PlayerStat>();

    }

    void Start()
    {
        cameraFollowSc = FindAnyObjectByType<CameraFollowPlayer>();
        // moveSpeed = ;
    }

    void Movement()
    {
        if (!canMove) return;

        if (Input.GetKey(KeyCode.W)) velocity.y = 1;
        else if (Input.GetKey(KeyCode.S)) velocity.y = -1;
        else velocity.y = 0;

        if (Input.GetKey(KeyCode.A)) velocity.x = -1;
        else if (Input.GetKey(KeyCode.D)) velocity.x = 1;
        else velocity.x = 0;

        if (velocity.x == 0 && velocity.y == 0) GameEvents.OnPlayerMove?.Invoke(false);
        else GameEvents.OnPlayerMove?.Invoke(true);

        velocity = velocity.normalized; // biar diagonal gak lebih cepat
        rb.linearVelocity = velocity * pStat.playerStatus.movespeed;


    }

    public Vector2 StoreLocationPlayer()
    {
        Vector2 loc = new Vector2(transform.position.x, transform.position.y);
        return loc;
    }

    void FixedUpdate()
    {
        Movement();

    }

    void Update()
    {
        cameraFollowSc.TransitionCamSize(rb);
    }
}
