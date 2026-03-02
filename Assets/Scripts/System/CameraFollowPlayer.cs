using Unity.VisualScripting;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public Transform player;          // Target player
    public Vector3 offset;            // Jarak kamera dari player
    public float smoothSpeed = 5f;    // Kecepatan follow (semakin besar semakin cepat)
    public float cameraSizeWhileNotMove = 6.5f, cameraSizeWhileMove = 5.5f, zoomSpeed =5f;
    Camera cam;

    void Awake()
    {
        cam = gameObject.GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 desiredPosition = player.position + offset;
        desiredPosition.z = -10;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.position = smoothedPosition;
        // transform.LookAt(player);
    }

    public void TransitionCamSize(Rigidbody2D rb)
    {
        float targetZoom;

        // Cek apakah player bergerak
        if (rb.linearVelocity.magnitude > 0.1f)
        {
            targetZoom = cameraSizeWhileMove;
        }
        else
        {
            targetZoom = cameraSizeWhileNotMove;
        }

        // Smooth zoom
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, zoomSpeed * Time.deltaTime);
    }
}
