using UnityEngine;
using System.Collections.Generic;
using TuanjieFramework;

namespace ZhengLiMaster
{
    /// <summary>
    /// 游戏状态
    /// </summary>
    public enum GameState
    {
        Waiting,   // 等待开始
        Playing,   // 游戏中
        Paused,    // 暂停
        LevelComplete, // 关卡完成
        GameOver   // 游戏结束
    }

    /// <summary>
    /// 游戏管理器 - 核心游戏逻辑
    /// </summary>
    public class GameManager : TuanjieSingleton<GameManager>
    {
        [Header("游戏状态")]
        public GameState currentState = GameState.Waiting;
        public int currentLevel = 1;
        public int movesLeft;           // 剩余步数
        public int score;                // 当前分数

        [Header("游戏数据")]
        public int totalItemsCount;      // 场上物品总数
        public int eliminatedCount;      // 已消除数量

        [Header("配置")]
        public int initialMoves = 20;   // 初始步数

        [Header("引用")]
        public LevelConfig currentLevelConfig;

        // 事件
        public System.Action<int> onMovesChanged;      // 步数变化
        public System.Action<int> onScoreChanged;      // 分数变化
        public System.Action onLevelComplete;          // 关卡完成
        public System.Action onGameOver;               // 游戏结束

        protected override void Awake()
        {
            base.Awake();
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        public void StartGame()
        {
            currentState = GameState.Playing;
            movesLeft = initialMoves;
            score = 0;
            eliminatedCount = 0;

            Debug.Log($"游戏开始! 初始步数: {movesLeft}");
        }

        /// <summary>
        /// 开始指定关卡
        /// </summary>
        public void StartLevel(int levelId)
        {
            currentLevel = levelId;
            currentState = GameState.Playing;

            // 加载关卡配置
            LoadLevel(levelId);

            Debug.Log($"开始关卡 {levelId}");
        }

        /// <summary>
        /// 加载关卡配置
        /// </summary>
        private void LoadLevel(int levelId)
        {
            // TODO: 从配置表加载关卡数据
            // 临时设置
            movesLeft = initialMoves;
            totalItemsCount = 9; // 示例
            eliminatedCount = 0;
        }

        /// <summary>
        /// 物品放入盒子
        /// </summary>
        public void OnItemPlacedInBox(Item item, Box box)
        {
            if (currentState != GameState.Playing) return;

            // 消耗步数
            ConsumeMove();

            // 检查是否触发消除
            CheckElimination(box);
        }

        /// <summary>
        /// 消耗一步
        /// </summary>
        public void ConsumeMove()
        {
            movesLeft--;
            onMovesChanged?.Invoke(movesLeft);

            if (movesLeft <= 0)
            {
                OnGameOver();
            }
        }

        /// <summary>
        /// 添加分数
        /// </summary>
        public void AddScore(int addScore)
        {
            score += addScore;
            onScoreChanged?.Invoke(score);
        }

        /// <summary>
        /// 物品消除回调
        /// </summary>
        public void OnItemsEliminated(int itemCount)
        {
            eliminatedCount += itemCount;

            // 检查是否通关
            if (eliminatedCount >= totalItemsCount)
            {
                OnLevelComplete();
            }
        }

        /// <summary>
        /// 检查消除
        /// </summary>
        private void CheckElimination(Box box)
        {
            int sameTypeCount = box.GetSameTypeCount(box.acceptType);

            if (sameTypeCount >= 3)
            {
                // 触发消除
                EliminateBox(box);
            }
        }

        /// <summary>
        /// 消除盒子
        /// </summary>
        public void EliminateBox(Box box)
        {
            // 记录消除的物品数量
            int eliminated = box.currentCount;
            eliminatedCount += eliminated;

            // 计算分数 (3个100分，4个150分，5个200分)
            score += eliminated * 50 + (eliminated - 3) * 50;
            onScoreChanged?.Invoke(score);

            // 清空盒子
            box.Clear();

            Debug.Log($"消除! 获得 {score} 分");

            // 检查是否通关
            if (eliminatedCount >= totalItemsCount)
            {
                OnLevelComplete();
            }
        }

        /// <summary>
        /// 盒子满了
        /// </summary>
        public void OnBoxFull(Box box)
        {
            // 盒子满了但没消除，弹回物品
            Debug.Log($"盒子 {box.boxId} 已满!");
        }

        /// <summary>
        /// 关卡完成
        /// </summary>
        private void OnLevelComplete()
        {
            currentState = GameState.LevelComplete;
            Debug.Log($"关卡 {currentLevel} 完成! 最终分数: {score}");
            onLevelComplete?.Invoke();
        }

        /// <summary>
        /// 游戏结束
        /// </summary>
        private void OnGameOver()
        {
            currentState = GameState.GameOver;
            Debug.Log($"游戏结束! 步数用尽");
            onGameOver?.Invoke();
        }

        /// <summary>
        /// 重新开始当前关卡
        /// </summary>
        public void RestartLevel()
        {
            // TODO: 重置场景中的所有物品和盒子
            StartLevel(currentLevel);
        }

        /// <summary>
        /// 退出游戏
        /// </summary>
        public void QuitGame()
        {
            Debug.Log("退出游戏");
            // TODO: 退出逻辑
        }
    }
}
