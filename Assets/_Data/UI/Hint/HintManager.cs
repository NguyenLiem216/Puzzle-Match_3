using System.Collections;
using UnityEngine;
using DG.Tweening;

public class HintManager : MonoBehaviour
{
    public float idleDelay = 5f;
    private float idleTimer = 0f;
    private bool hintShown = false;
    private HintData currentHint;
    private Tween moveA, moveB;

    void Update()
    {
        if (UIManager.Instance == null) return;
        if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
        {
            idleTimer = 0f;
            ClearHint();
            return;
        }

        idleTimer += Time.deltaTime;

        if (!hintShown && idleTimer >= idleDelay)
        {
            ShowHint();
        }
    }

    void ShowHint()
    {
        currentHint = GemManager.Instance.FindBestHint();
        if (currentHint == null) return;

        hintShown = true;

        Vector3 posA = currentHint.sourceGem.transform.position;
        Vector3 posB = currentHint.targetGem.transform.position;

        Vector3 dir = (posB - posA).normalized * 0.15f; // Đẩy nhẹ về phía nhau

        moveA = currentHint.sourceGem.transform
            .DOMove(posA + dir, 0.3f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);

        moveB = currentHint.targetGem.transform
            .DOMove(posB - dir, 0.3f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    void ClearHint()
    {
        hintShown = false;
        idleTimer = 0f;

        if (moveA != null && moveA.IsActive()) moveA.Kill(true);
        if (moveB != null && moveB.IsActive()) moveB.Kill(true);

        if (currentHint != null)
        {
            // Reset vị trí về đúng chỗ (đề phòng lệch)
            var gemA = currentHint.sourceGem;
            var gemB = currentHint.targetGem;

            if (gemA != null)
            {
                Transform parent = gemA.transform.parent;
                if (parent != null)
                    gemA.transform.position = parent.position;
            }

            if (gemB != null)
            {
                Transform parent = gemB.transform.parent;
                if (parent != null)
                    gemB.transform.position = parent.position;
            }
        }

        currentHint = null;
    }

}
