using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
public class Gem : LiemMonoBehaviour, IPointerDownHandler
{
    public int x, y;
    public string gemType;

    public void OnPointerDown(PointerEventData eventData)
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
