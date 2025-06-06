using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GemManager : LiemMonoBehaviour
{
    public static GemManager Instance { get; private set; }

    [SerializeField] private List<GameObject> gemPrefabs;

    private readonly List<Gem> gems = new();
    private BoardManager board;


    protected override void Awake()
    {
        base.Awake();
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    protected override void Start()
    {
        base.Start();
        board = FindObjectOfType<BoardManager>();
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadGems();
    }

    protected virtual void LoadGems()
    {
        if (this.gemPrefabs?.Count > 0) return;
        foreach (Transform child in transform)
        {
            GameObject obj = child.gameObject;
            if (obj.GetComponent<Gem>() == null) return;
            this.gemPrefabs.Add(obj);
        }
        Debug.Log(transform.name + ": LoadGems", gameObject);
    }


    //public Gem SpawnGem(int x, int y, Transform parent)
    //{
    //    int randomIndex = Random.Range(0, gemPrefabs.Count);
    //    GameObject gemGO = Instantiate(gemPrefabs[randomIndex], parent.position, Quaternion.identity, parent);
    //    gemGO.transform.localScale = Vector3.one * 0.21f;
    //    gemGO.transform.localPosition = Vector3.zero;

    //    if (gemGO.TryGetComponent<Gem>(out var gem))
    //    {
    //        gem.SetData(x, y);
    //        gems.Add(gem);
    //    }
    //    gem.gameObject.SetActive(true);

    //    return gem;
    //}
    public Gem SpawnGem(int x, int y, Transform parent)
    {
        int maxTry = 5;
        Gem gem = null;
        bool foundValid = false;

        GameObject gemGO;
        for (int tryCount = 0; tryCount < maxTry; tryCount++)
        {
            int randomIndex = Random.Range(0, gemPrefabs.Count);
            gemGO = Instantiate(gemPrefabs[randomIndex], parent.position, Quaternion.identity, parent);
            gemGO.transform.localScale = Vector3.one * 0.21f;
            gemGO.transform.localPosition = Vector3.zero;

            if (gemGO.TryGetComponent<Gem>(out gem))
            {
                gem.SetData(x, y);

                if (!WouldFormMatch(x, y, gem.gemType))
                {
                    foundValid = true;
                    break; // Tìm được viên không match
                }
            }
            Destroy(gemGO); // Destroy nếu không hợp lệ
        }

        if (!foundValid)
        {
            // Nếu thử 5 lần mà không tìm ra -> phải accept viên random
            int randomIndex = Random.Range(0, gemPrefabs.Count);
            gemGO = Instantiate(gemPrefabs[randomIndex], parent.position, Quaternion.identity, parent);
            gemGO.transform.localScale = Vector3.one * 0.21f;
            gemGO.transform.localPosition = Vector3.zero;

            if (gemGO.TryGetComponent<Gem>(out gem))
            {
                gem.SetData(x, y);
            }
        }

        gems.Add(gem);
        gem.transform.SetParent(parent);
        gem.gameObject.SetActive(true);
        return gem;
    }


    private bool WouldFormMatch(int x, int y, string gemType)
    {
        // Check horizontal
        int matchLeft = 0;
        int matchRight = 0;

        // Check left
        if (GetGemAt(x - 1, y) != null && GetGemAt(x - 1, y).gemType == gemType)
            matchLeft++;
        if (GetGemAt(x - 2, y) != null && GetGemAt(x - 2, y).gemType == gemType)
            matchLeft++;

        // Check right
        if (GetGemAt(x + 1, y) != null && GetGemAt(x + 1, y).gemType == gemType)
            matchRight++;
        if (GetGemAt(x + 2, y) != null && GetGemAt(x + 2, y).gemType == gemType)
            matchRight++;

        if (matchLeft >= 2 || matchRight >= 2 || (matchLeft == 1 && matchRight == 1))
            return true;

        // Check vertical
        int matchDown = 0;
        int matchUp = 0;

        if (GetGemAt(x, y - 1) != null && GetGemAt(x, y - 1).gemType == gemType)
            matchDown++;
        if (GetGemAt(x, y - 2) != null && GetGemAt(x, y - 2).gemType == gemType)
            matchDown++;

        if (GetGemAt(x, y + 1) != null && GetGemAt(x, y + 1).gemType == gemType)
            matchUp++;
        if (GetGemAt(x, y + 2) != null && GetGemAt(x, y + 2).gemType == gemType)
            matchUp++;

        if (matchDown >= 2 || matchUp >= 2 || (matchDown == 1 && matchUp == 1))
            return true;

        return false;
    }


    public void SwapGems(Gem a, Gem b)
    {
        var parentA = a.transform.parent;
        var parentB = b.transform.parent;

        // Animate move position
        float swapDuration = 0.25f;
        a.transform.DOMove(parentB.position, swapDuration);
        b.transform.DOMove(parentA.position, swapDuration);

        // Swap logic data ngay lập tức
        (a.x, a.y, b.x, b.y) = (b.x, b.y, a.x, a.y);

        a.transform.SetParent(parentB);
        b.transform.SetParent(parentA);
    }


    public Gem GetGemAt(int x, int y)
    {
        return gems.FirstOrDefault(g => g.x == x && g.y == y);
    }


    public List<Gem> CheckMatch()
    {
        int width = board.width;
        int height = board.height;

        HashSet<Gem> matchedGems = new();
        bool[,] alreadyMatched = new bool[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Gem current = GetGemAt(x, y);
                if (current == null || alreadyMatched[x, y]) continue;

                // Horizontal check
                List<Gem> horizontal = new() { current };
                for (int i = x + 1; i < width; i++)
                {
                    Gem next = GetGemAt(i, y);
                    if (next != null && next.gemType == current.gemType)
                        horizontal.Add(next);
                    else break;
                }
                if (horizontal.Count >= 3)
                {
                    foreach (var g in horizontal)
                        alreadyMatched[g.x, g.y] = true;
                    matchedGems.UnionWith(horizontal);
                }

                // Vertical check
                List<Gem> vertical = new() { current };
                for (int i = y + 1; i < height; i++)
                {
                    Gem next = GetGemAt(x, i);
                    if (next != null && next.gemType == current.gemType)
                        vertical.Add(next);
                    else break;
                }
                if (vertical.Count >= 3)
                {
                    foreach (var g in vertical)
                        alreadyMatched[g.x, g.y] = true;
                    matchedGems.UnionWith(vertical);
                }
            }
        }

        return matchedGems.ToList();

    }

    public void RemoveMatchedGems(List<Gem> matchedGems)
    {
        if (matchedGems.Count > 0)
            SoundManager.Instance.PlayMatch(); // 🎵 Play Match SFX

        foreach (Gem gem in matchedGems)
        {
            gems.Remove(gem);
            StartCoroutine(DestroyGemAfterDelay(gem, 0.3f));
        }

        int points = matchedGems.Count * 20;
        UIManager.Instance.AddScore(points);
    }


    private IEnumerator DestroyGemAfterDelay(Gem gem, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (gem != null)
        {
            if (gem.TryGetComponent<SpriteRenderer>(out var sr))
            {
                Sequence seq = DOTween.Sequence();
                seq.Join(gem.transform.DOScale(Vector3.zero, 0.3f)); // Scale nhỏ lại
                seq.Join(sr.DOFade(0, 0.3f)); // Fade out

                yield return seq.WaitForCompletion();
            }

            Destroy(gem.gameObject);
        }
    }

    public IEnumerator FillBoard()
    {
        int width = board.width;
        int height = board.height;
        List<Coroutine> activeCoroutines = new();

        for (int x = 0; x < width; x++)
        {
            int emptySpaces = 0;
            for (int y = 0; y < height; y++)
            {
                Gem gem = GetGemAt(x, y);
                if (gem == null)
                {
                    emptySpaces++;
                }
                else if (emptySpaces > 0)
                {
                    gem.y -= emptySpaces;
                    Transform parent = board.transform.Find($"Holder/TitleCell_({x},{gem.y})");
                    if (parent != null)
                    {
                        gem.transform.parent = parent;
                        Coroutine moveCoroutine = StartCoroutine(MoveGem(gem.transform, parent.position));
                        activeCoroutines.Add(moveCoroutine);
                    }
                }
            }

            for (int i = 0; i < emptySpaces; i++)
            {
                int spawnY = height - emptySpaces + i;
                Transform parent = board.transform.Find($"Holder/TitleCell_({x},{spawnY})");
                if (parent != null)
                {
                    Gem newGem = SpawnGem(x, spawnY, parent);
                    newGem.transform.localPosition = Vector3.zero;
                    newGem.transform.position += Vector3.up * 4f;
                    Coroutine moveCoroutine = StartCoroutine(MoveGem(newGem.transform, parent.position));
                    activeCoroutines.Add(moveCoroutine);
                }
            }
        }

        // Wait until all coroutines finished
        // Because Unity không cho yield return Coroutine[], ta đơn giản chờ
        yield return new WaitForSeconds(0.35f); // Giả lập thời gian rớt tất cả (MoveGem đang set 0.3s -> đợi 0.35s cho chắc)
    }

    private IEnumerator MoveGem(Transform gem, Vector3 target)
    {
        float elapsedTime = 0f;
        float duration = 0.3f;
        Vector3 start = gem.position;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            // Easing: SmoothStep (chậm đầu, nhanh cuối)
            float easedT = t * t * (3f - 2f * t);

            gem.position = Vector3.Lerp(start, target, easedT);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        gem.position = target; // Đặt chính xác
    }

}