using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioClip clipMusic;
    public AudioClip clipSelect;
    public AudioClip clipDelete;
    public AudioClip clipStepRotation;
    public AudioClip clipStepScale;
    public AudioClip clipStepMove;
    public AudioClip clipStepDimension;
    public AudioClip clipStartGame;
    public AudioClip clipStartLevel;
    public AudioClip clipEndGame;

    private AudioSource sfxSource;
    private AudioSource musicSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Setup Sources
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;

            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = true;

            if (clipMusic != null)
            {
                musicSource.clip = clipMusic;
                musicSource.Play();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySelect()
    {
        if (clipSelect) sfxSource.PlayOneShot(clipSelect);
    }

    public void PlayDelete()
    {
        if (clipDelete) sfxSource.PlayOneShot(clipDelete);
    }

    public void PlayStepRotation()
    {
        if (clipStepRotation) sfxSource.PlayOneShot(clipStepRotation);
    }

    public void PlayStepScale()
    {
        if (clipStepScale) sfxSource.PlayOneShot(clipStepScale);
    }

    public void PlayStepMove()
    {
        if (clipStepMove) sfxSource.PlayOneShot(clipStepMove);
    }

    public void PlayStepDimension()
    {
        if (clipStepDimension) sfxSource.PlayOneShot(clipStepDimension);
    }

    public void PlayStartGame()
    {
        if (clipStartGame) sfxSource.PlayOneShot(clipStartGame);
    }

    public void PlayStartLevel()
    {
        if (clipStartLevel) sfxSource.PlayOneShot(clipStartLevel);
    }

    public void PlayEndGame()
    {
        if (clipEndGame) sfxSource.PlayOneShot(clipEndGame);
    }
}
