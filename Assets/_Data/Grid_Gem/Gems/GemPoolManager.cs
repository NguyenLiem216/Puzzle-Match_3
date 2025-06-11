using System.Collections.Generic;
using UnityEngine;

public class GemPoolManager : LiemMonoBehaviour
{
    public static GemPoolManager Instance;

    [SerializeField] private List<GameObject> gemPrefabs;
    private Dictionary<string, Queue<GameObject>> poolDict = new();
    [Header("Debug Pool")]
    public List<GameObject> pooledObjects = new();


    protected override void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        foreach (var prefab in gemPrefabs)
        {
            string type = prefab.name;
            poolDict[type] = new Queue<GameObject>();
        }
    }
    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadGems();
    }

    protected virtual void LoadGems()
    {
        gemPrefabs = new List<GameObject>();

        GameObject gemManagerObj = GameObject.Find("GemManager");
        if (gemManagerObj == null) return;      

        foreach (Transform child in gemManagerObj.transform)
        {
            if (child == null || child == this.transform) continue;

            if (child.GetComponent<Gem>() != null)
            {
                gemPrefabs.Add(child.gameObject);
                string type = child.name;

                if (!poolDict.ContainsKey(type))
                    poolDict[type] = new Queue<GameObject>();
            }
        }
    }


    public GameObject GetGem(string gemType, Transform parent)
    {
        GameObject obj;

        if (poolDict.ContainsKey(gemType) && poolDict[gemType].Count > 0)
        {
            obj = poolDict[gemType].Dequeue();
            obj.transform.SetParent(parent);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one * 0.21f;
            obj.SetActive(true);

            pooledObjects.Remove(obj);
        }

        else
        {
            var prefab = gemPrefabs.Find(p => p.name == gemType);
            obj = Instantiate(prefab, parent.position, Quaternion.identity, parent);
            obj.transform.localScale = Vector3.one * 0.21f;
            obj.transform.localPosition = Vector3.zero;
        }

        return obj;
    }

    public GameObject GetRandomGem(Transform parent)
    {
        int index = Random.Range(0, gemPrefabs.Count);
        string type = gemPrefabs[index].name;
        return GetGem(type, parent);
    }

    public void ReturnGem(GameObject gem)
    {
        string type = gem.name.Split('(')[0];
        gem.SetActive(false);
        gem.transform.SetParent(this.transform);

        if (!poolDict.ContainsKey(type))
            poolDict[type] = new Queue<GameObject>();

        poolDict[type].Enqueue(gem);

        if (!pooledObjects.Contains(gem))
            pooledObjects.Add(gem);
    }


    public List<string> GetAllGemTypes()
    {
        return gemPrefabs.ConvertAll(p => p.name);
    }
}
