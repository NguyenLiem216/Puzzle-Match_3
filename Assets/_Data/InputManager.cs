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
            board?.HighlightGemTile(gem);
        }
        else
        {
            if (AreAdjacent(selectedGem, gem))
            {
                GemManager.Instance.SwapGems(selectedGem, gem);
                board?.UnhighlightGemTile(selectedGem);
                List<Gem> matched = GemManager.Instance.CheckMatch();
                if (matched.Count == 0)
                {
                    // Nếu không match, swap lại
                    GemManager.Instance.SwapGems(selectedGem, gem);
                }
                selectedGem = null;
            }
            else
            {
                // Unhighlight viên cũ
                board?.UnhighlightGemTile(selectedGem);

                // Chọn viên mới
                selectedGem = gem;
                board?.HighlightGemTile(gem);
            }
        }
    }

    private bool AreAdjacent(Gem a, Gem b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) == 1;
    }
}
