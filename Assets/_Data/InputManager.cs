using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    private Gem selectedGem;
    private BoardManager board;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        this.board = FindObjectOfType<BoardManager>();
    }


    public void OnGemClicked(Gem gem)
    {
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
    }


}