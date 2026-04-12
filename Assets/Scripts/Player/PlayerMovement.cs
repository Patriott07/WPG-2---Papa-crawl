
using UnityEngine;
using data.structs;
using Player.script;

public struct MovementPlayer
{
    // public float moveSpeed;
    public float bonusSpeed;
    public bool canMove;

}

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Setting")]

    [SerializeField] MovementPlayer movementPlayer;
    // [SerializeField] float moveSpeed = 5f;
    // [SerializeField] bool canMove = true;
    Rigidbody2D rb;
    Vector2 velocity;
    // float bonusSpeed;
    public SpriteRenderer sp, spWeapon, spArmor;

    public static PlayerMovement Instance;
    PlayerStat pStat;
    PlayerHit playerHit;
    CameraFollowPlayer cameraFollowSc;

    void Awake()
    {
        Instance = this;
        rb = gameObject.GetComponent<Rigidbody2D>();
        pStat = gameObject.GetComponent<PlayerStat>();
        movementPlayer.canMove = true;
        movementPlayer.bonusSpeed = 1f;
    }

    void Start()
    {
        playerHit = gameObject.GetComponent<PlayerHit>();
        cameraFollowSc = FindAnyObjectByType<CameraFollowPlayer>();
    }

    public void SetCanMove(bool state) => movementPlayer.canMove = state; 

    void Movement()
    {
        if (!movementPlayer.canMove) return;

        if (Input.GetKey(KeyCode.W)) velocity.y = 1;
        else if (Input.GetKey(KeyCode.S)) velocity.y = -1;
        else velocity.y = 0;

        if (Input.GetKey(KeyCode.A)) {velocity.x = -1; FLipAllSprite(false);}
        else if (Input.GetKey(KeyCode.D)) {velocity.x = 1; FLipAllSprite(true);}
        else velocity.x = 0;

        if (velocity.x == 0 && velocity.y == 0) GameEvents.OnPlayerMove?.Invoke(false);
        else GameEvents.OnPlayerMove?.Invoke(true);

        velocity = velocity.normalized; // biar diagonal gak lebih cepat
        rb.linearVelocity = velocity * CalculateMovementSpeed() * movementPlayer.bonusSpeed;
    }

    void FLipAllSprite(bool state)
    {
        sp.flipX = state;
        // spWeapon.flipX = state;
        spArmor.flipX = state;
    }

    public Vector2 StoreLocationPlayer()
    {
        Vector2 loc = new Vector2(transform.position.x, transform.position.y);
        return loc;
    }

    float CalculateMovementSpeed()
    {
        float result = playerHit.GetMovementImpact() + pStat.playerStatus.movespeed;
        return result;
    }


    void Running()
    {
        if (Input.GetKey(KeyCode.LeftShift)) movementPlayer.bonusSpeed = 1.5f;
        else movementPlayer.bonusSpeed = 1;
    }

    void FixedUpdate()
    {
        Movement();
    }

    void Update()
    {
        Running();
        // cameraFollowSc.TransitionCamSize(rb, movementPlayer.bonusSpeed == 1.5f);
    }


}
