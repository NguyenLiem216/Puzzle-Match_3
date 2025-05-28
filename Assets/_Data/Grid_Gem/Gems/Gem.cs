using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Gem : LiemMonoBehaviour
{
    public int x, y;
    public string gemType;

    private void OnMouseDown()
    {
        InputManager.Instance.OnGemClicked(this);
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadGemType();
    }

    protected virtual void LoadGemType()
    {
        if (!string.IsNullOrEmpty(this.gemType)) return;
        this.gemType = gameObject.name;
        Debug.Log($"GemType loaded: {gemType}", gameObject);
    }

    public void SetData(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}
