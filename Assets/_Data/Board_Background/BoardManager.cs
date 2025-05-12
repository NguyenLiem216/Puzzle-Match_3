using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;

public class BoardManager : LiemMonoBehaviour
{
    [SerializeField] protected int width = 4;
    [SerializeField] protected int height = 5;
    [SerializeField] protected float cellSize = 1f;
    [SerializeField] private List<GameObject> gemPrefabs;

    public GameObject titlePrefab;

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
        Debug.LogWarning(transform.name + " LoadTitlePrefab", gameObject);
    }

    protected virtual Transform FindOrCreateTilesContainer()
    {
        Transform tileContainer = transform.Find("Holder");
        if (tileContainer != null)
        {
            DestroyImmediate(tileContainer.gameObject);
        }
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

                int randomIndex = UnityEngine.Random.Range(0, gemPrefabs.Count);
                GameObject gem = Instantiate(gemPrefabs[randomIndex], tile.transform.position, Quaternion.identity);
                gem.transform.SetParent(tile.transform);
                gem.transform.localScale = Vector3.one * 0.21f;
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
}
