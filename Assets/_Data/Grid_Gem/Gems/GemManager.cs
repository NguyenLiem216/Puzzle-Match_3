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

    private bool initialMatchesHandled = false;

    protected virtual void Update()
    {
        if (!initialMatchesHandled && board != null)
        {
            StartCoroutine(board.HandleInitialMatches());
            initialMatchesHandled = true;
        }
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadGems();
    }

    protected virtual void LoadGems()
    {
        if (this.gemPrefabs != null && this.gemPrefabs.Count > 0) return;
        foreach (Transform child in transform)
        {
            GameObject obj = child.gameObject;
            if (obj.GetComponent<Gem>() == null) return;
            this.gemPrefabs.Add(obj);
        }
        Debug.Log(transform.name + ": LoadGems", gameObject);
    }


    public Gem SpawnGem(int x, int y, Transform parent)
    {
        int randomIndex = Random.Range(0, gemPrefabs.Count);
        GameObject gemGO = Instantiate(gemPrefabs[randomIndex], parent.position, Quaternion.identity, parent);
        gemGO.transform.localScale = Vector3.one * 0.21f;
        gemGO.transform.localPosition = Vector3.zero;

        if (gemGO.TryGetComponent<Gem>(out var gem))
        {
            gem.SetData(x, y);
            gems.Add(gem);
        }
        gem.gameObject.SetActive(true);

        return gem;
    }


    public void SwapGems(Gem a, Gem b)
    {
        // Swap vị trí thực tế
        (b.transform.position, a.transform.position) = (a.transform.position, b.transform.position);

        // Swap dữ liệu logic (tọa độ lưới)
        (a.x, a.y, b.x, b.y) = (b.x, b.y, a.x, a.y);

        // Swap cha
        Transform parentA = a.transform.parent;
        Transform parentB = b.transform.parent;
        a.transform.SetParent(parentB);
        b.transform.SetParent(parentA);
    }

    public Gem GetGemAt(int x, int y)
    {
        return gems.FirstOrDefault(g => g.x == x && g.y == y);
    }

    //public List<Gem> CheckMatch()
    //{
    //    int width = board.width;
    //    int height = board.height;

    //    List<Gem> matchedGems = new();

    //    for (int x = 0; x < width; x++)
    //    {
    //        for (int y = 0; y < height; y++)
    //        {
    //            Gem currentGem = GetGemAt(x, y);
    //            if (currentGem == null) continue;

    //            // Kiểm tra hàng ngang
    //            List<Gem> horizontalMatches = new() { currentGem };
    //            for (int i = 1; x + i < width; i++)
    //            {
    //                Gem nextGem = GetGemAt(x + i, y);
    //                if (nextGem != null && nextGem.gemType == currentGem.gemType)
    //                    horizontalMatches.Add(nextGem);
    //                else break;
    //            }

    //            if (horizontalMatches.Count >= 3)
    //                matchedGems.AddRange(horizontalMatches);

    //            // Kiểm tra hàng dọc
    //            List<Gem> verticalMatches = new() { currentGem };
    //            for (int i = 1; y + i < height; i++)
    //            {
    //                Gem nextGem = GetGemAt(x, y + i);
    //                if (nextGem != null && nextGem.gemType == currentGem.gemType)
    //                    verticalMatches.Add(nextGem);
    //                else break;
    //            }

    //            if (verticalMatches.Count >= 3)
    //                matchedGems.AddRange(verticalMatches);
    //        }
    //    }

    //    matchedGems = matchedGems.Distinct().ToList();

    //    foreach (var gem in matchedGems)
    //    {
    //        Debug.Log($"Matched Gem at ({gem.x}, {gem.y}) with type {gem.gemType}");
    //    }

    //    return matchedGems;
    //}
    public List<Gem> CheckMatch()
    {
        int width = board.width;
        int height = board.height;

        List<Gem> matchedGems = new();
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
                    matchedGems.AddRange(horizontal);
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
                    matchedGems.AddRange(vertical);
                }
            }
        }

        return matchedGems.Distinct().ToList();
    }

    public void RemoveMatchedGems(List<Gem> matchedGems)
    {
        foreach (Gem gem in matchedGems)
        {
            gems.Remove(gem);
            //Destroy(gem.gameObject);
            StartCoroutine(DestroyGemAfterDelay(gem, 0.3f));
        }
    }

    private IEnumerator DestroyGemAfterDelay(Gem gem, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (gem != null) Destroy(gem.gameObject);
    }


    //public IEnumerator DropGemsCoroutine()
    //{
    //    if (isDropping) yield break;
    //    isDropping = true;

    //    int width = board.width;

    //    bool anyGemDropped;

    //    do
    //    {
    //        anyGemDropped = false;

    //        // Tạo danh sách coroutine đang chạy cho từng cột
    //        List<Coroutine> activeCoroutines = new();

    //        for (int x = 0; x < width; x++)
    //        {
    //            //Coroutine columnRoutine = StartCoroutine(DropColumnRoutine(x, () => anyGemDropped = true));
    //            //activeCoroutines.Add(columnRoutine);
    //            yield return StartCoroutine(DropColumnRoutine(x, () => anyGemDropped = true));
    //        }

    //        // Đợi tất cả coroutine kết thúc
    //        foreach (Coroutine c in activeCoroutines)
    //        {
    //            yield return c;
    //        }

    //        yield return new WaitForSeconds(0.1f);

    //    } while (anyGemDropped);

    //    isDropping = false;
    //}
    public IEnumerator DropGemsCoroutine(System.Action onAnyGemDropped = null)
    {
        int width = board.width;
        List<Coroutine> dropCoroutines = new();

        for (int x = 0; x < width; x++)
        {
            Coroutine dropCoroutine = StartCoroutine(DropColumnRoutine(x, onAnyGemDropped));
            dropCoroutines.Add(dropCoroutine);
        }

        // Đợi tất cả coroutine xong
        foreach (var coroutine in dropCoroutines)
        {
            yield return coroutine;
        }

        yield return new WaitForSeconds(0.1f);
    }




    private Transform GetTileTransformAt(int x, int y)
    {
        Transform holder = board.transform.Find("Holder");
        if (holder == null) return null;

        // Có thể bạn sinh tile theo thứ tự (x * height + y)
        int index = x * board.height + y;
        if (index >= 0 && index < holder.childCount)
            return holder.GetChild(index);

        return null;
    }
    //private IEnumerator DropColumnRoutine(int x, System.Action onAnyGemDropped)
    //{
    //    int height = board.height;

    //    for (int y = 0; y < height; y++)
    //    {
    //        if (GetGemAt(x, y) == null)
    //        {
    //            for (int aboveY = y + 1; aboveY < height; aboveY++)
    //            {
    //                Gem aboveGem = GetGemAt(x, aboveY);
    //                if (aboveGem != null)
    //                {
    //                    Transform targetTile = GetTileTransformAt(x, y);
    //                    if (targetTile == null) continue;

    //                    Vector3 startPos = aboveGem.transform.position;
    //                    Vector3 endPos = targetTile.position;

    //                    aboveGem.transform.SetParent(targetTile);

    //                    float elapsed = 0f;
    //                    float duration = 0.2f;
    //                    while (elapsed < duration)
    //                    {
    //                        aboveGem.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
    //                        elapsed += Time.deltaTime;
    //                        yield return null;
    //                    }

    //                    aboveGem.transform.position = endPos;
    //                    aboveGem.transform.localPosition = Vector3.zero;

    //                    gems.Remove(aboveGem);
    //                    aboveGem.SetData(x, y);
    //                    gems.Add(aboveGem);

    //                    onAnyGemDropped?.Invoke(); // Gọi callback báo có gem rơi

    //                    y--; // kiểm tra lại ô này
    //                    yield return new WaitForSeconds(0.05f);
    //                    break;
    //                }
    //            }
    //        }
    //    }
    //}
    //private IEnumerator DropColumnRoutine(int x, System.Action onAnyGemDropped)
    //{
    //    int height = board.height;

    //    for (int y = 0; y < height - 1; y++)
    //    {
    //        Gem currentGem = GetGemAt(x, y);
    //        if (currentGem == null)
    //        {
    //            for (int y2 = y + 1; y2 < height; y2++)
    //            {
    //                Gem aboveGem = GetGemAt(x, y2);
    //                if (aboveGem != null)
    //                {
    //                    Transform targetTile = GetTileTransformAt(x, y);
    //                    if (targetTile == null) break;

    //                    gems.Remove(aboveGem);
    //                    aboveGem.SetData(x, y);
    //                    gems.Add(aboveGem);

    //                    Vector3 start = aboveGem.transform.position;
    //                    Vector3 end = targetTile.position;
    //                    aboveGem.transform.SetParent(targetTile);

    //                    float elapsed = 0f, duration = 0.2f;
    //                    while (elapsed < duration)
    //                    {
    //                        aboveGem.transform.position = Vector3.Lerp(start, end, elapsed / duration);
    //                        elapsed += Time.deltaTime;
    //                        yield return null;
    //                    }

    //                    aboveGem.transform.position = end;
    //                    aboveGem.transform.localPosition = Vector3.zero;

    //                    onAnyGemDropped?.Invoke();

    //                    y--; // kiểm tra lại ô này
    //                    yield return new WaitForSeconds(0.05f);
    //                    break;
    //                }
    //            }
    //        }
    //    }
    //}
    private IEnumerator DropColumnRoutine(int x, System.Action onAnyGemDropped)
    {
        int height = board.height;
        bool droppedAny = false;
        List<IEnumerator> dropAnimations = new();

        for (int y = 0; y < height - 1; y++)
        {
            if (GetGemAt(x, y) != null) continue;

            for (int y2 = y + 1; y2 < height; y2++)
            {
                Gem aboveGem = GetGemAt(x, y2);
                if (aboveGem != null)
                {
                    Transform targetTile = GetTileTransformAt(x, y);
                    if (targetTile == null) break;

                    // Logic chuyển gem xuống
                    gems.Remove(aboveGem);
                    aboveGem.SetData(x, y);
                    gems.Add(aboveGem);
                    aboveGem.transform.SetParent(targetTile);

                    dropAnimations.Add(MoveGemToPosition(aboveGem, targetTile.position));

                    droppedAny = true;
                    break;
                }
            }
        }

        if (droppedAny) onAnyGemDropped?.Invoke();

        // Chạy tất cả animation cùng lúc
        List<Coroutine> running = new();
        foreach (var anim in dropAnimations)
        {
            running.Add(StartCoroutine(anim));
        }

        foreach (var c in running)
        {
            yield return c;
        }

        yield return null;
    }
    private IEnumerator MoveGemToPosition(Gem gem, Vector3 targetPos)
    {
        Vector3 start = gem.transform.position;
        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            //gem.transform.position = Vector3.Lerp(start, targetPos, elapsed / duration);
            //elapsed += Time.deltaTime;
            //yield return null;
            float t = elapsed / duration;
            float easedT = EaseOutBounce(t);
            gem.transform.position = Vector3.Lerp(start, targetPos, easedT);
            elapsed += Time.deltaTime;
            yield return null;
        }

        gem.transform.position = targetPos;
        gem.transform.localPosition = Vector3.zero;
    }
    private float EaseOutBounce(float t)
    {
        if (t < (1 / 2.75f))
        {
            return 7.5625f * t * t;
        }
        else if (t < (2 / 2.75f))
        {
            t -= 1.5f / 2.75f;
            return 7.5625f * t * t + 0.75f;
        }
        else if (t < (2.5f / 2.75f))
        {
            t -= 2.25f / 2.75f;
            return 7.5625f * t * t + 0.9375f;
        }
        else
        {
            t -= 2.625f / 2.75f;
            return 7.5625f * t * t + 0.984375f;
        }
    }

}