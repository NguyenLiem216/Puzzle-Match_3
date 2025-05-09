using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;

public class GridGenerator : LiemMonoBehaviour
{
    [SerializeField] protected int width = 4;
    [SerializeField] protected int height = 5;
    [SerializeField] protected float cellSize = 1f;
    public GameObject titlePrefab;

    protected override void Start()
    {
        base.Start();
        this.GenerateGrid();
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
            }
        }
    }
}
