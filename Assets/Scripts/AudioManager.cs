using UnityEngine;

public class AudioManager : MonoBehaviour {
    AudioSource audioSource;
    [Header("Audio Clips")]
    public AudioClip walkSound;
    public AudioClip jumpSound;
    public AudioClip landSound;
    public AudioClip pickupSound;
    public AudioClip doorSound;
    public AudioClip teleportSound;
    public AudioClip liquidSound;

    public enum SoundType {
        Walk,
        Jump,
        Land,
        Pickup,
        Door,
        Teleport,
        Liquid
    }

    void Start() {
        audioSource = GetComponent<AudioSource>();
        SetVolume(PlayerPrefs.GetFloat("volume", 0.5f));
    }

    public void PlaySound(SoundType soundType) {
        switch (soundType) {
            case SoundType.Walk:
                audioSource.PlayOneShot(walkSound);
                break;
            case SoundType.Jump:
                audioSource.PlayOneShot(jumpSound);
                break;
            case SoundType.Land:
                audioSource.PlayOneShot(landSound);
                break;
            case SoundType.Pickup:
                audioSource.PlayOneShot(pickupSound);
                break;
            case SoundType.Door:
                audioSource.PlayOneShot(doorSound);
                break;
            case SoundType.Teleport:
                audioSource.PlayOneShot(teleportSound);
                break;
            case SoundType.Liquid:
                audioSource.PlayOneShot(liquidSound);
                break;
        }
    }

    public void SetVolume(float volume) {
        audioSource.volume = volume;
    }
}