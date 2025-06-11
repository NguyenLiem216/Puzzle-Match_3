using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource clickSource;
    public AudioSource swapSource;
    public AudioSource matchSource;

    public AudioClip clickClip;
    public AudioClip swapClip;
    public AudioClip matchClip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void PlayClick()
    {
        clickSource.pitch = Random.Range(0.95f, 1.05f);
        clickSource.PlayOneShot(clickClip);
    }

    public void PlaySwap()
    {
        swapSource.pitch = Random.Range(0.95f, 1.05f);
        swapSource.PlayOneShot(swapClip);
    }

    public void PlayMatch()
    {
        matchSource.pitch = Random.Range(0.95f, 1.05f);
        matchSource.PlayOneShot(matchClip);
    }
}
