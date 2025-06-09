using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    private Gem selectedGem;
    private BoardManager board;
    private bool isInputLocked = false;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        this.board = FindObjectOfType<BoardManager>();
    }


    public void OnGemClicked(Gem gem)
    {
        if (isInputLocked) return;
        SoundManager.Instance.PlayClick();

        if (selectedGem == null)
        {
            selectedGem = gem;
            selectedGem.transform.DOPunchScale(Vector3.one * 0.15f, 0.2f, 2, 0.5f);
            if (this.board != null) this.board.HighlightGemTile(gem);
        }
        else
        {
            if (AreAdjacent(selectedGem, gem))
            {
                StartCoroutine(SwapAndHandle(selectedGem, gem));
                gem.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 2, 0.5f);
                if (this.board != null) this.board.UnhighlightGemTile(selectedGem);

                selectedGem = null;
            }
            else
            {
                if (this.board != null) this.board.UnhighlightGemTile(selectedGem);
                selectedGem = gem;
                if (this.board != null) this.board.HighlightGemTile(gem);
            }
        }
    }


    private IEnumerator SwapAndHandle(Gem a, Gem b)
    {
        if (GemManager.Instance == null) yield break;
        isInputLocked = true;

        GemManager.Instance.SwapGems(a, b);
        SoundManager.Instance.PlaySwap();

        yield return new WaitForSeconds(0.25f);

        var matched = GemManager.Instance.CheckMatch();
        if (matched.Count == 0)
        {
            GemManager.Instance.SwapGems(a, b);
            SoundManager.Instance.PlaySwap();
        }
        else
        {
            UIManager.Instance.UseMove();
            yield return StartCoroutine(HandleMatchesAfterSwap());
        }
        isInputLocked = false;
    }




    private bool AreAdjacent(Gem a, Gem b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) == 1;
    }
    private IEnumerator HandleMatchesAfterSwap()
    {
        yield return new WaitForSeconds(0.1f); // Cho swap xong hẳn

        while (true)
        {
            List<Gem> matched = GemManager.Instance.CheckMatch();

            if (matched.Count == 0)
                break;

            GemManager.Instance.RemoveMatchedGems(matched);
            yield return new WaitForSeconds(0.3f); // Đợi gem nổ

            yield return StartCoroutine(GemManager.Instance.FillBoard()); // <-- Đợi fill xong
            yield return new WaitForSeconds(0.1f); // Đợi nhỏ sau fill
        }
        if (!HasValidMove())
        {
            yield return StartCoroutine(GemManager.Instance.ShuffleBoard());
        }

    }
    public bool HasValidMove()
    {
        int width = board.width;
        int height = board.height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Gem current = GemManager.Instance.GetGemAt(x, y);
                if (current == null) continue;

                // Check Right
                if (x < width - 1)
                {
                    Gem rightGem = GemManager.Instance.GetGemAt(x + 1, y);
                    if (rightGem != null && CanSwapAndMatch(current, rightGem))
                        return true;
                }

                // Check Up
                if (y < height - 1)
                {
                    Gem upGem = GemManager.Instance.GetGemAt(x, y + 1);
                    if (upGem != null && CanSwapAndMatch(current, upGem))
                        return true;
                }
            }
        }
        return false;
    }

    private bool CanSwapAndMatch(Gem a, Gem b)
    {
        SwapTemp(a, b);

        bool match = CheckMatchAt(a.x, a.y) || CheckMatchAt(b.x, b.y);

        SwapTemp(a, b); // swap lại
        return match;
    }

    private void SwapTemp(Gem a, Gem b)
    {
        (a.x, b.x) = (b.x, a.x);
        (a.y, b.y) = (b.y, a.y);
    }

    private bool CheckMatchAt(int x, int y)
    {
        Gem gem = GemManager.Instance.GetGemAt(x, y);
        if (gem == null) return false;

        string type = gem.gemType;

        // Horizontal check
        int count = 1;
        for (int i = x - 1; i >= 0; i--)
        {
            Gem neighbor = GemManager.Instance.GetGemAt(i, y);
            if (neighbor != null && neighbor.gemType == type) count++;
            else break;
        }
        for (int i = x + 1; i < board.width; i++)
        {
            Gem neighbor = GemManager.Instance.GetGemAt(i, y);
            if (neighbor != null && neighbor.gemType == type) count++;
            else break;
        }
        if (count >= 3) return true;

        // Vertical check
        count = 1;
        for (int j = y - 1; j >= 0; j--)
        {
            Gem neighbor = GemManager.Instance.GetGemAt(x, j);
            if (neighbor != null && neighbor.gemType == type) count++;
            else break;
        }
        for (int j = y + 1; j < board.height; j++)
        {
            Gem neighbor = GemManager.Instance.GetGemAt(x, j);
            if (neighbor != null && neighbor.gemType == type) count++;
            else break;
        }
        if (count >= 3) return true;

        return false;
    }

    public void LockInput()
    {
        isInputLocked = true;
    }

    public void UnlockInput()
    {
        isInputLocked = false;
    }

}