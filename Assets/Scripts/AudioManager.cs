using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioClip clipSelect;
    public AudioClip clipDelete;
    public AudioClip clipStepRotation;
    public AudioClip clipStepScale;
    public AudioClip clipStepMove;
    public AudioClip clipStepDimension;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySelect()
    {
        if (clipSelect) audioSource.PlayOneShot(clipSelect);
    }

    public void PlayDelete()
    {
        if (clipDelete) audioSource.PlayOneShot(clipDelete);
    }

    public void PlayStepRotation()
    {
        if (clipStepRotation) audioSource.PlayOneShot(clipStepRotation);
    }

    public void PlayStepScale()
    {
        if (clipStepScale) audioSource.PlayOneShot(clipStepScale);
    }

    public void PlayStepMove()
    {
        if (clipStepMove) audioSource.PlayOneShot(clipStepMove);
    }

    public void PlayStepDimension()
    {
        if (clipStepDimension) audioSource.PlayOneShot(clipStepDimension);
    }
}
