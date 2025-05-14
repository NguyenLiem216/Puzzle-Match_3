using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GemManager : LiemMonoBehaviour
{
    public static GemManager Instance { get; private set; }

    [SerializeField] private List<GameObject> gemPrefabs;

    private Dictionary<Vector2Int, Gem> gems = new Dictionary<Vector2Int, Gem>();
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
        int randomIndex = Random.Range(0, gemPrefabs.Count);
        GameObject gemGO = Instantiate(gemPrefabs[randomIndex], parent.position, Quaternion.identity, parent);
        gemGO.transform.localScale = Vector3.one * 0.21f;

        Gem gem = gemGO.GetComponent<Gem>();
        if (gem != null)
        {
            gem.SetData(x, y, gemPrefabs[randomIndex].name);
            gems[new Vector2Int(x, y)] = gem;
        }

        return gem;
    }

    public void SwapGems(Gem a, Gem b)
    {
        // Swap position
        Vector3 tempPos = a.transform.position;
        a.transform.position = b.transform.position;
        b.transform.position = tempPos;

        // Swap logical grid position
        Vector2Int posA = new Vector2Int(a.x, a.y);
        Vector2Int posB = new Vector2Int(b.x, b.y);

        gems[posA] = b;
        gems[posB] = a;

        (a.x, a.y, b.x, b.y) = (b.x, b.y, a.x, a.y);

        // Swap parents
        Transform parentA = a.transform.parent;
        Transform parentB = b.transform.parent;
        a.transform.SetParent(parentB);
        b.transform.SetParent(parentA);
    }

    public Gem GetGemAt(Vector2Int pos)
    {
        gems.TryGetValue(pos, out Gem gem);
        return gem;
    }
    public List<Gem> CheckMatch()
    {
        int width = board.width;
        int height = board.height;

        List<Gem> matchedGems = new List<Gem>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                Gem currentGem = GetGemAt(pos);
                if (currentGem == null) continue;

                // Check hàng ngang
                List<Gem> horizontalMatches = new List<Gem> { currentGem };
                for (int i = 1; x + i < width; i++)
                {
                    Gem nextGem = GetGemAt(new Vector2Int(x + i, y));
                    if (nextGem != null && nextGem.gemType == currentGem.gemType)
                        horizontalMatches.Add(nextGem);
                    else break;
                }

                if (horizontalMatches.Count >= 3)
                    matchedGems.AddRange(horizontalMatches);

                // Check hàng dọc
                List<Gem> verticalMatches = new List<Gem> { currentGem };
                for (int i = 1; y + i < height; i++)
                {
                    Gem nextGem = GetGemAt(new Vector2Int(x, y + i));
                    if (nextGem != null && nextGem.gemType == currentGem.gemType)
                        verticalMatches.Add(nextGem);
                    else break;
                }

                if (verticalMatches.Count >= 3)
                    matchedGems.AddRange(verticalMatches);
            }
        }

        // Gỡ trùng
        matchedGems = new HashSet<Gem>(matchedGems).ToList();

        foreach (var gem in matchedGems)
        {
            Debug.Log($"Matched Gem at ({gem.x}, {gem.y}) with type {gem.gemType}");
        }

        return matchedGems;
    }

}
