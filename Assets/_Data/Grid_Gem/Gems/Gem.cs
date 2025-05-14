using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Gem : MonoBehaviour
{
    public int x, y;
    public string gemType;

    private void OnMouseDown()
    {
        InputManager.Instance.OnGemClicked(this);
    }

    public void SetData(int x, int y, string type)
    {
        this.x = x;
        this.y = y;
        this.gemType = type;
    }
}
