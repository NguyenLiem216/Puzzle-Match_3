using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GemManager : LiemMonoBehaviour
{
    public static GemManager Instance { get; private set; }

    

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


    public Gem SpawnGem(int x, int y, Transform parent)
    {
        int maxTry = 5;
        Gem gem = null;
        bool foundValid = false;

        GameObject gemGO;

        for (int tryCount = 0; tryCount < maxTry; tryCount++)
        {
            gemGO = GemPoolManager.Instance.GetRandomGem(parent);
            if (gemGO.TryGetComponent<Gem>(out gem))
            {
                gem.SetData(x, y);

                if (!WouldFormMatch(x, y, gem.gemType))
                {
                    foundValid = true;
                    break;
                }
            }

            GemPoolManager.Instance.ReturnGem(gemGO);
        }

        if (!foundValid)
        {
            gemGO = GemPoolManager.Instance.GetRandomGem(parent);
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
    public void SwapGemsDataOnly(Gem a, Gem b)
    {
        // Chỉ swap logic data
        (a.x, a.y, b.x, b.y) = (b.x, b.y, a.x, a.y);

        var parentA = a.transform.parent;
        var parentB = b.transform.parent;

        a.transform.SetParent(parentB, true);
        b.transform.SetParent(parentA, true);

        // Không DOMove
    }

    private void SwapGemsData(Gem a, Gem b)
    {
        (a.x, b.x) = (b.x, a.x);
        (a.y, b.y) = (b.y, a.y);
    }
    private int CountPotentialMatch(Gem gem)
    {
        if (gem == null) return 0;

        string type = gem.gemType;
        int x = gem.x;
        int y = gem.y;

        int horizontal = 1;
        for (int i = x - 1; i >= 0; i--)
        {
            var g = GetGemAt(i, y);
            if (g != null && g.gemType == type) horizontal++;
            else break;
        }
        for (int i = x + 1; i < board.width; i++)
        {
            var g = GetGemAt(i, y);
            if (g != null && g.gemType == type) horizontal++;
            else break;
        }

        int vertical = 1;
        for (int j = y - 1; j >= 0; j--)
        {
            var g = GetGemAt(x, j);
            if (g != null && g.gemType == type) vertical++;
            else break;
        }
        for (int j = y + 1; j < board.height; j++)
        {
            var g = GetGemAt(x, j);
            if (g != null && g.gemType == type) vertical++;
            else break;
        }

        return Mathf.Max(horizontal, vertical);
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
            SoundManager.Instance.PlayMatch();

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
                seq.Join(gem.transform.DOScale(Vector3.zero, 0.3f));
                seq.Join(sr.DOFade(0, 0.3f));

                yield return seq.WaitForCompletion();
            }

            DOTween.Kill(gem.transform);
            GemPoolManager.Instance.ReturnGem(gem.gameObject);
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
                    Transform parent = board.GetTileAt(x, gem.y);
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
                Transform parent = board.GetTileAt(x, spawnY);
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
        if (gem == null) yield break; // thêm dòng này check null trước

        float elapsedTime = 0f;
        float duration = 0.3f;
        Vector3 start = gem.position;

        while (elapsedTime < duration)
        {
            if (gem == null) yield break; // thêm check trong khi chạy tween

            float t = elapsedTime / duration;
            float easedT = t * t * (3f - 2f * t);
            gem.position = Vector3.Lerp(start, target, easedT);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (gem != null) gem.position = target; // cuối cùng set đúng vị trí
    }
    public int GetBoardWidth()
    {
        return board.width;
    }

    public int GetBoardHeight()
    {
        return board.height;
    }
    public IEnumerator ShuffleBoard()
    {
        Debug.LogWarning("🔀 Shuffle board vì bị deadlock...");

        // Khoá Input
        InputManager.Instance.LockInput();

        // 1. Lấy tất cả các gem hiện tại
        var allGems = gems.ToList(); // clone list ra

        // 2. Shuffle bằng Fisher-Yates
        for (int i = 0; i < allGems.Count; i++)
        {
            int randomIndex = Random.Range(i, allGems.Count);
            (allGems[i], allGems[randomIndex]) = (allGems[randomIndex], allGems[i]);
        }

        // 3. Gán lại vị trí gem
        int width = board.width;
        int height = board.height;

        int index = 0;
        Sequence shuffleSequence = DOTween.Sequence();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (index >= allGems.Count) break;

                Gem gem = allGems[index];
                gem.x = x;
                gem.y = y;

                Transform newParent = board.GetTileAt(x, y);
                if (newParent != null)
                {
                    gem.transform.SetParent(newParent);

                    // Jump tới vị trí mới với hiệu ứng
                    var tween = gem.transform.DOJump(newParent.position, 0.5f, 1, 0.5f).SetEase(Ease.OutQuad);

                    // Join vào Sequence
                    shuffleSequence.Join(tween);
                }
                index++;
            }
        }

        // 4. Đợi tất cả các tween move xong
        yield return shuffleSequence.WaitForCompletion();

        // 5. Mở lại Input
        InputManager.Instance.UnlockInput();
    }
    public HintData FindBestHint()
    {
        int width = board.width;
        int height = board.height;
        HintData bestHint = null;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Gem a = GetGemAt(x, y);
                if (a == null) continue;

                Vector2Int[] dirs = { new(1, 0), new(0, 1) };

                foreach (var dir in dirs)
                {
                    int newX = x + dir.x;
                    int newY = y + dir.y;
                    Gem b = GetGemAt(newX, newY);
                    if (b == null) continue;

                    SwapGemsData(a, b);

                    int matchCount = CountPotentialMatch(a) + CountPotentialMatch(b);

                    SwapGemsData(a, b);

                    if (matchCount >= 3 && (bestHint == null || matchCount > bestHint.matchCount))
                    {
                        bestHint = new HintData(a, b, matchCount);
                    }
                }
            }
        }

        return bestHint;
    }
    public bool HasValidMove()
    {
        int width = board.width;
        int height = board.height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Gem currentGem = GetGemAt(x, y);
                if (currentGem == null) continue;

                if (x < width - 1)
                {
                    Gem rightGem = GetGemAt(x + 1, y);
                    if (rightGem != null && CanSwapAndMatch(currentGem, rightGem))
                        return true;
                }

                if (y < height - 1)
                {
                    Gem upGem = GetGemAt(x, y + 1);
                    if (upGem != null && CanSwapAndMatch(currentGem, upGem))
                        return true;
                }
            }
        }
        return false;
    }
    private bool CanSwapAndMatch(Gem a, Gem b)
    {
        GemManager.Instance.SwapGemsDataOnly(a, b);

        bool matchFound = HasMatch(a) || HasMatch(b);

        GemManager.Instance.SwapGemsDataOnly(a, b); // Swap lại về

        return matchFound;
    }
    private bool HasMatch(Gem gem)
    {
        if (gem == null) return false;

        string type = gem.gemType;
        int x = gem.x;
        int y = gem.y;

        int count = 1;

        // Check Horizontal
        for (int i = x - 1; i >= 0; i--)
        {
            Gem checkGem = GemManager.Instance.GetGemAt(i, y);
            if (checkGem != null && checkGem.gemType == type) count++;
            else break;
        }
        for (int i = x + 1; i < board.width; i++)
        {
            Gem checkGem = GemManager.Instance.GetGemAt(i, y);
            if (checkGem != null && checkGem.gemType == type) count++;
            else break;
        }
        if (count >= 3) return true;

        // Check Vertical
        count = 1;
        for (int j = y - 1; j >= 0; j--)
        {
            Gem checkGem = GemManager.Instance.GetGemAt(x, j);
            if (checkGem != null && checkGem.gemType == type) count++;
            else break;
        }
        for (int j = y + 1; j < board.height; j++)
        {
            Gem checkGem = GemManager.Instance.GetGemAt(x, j);
            if (checkGem != null && checkGem.gemType == type) count++;
            else break;
        }
        if (count >= 3) return true;

        return false;
    }
}