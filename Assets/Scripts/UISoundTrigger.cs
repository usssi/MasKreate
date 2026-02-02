using UnityEngine;

/// <summary>
/// A proxy script to safely trigger sounds from UI buttons without losing references
/// when a local AudioManager is destroyed due to Singleton behavior.
/// </summary>
public class UISoundTrigger : MonoBehaviour
{
    public void PlaySelect()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySelect();
    }

    public void PlayDelete()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayDelete();
    }

    public void PlayStartGame()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayStartGame();
    }

    public void PlayStartLevel()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayStartLevel();
    }

    public void PlayEndGame()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayEndGame();
    }

    public void PlaySave()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySave();
    }
}
