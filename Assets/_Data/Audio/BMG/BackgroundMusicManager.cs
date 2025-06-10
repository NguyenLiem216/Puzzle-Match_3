using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager Instance;
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // giữ nhạc qua các scene
            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject); // chỉ giữ 1
        }
    }

    public void ToggleMusic(bool isOn)
    {
        if (audioSource != null)
            audioSource.mute = !isOn;
    }
}
