using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level/LevelData")]
public class LevelData : ScriptableObject
{
    public GoalType goalType;

    public int targetScore;
    public int moveLimit;

    public List<ItemCollectGoal> collectGoals;
}
