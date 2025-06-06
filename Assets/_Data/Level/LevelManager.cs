using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public LevelData levelData;

    void Start()
    {
        UIManager.Instance.SetTarget(levelData.targetScore);
        UIManager.Instance.SetMoveLimit(levelData.moveLimit);
    }
}
