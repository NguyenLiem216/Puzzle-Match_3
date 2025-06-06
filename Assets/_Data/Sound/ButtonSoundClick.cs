using UnityEngine;

public class ButtonSoundPlayer : MonoBehaviour
{
    public void PlayClickSound()
    {
        SoundManager.Instance.PlayClick();
    }
}
