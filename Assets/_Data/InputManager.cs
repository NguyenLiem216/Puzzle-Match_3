using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    private Gem selectedGem;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void OnGemClicked(Gem gem)
    {
        var board = FindObjectOfType<BoardManager>();

        if (selectedGem == null)
        {
            selectedGem = gem;
            if (board != null) board.HighlightGemTile(gem);
        }
        else
        {
            if (AreAdjacent(selectedGem, gem))
            {
                //GemManager.Instance.SwapGems(selectedGem, gem);
                StartCoroutine(SwapAndHandle(selectedGem, gem));
                if (board != null) board.UnhighlightGemTile(selectedGem);

                selectedGem = null;
            }
            else
            {
                if (board != null) board.UnhighlightGemTile(selectedGem);
                selectedGem = gem;
                if (board != null) board.HighlightGemTile(gem);
            }
        }
    }

    private IEnumerator SwapAndHandle(Gem a, Gem b)
    {
        GemManager.Instance.SwapGems(a, b);
        yield return new WaitForSeconds(0.15f); // Cho animation swap chạy (nếu có)

        var matched = GemManager.Instance.CheckMatch();
        if (matched.Count == 0)
        {
            GemManager.Instance.SwapGems(a, b); // revert
        }
        else
        {
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
            yield return new WaitForSeconds(0.1f);

            yield return StartCoroutine(GemManager.Instance.DropGemsCoroutine());

            yield return new WaitForSeconds(0.2f); // Cho drop xong và ổn định
        }
    }


}