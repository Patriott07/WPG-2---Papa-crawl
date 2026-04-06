using UnityEngine;

public class SoundForItem : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;

    void Awake()
    {
        audioSource.playOnAwake = false;
    }
    public void PlaySound(bool isRandPitch)
    {
        Debug.Log($"Panggil dari {gameObject.name}");
        audioSource.Stop();

        if (isRandPitch)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f);
        }

        else audioSource.pitch = 1f;

        audioSource.Play();
    }
}
