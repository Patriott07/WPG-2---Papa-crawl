using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerSounds : MonoBehaviour
{
    public AudioSource levelup, collect1, collect2, slingshot, bow, dagger;

    public static PlayerSounds Instance;

    void Awake()
    {
        Instance = this;
        levelup.playOnAwake = false;
        collect1.playOnAwake = false;
        collect2.playOnAwake = false;
    }

    public void PlaySound(AudioSource audioSource, bool isRandPitch)
    {
        audioSource.Stop();

        if (isRandPitch)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
        }
        else audioSource.pitch = 1f;

        audioSource.Play();
    }

    public IEnumerator PlayDelayBeforeSound(float d, AudioSource audioSource)
    {
        audioSource.Stop();
        yield return new WaitForSeconds(d);
        audioSource.Play();
    }
}
