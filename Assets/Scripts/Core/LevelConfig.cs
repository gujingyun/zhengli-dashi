using UnityEngine;

namespace ZhengLiMaster
{
    /// <summary>
    /// 关卡配置数据
    /// </summary>
    [System.Serializable]
    public class LevelConfig
    {
        [Header("关卡信息")]
        public int levelId = 1;
        public string levelName = "Level 1";

        [Header("物品配置")]
        public ItemSpawnData[] items;

        [Header("盒子配置")]
        public BoxSpawnData[] boxes;

        [Header("关卡参数")]
        public int moves = 20;           // 步数
        public int targetScore = 0;      // 目标分数
        public float timeLimit = 0;      // 时间限制（0表示不限时）

        [Header("星星要求")]
        public int oneStarScore = 100;
        public int twoStarScore = 200;
        public int threeStarScore = 300;
    }

    /// <summary>
    /// 物品生成数据
    /// </summary>
    [System.Serializable]
    public class ItemSpawnData
    {
        public ItemType type;
        public int count;
        public Vector2Int[] positions;  // 生成位置
    }

    /// <summary>
    /// 盒子生成数据
    /// </summary>
    [System.Serializable]
    public class BoxSpawnData
    {
        public ItemType acceptType;     // 接受的物品类型
        public int capacity = 3;        // 容量
        public Vector2Int position;     // 位置
    }
}
