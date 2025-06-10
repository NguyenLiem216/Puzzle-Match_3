//using UnityEngine;
//using System.Collections.Generic;
//using DG.Tweening;

//public class HintManager : MonoBehaviour
//{
//    public static HintManager Instance;

//    private float idleTime = 0f;
//    public float timeBeforeHint = 5f;
//    private bool hintShown = false;

//    private void Awake()
//    {
//        if (Instance == null) Instance = this;
//        else Destroy(gameObject);
//    }

//    private void Update()
//    {
//        idleTime += Time.deltaTime;

//        if (idleTime >= timeBeforeHint && !hintShown)
//        {
//            ShowHint();
//            hintShown = true;
//        }
//    }

//    public void ResetTimer()
//    {
//        idleTime = 0f;
//        hintShown = false;
//    }

//    private void ShowHint()
//    {
//        // Tìm 1 cặp swap được
//        foreach (Gem gem in GemManager.Instance.GetAllGems())
//        {
//            foreach (Vector2Int dir in new Vector2Int[] { Vector2Int.right, Vector2Int.up })
//            {
//                Gem neighbor = GemManager.Instance.GetGemAt(gem.x + dir.x, gem.y + dir.y);
//                if (neighbor != null && CanSwapAndMatch(gem, neighbor))
//                {
//                    // Highlight
//                    gem.transform.DOShakeScale(1f, 0.1f, 5, 90f);
//                    neighbor.transform.DOShakeScale(1f, 0.1f, 5, 90f);
//                    return;
//                }
//            }
//        }
//    }

//    private bool CanSwapAndMatch(Gem a, Gem b)
//    {
//        (a.x, b.x) = (b.x, a.x);
//        (a.y, b.y) = (b.y, a.y);

//        bool matchFound = GemManager.Instance.CheckMatch().Count > 0;

//        (a.x, b.x) = (b.x, a.x);
//        (a.y, b.y) = (b.y, a.y);

//        return matchFound;
//    }
//}
