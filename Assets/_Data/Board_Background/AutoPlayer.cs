using System.Collections;
using UnityEngine;

public class AutoPlayer : MonoBehaviour
{
    public float delayBetweenActions = 0.5f; // 0.5s giữa các hành động
    private bool isPlaying = false;
    private BoardManager board;

    private void Start()
    {
        board = FindObjectOfType<BoardManager>(); // Tìm BoardManager
        StartCoroutine(AutoPlay());
    }

    private IEnumerator AutoPlay()
    {
        yield return new WaitForSeconds(1f); // Đợi game setup xong

        isPlaying = true;
        while (isPlaying)
        {
            HintManager hintMgr = FindObjectOfType<HintManager>();
            if (hintMgr != null)
                hintMgr.SendMessage("ClearHint");

            if (!HasValidMove())
            {
                Debug.LogWarning("❌ Không còn swap hợp lệ! Board bị khóa.");
                isPlaying = false;
                yield return StartCoroutine(GemManager.Instance.ShuffleBoard());
                yield break;
            }

            TryRandomSwap();
            yield return new WaitForSeconds(delayBetweenActions);
        }
    }

    private void TryRandomSwap()
    {
        if (GemManager.Instance == null) return;

        HintData bestHint = GemManager.Instance.FindBestHint();
        if (bestHint != null)
        {
            StartCoroutine(SimulateClick(bestHint.sourceGem, bestHint.targetGem));
        }
    }


    private IEnumerator SimulateClick(Gem gemA, Gem gemB)
    {
        InputManager.Instance.OnGemClicked(gemA);
        InputManager.Instance.OnGemClicked(gemB);
        yield return null;
    }
    private bool HasValidMove()
    {
        return GemManager.Instance.HasValidMove();
    }

}
