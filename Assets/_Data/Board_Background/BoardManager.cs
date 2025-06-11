using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : LiemMonoBehaviour
{

    [SerializeField] protected int spawnCount = 0;
    public int width = 4;
    public int height = 5;
    public float cellSize = 1f;


    public GameObject titlePrefab;

    private GameObject selectedTile;
    private Color originalTileColor = Color.white;
    private SpriteRenderer selectedTileSpriteRenderer;
    private Dictionary<Vector2Int, Transform> tileMap = new();


    protected override void Start()
    {
        base.Start();
        this.GenerateGrid();
        this.FitBoardToCamera();

    }
    public IEnumerator HandleInitialMatches()
    {
        yield return null; // đợi 1 frame để gem được spawn xong

        while (true)
        {
            List<Gem> initialMatched = GemManager.Instance.CheckMatch();
            if (initialMatched.Count == 0)
                break;

            GemManager.Instance.RemoveMatchedGems(initialMatched);
            yield return new WaitForSeconds(0.3f); // Đợi gem nổ

            yield return StartCoroutine(GemManager.Instance.FillBoard()); // Đợi fill xong
            yield return new WaitForSeconds(0.1f); // Chờ nhỏ
        }
    }



    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadTitlePrefab();
    }

    protected virtual void LoadTitlePrefab()
    {
        if (this.titlePrefab != null) return;
        Transform prefabTransform = transform.Find("TitleCell");
        if (prefabTransform == null) return;
        this.titlePrefab = prefabTransform.gameObject;
        prefabTransform.gameObject.SetActive(false);
    }

    protected virtual Transform FindOrCreateTilesContainer()
    {
        Transform tileContainer = transform.Find("Holder");
        if (tileContainer != null) Destroy(tileContainer.gameObject);       
        GameObject newContainer = new("Holder");
        newContainer.transform.parent = this.transform;
        newContainer.transform.localPosition = Vector3.zero;
        return newContainer.transform;
    }

    protected virtual void GenerateGrid()
    {
        this.spawnCount = 0;
        Transform tileContainer = this.FindOrCreateTilesContainer();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 position = new(
                    x * cellSize - (width * cellSize) / 2f + cellSize / 2f,
                    y * cellSize - (height * cellSize) / 2f + cellSize / 2f
                );

                GameObject tile = Instantiate(titlePrefab, position, Quaternion.identity);
                this.spawnCount++;
                this.UpdateName(tile.transform, x, y);
                tile.SetActive(true);
                tile.transform.parent = tileContainer;
                tile.transform.localScale = Vector3.one * cellSize;
                tileMap[new Vector2Int(x, y)] = tile.transform;


                GemManager.Instance.SpawnGem(x, y, tile.transform);
            }
        }

        StartCoroutine(HandleInitialMatches());
    }

    protected virtual void UpdateName(Transform newObject, int x, int y)
    {
        newObject.name = $"{titlePrefab.name}_({x},{y})";
    }


    protected virtual void FitBoardToCamera()
    {
        Camera cam = Camera.main;
        if (cam == null || !cam.orthographic) return;

        float screenHeight = 2f * cam.orthographicSize;
        float screenWidth = screenHeight * cam.aspect;

        float boardPixelWidth = width * cellSize;
        float boardPixelHeight = height * cellSize;

        float scaleX = screenWidth / boardPixelWidth;
        float scaleY = screenHeight / boardPixelHeight;

        float finalScale = Mathf.Min(scaleX, scaleY);
        transform.localScale = new Vector3(finalScale, finalScale, 1f);
    }

    public void HighlightGemTile(Gem gem)
    {
        if (selectedTile != null)
        {
            if (selectedTile.TryGetComponent<SpriteRenderer>(out var sr)) sr.color = originalTileColor;
        }

        GameObject tile = gem.transform.parent.gameObject;
        selectedTile = tile;

        Transform modelTransform = tile.transform.Find("Model");
        if (modelTransform != null && modelTransform.TryGetComponent<SpriteRenderer>(out var tileSR))
        {
            selectedTileSpriteRenderer = tileSR;
            originalTileColor = tileSR.color;
            tileSR.color = new Color(1f, 0.9f, 0f, 1f);
        }
    }

    public void UnhighlightGemTile(Gem gem)
    {
        if (gem == null)
        {
            throw new ArgumentNullException(nameof(gem));
        }

        if (selectedTileSpriteRenderer != null)
        {
            selectedTileSpriteRenderer.color = originalTileColor;
            selectedTileSpriteRenderer = null;
            selectedTile = null;
        }
    }
    public Transform GetTileAt(int x, int y)
    {
        tileMap.TryGetValue(new Vector2Int(x, y), out Transform tile);
        return tile;
    }

}