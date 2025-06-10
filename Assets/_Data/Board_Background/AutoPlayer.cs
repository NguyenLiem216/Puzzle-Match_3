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

        int width = GemManager.Instance.GetBoardWidth();
        int height = GemManager.Instance.GetBoardHeight();

        int x = Random.Range(0, width);
        int y = Random.Range(0, height);

        Gem gem = GemManager.Instance.GetGemAt(x, y);
        if (gem == null) return;

        Vector2Int[] directions = {
            new(0, 1),
            new(1, 0),
            new(0, -1),
            new(-1, 0)
        };

        Vector2Int dir = directions[Random.Range(0, directions.Length)];

        int newX = x + dir.x;
        int newY = y + dir.y;

        Gem neighbor = GemManager.Instance.GetGemAt(newX, newY);
        if (neighbor == null) return;

        StartCoroutine(SimulateClick(gem, neighbor));
    }

    private IEnumerator SimulateClick(Gem gemA, Gem gemB)
    {
        InputManager.Instance.OnGemClicked(gemA);
        InputManager.Instance.OnGemClicked(gemB);
        yield return null;
    }


    // ⚡ Hàm check move hợp lệ
    private bool HasValidMove()
    {
        int width = board.width;
        int height = board.height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Gem currentGem = GemManager.Instance.GetGemAt(x, y);
                if (currentGem == null) continue;

                // Check Right
                if (x < width - 1)
                {
                    Gem rightGem = GemManager.Instance.GetGemAt(x + 1, y);
                    if (rightGem != null && CanSwapAndMatch(currentGem, rightGem))
                        return true;
                }

                // Check Up
                if (y < height - 1)
                {
                    Gem upGem = GemManager.Instance.GetGemAt(x, y + 1);
                    if (upGem != null && CanSwapAndMatch(currentGem, upGem))
                        return true;
                }
            }
        }

        return false;
    }

    private bool CanSwapAndMatch(Gem a, Gem b)
    {
        GemManager.Instance.SwapGemsDataOnly(a, b);

        bool matchFound = HasMatch(a) || HasMatch(b);

        GemManager.Instance.SwapGemsDataOnly(a, b); // Swap lại về

        return matchFound;
    }

    private bool HasMatch(Gem gem)
    {
        if (gem == null) return false;

        string type = gem.gemType;
        int x = gem.x;
        int y = gem.y;

        int count = 1;

        // Check Horizontal
        for (int i = x - 1; i >= 0; i--)
        {
            Gem checkGem = GemManager.Instance.GetGemAt(i, y);
            if (checkGem != null && checkGem.gemType == type) count++;
            else break;
        }
        for (int i = x + 1; i < board.width; i++)
        {
            Gem checkGem = GemManager.Instance.GetGemAt(i, y);
            if (checkGem != null && checkGem.gemType == type) count++;
            else break;
        }
        if (count >= 3) return true;

        // Check Vertical
        count = 1;
        for (int j = y - 1; j >= 0; j--)
        {
            Gem checkGem = GemManager.Instance.GetGemAt(x, j);
            if (checkGem != null && checkGem.gemType == type) count++;
            else break;
        }
        for (int j = y + 1; j < board.height; j++)
        {
            Gem checkGem = GemManager.Instance.GetGemAt(x, j);
            if (checkGem != null && checkGem.gemType == type) count++;
            else break;
        }
        if (count >= 3) return true;

        return false;
    }
}
