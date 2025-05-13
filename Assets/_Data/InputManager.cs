using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance; // Singleton

    private Gem selectedGem;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void OnGemClicked(Gem gem)
    {
        if (selectedGem == null)
        {
            selectedGem = gem;
            gem.board.HighlightGemTile(gem);
        }
        else
        {
            if (AreAdjacent(selectedGem, gem))
            {
                SwapGems(selectedGem, gem);
            }
            else
            {
                // Nếu không cạnh nhau → bỏ highlight viên trước
                selectedGem.board.UnhighlightGemTile(selectedGem);
            }

            // Reset lựa chọn dù swap hay không
            selectedGem = null;
        }
    }

    private bool AreAdjacent(Gem a, Gem b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) == 1;
    }

    private void SwapGems(Gem a, Gem b)
    {
        // Đổi vị trí vật lý
        Vector3 tempPos = a.transform.position;
        a.transform.position = b.transform.position;
        b.transform.position = tempPos;

        // Đổi vị trí logic
        int tempX = a.x, tempY = a.y;
        a.x = b.x; a.y = b.y;
        b.x = tempX; b.y = tempY;

        // ✅ Sau khi swap, cập nhật lại vị trí con (parent)
        Transform parentA = a.transform.parent;
        Transform parentB = b.transform.parent;

        a.transform.SetParent(parentB);
        b.transform.SetParent(parentA);

        // ✅ Bỏ highlight
        a.board.UnhighlightGemTile(a);
    }

}
