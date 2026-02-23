using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TuanjieFramework;

namespace ZhengLiMaster
{
    /// <summary>
    /// 消除管理器 - 处理所有消除相关逻辑
    /// </summary>
    public class EliminationManager : TuanjieSingleton<EliminationManager>
    {
        [Header("消除配置")]
        public int eliminateCount = 3;           // 触发消除的数量
        public float eliminateDelay = 0.3f;      // 消除延迟（用于播放动画）

        [Header("动画配置")]
        public float itemScaleDownDuration = 0.3f; // 物品缩放动画时长
        public float itemRotateSpeed = 360f;     // 消除时旋转速度
        public float flyToScoreDuration = 0.5f;  // 飞向分数位置时长

        [Header("特效")]
        public GameObject eliminateEffectPrefab; // 消除特效预制体
        public Transform effectsParent;           // 特效父物体

        [Header("音效")]
        public AudioClip eliminateSound;         // 消除音效

        private List<Item> eliminatingItems = new List<Item>();

        protected override void Awake()
        {
            base.Awake();

            // 创建特效父物体
            if (effectsParent == null)
            {
                GameObject parent = new GameObject("Effects");
                parent.transform.SetParent(transform);
                effectsParent = parent.transform;
            }
        }

        /// <summary>
        /// 检查并触发消除
        /// </summary>
        public void CheckAndEliminate(Box box)
        {
            if (box == null || box.currentCount < eliminateCount)
                return;

            StartCoroutine(EliminateSequence(box));
        }

        /// <summary>
        /// 消除序列
        /// </summary>
        private IEnumerator EliminateSequence(Box box)
        {
            // 收集需要消除的物品
            List<Item> itemsToEliminate = new List<Item>(box.items);
            eliminatingItems = itemsToEliminate;

            // 1. 播放消除音效
            PlayEliminateSound();

            // 2. 播放消除动画（缩放+旋转）
            yield return StartCoroutine(PlayEliminateAnimation(itemsToEliminate));

            // 3. 播放消除特效
            PlayEliminateEffect(box.transform.position);

            // 4. 实际消除物品
            DestroyEliminatingItems();

            // 5. 计算并添加分数
            int earnedScore = CalculateScore(itemsToEliminate.Count);
            GameManager.Instance.AddScore(earnedScore);

            // 6. 清空盒子
            box.Clear();

            // 7. 通知消除完成
            OnEliminationComplete(itemsToEliminate.Count, earnedScore);

            eliminatingItems.Clear();
        }

        /// <summary>
        /// 播放消除动画
        /// </summary>
        private IEnumerator PlayEliminateAnimation(List<Item> items)
        {
            float elapsed = 0f;
            Vector3[] startScales = new Vector3[items.Count];
            Vector3[] targetScales = new Vector3[items.Count];

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null)
                {
                    startScales[i] = items[i].transform.localScale;
                    targetScales[i] = Vector3.zero;
                }
            }

            while (elapsed < itemScaleDownDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / itemScaleDownDuration;
                float smoothT = Mathf.SmoothStep(0f, 1f, t);

                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i] != null)
                    {
                        // 缩放
                        items[i].transform.localScale = Vector3.Lerp(startScales[i], targetScales[i], smoothT);

                        // 旋转
                        items[i].transform.Rotate(Vector3.forward * itemRotateSpeed * Time.deltaTime);
                    }
                }

                yield return null;
            }

            // 确保最终缩放为零
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null)
                {
                    items[i].transform.localScale = Vector3.zero;
                }
            }
        }

        /// <summary>
        /// 销毁消除的物品
        /// </summary>
        private void DestroyEliminatingItems()
        {
            foreach (var item in eliminatingItems)
            {
                if (item != null)
                {
                    Destroy(item.gameObject);
                }
            }
        }

        /// <summary>
        /// 播放消除特效
        /// </summary>
        public void PlayEliminateEffect(Vector3 position)
        {
            if (eliminateEffectPrefab != null)
            {
                GameObject effect = Instantiate(eliminateEffectPrefab, position, Quaternion.identity);
                effect.transform.SetParent(effectsParent);

                // 自动销毁特效
                ParticleSystem ps = effect.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    Destroy(effect, ps.main.duration + ps.main.startLifetime.constant);
                }
                else
                {
                    Destroy(effect, 2f);
                }
            }
        }

        /// <summary>
        /// 播放消除音效
        /// </summary>
        private void PlayEliminateSound()
        {
            if (eliminateSound != null)
            {
                AudioSource.PlayClipAtPoint(eliminateSound, Camera.main.transform.position);
            }
        }

        /// <summary>
        /// 计算分数
        /// </summary>
        public int CalculateScore(int itemCount)
        {
            // 3个: 100分, 4个: 150分, 5个: 200分
            int baseScore = 100;
            int extraPerItem = 50;
            return baseScore + (itemCount - eliminateCount) * extraPerItem;
        }

        /// <summary>
        /// 消除完成回调
        /// </summary>
        private void OnEliminationComplete(int itemCount, int score)
        {
            Debug.Log($"消除完成! 物品数: {itemCount}, 获得分数: {score}");

            // 检查是否通关
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnItemsEliminated(itemCount);
            }
        }

        /// <summary>
        /// 检测是否有3个相同类型物品
        /// </summary>
        public bool HasThreeSameType(Box box)
        {
            if (box == null || box.items == null || box.items.Count < eliminateCount)
                return false;

            return box.items.Count >= eliminateCount;
        }
    }
}
