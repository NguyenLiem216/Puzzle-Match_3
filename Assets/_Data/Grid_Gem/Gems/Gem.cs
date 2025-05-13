using UnityEngine;

public class Gem : MonoBehaviour
{
    public int x, y;
    public string gemType;

    public BoardManager board;

    private void Start()
    {
        board = FindObjectOfType<BoardManager>();
    }

    private void OnMouseDown()
    {
        if (board != null)
        {
            InputManager.Instance.OnGemClicked(this);
        }
    }

    public void SetData(int x, int y, string type, BoardManager boardManager)
    {
        this.x = x;
        this.y = y;
        this.gemType = type;
        this.board = boardManager;
    }
}
