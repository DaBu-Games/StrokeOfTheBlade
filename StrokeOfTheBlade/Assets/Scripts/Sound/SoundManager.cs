using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }
    }
    
    public void PlaySfx(AudioClip clip, float volume = 1.0f)
    {
        if (clip is null) 
            return;
        
        sfxSource.Stop();
        sfxSource.clip = clip;
        sfxSource.volume = volume;
        sfxSource.Play();
    }
}
