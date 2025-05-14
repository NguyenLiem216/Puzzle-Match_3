using UnityEngine;

public class BoardManager : LiemMonoBehaviour
{
    public int width = 4;
    public int height = 5;
    public float cellSize = 1f;


    public GameObject titlePrefab;

    private GameObject selectedTile;
    private Color originalTileColor = Color.white;

    protected override void Start()
    {
        base.Start();
        this.GenerateGrid();
        this.FitBoardToCamera();
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
        if (tileContainer != null) DestroyImmediate(tileContainer.gameObject);
        GameObject newContainer = new GameObject("Holder");
        newContainer.transform.parent = this.transform;
        newContainer.transform.localPosition = Vector3.zero;
        return newContainer.transform;
    }

    protected virtual void GenerateGrid()
    {
        Transform tileContainer = this.FindOrCreateTilesContainer();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 position = new Vector2(
                    x * cellSize - (width * cellSize) / 2f + cellSize / 2f,
                    y * cellSize - (height * cellSize) / 2f + cellSize / 2f
                );

                GameObject tile = Instantiate(titlePrefab, position, Quaternion.identity);
                tile.SetActive(true);
                tile.transform.parent = tileContainer;
                tile.transform.localScale = Vector3.one * cellSize;

                GemManager.Instance.SpawnGem(x, y, tile.transform);
            }
        }
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
            var sr = selectedTile.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = originalTileColor;
        }

        GameObject tile = gem.transform.parent.gameObject;
        selectedTile = tile;

        var tileSR = tile.GetComponent<SpriteRenderer>();
        if (tileSR != null)
        {
            originalTileColor = tileSR.color;
            tileSR.color = new Color(1f, 0.9f, 0f, 1f);
        }
    }

    public void UnhighlightGemTile(Gem gem)
    {
        if (selectedTile != null)
        {
            var sr = selectedTile.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = originalTileColor;
            selectedTile = null;
        }
    }
}
