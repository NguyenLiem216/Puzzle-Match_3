using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MainMenu : MonoBehaviour
{
    public GameObject popupPanel;
    private CanvasGroup popupCanvasGroup;

    private void Start()
    {
        popupCanvasGroup = popupPanel.GetComponent<CanvasGroup>();

        if (popupCanvasGroup == null)
        {
            popupCanvasGroup = popupPanel.AddComponent<CanvasGroup>();
        }

        popupPanel.SetActive(false);
    }

    public void LoadLevel(string sceneName)
    {
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            ShowPopup();
        }
    }

    public void ShowPopup()
    {
        popupPanel.SetActive(true);
        popupCanvasGroup.alpha = 0;
        popupPanel.transform.localScale = Vector3.zero;

        popupCanvasGroup.DOFade(1f, 0.3f);
        popupPanel.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
    }

    public void ClosePopup()
    {
        popupCanvasGroup.DOFade(0f, 0.3f);
        popupPanel.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack)
            .OnComplete(() => popupPanel.SetActive(false));
    }
}
